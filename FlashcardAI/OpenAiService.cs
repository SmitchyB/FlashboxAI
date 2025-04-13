using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace FlashcardAI
{
    // Service for interacting with the OpenAI API
    public class OpenAIService
    {
        private readonly HttpClient httpClient; // HTTP client
        private readonly string apiKey; // API key
        private const string ApiBaseUrl = "https://api.openai.com/v1/"; // Base URL for API calls

        // Chat model options
        private const string DefaultModel = "gpt-3.5-turbo";

        public OpenAIService(string? customApiKey = null)
        {
            // Initialize HTTP client
            httpClient = new HttpClient();

            // Get API key from environment variables or use custom key if provided
            apiKey = customApiKey ?? EnvironmentConfig.OpenAIApiKey;

            // Add authorization header if API key is available
            if (!string.IsNullOrEmpty(apiKey))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            }
        }

        #region Core API Methods

        // Generic method to call OpenAI's chat completion API
        private async Task<JObject?> CallChatCompletionAsync(
            string prompt,
            string? systemPrompt = null,
            string model = DefaultModel,
            int maxTokens = 500,
            bool requestJsonResponse = false)
        {
            try
            {
                // Check if API key is available
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new Exception("OpenAI API key is not available.");
                }

                // Create messages array with system message and user prompt
                var messages = new List<object>();

                // Add system message if provided
                if (!string.IsNullOrEmpty(systemPrompt))
                {
                    messages.Add(new { role = "system", content = systemPrompt });
                }

                // Add user message
                messages.Add(new { role = "user", content = prompt });

                // Prepare the API request
                object request;

                // Add response format for JSON responses if requested
                if (requestJsonResponse)
                {
                    request = new
                    {
                        model = model,
                        messages = messages.ToArray(),
                        response_format = new { type = "json_object" },
                        max_tokens = maxTokens
                    };
                }
                else
                {
                    request = new
                    {
                        model = model,
                        messages = messages.ToArray(),
                        max_tokens = maxTokens
                    };
                }

                // Convert to JSON
                string jsonRequest = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Send the request to OpenAI API
                var response = await httpClient.PostAsync($"{ApiBaseUrl}chat/completions", content);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(jsonResponse);
                }
                else
                {
                    // Handle API error
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error calling OpenAI API: {ex.Message}");
                throw;
            }
        }

        // Generic method to extract content from a chat completion response
        private string? ExtractContentFromChatResponse(JObject? response)
        {
            if (response == null)
                return null;

            try
            {
                return response["choices"]?[0]?["message"]?["content"]?.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting content from chat response: {ex.Message}");
                return null;
            }
        }

        // Unified method for file operations with OpenAI
        private async Task<HttpResponseMessage> CallFileApiAsync(HttpMethod method, string endpoint, MultipartFormDataContent? formContent = null)
        {
            try
            {
                // Check if API key is available
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new Exception("OpenAI API key is not available.");
                }

                var requestUri = $"{ApiBaseUrl}{endpoint}";
                var request = new HttpRequestMessage(method, requestUri);

                if (formContent != null && method == HttpMethod.Post)
                {
                    request.Content = formContent;
                }

                return await httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in file API call: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Convenience Methods for Specific Tasks

        // Uploads a file to OpenAI for processing - using the core methods
        public async Task<string> UploadFileAsync(string filePath)
        {
            try
            {
                // Check if file exists
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("The specified file was not found.", filePath);
                }

                using var formContent = new MultipartFormDataContent();

                var fileInfo = new FileInfo(filePath);

                // Read file bytes
                var fileBytes = await File.ReadAllBytesAsync(filePath);
                var fileContent = new ByteArrayContent(fileBytes);

                // Set content type based on file extension
                string contentType = GetContentType(fileInfo.Extension);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                formContent.Add(fileContent, "file", fileInfo.Name);
                formContent.Add(new StringContent("assistants"), "purpose");

                var response = await CallFileApiAsync(HttpMethod.Post, "files", formContent);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var responseObj = JObject.Parse(jsonResponse);

                    // Extract and return the file ID
                    string? fileId = responseObj["id"]?.ToString();
                    if (string.IsNullOrEmpty(fileId))
                    {
                        throw new Exception("Failed to get file ID from API response");
                    }
                    return fileId;
                }
                else
                {
                    // Handle API error
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                throw;
            }
        }

        // Deletes a file from OpenAI after use - using the core methods
        public async Task<bool> DeleteFileAsync(string? fileId)
        {
            try
            {
                // Check if API key is available
                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(fileId))
                {
                    return false;
                }

                var response = await CallFileApiAsync(HttpMethod.Delete, $"files/{fileId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error deleting file: {ex.Message}");
                return false;
            }
        }

        // Generates a memory tip for a flashcard - using the core methods
        public async Task<string> GenerateMemoryTip(string question, string answer)
        {
            try
            {
                // Check if API key is available
                if (string.IsNullOrEmpty(apiKey))
                {
                    return "Associate this answer with something meaningful to you.";
                }

                string systemPrompt = "You are a helpful assistant that provides memory tips for flashcards.";
                string userPrompt = $"Create a short, helpful memory tip for remembering that the answer to '{question}' is '{answer}'. Keep it concise and under 100 characters.";

                var response = await CallChatCompletionAsync(userPrompt, systemPrompt, DefaultModel, 100);
                string? tip = ExtractContentFromChatResponse(response);

                return tip ?? "Associate this answer with something meaningful to you.";
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error generating AI tip: {ex.Message}");
                return "Associate this answer with something meaningful to you.";
            }
        }

        // Gets the MIME content type based on file extension
        private string GetContentType(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return "application/octet-stream";

            return extension.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".doc" => "application/msword",
                ".txt" => "text/plain",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                _ => "application/octet-stream",
            };
        }

        // Generates dummy questions for testing or fallback
        private string[] GenerateDummyQuestions(string topic, int count)
        {
            string[] questions = new string[count];
            for (int i = 0; i < count; i++)
            {
                questions[i] = $"Question {i + 1} about {topic}?";
            }
            return questions;
        }

        // Generates dummy answers for testing or fallback
        private string[] GenerateDummyAnswers(string topic, int count)
        {
            string[] answers = new string[count];
            for (int i = 0; i < count; i++)
            {
                answers[i] = $"Answer {i + 1} about {topic}.";
            }
            return answers;
        }

        // Generates dummy wrong answers for testing or fallback
        private string[] GenerateDummyWrongAnswers(string correctAnswer, int count)
        {
            string[] options = new string[count];
            for (int i = 0; i < count; i++)
            {
                options[i] = $"Wrong answer {i + 1} (not {correctAnswer})";
            }
            return options;
        }

        #endregion

        #region Implementation Methods

        /// Generates wrong answers for a flashcard question
        public async Task<string[]> GenerateWrongAnswers(string question, string correctAnswer, int numberOfOptions = 3)
        {
            try
            {
                // Check if API key is available
                if (string.IsNullOrEmpty(apiKey))
                {
                    return GenerateDummyWrongAnswers(correctAnswer, numberOfOptions);
                }

                string systemPrompt = "You are a helpful assistant that creates plausible but incorrect answer options for flashcard tests.";
                string userPrompt = $"For the flashcard question: '{question}' with correct answer: '{correctAnswer}', generate {numberOfOptions} plausible but incorrect answer options. Make them challenging but clearly wrong. Format as JSON with an array called 'wrong_answers'.";

                var response = await CallChatCompletionAsync(userPrompt, systemPrompt, DefaultModel, 500, true);
                string? contentText = ExtractContentFromChatResponse(response);

                if (string.IsNullOrEmpty(contentText))
                {
                    return GenerateDummyWrongAnswers(correctAnswer, numberOfOptions);
                }

                // Parse the wrong answers from the JSON
                JObject? wrongAnswersObj = null;
                try
                {
                    wrongAnswersObj = JObject.Parse(contentText);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing wrong answers JSON: {ex.Message}");
                    return GenerateDummyWrongAnswers(correctAnswer, numberOfOptions);
                }

                // Prepare array for wrong answers
                string[] wrongAnswerOptions = new string[numberOfOptions];

                // Extract the wrong answers
                if (wrongAnswersObj?["wrong_answers"] != null)
                {
                    int index = 0;
                    foreach (var wrongAnswer in wrongAnswersObj["wrong_answers"]!)
                    {
                        if (index < numberOfOptions && wrongAnswer != null)
                        {
                            string answerText = wrongAnswer.ToString() ?? $"Incorrect option {index + 1}";
                            wrongAnswerOptions[index] = answerText;
                            index++;
                        }
                    }

                    // If we didn't get enough wrong answers, fill the rest with dummy data
                    for (int i = index; i < numberOfOptions; i++)
                    {
                        wrongAnswerOptions[i] = $"Incorrect option {i + 1} for {question}";
                    }

                    return wrongAnswerOptions;
                }
                else
                {
                    // Fallback to dummy answers if structure is unexpected
                    return GenerateDummyWrongAnswers(correctAnswer, numberOfOptions);
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error generating wrong answers: {ex.Message}");

                // Return dummy data on error
                return GenerateDummyWrongAnswers(correctAnswer, numberOfOptions);
            }
        }

        // Generates flashcards for a given topic or text
        public async Task<(string[] Questions, string[] Answers)> GenerateFlashcards(string promptText, int numberOfCards)
        {
            try
            {
                // Check if API key is available
                if (string.IsNullOrEmpty(apiKey))
                {
                    return (GenerateDummyQuestions(promptText, numberOfCards), GenerateDummyAnswers(promptText, numberOfCards));
                }

                string systemPrompt = "You are a helpful assistant that creates educational flashcards.";
                string userPrompt = $"Create {numberOfCards} flashcards about the following topic or text. Each flashcard should have a question and answer. Format as JSON array with 'question' and 'answer' fields for each flashcard. Return the array wrapped in a 'flashcards' object. Topic: {promptText}";

                var response = await CallChatCompletionAsync(userPrompt, systemPrompt, DefaultModel, 2000, true);
                string? contentText = ExtractContentFromChatResponse(response);

                if (string.IsNullOrEmpty(contentText))
                {
                    throw new Exception("API returned empty response content");
                }

                JObject? flashcardsObj = null;
                try
                {
                    flashcardsObj = JObject.Parse(contentText);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing flashcards JSON: {ex.Message}");
                    return (GenerateDummyQuestions(promptText, numberOfCards), GenerateDummyAnswers(promptText, numberOfCards));
                }

                // Prepare arrays for questions and answers
                string[] questions = new string[numberOfCards];
                string[] answers = new string[numberOfCards];

                // Store the generated flashcards
                int index = 0;
                if (flashcardsObj?["flashcards"] != null)
                {
                    foreach (var card in flashcardsObj["flashcards"]!)
                    {
                        if (index < numberOfCards && card != null)
                        {
                            var questionToken = card["question"];
                            var answerToken = card["answer"];

                            questions[index] = questionToken?.ToString() ?? $"Question {index + 1}";
                            answers[index] = answerToken?.ToString() ?? $"Answer {index + 1}";
                            index++;
                        }
                    }

                    // If we didn't get enough flashcards, fill the rest with dummy data
                    if (index < numberOfCards)
                    {
                        for (int i = index; i < numberOfCards; i++)
                        {
                            questions[i] = $"Question {i + 1} about {promptText}?";
                            answers[i] = $"Answer {i + 1} about {promptText}.";
                        }
                    }
                    return (questions, answers);
                }
                else
                {
                    throw new Exception("API returned invalid JSON format. No flashcards found.");
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error generating flashcards: {ex.Message}");

                // Return dummy data on error
                return (GenerateDummyQuestions(promptText, numberOfCards), GenerateDummyAnswers(promptText, numberOfCards));
            }
        }

        // Generates flashcards using a combination of text prompt and uploaded file
        public async Task<(string[] Questions, string[] Answers)> GenerateFlashcardsWithFile(string filePath, string additionalPrompt, int numberOfCards)
        {
            try
            {
                // Check if API key is available
                if (string.IsNullOrEmpty(apiKey))
                {
                    return (GenerateDummyQuestions(additionalPrompt, numberOfCards), GenerateDummyAnswers(additionalPrompt, numberOfCards));
                }

                // Check if file exists
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("The specified file was not found.", filePath);
                }

                string fileExtension = Path.GetExtension(filePath).ToLower();
                string fileName = Path.GetFileName(filePath);
                string fileContent = await ExtractFileContentAsync(filePath, fileExtension, fileName);

                // Build the prompt
                string prompt = BuildFilePrompt(fileName, fileExtension, fileContent, additionalPrompt, numberOfCards);

                // Call the API using our generic method
                string systemPrompt = "You are a helpful assistant that creates educational flashcards based on document content. Format your response as JSON with an array of flashcards, each with 'question' and 'answer' fields.";

                var response = await CallChatCompletionAsync(prompt, systemPrompt, DefaultModel, 4000, true);
                string? contentText = ExtractContentFromChatResponse(response);

                if (string.IsNullOrEmpty(contentText))
                {
                    Console.WriteLine("API returned empty response content");
                    throw new Exception("API returned empty response content");
                }

                // Process the response to extract flashcards
                var (questions, answers) = ProcessFlashcardResponse(contentText, fileName, numberOfCards);

                return (questions, answers);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error generating flashcards with file: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Return dummy data on error
                string topic = Path.GetFileNameWithoutExtension(filePath ?? additionalPrompt);
                return (GenerateDummyQuestions(topic, numberOfCards), GenerateDummyAnswers(topic, numberOfCards));
            }
        }

        // Helper method to extract content from a file
        private async Task<string> ExtractFileContentAsync(string filePath, string fileExtension, string fileName)
        {
            try
            {
                string fileContent = string.Empty;

                switch (fileExtension)
                {
                    case ".txt":
                        fileContent = await File.ReadAllTextAsync(filePath);
                        break;

                    case ".pdf":
                    case ".doc":
                    case ".docx":
                        // Limited implementation - would need additional libraries for full extraction
                        fileContent = $"[This is content from {fileName}. Please note that for full text extraction from {fileExtension} files, additional libraries would be needed.]";
                        Console.WriteLine($"Note: Full text extraction from {fileExtension} files requires additional libraries.");
                        fileContent = $"File: {fileName}\nPlease generate flashcards based on what would typically be in this type of document.";
                        break;

                    default:
                        throw new NotSupportedException($"File type {fileExtension} is not supported.");
                }

                return fileContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting content from file: {ex.Message}");
                throw new Exception($"Unable to process file {fileName}: {ex.Message}");
            }
        }

        // Helper method to build prompt for file-based generation
        private string BuildFilePrompt(
            string fileName,
            string fileExtension,
            string fileContent,
            string additionalPrompt,
            int numberOfCards)
        {
            StringBuilder promptBuilder = new StringBuilder();

            promptBuilder.AppendLine($"I'm providing content from a {fileExtension} file named '{fileName}'.\n");

            // Add the file content (limit length to avoid exceeding token limits)
            const int maxContentLength = 3000;
            if (fileContent.Length > maxContentLength)
            {
                promptBuilder.AppendLine($"File content (truncated):\n{fileContent.Substring(0, maxContentLength)}...\n");
            }
            else
            {
                promptBuilder.AppendLine($"File content:\n{fileContent}\n");
            }

            // Add any additional context or instructions
            if (!string.IsNullOrWhiteSpace(additionalPrompt))
            {
                promptBuilder.AppendLine($"Additional context or instructions: {additionalPrompt}\n");
            }

            // Add the request for flashcards
            promptBuilder.AppendLine($"Based on this content, create {numberOfCards} educational flashcards. Each flashcard should have a question and answer that helps learn the key information in this content.");

            return promptBuilder.ToString();
        }

        // Helper method to process flashcard response JSON
        private (string[] Questions, string[] Answers) ProcessFlashcardResponse(
            string contentText,
            string fileName,
            int numberOfCards)
        {
            // Prepare arrays for questions and answers
            string[] questions = new string[numberOfCards];
            string[] answers = new string[numberOfCards];
            int index = 0;

            JObject? jsonObj = null;
            try
            {
                jsonObj = JObject.Parse(contentText);
            }
            catch (JsonException)
            {
                // If not a JObject, try as JArray
                try
                {
                    JArray jsonArray = JArray.Parse(contentText);
                    index = ExtractFlashcardsFromArray(jsonArray, questions, answers, numberOfCards);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing JSON array: {ex.Message}");
                }
            }

            if (jsonObj != null)
            {
                // Try different known structures
                if (jsonObj["flashcards"] != null && jsonObj["flashcards"]?.Type == JTokenType.Array)
                {
                    Console.WriteLine("Found 'flashcards' array in response");
                    index = ExtractFlashcardsFromArray(jsonObj["flashcards"] as JArray, questions, answers, numberOfCards);
                }
                else if (jsonObj["cards"] != null && jsonObj["cards"]?.Type == JTokenType.Array)
                {
                    Console.WriteLine("Found 'cards' array in response");
                    index = ExtractFlashcardsFromArray(jsonObj["cards"] as JArray, questions, answers, numberOfCards);
                }
                else
                {
                    // Look for any property that might be an array of cards
                    foreach (var prop in jsonObj.Properties())
                    {
                        if (prop.Value.Type == JTokenType.Array)
                        {
                            Console.WriteLine($"Found array property: {prop.Name}");
                            JArray? arrayProp = prop.Value as JArray;
                            if (arrayProp != null)
                            {
                                var extractedCount = ExtractFlashcardsFromArray(arrayProp, questions, answers, numberOfCards);
                                if (extractedCount > 0)
                                {
                                    index = extractedCount;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            // Fill remaining slots with dummy data if needed
            if (index < numberOfCards)
            {
                string topic = Path.GetFileNameWithoutExtension(fileName);
                for (int i = index; i < numberOfCards; i++)
                {
                    questions[i] = $"Question {i + 1} about {topic}?";
                    answers[i] = $"Answer {i + 1} about {topic}.";
                }
            }

            return (questions, answers);
        }

        // Helper method to extract flashcards from a JSON array
        private int ExtractFlashcardsFromArray(
            JArray? array,
            string[] questions,
            string[] answers,
            int maxCards)
        {
            if (array == null) return 0;

            int index = 0;

            foreach (var item in array)
            {
                if (index >= maxCards) break;

                if (item != null)
                {
                    JToken? questionToken = item["question"];
                    JToken? answerToken = item["answer"];

                    if (questionToken != null && answerToken != null)
                    {
                        questions[index] = questionToken.ToString();
                        answers[index] = answerToken.ToString();
                        index++;
                    }
                }
            }

            return index;
        }

        #endregion
    }
}
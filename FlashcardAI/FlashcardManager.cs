using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlashcardAI
{
    // Unified class to handle all flashcard data management (combines LocalStorageHelper and FlashcardData)
    public static class FlashcardManager
    {
        #region Properties and Fields

        // Path to store flashcard data
        private static readonly string DataFolderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FlashcardAI",
            "Data");

        private static readonly string SetsIndexPath = Path.Combine(DataFolderPath, "flashcard_sets.json");

        // In-memory cache of flashcard sets
        private static List<FlashcardSetInfo> cachedSets = new List<FlashcardSetInfo>();

        // Public access to set data (for backward compatibility)
        public static string[] SetTitles { get; private set; } = new string[100];
        public static int[] HighScores { get; private set; } = new int[100];
        public static int SetCount { get; private set; } = 0;

        #endregion

        #region Models

        // Model class for set info (stored in index file)
        public class FlashcardSetInfo
        {
            public string Title { get; set; } = string.Empty;
            public int HighScore { get; set; } = 0;
            public DateTime CreatedDate { get; set; } = DateTime.Now;
        }

        // Model class for set data (stored in individual set files)
        public class FlashcardSet
        {
            public string Title { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; } = DateTime.Now;
            public int HighScore { get; set; } = 0;
            public List<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
        }

        // Model class for flashcard
        public class Flashcard
        {
            public string Question { get; set; } = string.Empty;
            public string Answer { get; set; } = string.Empty;
            public string AiTip { get; set; } = string.Empty;
            public List<string> WrongAnswers { get; set; } = new List<string>();
        }

        #endregion

        #region Initialization

        // Static constructor to initialize storage and load cached data
        static FlashcardManager()
        {
            InitializeStorage();
            LoadSetsFromLocalStorage();
        }

        // Initialize storage directories and files if they don't exist
        private static void InitializeStorage()
        {
            try
            {
                // Create data directory if it doesn't exist
                if (!Directory.Exists(DataFolderPath))
                {
                    Directory.CreateDirectory(DataFolderPath);
                }

                // Create index file if it doesn't exist
                if (!File.Exists(SetsIndexPath))
                {
                    File.WriteAllText(SetsIndexPath, JsonSerializer.Serialize(new List<FlashcardSetInfo>()));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing storage: {ex.Message}");
            }
        }

        #endregion

        #region Set Management

        // Load existing flashcard sets from local storage
        public static void LoadSetsFromLocalStorage()
        {
            try
            {
                // Read and parse sets index file
                string json = File.ReadAllText(SetsIndexPath);
                cachedSets = JsonSerializer.Deserialize<List<FlashcardSetInfo>>(json) ?? new List<FlashcardSetInfo>();

                // Clear existing data
                Array.Clear(SetTitles, 0, SetTitles.Length);
                Array.Clear(HighScores, 0, HighScores.Length);
                SetCount = 0;

                // Fill the arrays for backward compatibility
                foreach (var set in cachedSets)
                {
                    if (SetCount < SetTitles.Length)
                    {
                        SetTitles[SetCount] = set.Title;
                        HighScores[SetCount] = set.HighScore;
                        SetCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading flashcard sets: {ex.Message}");
                // Initialize empty data if loading fails
                cachedSets = new List<FlashcardSetInfo>();
                SetCount = 0;
            }
        }

        // Adds a new flashcard set or gets the index of an existing set
        public static int AddSet(string title)
        {
            // Check if a set with this title already exists
            for (int i = 0; i < SetCount; i++)
            {
                if (SetTitles[i] == title)
                {
                    return i; // Set already exists, return its index
                }
            }

            // Add the new set if we have room
            if (SetCount < SetTitles.Length)
            {
                SetTitles[SetCount] = title;
                HighScores[SetCount] = 0;
                return SetCount++;
            }

            return -1; // Array is full
        }

        // Get path for a specific set file
        private static string GetSetFilePath(string title)
        {
            // Sanitize filename
            string safeTitle = string.Join("_", title.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(DataFolderPath, $"{safeTitle}.json");
        }

        // Gets all flashcard sets
        public static List<(string Title, int HighScore)> GetAllSets()
        {
            // Ensure we have the latest data
            LoadSetsFromLocalStorage();

            var result = new List<(string, int)>();
            for (int i = 0; i < SetCount; i++)
            {
                result.Add((SetTitles[i], HighScores[i]));
            }
            return result;
        }

        #endregion

        #region Flashcard Operations

        // Save a flashcard set
        public static async Task<bool> SaveFlashcardSetAsync(string title, string[] questions, string[] answers, string[] aiTips, string[][] wrongAnswers, int count)
        {
            try
            {
                // Create flashcard set
                var set = new FlashcardSet
                {
                    Title = title,
                    CreatedDate = DateTime.Now,
                    HighScore = 0,
                    Flashcards = new List<Flashcard>()
                };

                // Add flashcards
                for (int i = 0; i < count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(questions[i]) && !string.IsNullOrWhiteSpace(answers[i]))
                    {
                        var card = new Flashcard
                        {
                            Question = questions[i],
                            Answer = answers[i],
                            AiTip = aiTips[i] ?? string.Empty
                        };

                        // Add wrong answers if available
                        if (wrongAnswers != null && i < wrongAnswers.Length && wrongAnswers[i] != null)
                        {
                            card.WrongAnswers = new List<string>(wrongAnswers[i]);
                        }

                        set.Flashcards.Add(card);
                    }
                }

                // Check if we're updating an existing set
                bool isUpdate = false;
                int existingSetIndex = -1;

                // Check for existing set with same title
                for (int i = 0; i < cachedSets.Count; i++)
                {
                    if (cachedSets[i].Title == title)
                    {
                        isUpdate = true;
                        existingSetIndex = i;
                        break;
                    }
                }

                // If updating an existing set, preserve the high score
                if (isUpdate && existingSetIndex >= 0)
                {
                    set.HighScore = cachedSets[existingSetIndex].HighScore;
                }

                // Serialize and save the set
                string setJson = JsonSerializer.Serialize(set, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(GetSetFilePath(title), setJson);

                // Update the index file
                await UpdateSetIndexAsync(title, set.HighScore);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving flashcard set: {ex.Message}");
                return false;
            }
        }

        // Get a flashcard set
        public static async Task<(string[] Questions, string[] Answers, string[] Tips, string[][] WrongAnswers)?> GetFlashcardSetAsync(string title)
        {
            try
            {
                string filePath = GetSetFilePath(title);

                if (!File.Exists(filePath))
                    return null;

                string json = await File.ReadAllTextAsync(filePath);
                var set = JsonSerializer.Deserialize<FlashcardSet>(json);

                if (set == null || set.Flashcards.Count == 0)
                    return null;

                int cardCount = set.Flashcards.Count;
                string[] questions = new string[cardCount];
                string[] answers = new string[cardCount];
                string[] tips = new string[cardCount];
                string[][] wrongAnswers = new string[cardCount][];

                for (int i = 0; i < cardCount; i++)
                {
                    questions[i] = set.Flashcards[i].Question;
                    answers[i] = set.Flashcards[i].Answer;
                    tips[i] = set.Flashcards[i].AiTip;

                    // Get wrong answers if available
                    if (set.Flashcards[i].WrongAnswers != null && set.Flashcards[i].WrongAnswers.Count > 0)
                    {
                        wrongAnswers[i] = set.Flashcards[i].WrongAnswers.ToArray();
                    }
                    else
                    {
                        wrongAnswers[i] = Array.Empty<string>();
                    }
                }

                return (questions, answers, tips, wrongAnswers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting flashcard set: {ex.Message}");
                return null;
            }
        }

        // Update set index
        private static async Task UpdateSetIndexAsync(string title, int highScore)
        {
            try
            {
                // Find existing set or add new one
                var existingSet = cachedSets.Find(s => s.Title == title);
                if (existingSet != null)
                {
                    // Update high score if higher
                    if (highScore > existingSet.HighScore)
                    {
                        existingSet.HighScore = highScore;
                    }
                }
                else
                {
                    // Add new set
                    cachedSets.Add(new FlashcardSetInfo
                    {
                        Title = title,
                        HighScore = highScore,
                        CreatedDate = DateTime.Now
                    });
                }

                // Serialize and save the index
                string json = JsonSerializer.Serialize(cachedSets, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(SetsIndexPath, json);

                // Update the arrays
                LoadSetsFromLocalStorage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating set index: {ex.Message}");
            }
        }

        // Update high score
        public static async Task<bool> UpdateHighScoreAsync(int setIndex, int score)
        {
            // Validate index
            if (setIndex < 0 || setIndex >= SetCount)
                return false;

            // Only update if the new score is higher
            if (score > HighScores[setIndex])
            {
                string title = SetTitles[setIndex];

                try
                {
                    string filePath = GetSetFilePath(title);

                    if (!File.Exists(filePath))
                        return false;

                    // Update the set file
                    string json = await File.ReadAllTextAsync(filePath);
                    var set = JsonSerializer.Deserialize<FlashcardSet>(json);

                    if (set == null)
                        return false;

                    // Update score if higher
                    if (score > set.HighScore)
                    {
                        set.HighScore = score;
                        json = JsonSerializer.Serialize(set, new JsonSerializerOptions { WriteIndented = true });
                        await File.WriteAllTextAsync(filePath, json);

                        // Update the index file
                        await UpdateSetIndexAsync(title, score);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating high score: {ex.Message}");
                }
            }

            return false;
        }

        #endregion
    }
}
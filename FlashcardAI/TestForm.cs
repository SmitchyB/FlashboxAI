using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace FlashcardAI
{
    public partial class TestForm : FlashcardFormBase
    {
        private string setTitle;
        private string[]? questions;
        private string[]? answers;
        private string[]? aiTips;
        private string[][]? wrongAnswers;
        private int currentCardIndex = 0;
        private int cardCount = 0;
        private int correctAnswers = 0;
        private Random random = new Random();

        // Track which button has the correct answer for each question
        private int[] correctButtonIndices = Array.Empty<int>();

        // Store all generated answer choices
        private List<string[]> allAnswerChoices = new List<string[]>();

        // Constructor to initialize the form with a title
        public TestForm(string title)
        {
            InitializeComponent();// Initialize the form

            // Set the hover effects for answer buttons
            UIUtilities.SetupButtonHoverEffects(
                btnAnswerA,
                UIUtilities.AppColors.ButtonBackground,
                UIUtilities.AppColors.LightText,
                UIUtilities.AppColors.ButtonHover,
                UIUtilities.AppColors.DarkText
            );

            UIUtilities.SetupButtonHoverEffects(
                btnAnswerB,
                UIUtilities.AppColors.ButtonBackground,
                UIUtilities.AppColors.LightText,
                UIUtilities.AppColors.ButtonHover,
                UIUtilities.AppColors.DarkText
            );

            UIUtilities.SetupButtonHoverEffects(
                btnAnswerC,
                UIUtilities.AppColors.ButtonBackground,
                UIUtilities.AppColors.LightText,
                UIUtilities.AppColors.ButtonHover,
                UIUtilities.AppColors.DarkText
            );

            UIUtilities.SetupButtonHoverEffects(
                btnAnswerD,
                UIUtilities.AppColors.ButtonBackground,
                UIUtilities.AppColors.LightText,
                UIUtilities.AppColors.ButtonHover,
                UIUtilities.AppColors.DarkText
            );

            setTitle = title; // Set the title

            // Update the form title to include the set title
            this.Text = $"Flash Box - Test: {title}";

            LoadFlashcardSetAsync(); // Load the flashcard set asynchronously
        }

        // Load the flashcard set asynchronously
        private async void LoadFlashcardSetAsync()
        {
            try
            {
                lblStatus.Text = "Loading flashcards..."; // Update the status label
                panelLoading.Visible = true; // Show the loading panel
                panelQuestion.Visible = false; // Hide the question panel

                var result = await FlashcardManager.GetFlashcardSetAsync(setTitle); // Load the flashcard set

                if (result.HasValue) // Flashcards loaded successfully
                {
                    (questions, answers, aiTips, wrongAnswers) = result.Value; // Unpack the 4-element tuple
                    cardCount = questions?.Length ?? 0; // Get the number of flashcards

                    if (cardCount > 0) // Flashcards found in the set
                    {
                        // Update loading status before preparing all answer choices
                        lblStatus.Text = "Preparing all questions...";
                        Application.DoEvents();

                        // Initialize arrays to store answer choices and correct indices
                        allAnswerChoices = new List<string[]>(cardCount);
                        correctButtonIndices = new int[cardCount];

                        // Use the new PrepareAllAnswerChoices method instead of GenerateAllAnswerChoices
                        await Task.Run(() => PrepareAllAnswerChoices());

                        lblStatus.Text = "Ready to start...";
                        Application.DoEvents();

                        await Task.Delay(750); // Simulate some processing time to show the loading screen

                        DisplayCurrentCard(); // Display first card
                    }
                    else // No flashcards found in the set
                    {
                        // Hide loading panel if no cards
                        panelLoading.Visible = false;
                        ShowErrorMessage("No flashcards found in this set.", "Empty Set");
                        this.Close();
                    }
                }
                else // Error loading flashcard set
                {
                    // Hide loading panel on error
                    panelLoading.Visible = false;
                    ShowErrorMessage($"Error loading flashcard set '{setTitle}'.", "Load Error");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // Hide loading panel on error
                panelLoading.Visible = false;
                ShowErrorMessage($"Error: {ex.Message}", "Error");
                this.Close();
            }
        }

        // Prepare all answer choices using pre-generated wrong answers when available
        private void PrepareAllAnswerChoices()
        {
            if (questions == null || answers == null || cardCount == 0) //Check for valid data
            {
                return; // Exit if data is invalid
            }

            for (int cardIndex = 0; cardIndex < cardCount; cardIndex++) //Loop through each card
            {
                string correctAnswer = answers[cardIndex]; // Get the correct answer for the current card
                List<string> currentWrongAnswers = new List<string>();

                // Check if we have pre-generated wrong answers for this card
                if (wrongAnswers != null && cardIndex < wrongAnswers.Length &&
                    wrongAnswers[cardIndex] != null && wrongAnswers[cardIndex].Length > 0)
                {
                    // Use the pre-generated wrong answers
                    currentWrongAnswers.AddRange(wrongAnswers[cardIndex]);
                }
                else
                {
                    // Fallback: Generate 3 wrong answers from other answers in the set
                    for (int i = 0; i < cardCount && currentWrongAnswers.Count < 3; i++)
                    {
                        if (i != cardIndex && answers[i] != correctAnswer &&
                            !currentWrongAnswers.Contains(answers[i])) // Avoid duplicates
                        {
                            currentWrongAnswers.Add(answers[i]); // Add the wrong answer
                        }
                    }

                    // If we still don't have enough wrong answers, create placeholder ones
                    while (currentWrongAnswers.Count < 3) // while wrong answer count less than 3 add placeholders if needed
                    {
                        string placeholderWrong = $"Wrong answer {currentWrongAnswers.Count + 1}"; // Placeholder text
                        currentWrongAnswers.Add(placeholderWrong); // Add the placeholder wrong answer
                    }
                }

                // Create the full list of answers including the correct one
                List<string> allAnswers = new List<string> { correctAnswer }; // Add the correct answer
                allAnswers.AddRange(currentWrongAnswers); // Add the wrong answers

                // Shuffle the answers
                for (int i = 0; i < allAnswers.Count; i++) // Loop through each answer
                {
                    int j = random.Next(i, allAnswers.Count); // Get a random index
                    string temp = allAnswers[i]; // Swap the answers
                    allAnswers[i] = allAnswers[j]; // Swap the answers
                    allAnswers[j] = temp; //Swap the answers
                }

                correctButtonIndices[cardIndex] = allAnswers.IndexOf(correctAnswer); // Store the index of the correct answer
                allAnswerChoices.Add(allAnswers.ToArray()); // Store the answer choices
            }
        }

        private void DisplayCurrentCard()
        {
            // Show loading panel while setting up the current card
            if (!panelLoading.Visible) // Check if the loading panel is not visible
            {
                panelLoading.Visible = true; // Show the loading panel
                lblStatus.Text = "Loading next question..."; // Update the status label
                panelQuestion.Visible = false; // Hide the question panel
                Application.DoEvents(); // Update the UI
            }

            lblProgress.Text = $"Card {currentCardIndex + 1} of {cardCount}"; // Update the progress label

            // Display the current question
            if (questions != null && currentCardIndex < questions.Length) // Check for valid data
            {
                lblQuestion.Text = questions[currentCardIndex]; // Set the question text
            }

            try
            {
                // Get the pre-generated answer choices for this card
                string[] currentAnswerChoices = allAnswerChoices[currentCardIndex];

                // Set the answer choices on buttons 
                btnAnswerA.Text = currentAnswerChoices[0];
                btnAnswerB.Text = currentAnswerChoices[1];
                btnAnswerC.Text = currentAnswerChoices[2];
                btnAnswerD.Text = currentAnswerChoices[3];

                Application.DoEvents(); // Update the UI
                System.Threading.Thread.Sleep(200); // Simulate a brief delay

                panelLoading.Visible = false; // Hide the loading panel
                panelQuestion.Visible = true; // Show the question
            }
            catch (Exception ex)
            {
                // Show error and continue
                Console.WriteLine($"Error displaying card: {ex.Message}");

                panelLoading.Visible = false;
                panelQuestion.Visible = true; // Show the question even if there's an error
            }
        }

        // Check the selected answer and show the result
        private async void CheckAnswer(int selectedButtonIndex)
        {
            if (answers == null || aiTips == null || currentCardIndex >= answers.Length) // Check for valid data
            {
                return; // Exit if data is invalid
            }

            string correctAnswer = answers[currentCardIndex]; // Get the correct answer for the current card

            // Check if the selected answer is correct
            bool isCorrect = selectedButtonIndex == correctButtonIndices[currentCardIndex]; // Check if the selected answer is correct

            if (isCorrect)// Correct answer
            {
                // Increment correct answers count
                correctAnswers++;

                // Show success message
                ShowInfoMessage("Correct!", "Result");
            }
            else // Incorrect answer
            {
                // Show AI tip for incorrect answer - reference the CORRECT answer
                string tip = string.IsNullOrEmpty(aiTips[currentCardIndex])
                    ? $"Remember that the correct answer is: {correctAnswer}"
                    : $"{aiTips[currentCardIndex]} The correct answer is: {correctAnswer}";

                ShowInfoMessage($"Incorrect.\nThe correct answer is: {correctAnswer}\n\nTip: {tip}", "Result");
            }

            // Move to the next card
            currentCardIndex++;

            // Check if we've reached the end
            if (currentCardIndex >= cardCount)
            {
                // Calculate score as percentage
                int score = (int)Math.Round((double)correctAnswers / cardCount * 100);

                // Update high score if needed
                for (int i = 0; i < FlashcardManager.SetCount; i++)
                {
                    if (FlashcardManager.SetTitles[i] == setTitle) //Find the set index
                    {
                        await FlashcardManager.UpdateHighScoreAsync(i, score); // Update the high score
                        break;
                    }
                }

                // Show results
                ShowInfoMessage($"Test Complete!\n\nCorrect Answers: {correctAnswers} of {cardCount}\nScore: {score}%",
                    "Test Results");

                // Close the form
                this.Close();
            }
            else
            {
                // Display the next card
                DisplayCurrentCard();
            }
        }

        // Add event handler for Answer A button
        private void btnAnswerA_Click(object sender, EventArgs e)
        {
            CheckAnswer(0);
        }

        // Add event handler for Answer B button
        private void btnAnswerB_Click(object sender, EventArgs e)
        {
            CheckAnswer(1);
        }

        // Add event handler for Answer C button
        private void btnAnswerC_Click(object sender, EventArgs e)
        {
            CheckAnswer(2);
        }

        // Add event handler for Answer D button
        private void btnAnswerD_Click(object sender, EventArgs e)
        {
            CheckAnswer(3);
        }
    }
}
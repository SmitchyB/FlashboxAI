using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace FlashcardAI
{
    public partial class CreateSetForm : FlashcardFormBase
    {
        // Arrays to store flashcard data
        private string[] questions = new string[100]; // First array type: string array for questions
        private string[] answers = new string[100];   // Second array type: string array for answers
        private string[] aiTips = new string[100];    // Third array for AI-generated memory tips
        private string[][] wrongAnswers = new string[100][]; // Array of wrong answer arrays (3 wrong answers per card)

        private int cardCount = 0; // Count of flashcards
        private bool aiPanelExpanded = false; // AI panel expanded state
        private bool isEditMode = false; // Edit mode flag
        private string editSetTitle = string.Empty; // Title of the set being edited

        // OpenAI Service
        private readonly OpenAIService openAIService;

        // File upload tracking
        private string uploadedFilePath = string.Empty;
        private string openAIFileId = string.Empty;
        private bool hasUploadedFile = false;

        // Constructor for creating a new set
        public CreateSetForm()
        {
            InitializeComponent();

            // Initialize OpenAI service
            openAIService = new OpenAIService();

            // Hide AI panel initially
            panelAI.Visible = false;
            this.Width = this.Width - panelAI.Width;

            // Set up DataGridView
            SetupDataGridView();
        }

        // Constructor for editing an existing set
        public CreateSetForm(string setTitle)
        {
            InitializeComponent();

            // Set edit mode
            isEditMode = true;
            editSetTitle = setTitle;

            // Initialize OpenAI service
            openAIService = new OpenAIService();

            // Hide AI panel initially
            panelAI.Visible = false;
            this.Width = this.Width - panelAI.Width;

            // Set up DataGridView
            SetupDataGridView();

            // Load the existing set data
            LoadExistingSet(setTitle);

            // Update the form title
            this.Text = "Edit Flashcard Set";
            lblTitle.Text = "Edit Flashcard Set";
        }

        // Load an existing set for editing
        private async void LoadExistingSet(string setTitle)
        {
            try
            {
                panelLoading.Visible = true; // Show loading panel
                lblLoadingStatus.Text = "Loading flashcard set..."; // Update loading status
                Application.DoEvents(); // Allow UI to update

                var result = await FlashcardManager.GetFlashcardSetAsync(setTitle); // Get the set data

                if (result.HasValue) // Check if data was loaded successfully
                {
                    var (loadedQuestions, loadedAnswers, loadedTips, loadedWrongAnswers) = result.Value; // Unpack the quad tuple

                    for (int i = 0; i < loadedQuestions.Length; i++) // Copy data to our arrays
                    {
                        questions[i] = loadedQuestions[i]; // Copy questions
                        answers[i] = loadedAnswers[i]; // Copy answers
                        // Copy AI tips if available
                        if (i < loadedTips.Length)
                        {
                            aiTips[i] = loadedTips[i];
                        }
                        // Copy wrong answers if available
                        if (i < loadedWrongAnswers.Length)
                        {
                            wrongAnswers[i] = loadedWrongAnswers[i];
                        }
                    }

                    txtSetTitle.Text = setTitle; // Set the title in the text box
                    cardCount = loadedQuestions.Length; // Update card count
                    UpdateDataGridView(); // Update the DataGridView
                }
                else // If data loading failed
                {
                    ShowErrorMessage($"Failed to load flashcard set '{setTitle}'.", "Load Error");
                    this.Close(); // Close the form
                }
                panelLoading.Visible = false; // Hide loading panel
            }
            catch (Exception ex) // Catch any exceptions
            {
                panelLoading.Visible = false; // Hide loading panel

                // Show error message
                ShowErrorMessage($"Error loading flashcard set: {ex.Message}", "Load Error");
                this.Close(); // Close the form
            }
        }

        private void SetupDataGridView()
        {
            // Create columns for the DataGridView
            dgvFlashcards.ColumnCount = 2; // Two columns: Question and Answer
            dgvFlashcards.Columns[0].Name = "Question"; // Set column names
            dgvFlashcards.Columns[1].Name = "Answer"; // Set column names

            // Set column properties
            dgvFlashcards.Columns[0].Width = 355; // Set column width
            dgvFlashcards.Columns[1].Width = 355; // Set column width

            dgvFlashcards.AllowUserToAddRows = true; // Enable row addition

            dgvFlashcards.AllowUserToDeleteRows = true; // Enable row deletion

            dgvFlashcards.CellValueChanged += DgvFlashcards_CellValueChanged; // Add event handler for cell value changes
            dgvFlashcards.UserAddedRow += DgvFlashcards_UserAddedRow; // Add event handler for user-added row
            dgvFlashcards.UserDeletedRow += DgvFlashcards_UserDeletedRow; // Add event handler for user-deleted row

            // Add context menu for right-click deletion
            ContextMenuStrip contextMenu = new ContextMenuStrip(); // Create context menu
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Delete Card"); // Create delete item
            deleteItem.Click += DeleteCard_Click; // Add click event
            contextMenu.Items.Add(deleteItem); // Add delete item to context menu
            dgvFlashcards.ContextMenuStrip = contextMenu; // Set context menu for DataGridView
            dgvFlashcards.KeyDown += DgvFlashcards_KeyDown; // Add key down event
        }

        // Handle key down event for DataGridView
        private void DgvFlashcards_KeyDown(object? sender, KeyEventArgs e) // Key down event handler
        {
            if (e.KeyCode == Keys.Delete && dgvFlashcards.SelectedRows.Count > 0) // Check if delete key is pressed
            {
                // Get the selected row
                int rowIndex = dgvFlashcards.SelectedRows[0].Index; // Get the selected row index

                // Confirm deletion
                DialogResult result = ShowConfirmDialog(
                    "Are you sure you want to delete this card?",
                    "Confirm Delete"
                );

                if (result == DialogResult.Yes) //Check if user confirms deletion
                {
                    // Delete the row if not the new row template
                    if (rowIndex < dgvFlashcards.Rows.Count - 1)
                    {
                        DeleteCardAtIndex(rowIndex); // Delete the card at the index
                    }
                }

                // Mark the event as handled
                e.Handled = true;
            }
        }

        // Handle delete card context menu click event        
        private void DeleteCard_Click(object? sender, EventArgs e)
        {
            // Check if a row is selected
            if (dgvFlashcards.SelectedRows.Count > 0)
            {
                // Get the selected row
                int rowIndex = dgvFlashcards.SelectedRows[0].Index;

                // Confirm deletion
                DialogResult result = ShowConfirmDialog(
                    "Are you sure you want to delete this card?",
                    "Confirm Delete"
                );

                if (result == DialogResult.Yes) //Check if user confirms deletion
                {
                    // Delete the row if not the new row template
                    if (rowIndex < dgvFlashcards.Rows.Count - 1)
                    {
                        DeleteCardAtIndex(rowIndex);
                    }
                }
            }
        }

        // Delete card at index
        private void DeleteCardAtIndex(int rowIndex)
        {
            dgvFlashcards.Rows.RemoveAt(rowIndex); // Remove the row from the DataGridView

            for (int i = rowIndex; i < questions.Length - 1; i++) // Shift data in arrays
            {
                questions[i] = questions[i + 1]; // Shift questions
                answers[i] = answers[i + 1]; // Shift answers
                aiTips[i] = aiTips[i + 1]; // Shift AI tips
                wrongAnswers[i] = wrongAnswers[i + 1]; // Shift wrong answers
            }

            questions[questions.Length - 1] = string.Empty; // Clear the last question
            answers[answers.Length - 1] = string.Empty; // Clear the last answer
            aiTips[aiTips.Length - 1] = string.Empty; // Clear the last AI tip
            wrongAnswers[wrongAnswers.Length - 1] = Array.Empty<string>(); // Clear the last wrong answers set

            UpdateCardCount(); // Update the card count
        }

        // Handle user-delete row event
        private void DgvFlashcards_UserDeletedRow(object? sender, DataGridViewRowEventArgs e)
        {
            UpdateCardCount(); // Update card count when user deletes a row
        }

        // Handle user-added row event
        private void DgvFlashcards_UserAddedRow(object? sender, DataGridViewRowEventArgs e)
        {
            UpdateCardCount(); // Update card count when user adds a row
        }

        // Handle cell value changed event
        private void DgvFlashcards_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            // Store values in arrays when cell values change
            if (e.RowIndex >= 0 && e.RowIndex < dgvFlashcards.Rows.Count - 1)  // -1 to account for the new row template
            {
                if (e.ColumnIndex == 0) // Question column
                {
                    var value = dgvFlashcards.Rows[e.RowIndex].Cells[0].Value; // Get the cell value
                    questions[e.RowIndex] = value != null ? value.ToString() ?? string.Empty : string.Empty; // Store the question

                    // Clear wrong answers when question changes to trigger regeneration
                    wrongAnswers[e.RowIndex] = Array.Empty<string>();
                }
                else if (e.ColumnIndex == 1) // Answer column
                {
                    var value = dgvFlashcards.Rows[e.RowIndex].Cells[1].Value; // Get the cell value
                    answers[e.RowIndex] = value != null ? value.ToString() ?? string.Empty : string.Empty;  // Store the answer

                    // Clear wrong answers when answer changes to trigger regeneration
                    wrongAnswers[e.RowIndex] = Array.Empty<string>();
                }
                UpdateCardCount(); // Update the card count
            }
        }

        // Update card count
        private void UpdateCardCount()
        {
            int count = 0; // Initialize count
            foreach (DataGridViewRow row in dgvFlashcards.Rows) // Loop through rows
            {
                if (row.Index < dgvFlashcards.Rows.Count - 1) // Exclude template row
                {
                    var question = row.Cells[0].Value; // Get question value
                    var answer = row.Cells[1].Value; // Get answer value

                    if (!string.IsNullOrWhiteSpace(question?.ToString()) ||
                        !string.IsNullOrWhiteSpace(answer?.ToString())) // Check if question or answer is not empty
                    {
                        count++; // Increment count
                    }
                }
            }

            cardCount = count; // Update card count
            lblCardCount.Text = $"Card Count: {count}"; // Update card count label
        }

        // Handle add card button click event
        private void btnAddCard_Click(object sender, EventArgs e)
        {
            dgvFlashcards.Focus(); // Focus on the DataGridView

            // If the last row is not empty, the DataGridView will automatically add a new row
            // Otherwise, select the last row to help the user
            if (dgvFlashcards.Rows.Count > 0) // Check if there are rows
            {
                dgvFlashcards.CurrentCell = dgvFlashcards.Rows[dgvFlashcards.Rows.Count - 1].Cells[0]; // Select the last row
            }
        }

        // Handle delete card button click event
        private void btnDeleteCard_Click(object sender, EventArgs e)
        {
            if (dgvFlashcards.SelectedRows.Count > 0) // Check if a row is selected
            {
                int rowIndex = dgvFlashcards.SelectedRows[0].Index; // Get the selected row index

                // Confirm deletion
                DialogResult result = ShowConfirmDialog(
                    "Are you sure you want to delete this card?",
                    "Confirm Delete"
                );

                if (result == DialogResult.Yes) //Check if user confirms deletion
                {
                    if (rowIndex < dgvFlashcards.Rows.Count - 1) // Check if row is not the new row template
                    {
                        DeleteCardAtIndex(rowIndex); // Delete the card at the index
                    }
                }
            }
            else // If no row is selected
            {
                ShowInfoMessage("Please select a card to delete.", "No Card Selected");
            }
        }

        // Handle save button click event
        private async void btnSave_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtSetTitle.Text)) // Validate set title
            {
                ShowErrorMessage("Please enter a title for the flashcard set.", "Missing Title");
                txtSetTitle.Focus();
                return;
            }

            if (cardCount == 0)// Validate that there are cards
            {
                ShowErrorMessage("Please add at least one flashcard to the set.", "No Cards");
                return;
            }

            // Show loading panel
            panelLoading.Visible = true;
            lblLoadingStatus.Text = "Generating AI tips and test options...";
            Application.DoEvents();

            try
            {
                await GenerateAiTipsForMissingCards(); // Generate AI tips and wrong answers for cards that don't have them

                // Update loading status
                lblLoadingStatus.Text = "Saving flashcards...";
                Application.DoEvents();

                if (isEditMode && txtSetTitle.Text != editSetTitle)// If we're in edit mode, confirm if the title has changed
                {
                    var result = ShowConfirmDialog(
                        "You've changed the set title. Do you want to save this as a new set?",
                        "Title Changed");

                    if (result == DialogResult.Cancel) // If user cancels, hide loading panel and cancel save
                    {
                        // Hide loading panel and cancel save
                        panelLoading.Visible = false;
                        return;
                    }
                    else if (result == DialogResult.No) //If user doesnt want to save as new set
                    {
                        // Revert to original title
                        txtSetTitle.Text = editSetTitle;
                    }
                }

                // Save the flashcard set to local storage - include wrong answers
                bool success = await FlashcardManager.SaveFlashcardSetAsync(txtSetTitle.Text, questions, answers, aiTips, wrongAnswers, cardCount); // Save the flashcard set

                // Hide loading panel
                panelLoading.Visible = false;

                if (success) // Check if save was successful
                {
                    // Show success message
                    string message = isEditMode ?
                        $"Flashcard set '{txtSetTitle.Text}' updated successfully with {cardCount} cards." :
                        $"Flashcard set '{txtSetTitle.Text}' saved successfully with {cardCount} cards.";

                    ShowInfoMessage(message, "Set Saved");

                    // Return to main form
                    this.Close();
                }
                else //If save failed
                {
                    // Show error message
                    ShowErrorMessage("Failed to save flashcard set. Please try again.", "Save Error");
                }
            }
            catch (Exception ex) //Catch any exceptions
            {
                // Hide loading panel
                panelLoading.Visible = false;

                // Show error message
                ShowErrorMessage($"Error saving flashcard set: {ex.Message}", "Save Error");
            }
        }

        // Generate AI tips and wrong answers for cards that don't have them
        private async Task GenerateAiTipsForMissingCards()
        {
            try
            {
                // Generate AI tips and wrong answers for cards that don't have them
                for (int i = 0; i < cardCount; i++)
                {
                    // Check if question and answer are present
                    if (!string.IsNullOrEmpty(questions[i]) && !string.IsNullOrEmpty(answers[i])) // Check if question and answer are present
                    {
                        // Generate AI tip if missing
                        if (string.IsNullOrEmpty(aiTips[i])) // Check if AI tip is missing
                        {
                            lblLoadingStatus.Text = $"Generating tip for card {i + 1} of {cardCount}..."; // Update loading status
                            Application.DoEvents(); // Allow UI to update

                            aiTips[i] = await openAIService.GenerateMemoryTip(questions[i], answers[i]); // Generate AI tip
                            await Task.Delay(300); // Small delay to avoid API rate limits
                        }

                        // Generate wrong answers if missing
                        if (wrongAnswers[i] == null || wrongAnswers[i].Length == 0) // Check if wrong answers are missing
                        {
                            lblLoadingStatus.Text = $"Generating test options for card {i + 1} of {cardCount}..."; // Update loading status
                            Application.DoEvents(); // Allow UI to update

                            wrongAnswers[i] = await openAIService.GenerateWrongAnswers(questions[i], answers[i], 3); // Generate 3 wrong answers
                            await Task.Delay(300); // Small delay to avoid API rate limits
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // If API fails, use generic tips and dummy wrong answers
                for (int i = 0; i < cardCount; i++)// Loop through cards
                {
                    if (string.IsNullOrEmpty(aiTips[i])) // Check if AI tip is missing
                    {
                        aiTips[i] = "Remember this by associating the answer with something familiar."; // Set generic tip
                    }

                    if (wrongAnswers[i] == null || wrongAnswers[i].Length == 0) // Check if wrong answers are missing
                    {
                        wrongAnswers[i] = new string[3] { // Set dummy wrong answers
                            $"Wrong option 1 for {questions[i]}", // Set dummy wrong answers
                            $"Wrong option 2 for {questions[i]}", // Set dummy wrong answers
                            $"Wrong option 3 for {questions[i]}" // Set dummy wrong answers
                        };
                    }
                }

                Console.WriteLine($"Error generating AI content: {ex.Message}");
            }
        }

        // Handle cancel button click event
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Confirm before discarding changes
            DialogResult result = ShowConfirmDialog(
                "Are you sure you want to cancel? All unsaved changes will be lost.",
                "Confirm Cancel"
            );

            if (result == DialogResult.Yes) // Check if user confirms cancel
            {
                CleanupOpenAIFile(); // Clean up any uploaded file

                this.Close(); // Close the form
            }
        }

        // Delete any uploaded file from OpenAI when form is closed
        private async void CleanupOpenAIFile()
        {
            if (!string.IsNullOrEmpty(openAIFileId)) // Check if file ID is present
            {
                try
                {
                    await openAIService.DeleteFileAsync(openAIFileId); // Delete the file using the OpenAI service
                    openAIFileId = string.Empty; // Clear the file ID
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error cleaning up OpenAI file: {ex.Message}");
                }
            }
        }

        // Handle form closing event to clean up resources
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            CleanupOpenAIFile(); // Clean up any OpenAI file if it was uploaded

            base.OnFormClosing(e); // Call the base method
        }

        // Handle AI panel toggle button click event
        private void btnToggleAI_Click(object sender, EventArgs e)
        {
            aiPanelExpanded = !aiPanelExpanded; // Toggle the AI panel

            if (aiPanelExpanded) // Check if AI panel is expanded
            {
                panelAI.Visible = true; // Show the AI panel
                this.Width = this.Width + panelAI.Width; // Adjust the form width
                btnToggleAI.Text = "Hide AI >>"; // Update the button text

                // Show usage instructions the first time the AI panel is opened
                ShowAIInstructions();
            }
            else // If AI panel is hidden
            {
                panelAI.Visible = false; // Hide the AI panel
                this.Width = this.Width - panelAI.Width; // Adjust the form width
                btnToggleAI.Text = "<< AI Generate"; // Update the button text
            }
        }

        // Show AI instructions popup
        private void ShowAIInstructions()
        {
            // Show instructions in a message box
            ShowInfoMessage(
                "How to Generate Flashcards with AI:\n\n" +
                "1. TEXT ONLY: Enter a topic or paste text in the text area\n\n" +
                "2. FILE ONLY: Upload a file (PDF, Word, Image, Text) using the 'Upload File' button\n\n" +
                "3. COMBINED: Upload a file AND add specific instructions in the text area\n\n" +
                "4. Set the number of cards you want to generate\n\n" +
                "5. Click 'Generate Flashcards with AI'\n\n" +
                "The AI will create flashcards based on your input.",
                "AI Flashcard Generation Instructions"
            );
        }

        // Handle AI generate button click event
        private async void btnGenerateAI_Click(object sender, EventArgs e)
        {
            // Validate input - either text or file must be provided
            if (string.IsNullOrWhiteSpace(txtAIPrompt.Text) && !hasUploadedFile) // Check if text is empty and no file is uploaded
            {
                ShowErrorMessage("Please either enter a topic/text or upload a file to generate flashcards from.",
                    "Missing Input");
                txtAIPrompt.Focus(); // Focus on the text input
                return;
            }

            int cardsToGenerate = (int)numCardCount.Value; // Get the card count
            if (cardsToGenerate <= 0) //Validate card count
            {
                ShowErrorMessage("Please enter a valid number of cards to generate.",
                    "Invalid Card Count");
                numCardCount.Focus(); // Focus on the card count input
                return;
            }

            panelLoading.Visible = true; // Show loading panel
            lblLoadingStatus.Text = "Generating flashcards..."; // Update loading status
            Application.DoEvents(); // Allow UI to update

            try
            {
                if (hasUploadedFile)
                {
                    // If a file is uploaded, use the file-based generation
                    await GenerateFlashcardsWithFileAndAI(uploadedFilePath, txtAIPrompt.Text, cardsToGenerate);
                }
                else
                {
                    // Use text-only generation
                    await GenerateFlashcardsWithAI(txtAIPrompt.Text, cardsToGenerate);
                }

                UpdateDataGridView(); // Update the UI

                // Set the title if it's empty
                if (string.IsNullOrWhiteSpace(txtSetTitle.Text))// Check if title is empty
                {
                    string titleText = hasUploadedFile && !string.IsNullOrEmpty(uploadedFilePath) ? // Check if file is uploaded
                        Path.GetFileNameWithoutExtension(uploadedFilePath) : // Use file name
                        txtAIPrompt.Text; // Use file name or text prompt

                    txtSetTitle.Text = $"{titleText.Substring(0, Math.Min(30, titleText.Length))} Flashcards"; // Set the title
                }

                panelLoading.Visible = false; // Hide loading panel

                // Show success and warning message
                ShowInfoMessage(
                    $"Successfully generated {cardCount} flashcards.\n\n" +
                    "IMPORTANT: AI-generated content may contain errors or inaccuracies. " +
                    "Please review and verify the content of each card before using for study.",
                    "Generation Complete - Please Verify Cards");
            }
            catch (Exception ex)
            {
                panelLoading.Visible = false; // Hide loading panel

                // Show error message
                ShowErrorMessage($"Error generating flashcards: {ex.Message}", "Generation Error");
            }
        }

        // Generate flashcards using AI with text prompt only
        private async Task GenerateFlashcardsWithAI(string prompt, int numberOfCards)
        {
            try
            {
                var result = await openAIService.GenerateFlashcards(prompt, numberOfCards); // Generate flashcards using our OpenAI service

                if (isEditMode) // Clear existing data or keep them in edit mode
                {
                    // Ask if user wants to replace or append
                    DialogResult dialogResult = ShowConfirmDialog(
                        "Do you want to append these new cards to your existing set?",
                        "Append or Replace"
                    );

                    if (dialogResult == DialogResult.Yes) // Check if user wants to append
                    {
                        // Append - find the first empty slot
                        int startIndex = 0;
                        for (int i = 0; i < questions.Length; i++) // Find the first empty slot
                        {
                            if (string.IsNullOrEmpty(questions[i])) //Check if question is empty
                            {
                                startIndex = i; // Set the start index
                                break; // Break the loop
                            }
                        }

                        // Store the generated flashcards starting at the first empty slot
                        string[] generatedQuestions = result.Questions;
                        string[] generatedAnswers = result.Answers;

                        for (int i = 0; i < Math.Min(generatedQuestions.Length, questions.Length - startIndex); i++) // Copy data to our class arrays
                        {
                            questions[startIndex + i] = generatedQuestions[i]; // Copy questions
                            answers[startIndex + i] = generatedAnswers[i]; // Copy answers
                            aiTips[startIndex + i] = string.Empty; // Clear any existing tips for generated cards
                            wrongAnswers[startIndex + i] = Array.Empty<string>(); // Initialize wrong answers as null for new cards
                        }

                        this.cardCount = Math.Min(startIndex + generatedQuestions.Length, questions.Length); // Update the card count
                    }
                    else // If user wants to replace
                    {
                        // Replace all cards
                        Array.Clear(questions, 0, questions.Length); // Clear questions
                        Array.Clear(answers, 0, answers.Length); // Clear answers
                        Array.Clear(aiTips, 0, aiTips.Length); // Clear AI tips

                        // Initialize wrong answers to null
                        for (int i = 0; i < wrongAnswers.Length; i++) // Loop through wrong answers
                        {
                            wrongAnswers[i] = Array.Empty<string>(); // Initialize wrong answers as null
                        }

                        // Store the generated flashcards
                        string[] generatedQuestions = result.Questions;
                        string[] generatedAnswers = result.Answers;

                        for (int i = 0; i < Math.Min(generatedQuestions.Length, questions.Length); i++) // Copy data to our class arrays
                        {
                            questions[i] = generatedQuestions[i]; // Copy questions
                            answers[i] = generatedAnswers[i]; // Copy answers
                        }

                        this.cardCount = Math.Min(generatedQuestions.Length, questions.Length); // Update the card count
                    }
                }
                else // If not in edit mode
                {
                    Array.Clear(questions, 0, questions.Length); // Clear questions
                    Array.Clear(answers, 0, answers.Length); // Clear answers
                    Array.Clear(aiTips, 0, aiTips.Length); // Clear AI tips

                    // Initialize wrong answers to null
                    for (int i = 0; i < wrongAnswers.Length; i++)
                    {
                        wrongAnswers[i] = Array.Empty<string>();
                    }

                    // Store the generated flashcards
                    string[] generatedQuestions = result.Questions;
                    string[] generatedAnswers = result.Answers;

                    for (int i = 0; i < Math.Min(generatedQuestions.Length, questions.Length); i++) // Copy data to our class arrays
                    {
                        questions[i] = generatedQuestions[i]; // Copy questions
                        answers[i] = generatedAnswers[i]; // Copy answers
                    }

                    this.cardCount = Math.Min(generatedQuestions.Length, questions.Length);// Update the card count
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating flashcards: {ex.Message}"); // Log the error

                GenerateDummyData(prompt, numberOfCards); // Generate dummy data for testing if API fails

                throw; // Rethrow the exception for the caller to handle
            }
        }

        // Generate flashcards using AI with file and optional text prompt
        private async Task GenerateFlashcardsWithFileAndAI(string filePath, string additionalPrompt, int numberOfCards)
        {
            try
            {
                // Update loading status
                lblLoadingStatus.Text = "Processing file...";
                Application.DoEvents();

                var result = await openAIService.GenerateFlashcardsWithFile(filePath, additionalPrompt, numberOfCards); // Generate flashcards directly with the file

                if (isEditMode) // Clear existing data or keep them in edit mode
                {
                    // Ask if user wants to replace or append
                    DialogResult dialogResult = ShowConfirmDialog(
                        "Do you want to append these new cards to your existing set?",
                        "Append or Replace"
                    );

                    if (dialogResult == DialogResult.Yes) // Check if user wants to append
                    {
                        // Append - find the first empty slot
                        int startIndex = 0;
                        for (int i = 0; i < questions.Length; i++) // Find the first empty slot
                        {
                            if (string.IsNullOrEmpty(questions[i])) // Check if question is empty
                            {
                                startIndex = i; // Set the start index
                                break; // Break the loop
                            }
                        }

                        // Store the generated flashcards starting at the first empty slot
                        string[] generatedQuestions = result.Questions;
                        string[] generatedAnswers = result.Answers;

                        for (int i = 0; i < Math.Min(generatedQuestions.Length, questions.Length - startIndex); i++) // Copy data to our class arrays
                        {
                            questions[startIndex + i] = generatedQuestions[i]; // Copy questions
                            answers[startIndex + i] = generatedAnswers[i]; // Copy answers
                            aiTips[startIndex + i] = string.Empty; // Clear any existing tips for generated cards
                            wrongAnswers[startIndex + i] = Array.Empty<string>(); // Initialize wrong answers as null for new cards
                        }

                        this.cardCount = Math.Min(startIndex + generatedQuestions.Length, questions.Length); // Update the card count
                    }
                    else // If user wants to replace
                    {
                        // Replace all cards
                        Array.Clear(questions, 0, questions.Length); // Clear questions
                        Array.Clear(answers, 0, answers.Length); // Clear answers
                        Array.Clear(aiTips, 0, aiTips.Length); // Clear AI tips

                        // Initialize wrong answers to null
                        for (int i = 0; i < wrongAnswers.Length; i++) // Loop through wrong answers
                        {
                            wrongAnswers[i] = Array.Empty<string>(); // Initialize wrong answers as null
                        }

                        // Store the generated flashcards
                        string[] generatedQuestions = result.Questions; // Get generated questions
                        string[] generatedAnswers = result.Answers; // Get generated answers

                        for (int i = 0; i < Math.Min(generatedQuestions.Length, questions.Length); i++) // Copy data to our class arrays
                        {
                            questions[i] = generatedQuestions[i]; // Copy questions
                            answers[i] = generatedAnswers[i]; // Copy answers
                        }

                        this.cardCount = Math.Min(generatedQuestions.Length, questions.Length); // Update the card count
                    }
                }
                else // If not in edit mode
                {
                    Array.Clear(questions, 0, questions.Length); // Clear questions
                    Array.Clear(answers, 0, answers.Length); // Clear answers
                    Array.Clear(aiTips, 0, aiTips.Length); // Clear AI tips

                    // Initialize wrong answers to null
                    for (int i = 0; i < wrongAnswers.Length; i++) // Loop through wrong answers
                    {
                        wrongAnswers[i] = Array.Empty<string>(); // Initialize wrong answers as null    
                    }

                    // Store the generated flashcards
                    string[] generatedQuestions = result.Questions; // Get generated questions
                    string[] generatedAnswers = result.Answers; // Get generated answers

                    for (int i = 0; i < Math.Min(generatedQuestions.Length, questions.Length); i++) // Copy data to our class arrays
                    {
                        questions[i] = generatedQuestions[i]; // Copy questions
                        answers[i] = generatedAnswers[i]; // Copy answers
                    }

                    this.cardCount = Math.Min(generatedQuestions.Length, questions.Length); // Update the card count
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating flashcards from file: {ex.Message}"); // Log the error
                GenerateDummyData(Path.GetFileNameWithoutExtension(filePath), numberOfCards); // Generate dummy data for testing if API fails
                throw;
            }
        }

        // Fallback method to generate dummy data if API fails
        private void GenerateDummyData(string topic, int count) // Generate dummy data
        {
            int startIndex = 0; // Start index for appending
            bool append = false; // Append flag

            if (isEditMode && cardCount > 0) // Check if in edit mode and cards exist
            {
                // Ask if user wants to append or replace
                DialogResult dialogResult = ShowConfirmDialog(
                    "Do you want to append these new cards to your existing set?",
                    "Append or Replace"
                );

                if (dialogResult == DialogResult.Yes) //Check if user wants to append
                {
                    // Append - find the first empty slot
                    append = true; // Set append flag
                    for (int i = 0; i < questions.Length; i++) //Find the first empty slot
                    {
                        if (string.IsNullOrEmpty(questions[i])) // Check if question is empty
                        {
                            startIndex = i; // Set the start index
                            break; // Break the loop
                        }
                    }
                }
            }

            if (!append) // If not appending
            {
                // Clear existing data
                Array.Clear(questions, 0, questions.Length); // Clear questions
                Array.Clear(answers, 0, answers.Length); // Clear answers
                Array.Clear(aiTips, 0, aiTips.Length); // Clear AI tips

                // Initialize wrong answers to null
                for (int i = 0; i < wrongAnswers.Length; i++) // Loop through wrong answers
                {
                    wrongAnswers[i] = Array.Empty<string>(); // Initialize wrong answers as null
                }
            }

            // Generate dummy flashcards
            for (int i = 0; i < Math.Min(count, questions.Length - startIndex); i++) // Loop through count
            {
                questions[startIndex + i] = $"Question {i + 1} about {topic}?"; // Generate question
                answers[startIndex + i] = $"Answer {i + 1} about {topic}."; // Generate answer
                // Wrong answers will be generated during save
            }

            this.cardCount = append ? Math.Max(startIndex + count, cardCount) : count; // Update card count
        }

        // Update DataGridView with generated flashcards
        private void UpdateDataGridView()
        {
            dgvFlashcards.Rows.Clear(); // Clear existing rows

            // Add generated flashcards to the DataGridView
            for (int i = 0; i < cardCount; i++) // Loop through card count
            {
                if (!string.IsNullOrEmpty(questions[i]) && !string.IsNullOrEmpty(answers[i])) // Check if question and answer are not empty
                {
                    dgvFlashcards.Rows.Add(questions[i], answers[i]); // Add question and answer to the DataGridView
                }
            }

            lblCardCount.Text = $"Card Count: {cardCount}"; // Update card count label
        }

        // Handle upload file button click event
        private void btnUploadFile_Click(object sender, EventArgs e)
        {
            // Show file dialog with expanded file types
            OpenFileDialog openFileDialog = new OpenFileDialog // Create file dialog
            {
                Filter = "Document Files|*.txt;*.pdf;*.doc;*.docx|Image Files|*.jpg;*.jpeg;*.png|All Files (*.*)|*.*", // Set file types
                Title = "Select a File to Generate Flashcards From" // Set dialog title
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK) // Check if file was selected
            {
                // Show loading panel
                panelLoading.Visible = true; // Show loading panel
                lblLoadingStatus.Text = "Processing file..."; // Update loading status
                Application.DoEvents(); // Allow UI to update

                try
                {
                    string fileExtension = Path.GetExtension(openFileDialog.FileName).ToLower(); // Get file extension
                    string fileName = Path.GetFileName(openFileDialog.FileName); // Get file name

                    uploadedFilePath = openFileDialog.FileName; // Store file path
                    hasUploadedFile = true; // Set file flag

                    UpdateFileLabel(fileName, fileExtension); // Update the UI with file information

                    panelLoading.Visible = false; // Hide loading panel

                    // Show success message
                    ShowInfoMessage(
                        $"File '{fileName}' loaded successfully. Add any additional instructions in the text area if needed, then click Generate to create flashcards.",
                        "File Loaded");
                }
                catch (Exception ex)
                {
                    panelLoading.Visible = false; // Hide loading panel

                    // Reset file tracking
                    uploadedFilePath = string.Empty;
                    hasUploadedFile = false;

                    // Show error message
                    ShowErrorMessage($"Error processing file: {ex.Message}", "File Error");
                }
            }
        }

        // Update the UI to show the uploaded file information
        private void UpdateFileLabel(string fileName, string fileExtension)
        {
            // Update the text area with file information
            lblUploadedFile.Visible = true; // Show file label
            lblUploadedFile.Text = $"Uploaded File: {fileName}"; // Update file label
            lblAIPrompt.Text = "Additional Context (Optional):"; // Update prompt label

            // Add a hint to the text area if it's empty
            if (string.IsNullOrWhiteSpace(txtAIPrompt.Text))
            {
                txtAIPrompt.Text = "Add any additional instructions or context for the AI here..."; // Add sample text
            }
        }

        // Reset file upload tracking when user wants to clear
        private void btnClearFile_Click(object sender, EventArgs e)
        {
            // Reset file tracking
            uploadedFilePath = string.Empty; // Clear file path
            hasUploadedFile = false; // Reset file flag

            // Reset UI
            lblUploadedFile.Visible = false; // Hide file label
            lblAIPrompt.Text = "Text or Topic:"; // Reset prompt label

            // Clear any sample text
            if (txtAIPrompt.Text == "Add any additional instructions or context for the AI here...") // Check if text area has sample text
            {
                txtAIPrompt.Text = string.Empty; // Clear text area
            }

            ShowInfoMessage("Uploaded file has been cleared. You can now enter text or upload a different file.",
                "File Cleared");
        }

        // Read text from a file asynchronously
        private static async Task<string> ReadFileAsync(string filePath)
        {
            try
            {
                return await File.ReadAllTextAsync(filePath); // Read text file
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                throw;
            }
        }

    }
}
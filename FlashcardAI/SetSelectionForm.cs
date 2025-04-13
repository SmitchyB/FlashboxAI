using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace FlashcardAI
{
    // Define an enum for the different modes
    public enum SelectionMode
    {
        Edit,
        Test
    }

    public partial class SetSelectionForm : FlashcardFormBase
    {
        private SelectionMode currentMode; // Store the current mode

        public SetSelectionForm(SelectionMode mode)
        {
            InitializeComponent();

            // Set the current mode
            currentMode = mode;

            // Configure UI based on mode
            ConfigureUIForMode();

            // Load flashcard sets
            LoadFlashcardSets();
        }

        // Configure UI elements based on the selected mode
        private void ConfigureUIForMode()
        {
            switch (currentMode)
            {
                case SelectionMode.Edit:
                    this.Text = "Flash Box - Edit Set";
                    lblTitle.Text = "Edit Set";
                    lblSelectSet.Text = "Select a set to edit:";
                    btnAction.Text = "Edit Selected Set";
                    break;

                case SelectionMode.Test:
                    this.Text = "Flash Box - Test Selection";
                    lblTitle.Text = "Test Mode";
                    lblSelectSet.Text = "Select a set to test from:";
                    btnAction.Text = "Start Test";
                    break;
            }
        }

        // Load flashcard sets (combined from both original forms)
        private void LoadFlashcardSets()
        {
            var sets = FlashcardManager.GetAllSets(); // Get all flashcard sets

            if (sets.Count == 0) // If no sets found
            {
                ShowInfoMessage("No flashcard sets found. Please create some sets first.", "No Sets");
                this.Close();
                return;
            }

            lstSets.Items.Clear(); // Clear the list box

            // Add each set to the list box
            foreach (var set in sets) // Loop through each set
            {
                lstSets.Items.Add($"{set.Title} ({set.HighScore}%)"); // Add set title and high score
            }

            if (lstSets.Items.Count > 0) // If there are items in the list
            {
                lstSets.SelectedIndex = 0; // Select the first item
            }
        }

        // Handle action button click
        private void btnAction_Click(object sender, EventArgs e)
        {
            if (lstSets.SelectedIndex == -1) // If no set selected
            {
                ShowErrorMessage($"Please select a flashcard set to {(currentMode == SelectionMode.Edit ? "edit" : "test")}.",
                    "No Set Selected");
                return;
            }

            string selectedTitle = FlashcardManager.SetTitles[lstSets.SelectedIndex]; // Get the selected set title

            // Perform action based on the current mode
            switch (currentMode)
            {
                case SelectionMode.Edit:
                    // Open the create form in edit mode with the selected set
                    using (CreateSetForm editForm = new CreateSetForm(selectedTitle))
                    {
                        editForm.ShowDialog();
                    }
                    break;

                case SelectionMode.Test:
                    // Open the test form with the selected set
                    using (TestForm testForm = new TestForm(selectedTitle))
                    {
                        testForm.ShowDialog();
                    }
                    break;
            }

            // Refresh the list after action
            LoadFlashcardSets();
        }

        // Cancel button click event
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Close the form
            this.Close();
        }
    }
}
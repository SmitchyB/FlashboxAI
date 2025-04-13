using System;
using System.Windows.Forms;

namespace FlashcardAI
{
    public partial class MainForm : FlashcardFormBase
    {
        public MainForm()
        {
            InitializeComponent();
            // Base class will handle logo loading and UI styling
        }

        /// Handles Create button click event
        private void btnCreate_Click(object sender, EventArgs e)
        {
            // Open the Create Flashcard Set form
            using (CreateSetForm createForm = new CreateSetForm())
            {
                createForm.ShowDialog();
            }
        }

        /// Handles Test button click event
        private void btnTest_Click(object sender, EventArgs e)
        {
            // Open the combined Set Selection form in Test mode
            using (SetSelectionForm testSelectionForm = new SetSelectionForm(SelectionMode.Test))
            {
                testSelectionForm.ShowDialog();
            }
        }

        /// Handles Edit button click event
        private void btnEdit_Click(object sender, EventArgs e)
        {
            // Open the combined Set Selection form in Edit mode
            using (SetSelectionForm editSelectionForm = new SetSelectionForm(SelectionMode.Edit))
            {
                editSelectionForm.ShowDialog();
            }
        }

        /// Handles Quit button click event
        private void btnQuit_Click(object sender, EventArgs e)
        {
            // Close the application
            Application.Exit();
            
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FlashcardAI
{
    /// <summary>
    /// Base class for all flashcard application forms.
    /// Provides common styling and initialization.
    /// </summary>
    public class FlashcardFormBase : Form
    {
        // Form initialization
        public FlashcardFormBase()
        {
            // Set basic form properties
            this.BackColor = UIUtilities.AppColors.Background;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        /// <summary>
        /// Override OnLoad to apply common styling to all controls
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitializeCommonUI();
        }

        /// <summary>
        /// Initializes common UI elements and applies styling
        /// </summary>
        private void InitializeCommonUI()
        {
            // Apply styles to all controls recursively
            ApplyStylesToControls(this.Controls);

            // Look for loading panels and set their properties
            SetupLoadingPanels();
        }

        /// <summary>
        /// Recursively applies styles to all controls based on their type
        /// </summary>
        private void ApplyStylesToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                // Apply specific styling based on control type
                if (control is PictureBox pictureBox && pictureBox.Name.Contains("Logo"))
                {
                    UIUtilities.LoadLogo(pictureBox);
                }
                else if (control is Button button)
                {
                    ApplyStandardButtonStyle(button);
                }
                else if (control is Label label)
                {
                    ApplyStandardLabelStyle(label);
                }
                else if (control is DataGridView dataGridView)
                {
                    UIUtilities.ApplyDataGridViewStyling(dataGridView);
                }
                else if (control is TextBox textBox)
                {
                    ApplyStandardTextBoxStyle(textBox);
                }
                else if (control is ListBox listBox)
                {
                    ApplyStandardListBoxStyle(listBox);
                }
                else if (control is Panel panel)
                {
                    // Only style specific panels, not all of them
                    if (panel.Name.Contains("AI"))
                    {
                        panel.BackColor = UIUtilities.AppColors.PanelBackground;
                    }
                }

                // Recursively apply to child controls if any
                if (control.HasChildren)
                {
                    ApplyStylesToControls(control.Controls);
                }
            }
        }

        /// <summary>
        /// Applies standard style to buttons
        /// </summary>
        private void ApplyStandardButtonStyle(Button button)
        {
            // Skip action button styling for cancel, save or buttons that don't end with "button"
            if (button.Name.Contains("Cancel") || button.Name.Contains("Save") ||
                !button.Name.EndsWith("Button", StringComparison.OrdinalIgnoreCase))
            {
                // Style standard buttons (typically dark blue with light text)
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.BackColor = UIUtilities.AppColors.ButtonBackground;
                button.ForeColor = UIUtilities.AppColors.LightText;
            }
            else
            {
                // Style action buttons (typically brighter with dark text)
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.BackColor = UIUtilities.AppColors.ButtonHover;
                button.ForeColor = UIUtilities.AppColors.DarkText;
            }

            // Apply hover effects to all buttons
            UIUtilities.SetupButtonHoverEffects(
                button,
                button.BackColor,
                button.ForeColor,
                button.BackColor == UIUtilities.AppColors.ButtonHover ?
                    UIUtilities.AppColors.ButtonBackground : UIUtilities.AppColors.ButtonHover,
                button.ForeColor == UIUtilities.AppColors.DarkText ?
                    UIUtilities.AppColors.LightText : UIUtilities.AppColors.DarkText);
        }

        /// <summary>
        /// Applies standard style to labels
        /// </summary>
        private void ApplyStandardLabelStyle(Label label)
        {
            // Style title labels (typically larger and bold)
            if (label.Name.Contains("Title"))
            {
                label.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Bold);
                label.ForeColor = UIUtilities.AppColors.ButtonBackground; // Dark blue
            }
            // Style regular labels
            else
            {
                label.ForeColor = UIUtilities.AppColors.DarkText;
            }
        }

        /// <summary>
        /// Applies standard style to text boxes
        /// </summary>
        private void ApplyStandardTextBoxStyle(TextBox textBox)
        {
            textBox.BackColor = Color.FromArgb(180, 255, 230);
            textBox.ForeColor = UIUtilities.AppColors.DarkText;
        }

        /// <summary>
        /// Applies standard style to list boxes
        /// </summary>
        private void ApplyStandardListBoxStyle(ListBox listBox)
        {
            listBox.BackColor = Color.FromArgb(180, 255, 230);
            listBox.ForeColor = UIUtilities.AppColors.DarkText;
        }

        /// <summary>
        /// Sets up loading panels with consistent styling
        /// </summary>
        private void SetupLoadingPanels()
        {
            foreach (Control control in this.Controls)
            {
                if (control is Panel panel && panel.Name.Contains("Loading"))
                {
                    panel.BackColor = UIUtilities.AppColors.PanelBackground;
                    panel.Visible = false;

                    // Style the progress bar and labels inside
                    foreach (Control child in panel.Controls)
                    {
                        if (child is ProgressBar progressBar)
                        {
                            progressBar.Style = ProgressBarStyle.Marquee;
                            progressBar.MarqueeAnimationSpeed = 30;
                        }
                        else if (child is Label label)
                        {
                            label.ForeColor = UIUtilities.AppColors.LightText;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Shows a standard error message
        /// </summary>
        protected void ShowErrorMessage(string message, string title = "Error")
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows a standard information message
        /// </summary>
        protected void ShowInfoMessage(string message, string title = "Information")
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows a standard confirmation dialog and returns the result
        /// </summary>
        protected DialogResult ShowConfirmDialog(string message, string title = "Confirm")
        {
            return MessageBox.Show(
                message,
                title,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
        }
    }
}

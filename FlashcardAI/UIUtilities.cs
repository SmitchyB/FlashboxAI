using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FlashcardAI
{

    // Provides common UI utility methods for the application
    public static class UIUtilities
    {
        // Default path to application assets
        private static readonly string DefaultLogoPath = @"B:\VisualStudio2022Apps\FlashcardAI\FlashcardAI\Assets\FlashBox.png";

        // Loads the application logo into a PictureBox
        public static void LoadLogo(PictureBox pictureBox, string ?customPath = null)
        {
            if (pictureBox == null)
                return;

            try
            {
                string logoPath = customPath ?? DefaultLogoPath;

                if (File.Exists(logoPath))
                {
                    pictureBox.Image = Image.FromFile(logoPath);
                }
                else
                {
                    Console.WriteLine($"Logo file not found at: {logoPath}");
                    pictureBox.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading logo: {ex.Message}");
                pictureBox.Visible = false;
            }
        }

        //Sets up hover effects for a button with custom colors
        public static void SetupButtonHoverEffects(
            Button button,
            Color normalBackColor,
            Color normalForeColor,
            Color hoverBackColor,
            Color hoverForeColor)
        {
            if (button == null)
                return;

            button.MouseEnter += (s, e) =>
            {
                button.BackColor = hoverBackColor;
                button.ForeColor = hoverForeColor;
            };

            button.MouseLeave += (s, e) =>
            {
                button.BackColor = normalBackColor;
                button.ForeColor = normalForeColor;
            };
        }

        // Applies the application's standard styling to a DataGridView
        public static void ApplyDataGridViewStyling(DataGridView dataGridView)
        {
            if (dataGridView == null)
                return;

            // Set header styles
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 63, 71);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(240, 250, 250);
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);

            // Set cell styles
            dataGridView.DefaultCellStyle.BackColor = Color.FromArgb(180, 255, 230);
            dataGridView.DefaultCellStyle.ForeColor = Color.FromArgb(0, 40, 45);
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(48, 206, 209);
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.FromArgb(0, 40, 45);

            // Set grid styles
            dataGridView.GridColor = Color.FromArgb(0, 63, 71);
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.BackgroundColor = Color.FromArgb(9, 255, 199);
        }

        // Gets common application colors used across the UI
        public static class AppColors
        {
            // Main colors
            public static readonly Color Background = Color.FromArgb(9, 255, 199);
            public static readonly Color DarkText = Color.FromArgb(0, 40, 45);
            public static readonly Color LightText = Color.FromArgb(240, 250, 250);

            // Button colors
            public static readonly Color ButtonBackground = Color.FromArgb(0, 63, 71);
            public static readonly Color ButtonHover = Color.FromArgb(48, 206, 209);

            // Panel colors
            public static readonly Color PanelBackground = Color.FromArgb(20, 83, 92);
        }
    }
}
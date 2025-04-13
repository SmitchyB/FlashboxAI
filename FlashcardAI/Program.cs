using FlashcardAI;
using System;
using System.Windows.Forms;

namespace FlashcardAI
{
    static class Program
    {
        /// The main entry point for the application.
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles(); // Enable visual styles for the application
            Application.SetCompatibleTextRenderingDefault(false); // Set the application to use the default text rendering

            Application.Run(new MainForm()); // Run the main form
        }
    }
}
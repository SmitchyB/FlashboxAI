namespace FlashcardAI
{
    partial class CreateSetForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary> 
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateSetForm));
            lblTitle = new Label();
            txtSetTitle = new TextBox();
            lblSetTitle = new Label();
            dgvFlashcards = new DataGridView();
            btnAddCard = new Button();
            btnDeleteCard = new Button();
            btnSave = new Button();
            btnCancel = new Button();
            panelAI = new Panel();
            lblUploadedFile = new Label();
            btnClearFile = new Button();
            lblAIGeneration = new Label();
            txtAIPrompt = new TextBox();
            lblAIPrompt = new Label();
            btnGenerateAI = new Button();
            btnUploadFile = new Button();
            lblCardCountAI = new Label();
            numCardCount = new NumericUpDown();
            btnToggleAI = new Button();
            lblCardCount = new Label();
            panelLoading = new Panel();
            lblLoadingStatus = new Label();
            progressBar = new ProgressBar();
            pictureBoxLogo = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)dgvFlashcards).BeginInit();
            panelAI.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numCardCount).BeginInit();
            panelLoading.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).BeginInit();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.FromArgb(0, 63, 71);
            lblTitle.Location = new Point(14, 10);
            lblTitle.Margin = new Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(259, 29);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Create Flashcard Set";
            // 
            // txtSetTitle
            // 
            txtSetTitle.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtSetTitle.Location = new Point(113, 61);
            txtSetTitle.Margin = new Padding(4, 3, 4, 3);
            txtSetTitle.Name = "txtSetTitle";
            txtSetTitle.Size = new Size(535, 26);
            txtSetTitle.TabIndex = 1;
            txtSetTitle.BackColor = Color.FromArgb(180, 255, 230);
            txtSetTitle.ForeColor = Color.FromArgb(0, 40, 45);
            // 
            // lblSetTitle
            // 
            lblSetTitle.AutoSize = true;
            lblSetTitle.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSetTitle.ForeColor = Color.FromArgb(0, 63, 71);
            lblSetTitle.Location = new Point(18, 65);
            lblSetTitle.Margin = new Padding(4, 0, 4, 0);
            lblSetTitle.Name = "lblSetTitle";
            lblSetTitle.Size = new Size(71, 20);
            lblSetTitle.TabIndex = 2;
            lblSetTitle.Text = "Set Title:";
            // 
            // dgvFlashcards
            // 
            dgvFlashcards.BackgroundColor = Color.FromArgb(9, 255, 199);
            dgvFlashcards.BorderStyle = BorderStyle.None;
            dgvFlashcards.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvFlashcards.EnableHeadersVisualStyles = false;
            dgvFlashcards.GridColor = Color.FromArgb(0, 63, 71);
            dgvFlashcards.Location = new Point(20, 108);
            dgvFlashcards.Margin = new Padding(4, 3, 4, 3);
            dgvFlashcards.Name = "dgvFlashcards";
            dgvFlashcards.Size = new Size(758, 414);
            dgvFlashcards.TabIndex = 3;
            // 
            // btnAddCard
            // 
            btnAddCard.BackColor = Color.FromArgb(0, 63, 71);
            btnAddCard.FlatAppearance.BorderSize = 0;
            btnAddCard.FlatStyle = FlatStyle.Flat;
            btnAddCard.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnAddCard.ForeColor = Color.FromArgb(240, 250, 250);
            btnAddCard.Location = new Point(20, 530);
            btnAddCard.Margin = new Padding(4, 3, 4, 3);
            btnAddCard.Name = "btnAddCard";
            btnAddCard.Size = new Size(140, 40);
            btnAddCard.TabIndex = 4;
            btnAddCard.Text = "Add Card";
            btnAddCard.UseVisualStyleBackColor = false;
            btnAddCard.Click += btnAddCard_Click;
            // 
            // btnDeleteCard
            // 
            btnDeleteCard.BackColor = Color.FromArgb(0, 63, 71);
            btnDeleteCard.FlatAppearance.BorderSize = 0;
            btnDeleteCard.FlatStyle = FlatStyle.Flat;
            btnDeleteCard.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnDeleteCard.ForeColor = Color.FromArgb(240, 250, 250);
            btnDeleteCard.Location = new Point(168, 530);
            btnDeleteCard.Margin = new Padding(4, 3, 4, 3);
            btnDeleteCard.Name = "btnDeleteCard";
            btnDeleteCard.Size = new Size(140, 40);
            btnDeleteCard.TabIndex = 5;
            btnDeleteCard.Text = "Delete Card";
            btnDeleteCard.UseVisualStyleBackColor = false;
            btnDeleteCard.Click += btnDeleteCard_Click;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.FromArgb(0, 63, 71);
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnSave.ForeColor = Color.FromArgb(240, 250, 250);
            btnSave.Location = new Point(509, 530);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(140, 40);
            btnSave.TabIndex = 6;
            btnSave.Text = "Save Set";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(0, 63, 71);
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnCancel.ForeColor = Color.FromArgb(240, 250, 250);
            btnCancel.Location = new Point(656, 530);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(122, 40);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // panelAI
            // 
            panelAI.BackColor = Color.FromArgb(20, 83, 92);
            panelAI.BorderStyle = BorderStyle.FixedSingle;
            panelAI.Controls.Add(lblUploadedFile);
            panelAI.Controls.Add(btnClearFile);
            panelAI.Controls.Add(lblAIGeneration);
            panelAI.Controls.Add(txtAIPrompt);
            panelAI.Controls.Add(lblAIPrompt);
            panelAI.Controls.Add(btnGenerateAI);
            panelAI.Controls.Add(btnUploadFile);
            panelAI.Controls.Add(lblCardCountAI);
            panelAI.Controls.Add(numCardCount);
            panelAI.Dock = DockStyle.Right;
            panelAI.Location = new Point(786, 0);
            panelAI.Margin = new Padding(4, 3, 4, 3);
            panelAI.Name = "panelAI";
            panelAI.Size = new Size(350, 578);
            panelAI.TabIndex = 8;
            // 
            // lblUploadedFile
            // 
            lblUploadedFile.AutoSize = true;
            lblUploadedFile.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblUploadedFile.ForeColor = Color.FromArgb(240, 250, 250);
            lblUploadedFile.Location = new Point(19, 292);
            lblUploadedFile.Name = "lblUploadedFile";
            lblUploadedFile.Size = new Size(111, 15);
            lblUploadedFile.TabIndex = 7;
            lblUploadedFile.Text = "Uploaded File: ";
            lblUploadedFile.Visible = false;
            // 
            // btnClearFile
            // 
            btnClearFile.BackColor = Color.FromArgb(0, 63, 71);
            btnClearFile.FlatAppearance.BorderSize = 0;
            btnClearFile.FlatStyle = FlatStyle.Flat;
            btnClearFile.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnClearFile.ForeColor = Color.FromArgb(240, 250, 250);
            btnClearFile.Location = new Point(246, 290);
            btnClearFile.Name = "btnClearFile";
            btnClearFile.Size = new Size(85, 23);
            btnClearFile.TabIndex = 8;
            btnClearFile.Text = "Clear File";
            btnClearFile.UseVisualStyleBackColor = false;
            btnClearFile.Click += btnClearFile_Click;
            // 
            // lblAIGeneration
            // 
            lblAIGeneration.AutoSize = true;
            lblAIGeneration.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAIGeneration.ForeColor = Color.FromArgb(240, 250, 250);
            lblAIGeneration.Location = new Point(92, 23);
            lblAIGeneration.Margin = new Padding(4, 0, 4, 0);
            lblAIGeneration.Name = "lblAIGeneration";
            lblAIGeneration.Size = new Size(138, 24);
            lblAIGeneration.TabIndex = 0;
            lblAIGeneration.Text = "AI Generation";
            // 
            // txtAIPrompt
            // 
            txtAIPrompt.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtAIPrompt.Location = new Point(19, 95);
            txtAIPrompt.Margin = new Padding(4, 3, 4, 3);
            txtAIPrompt.Multiline = true;
            txtAIPrompt.Name = "txtAIPrompt";
            txtAIPrompt.Size = new Size(311, 185);
            txtAIPrompt.TabIndex = 1;
            txtAIPrompt.BackColor = Color.FromArgb(220, 255, 240);
            txtAIPrompt.ForeColor = Color.FromArgb(0, 40, 45);
            // 
            // lblAIPrompt
            // 
            lblAIPrompt.AutoSize = true;
            lblAIPrompt.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblAIPrompt.ForeColor = Color.FromArgb(240, 250, 250);
            lblAIPrompt.Location = new Point(15, 72);
            lblAIPrompt.Margin = new Padding(4, 0, 4, 0);
            lblAIPrompt.Name = "lblAIPrompt";
            lblAIPrompt.Size = new Size(95, 17);
            lblAIPrompt.TabIndex = 2;
            lblAIPrompt.Text = "Text or Topic:";
            // 
            // btnGenerateAI
            // 
            btnGenerateAI.BackColor = Color.FromArgb(48, 206, 209);
            btnGenerateAI.FlatAppearance.BorderSize = 0;
            btnGenerateAI.FlatStyle = FlatStyle.Flat;
            btnGenerateAI.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGenerateAI.ForeColor = Color.FromArgb(0, 40, 45);
            btnGenerateAI.Location = new Point(19, 402);
            btnGenerateAI.Margin = new Padding(4, 3, 4, 3);
            btnGenerateAI.Name = "btnGenerateAI";
            btnGenerateAI.Size = new Size(312, 52);
            btnGenerateAI.TabIndex = 3;
            btnGenerateAI.Text = "Generate Flashcards with AI";
            btnGenerateAI.UseVisualStyleBackColor = false;
            btnGenerateAI.Click += btnGenerateAI_Click;
            // 
            // btnUploadFile
            // 
            btnUploadFile.BackColor = Color.FromArgb(0, 63, 71);
            btnUploadFile.FlatAppearance.BorderSize = 0;
            btnUploadFile.FlatStyle = FlatStyle.Flat;
            btnUploadFile.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnUploadFile.ForeColor = Color.FromArgb(240, 250, 250);
            btnUploadFile.Location = new Point(19, 317);
            btnUploadFile.Margin = new Padding(4, 3, 4, 3);
            btnUploadFile.Name = "btnUploadFile";
            btnUploadFile.Size = new Size(312, 40);
            btnUploadFile.TabIndex = 4;
            btnUploadFile.Text = "Upload File (PDF, Word, Image, etc.)";
            btnUploadFile.UseVisualStyleBackColor = false;
            btnUploadFile.Click += btnUploadFile_Click;
            // 
            // lblCardCountAI
            // 
            lblCardCountAI.AutoSize = true;
            lblCardCountAI.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCardCountAI.ForeColor = Color.FromArgb(240, 250, 250);
            lblCardCountAI.Location = new Point(15, 367);
            lblCardCountAI.Margin = new Padding(4, 0, 4, 0);
            lblCardCountAI.Name = "lblCardCountAI";
            lblCardCountAI.Size = new Size(119, 17);
            lblCardCountAI.TabIndex = 5;
            lblCardCountAI.Text = "Number of Cards:";
            // 
            // numCardCount
            // 
            numCardCount.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            numCardCount.Location = new Point(175, 365);
            numCardCount.Margin = new Padding(4, 3, 4, 3);
            numCardCount.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            numCardCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numCardCount.Name = "numCardCount";
            numCardCount.Size = new Size(155, 23);
            numCardCount.TabIndex = 6;
            numCardCount.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // btnToggleAI
            // 
            btnToggleAI.BackColor = Color.FromArgb(48, 206, 209);
            btnToggleAI.FlatAppearance.BorderSize = 0;
            btnToggleAI.FlatStyle = FlatStyle.Flat;
            btnToggleAI.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnToggleAI.ForeColor = Color.FromArgb(0, 40, 45);
            btnToggleAI.Location = new Point(656, 61);
            btnToggleAI.Margin = new Padding(4, 3, 4, 3);
            btnToggleAI.Name = "btnToggleAI";
            btnToggleAI.Size = new Size(122, 30);
            btnToggleAI.TabIndex = 9;
            btnToggleAI.Text = "<< AI Generate";
            btnToggleAI.UseVisualStyleBackColor = false;
            btnToggleAI.Click += btnToggleAI_Click;
            // 
            // lblCardCount
            // 
            lblCardCount.AutoSize = true;
            lblCardCount.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCardCount.ForeColor = Color.FromArgb(0, 63, 71);
            lblCardCount.Location = new Point(357, 540);
            lblCardCount.Margin = new Padding(4, 0, 4, 0);
            lblCardCount.Name = "lblCardCount";
            lblCardCount.Size = new Size(95, 17);
            lblCardCount.TabIndex = 10;
            lblCardCount.Text = "Card Count: 0";
            // 
            // panelLoading
            // 
            panelLoading.BackColor = Color.FromArgb(20, 83, 92);
            panelLoading.BorderStyle = BorderStyle.FixedSingle;
            panelLoading.Controls.Add(lblLoadingStatus);
            panelLoading.Controls.Add(progressBar);
            panelLoading.Location = new Point(320, 254);
            panelLoading.Margin = new Padding(4, 3, 4, 3);
            panelLoading.Name = "panelLoading";
            panelLoading.Size = new Size(350, 115);
            panelLoading.TabIndex = 11;
            panelLoading.Visible = false;
            // 
            // lblLoadingStatus
            // 
            lblLoadingStatus.AutoSize = true;
            lblLoadingStatus.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblLoadingStatus.ForeColor = Color.FromArgb(240, 250, 250);
            lblLoadingStatus.Location = new Point(92, 17);
            lblLoadingStatus.Margin = new Padding(4, 0, 4, 0);
            lblLoadingStatus.Name = "lblLoadingStatus";
            lblLoadingStatus.Size = new Size(130, 17);
            lblLoadingStatus.TabIndex = 0;
            lblLoadingStatus.Text = "Generating cards...";
            // 
            // progressBar
            // 
            progressBar.Location = new Point(16, 51);
            progressBar.Margin = new Padding(4, 3, 4, 3);
            progressBar.MarqueeAnimationSpeed = 30;
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(316, 27);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.TabIndex = 1;
            // 
            // pictureBoxLogo
            // 
            pictureBoxLogo.Location = new Point(697, 10);
            pictureBoxLogo.Margin = new Padding(4, 3, 4, 3);
            pictureBoxLogo.Name = "pictureBoxLogo";
            pictureBoxLogo.Size = new Size(40, 40);
            pictureBoxLogo.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxLogo.TabIndex = 12;
            pictureBoxLogo.TabStop = false;
            // 
            // CreateSetForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(9, 255, 199);
            ClientSize = new Size(1136, 578);
            Controls.Add(pictureBoxLogo);
            Controls.Add(panelLoading);
            Controls.Add(lblCardCount);
            Controls.Add(btnToggleAI);
            Controls.Add(panelAI);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(btnDeleteCard);
            Controls.Add(btnAddCard);
            Controls.Add(dgvFlashcards);
            Controls.Add(lblSetTitle);
            Controls.Add(txtSetTitle);
            Controls.Add(lblTitle);
            Margin = new Padding(4, 3, 4, 3);
            Name = "CreateSetForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Flash Box - Create Set";
            ((System.ComponentModel.ISupportInitialize)dgvFlashcards).EndInit();
            panelAI.ResumeLayout(false);
            panelAI.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numCardCount).EndInit();
            panelLoading.ResumeLayout(false);
            panelLoading.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtSetTitle;
        private System.Windows.Forms.Label lblSetTitle;
        private System.Windows.Forms.DataGridView dgvFlashcards;
        private System.Windows.Forms.Button btnAddCard;
        private System.Windows.Forms.Button btnDeleteCard;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panelAI;
        private System.Windows.Forms.Label lblAIGeneration;
        private System.Windows.Forms.TextBox txtAIPrompt;
        private System.Windows.Forms.Label lblAIPrompt;
        private System.Windows.Forms.Button btnGenerateAI;
        private System.Windows.Forms.Button btnUploadFile;
        private System.Windows.Forms.Label lblCardCountAI;
        private System.Windows.Forms.NumericUpDown numCardCount;
        private System.Windows.Forms.Button btnToggleAI;
        private System.Windows.Forms.Label lblCardCount;
        private System.Windows.Forms.Panel panelLoading;
        private System.Windows.Forms.Label lblLoadingStatus;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblUploadedFile;
        private System.Windows.Forms.Button btnClearFile;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
    }
}
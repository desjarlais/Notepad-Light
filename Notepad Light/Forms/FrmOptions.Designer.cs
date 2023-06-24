namespace Notepad_Light.Forms
{
    partial class FrmOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmOptions));
            groupBox1 = new GroupBox();
            LstMRU = new ListBox();
            BtnClearMRU = new Button();
            BtnOK = new Button();
            BtnCancel = new Button();
            groupBox3 = new GroupBox();
            ckbClearTimersOnExit = new CheckBox();
            cbxIncludeDateTimeWithTemplates = new CheckBox();
            cbxInsertPictureWithTransparency = new CheckBox();
            cbxCleanupTempAppFilesOnExit = new CheckBox();
            label2 = new Label();
            cbxNewFileFormat = new ComboBox();
            label1 = new Label();
            nudAutoSaveInterval = new NumericUpDown();
            ckbUsePasteUI = new CheckBox();
            groupBox5 = new GroupBox();
            rdoWholeWord = new RadioButton();
            rdoMatchCase = new RadioButton();
            rdoFindDirectionDown = new RadioButton();
            rdoFindDirectionUp = new RadioButton();
            groupBox2 = new GroupBox();
            rdoLightMode = new RadioButton();
            rdoDarkMode = new RadioButton();
            groupBox4 = new GroupBox();
            ckbPasteRtfUnformatted = new CheckBox();
            groupBox1.SuspendLayout();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudAutoSaveInterval).BeginInit();
            groupBox5.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox4.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(LstMRU);
            groupBox1.Controls.Add(BtnClearMRU);
            groupBox1.Location = new Point(6, 12);
            groupBox1.Margin = new Padding(2, 1, 2, 1);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(2, 1, 2, 1);
            groupBox1.Size = new Size(639, 178);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "File MRU";
            // 
            // LstMRU
            // 
            LstMRU.FormattingEnabled = true;
            LstMRU.ItemHeight = 15;
            LstMRU.Location = new Point(9, 18);
            LstMRU.Margin = new Padding(2, 1, 2, 1);
            LstMRU.Name = "LstMRU";
            LstMRU.Size = new Size(614, 124);
            LstMRU.TabIndex = 4;
            // 
            // BtnClearMRU
            // 
            BtnClearMRU.Location = new Point(9, 149);
            BtnClearMRU.Margin = new Padding(2, 1, 2, 1);
            BtnClearMRU.Name = "BtnClearMRU";
            BtnClearMRU.Size = new Size(81, 22);
            BtnClearMRU.TabIndex = 3;
            BtnClearMRU.Text = "Clear MRU";
            BtnClearMRU.UseVisualStyleBackColor = true;
            BtnClearMRU.Click += BtnClearMRU_Click;
            // 
            // BtnOK
            // 
            BtnOK.Location = new Point(479, 362);
            BtnOK.Margin = new Padding(2, 1, 2, 1);
            BtnOK.Name = "BtnOK";
            BtnOK.Size = new Size(81, 22);
            BtnOK.TabIndex = 0;
            BtnOK.Text = "OK";
            BtnOK.UseVisualStyleBackColor = true;
            BtnOK.Click += BtnOK_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(562, 362);
            BtnCancel.Margin = new Padding(2, 1, 2, 1);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(81, 22);
            BtnCancel.TabIndex = 2;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(ckbClearTimersOnExit);
            groupBox3.Controls.Add(cbxIncludeDateTimeWithTemplates);
            groupBox3.Controls.Add(cbxInsertPictureWithTransparency);
            groupBox3.Controls.Add(cbxCleanupTempAppFilesOnExit);
            groupBox3.Controls.Add(label2);
            groupBox3.Controls.Add(cbxNewFileFormat);
            groupBox3.Controls.Add(label1);
            groupBox3.Controls.Add(nudAutoSaveInterval);
            groupBox3.Location = new Point(6, 192);
            groupBox3.Margin = new Padding(2, 1, 2, 1);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(2, 1, 2, 1);
            groupBox3.Size = new Size(277, 192);
            groupBox3.TabIndex = 0;
            groupBox3.TabStop = false;
            groupBox3.Text = "App Options";
            // 
            // ckbClearTimersOnExit
            // 
            ckbClearTimersOnExit.AutoSize = true;
            ckbClearTimersOnExit.Location = new Point(14, 164);
            ckbClearTimersOnExit.Name = "ckbClearTimersOnExit";
            ckbClearTimersOnExit.Size = new Size(157, 19);
            ckbClearTimersOnExit.TabIndex = 16;
            ckbClearTimersOnExit.Text = "Clear Timers On App Exit";
            ckbClearTimersOnExit.UseVisualStyleBackColor = true;
            // 
            // cbxIncludeDateTimeWithTemplates
            // 
            cbxIncludeDateTimeWithTemplates.AutoSize = true;
            cbxIncludeDateTimeWithTemplates.Location = new Point(14, 139);
            cbxIncludeDateTimeWithTemplates.Name = "cbxIncludeDateTimeWithTemplates";
            cbxIncludeDateTimeWithTemplates.Size = new Size(226, 19);
            cbxIncludeDateTimeWithTemplates.TabIndex = 15;
            cbxIncludeDateTimeWithTemplates.Text = "Include Date When Inserting Template";
            cbxIncludeDateTimeWithTemplates.UseVisualStyleBackColor = true;
            // 
            // cbxInsertPictureWithTransparency
            // 
            cbxInsertPictureWithTransparency.AutoSize = true;
            cbxInsertPictureWithTransparency.Location = new Point(14, 114);
            cbxInsertPictureWithTransparency.Name = "cbxInsertPictureWithTransparency";
            cbxInsertPictureWithTransparency.Size = new Size(200, 19);
            cbxInsertPictureWithTransparency.TabIndex = 14;
            cbxInsertPictureWithTransparency.Text = "Insert Pictures With Transparency";
            cbxInsertPictureWithTransparency.UseVisualStyleBackColor = true;
            // 
            // cbxCleanupTempAppFilesOnExit
            // 
            cbxCleanupTempAppFilesOnExit.AutoSize = true;
            cbxCleanupTempAppFilesOnExit.Location = new Point(14, 89);
            cbxCleanupTempAppFilesOnExit.Name = "cbxCleanupTempAppFilesOnExit";
            cbxCleanupTempAppFilesOnExit.Size = new Size(245, 19);
            cbxCleanupTempAppFilesOnExit.TabIndex = 13;
            cbxCleanupTempAppFilesOnExit.Text = "Delete Unused Template Files On App Exit";
            cbxCleanupTempAppFilesOnExit.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(14, 27);
            label2.Name = "label2";
            label2.Size = new Size(93, 15);
            label2.TabIndex = 12;
            label2.Text = "New File Format";
            // 
            // cbxNewFileFormat
            // 
            cbxNewFileFormat.FormattingEnabled = true;
            cbxNewFileFormat.Items.AddRange(new object[] { "Plain Text", "Rtf", "Markdown" });
            cbxNewFileFormat.Location = new Point(173, 23);
            cbxNewFileFormat.Name = "cbxNewFileFormat";
            cbxNewFileFormat.Size = new Size(86, 23);
            cbxNewFileFormat.TabIndex = 11;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(14, 61);
            label1.Name = "label1";
            label1.Size = new Size(153, 15);
            label1.TabIndex = 10;
            label1.Text = "AutoSave Interval (minutes)";
            // 
            // nudAutoSaveInterval
            // 
            nudAutoSaveInterval.Location = new Point(217, 59);
            nudAutoSaveInterval.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            nudAutoSaveInterval.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudAutoSaveInterval.Name = "nudAutoSaveInterval";
            nudAutoSaveInterval.Size = new Size(42, 23);
            nudAutoSaveInterval.TabIndex = 9;
            nudAutoSaveInterval.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // ckbUsePasteUI
            // 
            ckbUsePasteUI.AutoSize = true;
            ckbUsePasteUI.Location = new Point(5, 20);
            ckbUsePasteUI.Margin = new Padding(2, 1, 2, 1);
            ckbUsePasteUI.Name = "ckbUsePasteUI";
            ckbUsePasteUI.Size = new Size(90, 19);
            ckbUsePasteUI.TabIndex = 1;
            ckbUsePasteUI.Text = "Use Paste UI";
            ckbUsePasteUI.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(rdoWholeWord);
            groupBox5.Controls.Add(rdoMatchCase);
            groupBox5.Controls.Add(rdoFindDirectionDown);
            groupBox5.Controls.Add(rdoFindDirectionUp);
            groupBox5.Location = new Point(288, 292);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(186, 92);
            groupBox5.TabIndex = 0;
            groupBox5.TabStop = false;
            groupBox5.Text = "Find Options";
            // 
            // rdoWholeWord
            // 
            rdoWholeWord.AutoSize = true;
            rdoWholeWord.Location = new Point(82, 48);
            rdoWholeWord.Name = "rdoWholeWord";
            rdoWholeWord.Size = new Size(91, 19);
            rdoWholeWord.TabIndex = 3;
            rdoWholeWord.TabStop = true;
            rdoWholeWord.Text = "Whole Word";
            rdoWholeWord.UseVisualStyleBackColor = true;
            // 
            // rdoMatchCase
            // 
            rdoMatchCase.AutoSize = true;
            rdoMatchCase.Location = new Point(82, 23);
            rdoMatchCase.Name = "rdoMatchCase";
            rdoMatchCase.Size = new Size(87, 19);
            rdoMatchCase.TabIndex = 2;
            rdoMatchCase.TabStop = true;
            rdoMatchCase.Text = "Match Case";
            rdoMatchCase.UseVisualStyleBackColor = true;
            // 
            // rdoFindDirectionDown
            // 
            rdoFindDirectionDown.AutoSize = true;
            rdoFindDirectionDown.Location = new Point(14, 48);
            rdoFindDirectionDown.Name = "rdoFindDirectionDown";
            rdoFindDirectionDown.Size = new Size(56, 19);
            rdoFindDirectionDown.TabIndex = 1;
            rdoFindDirectionDown.TabStop = true;
            rdoFindDirectionDown.Text = "Down";
            rdoFindDirectionDown.UseVisualStyleBackColor = true;
            // 
            // rdoFindDirectionUp
            // 
            rdoFindDirectionUp.AutoSize = true;
            rdoFindDirectionUp.Location = new Point(14, 22);
            rdoFindDirectionUp.Name = "rdoFindDirectionUp";
            rdoFindDirectionUp.Size = new Size(40, 19);
            rdoFindDirectionUp.TabIndex = 0;
            rdoFindDirectionUp.TabStop = true;
            rdoFindDirectionUp.Text = "Up";
            rdoFindDirectionUp.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(rdoLightMode);
            groupBox2.Controls.Add(rdoDarkMode);
            groupBox2.Location = new Point(479, 200);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(166, 84);
            groupBox2.TabIndex = 7;
            groupBox2.TabStop = false;
            groupBox2.Text = "Theme";
            // 
            // rdoLightMode
            // 
            rdoLightMode.AutoSize = true;
            rdoLightMode.Location = new Point(10, 51);
            rdoLightMode.Name = "rdoLightMode";
            rdoLightMode.Size = new Size(86, 19);
            rdoLightMode.TabIndex = 1;
            rdoLightMode.TabStop = true;
            rdoLightMode.Text = "Light Mode";
            rdoLightMode.UseVisualStyleBackColor = true;
            // 
            // rdoDarkMode
            // 
            rdoDarkMode.AutoSize = true;
            rdoDarkMode.Location = new Point(10, 22);
            rdoDarkMode.Name = "rdoDarkMode";
            rdoDarkMode.Size = new Size(83, 19);
            rdoDarkMode.TabIndex = 0;
            rdoDarkMode.TabStop = true;
            rdoDarkMode.Text = "Dark Mode";
            rdoDarkMode.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(ckbPasteRtfUnformatted);
            groupBox4.Controls.Add(ckbUsePasteUI);
            groupBox4.Location = new Point(288, 192);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(185, 92);
            groupBox4.TabIndex = 16;
            groupBox4.TabStop = false;
            groupBox4.Text = "Paste Options";
            // 
            // ckbPasteRtfUnformatted
            // 
            ckbPasteRtfUnformatted.AutoSize = true;
            ckbPasteRtfUnformatted.Location = new Point(5, 43);
            ckbPasteRtfUnformatted.Name = "ckbPasteRtfUnformatted";
            ckbPasteRtfUnformatted.Size = new Size(146, 19);
            ckbPasteRtfUnformatted.TabIndex = 2;
            ckbPasteRtfUnformatted.Text = "Paste RTF Unformatted";
            ckbPasteRtfUnformatted.UseVisualStyleBackColor = true;
            // 
            // FrmOptions
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(654, 394);
            Controls.Add(groupBox4);
            Controls.Add(groupBox2);
            Controls.Add(groupBox5);
            Controls.Add(groupBox3);
            Controls.Add(BtnCancel);
            Controls.Add(BtnOK);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Margin = new Padding(2, 1, 2, 1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmOptions";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Options";
            KeyDown += FrmOptions_KeyDown;
            groupBox1.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudAutoSaveInterval).EndInit();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private ListBox LstMRU;
        private Button BtnClearMRU;
        private Button BtnOK;
        private Button BtnCancel;
        private GroupBox groupBox3;
        private CheckBox ckbUsePasteUI;
        private GroupBox groupBox5;
        private RadioButton rdoFindDirectionDown;
        private RadioButton rdoFindDirectionUp;
        private RadioButton rdoWholeWord;
        private RadioButton rdoMatchCase;
        private GroupBox groupBox2;
        private RadioButton rdoLightMode;
        private RadioButton rdoDarkMode;
        private NumericUpDown nudAutoSaveInterval;
        private Label label1;
        private Label label2;
        private ComboBox cbxNewFileFormat;
        private CheckBox cbxCleanupTempAppFilesOnExit;
        private CheckBox cbxInsertPictureWithTransparency;
        private CheckBox cbxIncludeDateTimeWithTemplates;
        private GroupBox groupBox4;
        private CheckBox ckbPasteRtfUnformatted;
        private CheckBox ckbClearTimersOnExit;
    }
}
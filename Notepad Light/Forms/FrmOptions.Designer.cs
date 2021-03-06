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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.LstMRU = new System.Windows.Forms.ListBox();
            this.BtnClearMRU = new System.Windows.Forms.Button();
            this.BtnOK = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbxInsertPictureWithTransparency = new System.Windows.Forms.CheckBox();
            this.cbxCleanupTempAppFilesOnExit = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxNewFileFormat = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nudAutoSaveInterval = new System.Windows.Forms.NumericUpDown();
            this.ckbUsePasteUI = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rdoWholeWord = new System.Windows.Forms.RadioButton();
            this.rdoMatchCase = new System.Windows.Forms.RadioButton();
            this.rdoFindDirectionDown = new System.Windows.Forms.RadioButton();
            this.rdoFindDirectionUp = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdoLightMode = new System.Windows.Forms.RadioButton();
            this.rdoDarkMode = new System.Windows.Forms.RadioButton();
            this.cbxIncludeDateTimeWithTemplates = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAutoSaveInterval)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.LstMRU);
            this.groupBox1.Controls.Add(this.BtnClearMRU);
            this.groupBox1.Location = new System.Drawing.Point(6, 12);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.groupBox1.Size = new System.Drawing.Size(639, 178);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File MRU";
            // 
            // LstMRU
            // 
            this.LstMRU.FormattingEnabled = true;
            this.LstMRU.ItemHeight = 15;
            this.LstMRU.Location = new System.Drawing.Point(9, 18);
            this.LstMRU.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.LstMRU.Name = "LstMRU";
            this.LstMRU.Size = new System.Drawing.Size(614, 124);
            this.LstMRU.TabIndex = 4;
            // 
            // BtnClearMRU
            // 
            this.BtnClearMRU.Location = new System.Drawing.Point(9, 149);
            this.BtnClearMRU.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.BtnClearMRU.Name = "BtnClearMRU";
            this.BtnClearMRU.Size = new System.Drawing.Size(81, 22);
            this.BtnClearMRU.TabIndex = 3;
            this.BtnClearMRU.Text = "Clear MRU";
            this.BtnClearMRU.UseVisualStyleBackColor = true;
            this.BtnClearMRU.Click += new System.EventHandler(this.BtnClearMRU_Click);
            // 
            // BtnOK
            // 
            this.BtnOK.Location = new System.Drawing.Point(479, 362);
            this.BtnOK.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(81, 22);
            this.BtnOK.TabIndex = 0;
            this.BtnOK.Text = "OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Location = new System.Drawing.Point(562, 362);
            this.BtnCancel.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(81, 22);
            this.BtnCancel.TabIndex = 2;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbxIncludeDateTimeWithTemplates);
            this.groupBox3.Controls.Add(this.cbxInsertPictureWithTransparency);
            this.groupBox3.Controls.Add(this.cbxCleanupTempAppFilesOnExit);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.cbxNewFileFormat);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.nudAutoSaveInterval);
            this.groupBox3.Controls.Add(this.ckbUsePasteUI);
            this.groupBox3.Location = new System.Drawing.Point(6, 192);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.groupBox3.Size = new System.Drawing.Size(442, 192);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "App Options";
            // 
            // cbxInsertPictureWithTransparency
            // 
            this.cbxInsertPictureWithTransparency.AutoSize = true;
            this.cbxInsertPictureWithTransparency.Location = new System.Drawing.Point(17, 104);
            this.cbxInsertPictureWithTransparency.Name = "cbxInsertPictureWithTransparency";
            this.cbxInsertPictureWithTransparency.Size = new System.Drawing.Size(200, 19);
            this.cbxInsertPictureWithTransparency.TabIndex = 14;
            this.cbxInsertPictureWithTransparency.Text = "Insert Pictures With Transparency";
            this.cbxInsertPictureWithTransparency.UseVisualStyleBackColor = true;
            // 
            // cbxCleanupTempAppFilesOnExit
            // 
            this.cbxCleanupTempAppFilesOnExit.AutoSize = true;
            this.cbxCleanupTempAppFilesOnExit.Location = new System.Drawing.Point(17, 79);
            this.cbxCleanupTempAppFilesOnExit.Name = "cbxCleanupTempAppFilesOnExit";
            this.cbxCleanupTempAppFilesOnExit.Size = new System.Drawing.Size(220, 19);
            this.cbxCleanupTempAppFilesOnExit.TabIndex = 13;
            this.cbxCleanupTempAppFilesOnExit.Text = "Delete Unused Template Files On Exit";
            this.cbxCleanupTempAppFilesOnExit.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 15);
            this.label2.TabIndex = 12;
            this.label2.Text = "New File Format";
            // 
            // cbxNewFileFormat
            // 
            this.cbxNewFileFormat.FormattingEnabled = true;
            this.cbxNewFileFormat.Items.AddRange(new object[] {
            "Plain Text",
            "Rtf"});
            this.cbxNewFileFormat.Location = new System.Drawing.Point(113, 23);
            this.cbxNewFileFormat.Name = "cbxNewFileFormat";
            this.cbxNewFileFormat.Size = new System.Drawing.Size(105, 23);
            this.cbxNewFileFormat.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 161);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 15);
            this.label1.TabIndex = 10;
            this.label1.Text = "AutoSave Interval (minutes)";
            // 
            // nudAutoSaveInterval
            // 
            this.nudAutoSaveInterval.Location = new System.Drawing.Point(176, 159);
            this.nudAutoSaveInterval.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudAutoSaveInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAutoSaveInterval.Name = "nudAutoSaveInterval";
            this.nudAutoSaveInterval.Size = new System.Drawing.Size(42, 23);
            this.nudAutoSaveInterval.TabIndex = 9;
            this.nudAutoSaveInterval.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // ckbUsePasteUI
            // 
            this.ckbUsePasteUI.AutoSize = true;
            this.ckbUsePasteUI.Location = new System.Drawing.Point(17, 56);
            this.ckbUsePasteUI.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.ckbUsePasteUI.Name = "ckbUsePasteUI";
            this.ckbUsePasteUI.Size = new System.Drawing.Size(90, 19);
            this.ckbUsePasteUI.TabIndex = 1;
            this.ckbUsePasteUI.Text = "Use Paste UI";
            this.ckbUsePasteUI.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.rdoWholeWord);
            this.groupBox5.Controls.Add(this.rdoMatchCase);
            this.groupBox5.Controls.Add(this.rdoFindDirectionDown);
            this.groupBox5.Controls.Add(this.rdoFindDirectionUp);
            this.groupBox5.Location = new System.Drawing.Point(453, 192);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(192, 92);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Find Options";
            // 
            // rdoWholeWord
            // 
            this.rdoWholeWord.AutoSize = true;
            this.rdoWholeWord.Location = new System.Drawing.Point(94, 48);
            this.rdoWholeWord.Name = "rdoWholeWord";
            this.rdoWholeWord.Size = new System.Drawing.Size(91, 19);
            this.rdoWholeWord.TabIndex = 3;
            this.rdoWholeWord.TabStop = true;
            this.rdoWholeWord.Text = "Whole Word";
            this.rdoWholeWord.UseVisualStyleBackColor = true;
            // 
            // rdoMatchCase
            // 
            this.rdoMatchCase.AutoSize = true;
            this.rdoMatchCase.Location = new System.Drawing.Point(94, 23);
            this.rdoMatchCase.Name = "rdoMatchCase";
            this.rdoMatchCase.Size = new System.Drawing.Size(87, 19);
            this.rdoMatchCase.TabIndex = 2;
            this.rdoMatchCase.TabStop = true;
            this.rdoMatchCase.Text = "Match Case";
            this.rdoMatchCase.UseVisualStyleBackColor = true;
            // 
            // rdoFindDirectionDown
            // 
            this.rdoFindDirectionDown.AutoSize = true;
            this.rdoFindDirectionDown.Location = new System.Drawing.Point(9, 48);
            this.rdoFindDirectionDown.Name = "rdoFindDirectionDown";
            this.rdoFindDirectionDown.Size = new System.Drawing.Size(56, 19);
            this.rdoFindDirectionDown.TabIndex = 1;
            this.rdoFindDirectionDown.TabStop = true;
            this.rdoFindDirectionDown.Text = "Down";
            this.rdoFindDirectionDown.UseVisualStyleBackColor = true;
            // 
            // rdoFindDirectionUp
            // 
            this.rdoFindDirectionUp.AutoSize = true;
            this.rdoFindDirectionUp.Location = new System.Drawing.Point(9, 23);
            this.rdoFindDirectionUp.Name = "rdoFindDirectionUp";
            this.rdoFindDirectionUp.Size = new System.Drawing.Size(40, 19);
            this.rdoFindDirectionUp.TabIndex = 0;
            this.rdoFindDirectionUp.TabStop = true;
            this.rdoFindDirectionUp.Text = "Up";
            this.rdoFindDirectionUp.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdoLightMode);
            this.groupBox2.Controls.Add(this.rdoDarkMode);
            this.groupBox2.Location = new System.Drawing.Point(453, 290);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(192, 68);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Theme";
            // 
            // rdoLightMode
            // 
            this.rdoLightMode.AutoSize = true;
            this.rdoLightMode.Location = new System.Drawing.Point(103, 19);
            this.rdoLightMode.Name = "rdoLightMode";
            this.rdoLightMode.Size = new System.Drawing.Size(86, 19);
            this.rdoLightMode.TabIndex = 1;
            this.rdoLightMode.TabStop = true;
            this.rdoLightMode.Text = "Light Mode";
            this.rdoLightMode.UseVisualStyleBackColor = true;
            // 
            // rdoDarkMode
            // 
            this.rdoDarkMode.AutoSize = true;
            this.rdoDarkMode.Location = new System.Drawing.Point(3, 19);
            this.rdoDarkMode.Name = "rdoDarkMode";
            this.rdoDarkMode.Size = new System.Drawing.Size(83, 19);
            this.rdoDarkMode.TabIndex = 0;
            this.rdoDarkMode.TabStop = true;
            this.rdoDarkMode.Text = "Dark Mode";
            this.rdoDarkMode.UseVisualStyleBackColor = true;
            // 
            // cbxIncludeDateTimeWithTemplates
            // 
            this.cbxIncludeDateTimeWithTemplates.AutoSize = true;
            this.cbxIncludeDateTimeWithTemplates.Location = new System.Drawing.Point(17, 129);
            this.cbxIncludeDateTimeWithTemplates.Name = "cbxIncludeDateTimeWithTemplates";
            this.cbxIncludeDateTimeWithTemplates.Size = new System.Drawing.Size(262, 19);
            this.cbxIncludeDateTimeWithTemplates.TabIndex = 15;
            this.cbxIncludeDateTimeWithTemplates.Text = "Include Date\\Time When Inserting Templates";
            this.cbxIncludeDateTimeWithTemplates.UseVisualStyleBackColor = true;
            // 
            // FrmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 394);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmOptions_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAutoSaveInterval)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

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
    }
}
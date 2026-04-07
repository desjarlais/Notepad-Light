namespace Notepad_Light.Forms
{
    partial class FrmReplace
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmReplace));
            label1 = new Label();
            label2 = new Label();
            tbxFind = new TextBox();
            tbxReplace = new TextBox();
            BtnFindNext = new Button();
            BtnReplace = new Button();
            BtnReplaceAll = new Button();
            BtnClose = new Button();
            cbxMatchCase = new CheckBox();
            cbxWholeWord = new CheckBox();
            lblStatus = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(62, 15);
            label1.TabIndex = 0;
            label1.Text = "Find what:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 47);
            label2.Name = "label2";
            label2.Size = new Size(77, 15);
            label2.TabIndex = 1;
            label2.Text = "Replace with:";
            // 
            // tbxFind
            // 
            tbxFind.Location = new Point(95, 12);
            tbxFind.Name = "tbxFind";
            tbxFind.Size = new Size(310, 23);
            tbxFind.TabIndex = 2;
            tbxFind.TextChanged += TbxFind_TextChanged;
            // 
            // tbxReplace
            // 
            tbxReplace.Location = new Point(95, 44);
            tbxReplace.Name = "tbxReplace";
            tbxReplace.Size = new Size(310, 23);
            tbxReplace.TabIndex = 3;
            // 
            // BtnFindNext
            // 
            BtnFindNext.Enabled = false;
            BtnFindNext.Location = new Point(420, 11);
            BtnFindNext.Name = "BtnFindNext";
            BtnFindNext.Size = new Size(100, 25);
            BtnFindNext.TabIndex = 4;
            BtnFindNext.Text = "Find Next";
            BtnFindNext.UseVisualStyleBackColor = true;
            BtnFindNext.Click += BtnFindNext_Click;
            // 
            // BtnReplace
            // 
            BtnReplace.Enabled = false;
            BtnReplace.Location = new Point(420, 43);
            BtnReplace.Name = "BtnReplace";
            BtnReplace.Size = new Size(100, 25);
            BtnReplace.TabIndex = 5;
            BtnReplace.Text = "Replace";
            BtnReplace.UseVisualStyleBackColor = true;
            BtnReplace.Click += BtnReplace_Click;
            // 
            // BtnReplaceAll
            // 
            BtnReplaceAll.Enabled = false;
            BtnReplaceAll.Location = new Point(420, 75);
            BtnReplaceAll.Name = "BtnReplaceAll";
            BtnReplaceAll.Size = new Size(100, 25);
            BtnReplaceAll.TabIndex = 6;
            BtnReplaceAll.Text = "Replace All";
            BtnReplaceAll.UseVisualStyleBackColor = true;
            BtnReplaceAll.Click += BtnReplaceAll_Click;
            // 
            // BtnClose
            // 
            BtnClose.Location = new Point(420, 107);
            BtnClose.Name = "BtnClose";
            BtnClose.Size = new Size(100, 25);
            BtnClose.TabIndex = 7;
            BtnClose.Text = "Close";
            BtnClose.UseVisualStyleBackColor = true;
            BtnClose.Click += BtnClose_Click;
            // 
            // cbxMatchCase
            // 
            cbxMatchCase.AutoSize = true;
            cbxMatchCase.Location = new Point(95, 78);
            cbxMatchCase.Name = "cbxMatchCase";
            cbxMatchCase.Size = new Size(86, 19);
            cbxMatchCase.TabIndex = 8;
            cbxMatchCase.Text = "Match case";
            cbxMatchCase.UseVisualStyleBackColor = true;
            // 
            // cbxWholeWord
            // 
            cbxWholeWord.AutoSize = true;
            cbxWholeWord.Location = new Point(195, 78);
            cbxWholeWord.Name = "cbxWholeWord";
            cbxWholeWord.Size = new Size(107, 19);
            cbxWholeWord.TabIndex = 9;
            cbxWholeWord.Text = "Match whole word";
            cbxWholeWord.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = SystemColors.GrayText;
            lblStatus.Location = new Point(95, 106);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 15);
            lblStatus.TabIndex = 10;
            // 
            // FrmReplace
            // 
            AcceptButton = BtnFindNext;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(534, 140);
            Controls.Add(lblStatus);
            Controls.Add(cbxWholeWord);
            Controls.Add(cbxMatchCase);
            Controls.Add(BtnClose);
            Controls.Add(BtnReplaceAll);
            Controls.Add(BtnReplace);
            Controls.Add(BtnFindNext);
            Controls.Add(tbxReplace);
            Controls.Add(tbxFind);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmReplace";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Find and Replace";
            KeyDown += FrmReplace_KeyDown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private Label label2;
        private TextBox tbxFind;
        private TextBox tbxReplace;
        private Button BtnFindNext;
        private Button BtnReplace;
        private Button BtnReplaceAll;
        private Button BtnClose;
        private CheckBox cbxMatchCase;
        private CheckBox cbxWholeWord;
        private Label lblStatus;
    }
}
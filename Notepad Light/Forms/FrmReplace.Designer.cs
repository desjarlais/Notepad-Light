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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxFind = new System.Windows.Forms.TextBox();
            this.tbxReplace = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.BtnFindNext = new System.Windows.Forms.Button();
            this.BtnReplace = new System.Windows.Forms.Button();
            this.cbxReplaceAll = new System.Windows.Forms.CheckBox();
            this.RtbSearchResults = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Find what:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Replace with:";
            // 
            // tbxFind
            // 
            this.tbxFind.Location = new System.Drawing.Point(95, 9);
            this.tbxFind.Name = "tbxFind";
            this.tbxFind.Size = new System.Drawing.Size(478, 23);
            this.tbxFind.TabIndex = 2;
            // 
            // tbxReplace
            // 
            this.tbxReplace.Location = new System.Drawing.Point(95, 136);
            this.tbxReplace.Name = "tbxReplace";
            this.tbxReplace.Size = new System.Drawing.Size(478, 23);
            this.tbxReplace.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Found:";
            // 
            // BtnFindNext
            // 
            this.BtnFindNext.Location = new System.Drawing.Point(498, 38);
            this.BtnFindNext.Name = "BtnFindNext";
            this.BtnFindNext.Size = new System.Drawing.Size(75, 23);
            this.BtnFindNext.TabIndex = 6;
            this.BtnFindNext.Text = "Find Next";
            this.BtnFindNext.UseVisualStyleBackColor = true;
            this.BtnFindNext.Click += new System.EventHandler(this.BtnFindNext_Click);
            // 
            // BtnReplace
            // 
            this.BtnReplace.Location = new System.Drawing.Point(498, 166);
            this.BtnReplace.Name = "BtnReplace";
            this.BtnReplace.Size = new System.Drawing.Size(75, 23);
            this.BtnReplace.TabIndex = 7;
            this.BtnReplace.Text = "Replace";
            this.BtnReplace.UseVisualStyleBackColor = true;
            this.BtnReplace.Click += new System.EventHandler(this.BtnReplace_Click);
            // 
            // cbxReplaceAll
            // 
            this.cbxReplaceAll.AutoSize = true;
            this.cbxReplaceAll.Location = new System.Drawing.Point(95, 169);
            this.cbxReplaceAll.Name = "cbxReplaceAll";
            this.cbxReplaceAll.Size = new System.Drawing.Size(153, 19);
            this.cbxReplaceAll.TabIndex = 8;
            this.cbxReplaceAll.Text = "Replace All Occurrences";
            this.cbxReplaceAll.UseVisualStyleBackColor = true;
            // 
            // RtbSearchResults
            // 
            this.RtbSearchResults.Enabled = false;
            this.RtbSearchResults.Location = new System.Drawing.Point(95, 71);
            this.RtbSearchResults.Name = "RtbSearchResults";
            this.RtbSearchResults.Size = new System.Drawing.Size(478, 59);
            this.RtbSearchResults.TabIndex = 9;
            this.RtbSearchResults.Text = "";
            // 
            // FrmReplace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 207);
            this.Controls.Add(this.RtbSearchResults);
            this.Controls.Add(this.cbxReplaceAll);
            this.Controls.Add(this.BtnReplace);
            this.Controls.Add(this.BtnFindNext);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbxReplace);
            this.Controls.Add(this.tbxFind);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FrmReplace";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Replace Text";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox tbxFind;
        private TextBox tbxReplace;
        private Label label3;
        private Button BtnFindNext;
        private Button BtnReplace;
        private CheckBox cbxReplaceAll;
        private RichTextBox RtbSearchResults;
    }
}
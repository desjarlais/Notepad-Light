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
            label2 = new Label();
            tbxReplace = new TextBox();
            BtnReplace = new Button();
            cbxReplaceAll = new CheckBox();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(77, 15);
            label2.TabIndex = 1;
            label2.Text = "Replace with:";
            // 
            // tbxReplace
            // 
            tbxReplace.Location = new Point(95, 6);
            tbxReplace.Name = "tbxReplace";
            tbxReplace.Size = new Size(478, 23);
            tbxReplace.TabIndex = 3;
            // 
            // BtnReplace
            // 
            BtnReplace.Location = new Point(488, 36);
            BtnReplace.Name = "BtnReplace";
            BtnReplace.Size = new Size(85, 23);
            BtnReplace.TabIndex = 7;
            BtnReplace.Text = "Replace";
            BtnReplace.UseVisualStyleBackColor = true;
            BtnReplace.Click += BtnReplace_Click;
            // 
            // cbxReplaceAll
            // 
            cbxReplaceAll.AutoSize = true;
            cbxReplaceAll.Enabled = false;
            cbxReplaceAll.Location = new Point(95, 39);
            cbxReplaceAll.Name = "cbxReplaceAll";
            cbxReplaceAll.Size = new Size(153, 19);
            cbxReplaceAll.TabIndex = 8;
            cbxReplaceAll.Text = "Replace All Occurrences";
            cbxReplaceAll.UseVisualStyleBackColor = true;
            // 
            // FrmReplace
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(582, 72);
            Controls.Add(cbxReplaceAll);
            Controls.Add(BtnReplace);
            Controls.Add(tbxReplace);
            Controls.Add(label2);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmReplace";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Replace Text";
            KeyDown += FrmReplace_KeyDown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label2;
        private TextBox tbxReplace;
        private Button BtnReplace;
        private CheckBox cbxReplaceAll;
    }
}
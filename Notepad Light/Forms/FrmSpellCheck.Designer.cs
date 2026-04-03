namespace Notepad_Light.Forms
{
    partial class FrmSpellCheck
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
            label1 = new Label();
            lblMisspelledWord = new Label();
            label2 = new Label();
            txtReplaceWith = new TextBox();
            label3 = new Label();
            lstSuggestions = new ListBox();
            btnIgnore = new Button();
            btnIgnoreAll = new Button();
            btnReplace = new Button();
            btnAdd = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(135, 25);
            label1.TabIndex = 0;
            label1.Text = "Not in dictionary:";
            // 
            // lblMisspelledWord
            // 
            lblMisspelledWord.AutoSize = true;
            lblMisspelledWord.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMisspelledWord.ForeColor = Color.Red;
            lblMisspelledWord.Location = new Point(153, 15);
            lblMisspelledWord.Name = "lblMisspelledWord";
            lblMisspelledWord.Size = new Size(60, 25);
            lblMisspelledWord.TabIndex = 1;
            lblMisspelledWord.Text = "<word>";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 55);
            label2.Name = "label2";
            label2.Size = new Size(114, 25);
            label2.TabIndex = 2;
            label2.Text = "Replace with:";
            // 
            // txtReplaceWith
            // 
            txtReplaceWith.Location = new Point(12, 83);
            txtReplaceWith.Name = "txtReplaceWith";
            txtReplaceWith.Size = new Size(380, 31);
            txtReplaceWith.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 127);
            label3.Name = "label3";
            label3.Size = new Size(114, 25);
            label3.TabIndex = 4;
            label3.Text = "Suggestions:";
            // 
            // lstSuggestions
            // 
            lstSuggestions.FormattingEnabled = true;
            lstSuggestions.Location = new Point(12, 155);
            lstSuggestions.Name = "lstSuggestions";
            lstSuggestions.Size = new Size(380, 154);
            lstSuggestions.TabIndex = 5;
            lstSuggestions.SelectedIndexChanged += lstSuggestions_SelectedIndexChanged;
            // 
            // btnIgnore
            // 
            btnIgnore.Location = new Point(410, 15);
            btnIgnore.Name = "btnIgnore";
            btnIgnore.Size = new Size(130, 34);
            btnIgnore.TabIndex = 6;
            btnIgnore.Text = "Ignore";
            btnIgnore.UseVisualStyleBackColor = true;
            btnIgnore.Click += btnIgnore_Click;
            // 
            // btnIgnoreAll
            // 
            btnIgnoreAll.Location = new Point(410, 60);
            btnIgnoreAll.Name = "btnIgnoreAll";
            btnIgnoreAll.Size = new Size(130, 34);
            btnIgnoreAll.TabIndex = 7;
            btnIgnoreAll.Text = "Ignore All";
            btnIgnoreAll.UseVisualStyleBackColor = true;
            btnIgnoreAll.Click += btnIgnoreAll_Click;
            // 
            // btnReplace
            // 
            btnReplace.Location = new Point(410, 110);
            btnReplace.Name = "btnReplace";
            btnReplace.Size = new Size(130, 34);
            btnReplace.TabIndex = 8;
            btnReplace.Text = "Replace";
            btnReplace.UseVisualStyleBackColor = true;
            btnReplace.Click += btnReplace_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(410, 160);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(130, 34);
            btnAdd.TabIndex = 9;
            btnAdd.Text = "Add to Dictionary";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(410, 275);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(130, 34);
            btnCancel.TabIndex = 10;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // FrmSpellCheck
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(560, 330);
            Controls.Add(btnCancel);
            Controls.Add(btnAdd);
            Controls.Add(btnReplace);
            Controls.Add(btnIgnoreAll);
            Controls.Add(btnIgnore);
            Controls.Add(lstSuggestions);
            Controls.Add(label3);
            Controls.Add(txtReplaceWith);
            Controls.Add(label2);
            Controls.Add(lblMisspelledWord);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmSpellCheck";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Spell Check";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label lblMisspelledWord;
        private Label label2;
        private TextBox txtReplaceWith;
        private Label label3;
        private ListBox lstSuggestions;
        private Button btnIgnore;
        private Button btnIgnoreAll;
        private Button btnReplace;
        private Button btnAdd;
        private Button btnCancel;
    }
}

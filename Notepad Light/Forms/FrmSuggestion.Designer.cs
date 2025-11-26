namespace Notepad_Light.Forms
{
    partial class FrmSuggestion
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
            RtbTextToCheck = new RichTextBox();
            BtnIgnore = new Button();
            BtnIgnoreAll = new Button();
            BtnAdd = new Button();
            BtnReplace = new Button();
            BtnReplaceAll = new Button();
            BtnCancel = new Button();
            label1 = new Label();
            TxReplaceWith = new TextBox();
            label2 = new Label();
            LstSuggestions = new ListBox();
            label3 = new Label();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripStatusLabel3 = new ToolStripStatusLabel();
            toolStripStatusLabel4 = new ToolStripStatusLabel();
            toolStripStatusLabel5 = new ToolStripStatusLabel();
            label4 = new Label();
            ChkIgnoreDigits = new CheckBox();
            ChkIgnoreUpper = new CheckBox();
            ChkIgnoreHtml = new CheckBox();
            nudMaxSuggestions = new NumericUpDown();
            label5 = new Label();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudMaxSuggestions).BeginInit();
            SuspendLayout();
            // 
            // RtbTextToCheck
            // 
            RtbTextToCheck.Location = new Point(12, 37);
            RtbTextToCheck.Name = "RtbTextToCheck";
            RtbTextToCheck.Size = new Size(689, 241);
            RtbTextToCheck.TabIndex = 0;
            RtbTextToCheck.Text = "";
            // 
            // BtnIgnore
            // 
            BtnIgnore.Location = new Point(707, 37);
            BtnIgnore.Name = "BtnIgnore";
            BtnIgnore.Size = new Size(138, 34);
            BtnIgnore.TabIndex = 1;
            BtnIgnore.Text = "Ignore";
            BtnIgnore.UseVisualStyleBackColor = true;
            BtnIgnore.Click += BtnIgnore_Click;
            // 
            // BtnIgnoreAll
            // 
            BtnIgnoreAll.Location = new Point(707, 87);
            BtnIgnoreAll.Name = "BtnIgnoreAll";
            BtnIgnoreAll.Size = new Size(138, 34);
            BtnIgnoreAll.TabIndex = 2;
            BtnIgnoreAll.Text = "Ignore All";
            BtnIgnoreAll.UseVisualStyleBackColor = true;
            BtnIgnoreAll.Click += BtnIgnoreAll_Click;
            // 
            // BtnAdd
            // 
            BtnAdd.Location = new Point(707, 244);
            BtnAdd.Name = "BtnAdd";
            BtnAdd.Size = new Size(138, 34);
            BtnAdd.TabIndex = 3;
            BtnAdd.Text = "Add";
            BtnAdd.UseVisualStyleBackColor = true;
            BtnAdd.Click += BtnAdd_Click;
            // 
            // BtnReplace
            // 
            BtnReplace.Location = new Point(707, 301);
            BtnReplace.Name = "BtnReplace";
            BtnReplace.Size = new Size(138, 34);
            BtnReplace.TabIndex = 4;
            BtnReplace.Text = "Replace";
            BtnReplace.UseVisualStyleBackColor = true;
            BtnReplace.Click += BtnReplace_Click;
            // 
            // BtnReplaceAll
            // 
            BtnReplaceAll.Location = new Point(707, 341);
            BtnReplaceAll.Name = "BtnReplaceAll";
            BtnReplaceAll.Size = new Size(138, 34);
            BtnReplaceAll.TabIndex = 5;
            BtnReplaceAll.Text = "Replace All";
            BtnReplaceAll.UseVisualStyleBackColor = true;
            BtnReplaceAll.Click += BtnReplaceAll_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(707, 641);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(138, 34);
            BtnCancel.TabIndex = 7;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(167, 25);
            label1.TabIndex = 8;
            label1.Text = "Text Being Checked:";
            // 
            // TxReplaceWith
            // 
            TxReplaceWith.Location = new Point(12, 303);
            TxReplaceWith.Name = "TxReplaceWith";
            TxReplaceWith.Size = new Size(689, 31);
            TxReplaceWith.TabIndex = 9;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 281);
            label2.Name = "label2";
            label2.Size = new Size(114, 25);
            label2.TabIndex = 10;
            label2.Text = "Replace with:";
            // 
            // LstSuggestions
            // 
            LstSuggestions.FormattingEnabled = true;
            LstSuggestions.Location = new Point(12, 389);
            LstSuggestions.Name = "LstSuggestions";
            LstSuggestions.Size = new Size(689, 154);
            LstSuggestions.TabIndex = 11;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 360);
            label3.Name = "label3";
            label3.Size = new Size(114, 25);
            label3.TabIndex = 12;
            label3.Text = "Suggestions:";
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(24, 24);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabel2, toolStripStatusLabel3, toolStripStatusLabel4, toolStripStatusLabel5 });
            statusStrip1.Location = new Point(0, 698);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(871, 32);
            statusStrip1.TabIndex = 13;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(112, 25);
            toolStripStatusLabel1.Text = "Word: 0 of 0";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(16, 25);
            toolStripStatusLabel2.Text = "|";
            // 
            // toolStripStatusLabel3
            // 
            toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            toolStripStatusLabel3.Size = new Size(74, 25);
            toolStripStatusLabel3.Text = "Index: 0";
            // 
            // toolStripStatusLabel4
            // 
            toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            toolStripStatusLabel4.Size = new Size(16, 25);
            toolStripStatusLabel4.Text = "|";
            // 
            // toolStripStatusLabel5
            // 
            toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            toolStripStatusLabel5.Size = new Size(77, 25);
            toolStripStatusLabel5.Text = "<word>";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 565);
            label4.Name = "label4";
            label4.Size = new Size(80, 25);
            label4.TabIndex = 14;
            label4.Text = "Options:";
            // 
            // ChkIgnoreDigits
            // 
            ChkIgnoreDigits.AutoSize = true;
            ChkIgnoreDigits.Location = new Point(12, 593);
            ChkIgnoreDigits.Name = "ChkIgnoreDigits";
            ChkIgnoreDigits.Size = new Size(141, 29);
            ChkIgnoreDigits.TabIndex = 15;
            ChkIgnoreDigits.Text = "Ignore Digits";
            ChkIgnoreDigits.UseVisualStyleBackColor = true;
            // 
            // ChkIgnoreUpper
            // 
            ChkIgnoreUpper.AutoSize = true;
            ChkIgnoreUpper.Location = new Point(159, 593);
            ChkIgnoreUpper.Name = "ChkIgnoreUpper";
            ChkIgnoreUpper.Size = new Size(186, 29);
            ChkIgnoreUpper.TabIndex = 16;
            ChkIgnoreUpper.Text = "Ignore Upper Case";
            ChkIgnoreUpper.UseVisualStyleBackColor = true;
            // 
            // ChkIgnoreHtml
            // 
            ChkIgnoreHtml.AutoSize = true;
            ChkIgnoreHtml.Location = new Point(351, 593);
            ChkIgnoreHtml.Name = "ChkIgnoreHtml";
            ChkIgnoreHtml.Size = new Size(134, 29);
            ChkIgnoreHtml.TabIndex = 17;
            ChkIgnoreHtml.Text = "Ignore Html";
            ChkIgnoreHtml.UseVisualStyleBackColor = true;
            // 
            // nudMaxSuggestions
            // 
            nudMaxSuggestions.Location = new Point(170, 644);
            nudMaxSuggestions.Maximum = new decimal(new int[] { 25, 0, 0, 0 });
            nudMaxSuggestions.Name = "nudMaxSuggestions";
            nudMaxSuggestions.Size = new Size(91, 31);
            nudMaxSuggestions.TabIndex = 18;
            nudMaxSuggestions.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 646);
            label5.Name = "label5";
            label5.Size = new Size(152, 25);
            label5.TabIndex = 19;
            label5.Text = "Max Suggestions:";
            // 
            // SuggestionForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(871, 730);
            Controls.Add(label5);
            Controls.Add(nudMaxSuggestions);
            Controls.Add(ChkIgnoreHtml);
            Controls.Add(ChkIgnoreUpper);
            Controls.Add(ChkIgnoreDigits);
            Controls.Add(label4);
            Controls.Add(statusStrip1);
            Controls.Add(label3);
            Controls.Add(LstSuggestions);
            Controls.Add(label2);
            Controls.Add(TxReplaceWith);
            Controls.Add(label1);
            Controls.Add(BtnCancel);
            Controls.Add(BtnReplaceAll);
            Controls.Add(BtnReplace);
            Controls.Add(BtnAdd);
            Controls.Add(BtnIgnoreAll);
            Controls.Add(BtnIgnore);
            Controls.Add(RtbTextToCheck);
            Name = "SuggestionForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Spell Checker";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudMaxSuggestions).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox RtbTextToCheck;
        private Button BtnIgnore;
        private Button BtnIgnoreAll;
        private Button BtnAdd;
        private Button BtnReplace;
        private Button BtnReplaceAll;
        private Button BtnCancel;
        private Label label1;
        private TextBox TxReplaceWith;
        private Label label2;
        private ListBox LstSuggestions;
        private Label label3;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripStatusLabel toolStripStatusLabel3;
        private ToolStripStatusLabel toolStripStatusLabel4;
        private ToolStripStatusLabel toolStripStatusLabel5;
        private Label label4;
        private CheckBox ChkIgnoreDigits;
        private CheckBox ChkIgnoreUpper;
        private CheckBox ChkIgnoreHtml;
        private NumericUpDown nudMaxSuggestions;
        private Label label5;
    }
}
﻿namespace Notepad_Light.Forms
{
    partial class FrmSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSettings));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.LstMRU = new System.Windows.Forms.ListBox();
            this.BtnClearMRU = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdoPasteUI = new System.Windows.Forms.RadioButton();
            this.rdoPasteText = new System.Windows.Forms.RadioButton();
            this.BtnOK = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboNewDocFormat = new System.Windows.Forms.ComboBox();
            this.ckbUsePasteUI = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rdoFindDirectionUp = new System.Windows.Forms.RadioButton();
            this.rdoFindDirectionDown = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdoPasteUI);
            this.groupBox2.Controls.Add(this.rdoPasteText);
            this.groupBox2.Location = new System.Drawing.Point(11, 202);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.groupBox2.Size = new System.Drawing.Size(176, 84);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Paste Options";
            // 
            // rdoPasteUI
            // 
            this.rdoPasteUI.AutoSize = true;
            this.rdoPasteUI.Location = new System.Drawing.Point(12, 47);
            this.rdoPasteUI.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.rdoPasteUI.Name = "rdoPasteUI";
            this.rdoPasteUI.Size = new System.Drawing.Size(112, 19);
            this.rdoPasteUI.TabIndex = 1;
            this.rdoPasteUI.TabStop = true;
            this.rdoPasteUI.Text = "Use Paste Dialog";
            this.rdoPasteUI.UseVisualStyleBackColor = true;
            // 
            // rdoPasteText
            // 
            this.rdoPasteText.AutoSize = true;
            this.rdoPasteText.Location = new System.Drawing.Point(12, 22);
            this.rdoPasteText.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.rdoPasteText.Name = "rdoPasteText";
            this.rdoPasteText.Size = new System.Drawing.Size(74, 19);
            this.rdoPasteText.TabIndex = 0;
            this.rdoPasteText.TabStop = true;
            this.rdoPasteText.Text = "Text Only";
            this.rdoPasteText.UseVisualStyleBackColor = true;
            // 
            // BtnOK
            // 
            this.BtnOK.Location = new System.Drawing.Point(482, 298);
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
            this.BtnCancel.Location = new System.Drawing.Point(566, 298);
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
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.cboNewDocFormat);
            this.groupBox3.Controls.Add(this.ckbUsePasteUI);
            this.groupBox3.Location = new System.Drawing.Point(295, 202);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.groupBox3.Size = new System.Drawing.Size(350, 84);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "App Options";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "New Document Format:";
            // 
            // cboNewDocFormat
            // 
            this.cboNewDocFormat.FormattingEnabled = true;
            this.cboNewDocFormat.Items.AddRange(new object[] {
            "Plain Text",
            "RTF"});
            this.cboNewDocFormat.Location = new System.Drawing.Point(152, 19);
            this.cboNewDocFormat.Name = "cboNewDocFormat";
            this.cboNewDocFormat.Size = new System.Drawing.Size(157, 23);
            this.cboNewDocFormat.TabIndex = 5;
            // 
            // ckbUsePasteUI
            // 
            this.ckbUsePasteUI.AutoSize = true;
            this.ckbUsePasteUI.Location = new System.Drawing.Point(12, 48);
            this.ckbUsePasteUI.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.ckbUsePasteUI.Name = "ckbUsePasteUI";
            this.ckbUsePasteUI.Size = new System.Drawing.Size(90, 19);
            this.ckbUsePasteUI.TabIndex = 1;
            this.ckbUsePasteUI.Text = "Use Paste UI";
            this.ckbUsePasteUI.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.rdoFindDirectionDown);
            this.groupBox5.Controls.Add(this.rdoFindDirectionUp);
            this.groupBox5.Location = new System.Drawing.Point(192, 202);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(98, 84);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Find Options";
            // 
            // rdoFindDirectionUp
            // 
            this.rdoFindDirectionUp.AutoSize = true;
            this.rdoFindDirectionUp.Location = new System.Drawing.Point(3, 19);
            this.rdoFindDirectionUp.Name = "rdoFindDirectionUp";
            this.rdoFindDirectionUp.Size = new System.Drawing.Size(40, 19);
            this.rdoFindDirectionUp.TabIndex = 0;
            this.rdoFindDirectionUp.TabStop = true;
            this.rdoFindDirectionUp.Text = "Up";
            this.rdoFindDirectionUp.UseVisualStyleBackColor = true;
            // 
            // rdoFindDirectionDown
            // 
            this.rdoFindDirectionDown.AutoSize = true;
            this.rdoFindDirectionDown.Location = new System.Drawing.Point(3, 48);
            this.rdoFindDirectionDown.Name = "rdoFindDirectionDown";
            this.rdoFindDirectionDown.Size = new System.Drawing.Size(56, 19);
            this.rdoFindDirectionDown.TabIndex = 1;
            this.rdoFindDirectionDown.TabStop = true;
            this.rdoFindDirectionDown.Text = "Down";
            this.rdoFindDirectionDown.UseVisualStyleBackColor = true;
            // 
            // FrmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 329);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox groupBox1;
        private ListBox LstMRU;
        private Button BtnClearMRU;
        private GroupBox groupBox2;
        private RadioButton rdoPasteUI;
        private RadioButton rdoPasteText;
        private Button BtnOK;
        private Button BtnCancel;
        private GroupBox groupBox3;
        private CheckBox ckbUsePasteUI;
        private Label label2;
        private ComboBox cboNewDocFormat;
        private GroupBox groupBox5;
        private RadioButton rdoFindDirectionDown;
        private RadioButton rdoFindDirectionUp;
    }
}
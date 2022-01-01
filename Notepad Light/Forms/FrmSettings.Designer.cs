namespace Notepad_Light.Forms
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.LstMRU = new System.Windows.Forms.ListBox();
            this.BtnClearMRU = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdoPasteUI = new System.Windows.Forms.RadioButton();
            this.rdoPasteText = new System.Windows.Forms.RadioButton();
            this.BtnOK = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ckbUsePasteUI = new System.Windows.Forms.CheckBox();
            this.ckbUseRtf = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.LstMRU);
            this.groupBox1.Controls.Add(this.BtnClearMRU);
            this.groupBox1.Location = new System.Drawing.Point(12, 26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(716, 546);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File MRU";
            // 
            // LstMRU
            // 
            this.LstMRU.FormattingEnabled = true;
            this.LstMRU.ItemHeight = 32;
            this.LstMRU.Location = new System.Drawing.Point(16, 38);
            this.LstMRU.Name = "LstMRU";
            this.LstMRU.Size = new System.Drawing.Size(679, 420);
            this.LstMRU.TabIndex = 4;
            // 
            // BtnClearMRU
            // 
            this.BtnClearMRU.Location = new System.Drawing.Point(16, 483);
            this.BtnClearMRU.Name = "BtnClearMRU";
            this.BtnClearMRU.Size = new System.Drawing.Size(150, 46);
            this.BtnClearMRU.TabIndex = 3;
            this.BtnClearMRU.Text = "Clear MRU";
            this.BtnClearMRU.UseVisualStyleBackColor = true;
            this.BtnClearMRU.Click += new System.EventHandler(this.BtnClearMRU_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdoPasteUI);
            this.groupBox2.Controls.Add(this.rdoPasteText);
            this.groupBox2.Location = new System.Drawing.Point(749, 26);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(497, 180);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Paste Options";
            // 
            // rdoPasteUI
            // 
            this.rdoPasteUI.AutoSize = true;
            this.rdoPasteUI.Location = new System.Drawing.Point(22, 101);
            this.rdoPasteUI.Name = "rdoPasteUI";
            this.rdoPasteUI.Size = new System.Drawing.Size(222, 36);
            this.rdoPasteUI.TabIndex = 1;
            this.rdoPasteUI.TabStop = true;
            this.rdoPasteUI.Text = "Use Paste Dialog";
            this.rdoPasteUI.UseVisualStyleBackColor = true;
            // 
            // rdoPasteText
            // 
            this.rdoPasteText.AutoSize = true;
            this.rdoPasteText.Location = new System.Drawing.Point(22, 47);
            this.rdoPasteText.Name = "rdoPasteText";
            this.rdoPasteText.Size = new System.Drawing.Size(145, 36);
            this.rdoPasteText.TabIndex = 0;
            this.rdoPasteText.TabStop = true;
            this.rdoPasteText.Text = "Text Only";
            this.rdoPasteText.UseVisualStyleBackColor = true;
            // 
            // BtnOK
            // 
            this.BtnOK.Location = new System.Drawing.Point(939, 520);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(150, 46);
            this.BtnOK.TabIndex = 0;
            this.BtnOK.Text = "OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Location = new System.Drawing.Point(1095, 520);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(150, 46);
            this.BtnCancel.TabIndex = 2;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ckbUsePasteUI);
            this.groupBox3.Controls.Add(this.ckbUseRtf);
            this.groupBox3.Location = new System.Drawing.Point(749, 212);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(497, 302);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "App Options";
            // 
            // ckbUsePasteUI
            // 
            this.ckbUsePasteUI.AutoSize = true;
            this.ckbUsePasteUI.Location = new System.Drawing.Point(22, 106);
            this.ckbUsePasteUI.Name = "ckbUsePasteUI";
            this.ckbUsePasteUI.Size = new System.Drawing.Size(176, 36);
            this.ckbUsePasteUI.TabIndex = 1;
            this.ckbUsePasteUI.Text = "Use Paste UI";
            this.ckbUsePasteUI.UseVisualStyleBackColor = true;
            // 
            // ckbUseRtf
            // 
            this.ckbUseRtf.AutoSize = true;
            this.ckbUseRtf.Location = new System.Drawing.Point(22, 49);
            this.ckbUseRtf.Name = "ckbUseRtf";
            this.ckbUseRtf.Size = new System.Drawing.Size(281, 36);
            this.ckbUseRtf.TabIndex = 0;
            this.ckbUseRtf.Text = "New Document = RTF";
            this.ckbUseRtf.UseVisualStyleBackColor = true;
            // 
            // FrmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1267, 584);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
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
        private CheckBox ckbUseRtf;
    }
}
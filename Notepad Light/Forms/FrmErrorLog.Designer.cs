namespace Notepad_Light.Forms
{
    partial class FrmErrorLog
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
            this.LstOutput = new System.Windows.Forms.ListBox();
            this.BtnCopy = new System.Windows.Forms.Button();
            this.BtnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LstOutput
            // 
            this.LstOutput.FormattingEnabled = true;
            this.LstOutput.ItemHeight = 15;
            this.LstOutput.Location = new System.Drawing.Point(6, 6);
            this.LstOutput.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.LstOutput.Name = "LstOutput";
            this.LstOutput.Size = new System.Drawing.Size(641, 364);
            this.LstOutput.TabIndex = 0;
            // 
            // BtnCopy
            // 
            this.BtnCopy.Location = new System.Drawing.Point(6, 370);
            this.BtnCopy.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.BtnCopy.Name = "BtnCopy";
            this.BtnCopy.Size = new System.Drawing.Size(81, 22);
            this.BtnCopy.TabIndex = 1;
            this.BtnCopy.Text = "Copy";
            this.BtnCopy.UseVisualStyleBackColor = true;
            this.BtnCopy.Click += new System.EventHandler(this.BtnCopy_Click);
            // 
            // BtnOK
            // 
            this.BtnOK.Location = new System.Drawing.Point(565, 370);
            this.BtnOK.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(81, 22);
            this.BtnOK.TabIndex = 2;
            this.BtnOK.Text = "OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // FrmErrorLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 398);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.BtnCopy);
            this.Controls.Add(this.LstOutput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmErrorLog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Error Log";
            this.ResumeLayout(false);

        }

        #endregion

        private ListBox LstOutput;
        private Button BtnCopy;
        private Button BtnOK;
    }
}
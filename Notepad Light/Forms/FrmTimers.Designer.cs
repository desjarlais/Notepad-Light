﻿namespace Notepad_Light.Forms
{
    partial class FrmTimers
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTimers));
            dataGridView1 = new DataGridView();
            DescriptionCol = new DataGridViewTextBoxColumn();
            DateCol = new DataGridViewTextBoxColumn();
            TimeCol = new DataGridViewTextBoxColumn();
            gridViewContextMenuStrip = new ContextMenuStrip(components);
            CopyDescriptionContextMenu = new ToolStripMenuItem();
            CopyTimeContextMenu = new ToolStripMenuItem();
            BtnClearTimers = new Button();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            BtnResumeTimer = new Button();
            BtnDeleteSelectedTime = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            gridViewContextMenuStrip.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { DescriptionCol, DateCol, TimeCol });
            dataGridView1.ContextMenuStrip = gridViewContextMenuStrip;
            dataGridView1.Location = new Point(10, 12);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new Size(414, 299);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            // 
            // DescriptionCol
            // 
            DescriptionCol.HeaderText = "Description";
            DescriptionCol.Name = "DescriptionCol";
            DescriptionCol.ReadOnly = true;
            DescriptionCol.Width = 200;
            // 
            // DateCol
            // 
            DateCol.HeaderText = "Date";
            DateCol.Name = "DateCol";
            DateCol.ReadOnly = true;
            DateCol.Width = 75;
            // 
            // TimeCol
            // 
            TimeCol.HeaderText = "Time";
            TimeCol.Name = "TimeCol";
            TimeCol.ReadOnly = true;
            TimeCol.Width = 75;
            // 
            // gridViewContextMenuStrip
            // 
            gridViewContextMenuStrip.Items.AddRange(new ToolStripItem[] { CopyDescriptionContextMenu, CopyTimeContextMenu });
            gridViewContextMenuStrip.Name = "gridViewContextMenuStrip";
            gridViewContextMenuStrip.Size = new Size(166, 48);
            // 
            // CopyDescriptionContextMenu
            // 
            CopyDescriptionContextMenu.Name = "CopyDescriptionContextMenu";
            CopyDescriptionContextMenu.Size = new Size(165, 22);
            CopyDescriptionContextMenu.Text = "Copy Description";
            CopyDescriptionContextMenu.Click += CopyDescriptionContextMenu_Click;
            // 
            // CopyTimeContextMenu
            // 
            CopyTimeContextMenu.Name = "CopyTimeContextMenu";
            CopyTimeContextMenu.Size = new Size(165, 22);
            CopyTimeContextMenu.Text = "Copy Time";
            CopyTimeContextMenu.Click += CopyTimeContextMenu_Click;
            // 
            // BtnClearTimers
            // 
            BtnClearTimers.Location = new Point(318, 317);
            BtnClearTimers.Name = "BtnClearTimers";
            BtnClearTimers.Size = new Size(106, 23);
            BtnClearTimers.TabIndex = 3;
            BtnClearTimers.Text = "Clear All Timers";
            BtnClearTimers.UseVisualStyleBackColor = true;
            BtnClearTimers.Click += BtnClearTimers_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabel2 });
            statusStrip1.Location = new Point(0, 346);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(436, 22);
            statusStrip1.TabIndex = 4;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(64, 17);
            toolStripStatusLabel1.Text = "Total Time:";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(49, 17);
            toolStripStatusLabel2.Text = "00:00:00";
            // 
            // BtnResumeTimer
            // 
            BtnResumeTimer.Location = new Point(209, 317);
            BtnResumeTimer.Name = "BtnResumeTimer";
            BtnResumeTimer.Size = new Size(103, 23);
            BtnResumeTimer.TabIndex = 5;
            BtnResumeTimer.Text = "Resume Timer";
            BtnResumeTimer.UseVisualStyleBackColor = true;
            BtnResumeTimer.Click += BtnResumeTimer_Click;
            // 
            // BtnDeleteSelectedTime
            // 
            BtnDeleteSelectedTime.Location = new Point(71, 317);
            BtnDeleteSelectedTime.Name = "BtnDeleteSelectedTime";
            BtnDeleteSelectedTime.Size = new Size(132, 23);
            BtnDeleteSelectedTime.TabIndex = 6;
            BtnDeleteSelectedTime.Text = "Delete Selected Time";
            BtnDeleteSelectedTime.UseVisualStyleBackColor = true;
            BtnDeleteSelectedTime.Click += BtnDeleteSelectedTime_Click;
            // 
            // FrmTimers
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(436, 368);
            Controls.Add(BtnDeleteSelectedTime);
            Controls.Add(BtnResumeTimer);
            Controls.Add(statusStrip1);
            Controls.Add(BtnClearTimers);
            Controls.Add(dataGridView1);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmTimers";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Timers";
            KeyDown += FrmTimers_KeyDown;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            gridViewContextMenuStrip.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private Button BtnClearTimers;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private DataGridViewTextBoxColumn DescriptionCol;
        private DataGridViewTextBoxColumn DateCol;
        private DataGridViewTextBoxColumn TimeCol;
        private ContextMenuStrip gridViewContextMenuStrip;
        private ToolStripMenuItem CopyDescriptionContextMenu;
        private ToolStripMenuItem CopyTimeContextMenu;
        private Button BtnResumeTimer;
        private Button BtnDeleteSelectedTime;
    }
}
using Notepad_Light.Helpers;

namespace Notepad_Light.Forms
{
    public partial class FrmTimers : Form
    {
        public string resumeTime = string.Empty;
        public string resumeDescription = string.Empty;
        public bool isResumeTimer = false;

        private int totalHours = 0;
        private int totalMinutes = 0;
        private int totalSeconds = 0;

        public FrmTimers()
        {
            InitializeComponent();
            if (Properties.Settings.Default.TimersList.Count > 0)
            {
                UpdateTimeDisplay();
            }

            if (Properties.Settings.Default.DarkMode)
            {
                statusStrip1.BackColor = Color.Black;
                toolStripStatusLabel1.ForeColor = Color.White;
                toolStripStatusLabel2.ForeColor = Color.White;
            }
            else
            {
                statusStrip1.BackColor = Color.White;
                toolStripStatusLabel1.ForeColor = Color.Black;
                toolStripStatusLabel2.ForeColor = Color.Black;
            }
        }

        public void UpdateTotalTime()
        {
            totalHours = 0;
            totalMinutes = 0;
            totalSeconds = 0;

            foreach (DataGridViewRow dgvr in dataGridView1.Rows)
            {
                totalHours = totalHours + Convert.ToInt32(dgvr.Cells[0].Value);
                totalMinutes = totalMinutes + Convert.ToInt32(dgvr.Cells[1].Value);
                totalSeconds = totalSeconds + Convert.ToInt32(dgvr.Cells[2].Value);
            }

            // update the total time
            TimeSpan ts = new TimeSpan();
            ts = ts.Add(new TimeSpan(totalHours, totalMinutes, totalSeconds));
            toolStripStatusLabel2.Text = ts.ToString();
        }

        public void UpdateTimeDisplay()
        {
            try
            {
                totalHours = 0;
                totalMinutes = 0;
                totalSeconds = 0;

                foreach (string? s in Properties.Settings.Default.TimersList)
                {
                    string[] timerData = s!.Split(Strings.pipeDelim);
                    int n = dataGridView1.Rows.Add();
                    dataGridView1.Rows[n].Cells[0].Value = timerData[0];
                    dataGridView1.Rows[n].Cells[1].Value = timerData[1];
                    dataGridView1.Rows[n].Cells[2].Value = timerData[2];

                    // parse out the time to add to the total time
                    string[] totalTimerData = timerData[2].Split(Strings.semiColonNoSpaces);
                    totalHours = totalHours + Convert.ToInt32(totalTimerData[0]);
                    totalMinutes = totalMinutes + Convert.ToInt32(totalTimerData[1]);
                    totalSeconds = totalSeconds + Convert.ToInt32(totalTimerData[2]);
                }

                // update the total time
                TimeSpan ts = new TimeSpan();
                ts = ts.Add(new TimeSpan(totalHours, totalMinutes, totalSeconds));
                toolStripStatusLabel2.Text = ts.ToString();
            }
            catch (Exception ex)
            {
                App.WriteErrorLogContent(ex.Message, Strings.errorLogFile);
                ResetTimers();
            }
        }

        private void BtnClearTimers_Click(object sender, EventArgs e)
        {
            // clear the app setting, then the grid and finally update the total time
            Properties.Settings.Default.TimersList.Clear();
            dataGridView1.Rows.Clear();
            UpdateTimeDisplay();
        }

        private void CopyDescriptionContextMenu_Click(object sender, EventArgs e)
        {
            try
            {
                // don't do anything if the grid is empty
                if (dataGridView1.Rows.Count == 0 || dataGridView1.CurrentCell == null || dataGridView1.CurrentCell.Value == null)
                {
                    return;
                }
                var cellValue = dataGridView1.CurrentCell.Value.ToString();
                if (!string.IsNullOrEmpty(cellValue))
                {
                    Clipboard.SetText(cellValue);
                }
            }
            catch (Exception ex)
            {
                App.WriteErrorLogContent("CopyDescription Exception = " + ex.Message, Strings.errorLogFile);
            }
        }

        private void CopyTimeContextMenu_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count == 0 || dataGridView1.CurrentRow == null)
                {
                    return;
                }
                var cellValue = dataGridView1.CurrentRow.Cells[2].Value;
                if (cellValue is string text && !string.IsNullOrEmpty(text))
                {
                    Clipboard.SetText(text);
                }
            }
            catch (Exception ex)
            {
                App.WriteErrorLogContent("CopyTime Exception = " + ex.Message, Strings.errorLogFile);
            }
        }

        private void FrmTimers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) { Close(); }
        }

        public void ResumeTimer(bool fromDoubleClick)
        {
            // check for no selection
            if (dataGridView1.CurrentCell is null || dataGridView1.SelectedCells.Count == 0)
            {
                return;
            }

            if (dataGridView1.CurrentCell.Value is not null)
            {
                // Fix: Ensure CurrentRow is not null before dereferencing
                if (dataGridView1.CurrentRow != null)
                {
                    resumeTime = dataGridView1.CurrentRow.Cells[2].Value?.ToString() ?? string.Empty;
                    resumeDescription = dataGridView1.CurrentRow.Cells[0].Value?.ToString() ?? string.Empty;
                }
                else
                {
                    resumeTime = string.Empty;
                    resumeDescription = string.Empty;
                }
                isResumeTimer = true;
                
                dataGridView1.Rows.RemoveAt(dataGridView1.CurrentCellAddress.Y);
                Properties.Settings.Default.TimersList.Clear();

                // update the timer list
                foreach (DataGridViewRow dgvr in dataGridView1.Rows)
                {
                    if (dgvr is null) { return; }

                    // add the timer data to the app setting
                    string[] timerData = new string[3];
                    timerData[0] = dgvr.Cells[0].Value?.ToString() ?? string.Empty;
                    timerData[1] = dgvr.Cells[1].Value?.ToString() ?? string.Empty;
                    timerData[2] = dgvr.Cells[2].Value?.ToString() ?? string.Empty;
                    Properties.Settings.Default.TimersList.Add(string.Join(Strings.pipeDelim, timerData));
                }
            }
            Close();
        }

        private void BtnResumeTimer_Click(object sender, EventArgs e)
        {
            ResumeTimer(false);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ResumeTimer(true);
        }

        /// <summary>
        /// if the timer list gets corrupted somehow, clear timers so the app doesn't crash
        /// </summary>
        public void ResetTimers()
        {
            Properties.Settings.Default.TimersList.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
        }

        private void BtnDeleteSelectedTime_Click(object sender, EventArgs e)
        {
            try
            {
                int n = dataGridView1.CurrentCellAddress.Y;

                // check for no selection
                if (dataGridView1.CurrentCell is null || dataGridView1.SelectedCells.Count == 0)
                {
                    return;
                }
                else
                {
                    // delete the selected row
                    dataGridView1.Rows.RemoveAt(n);
                    Properties.Settings.Default.TimersList.Clear();

                    // update the timer list
                    foreach (DataGridViewRow dgvr in dataGridView1.Rows)
                    {
                        if (dgvr is null) { return; }

                        // add the timer data to the app setting
                        string[] timerData = new string[3];
                        timerData[0] = dgvr.Cells[0].Value?.ToString() ?? string.Empty;
                        timerData[1] = dgvr.Cells[1].Value?.ToString() ?? string.Empty;
                        timerData[2] = dgvr.Cells[2].Value?.ToString() ?? string.Empty;
                        Properties.Settings.Default.TimersList.Add(string.Join(Strings.pipeDelim, timerData));
                    }

                    // update the UI
                    UpdateTotalTime();
                }
            }
            catch (Exception ex)
            {
                App.WriteErrorLogContent(ex.Message, Strings.errorLogFile);
            }
        }
    }
}

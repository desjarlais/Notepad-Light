using Notepad_Light.Helpers;

namespace Notepad_Light.Forms
{
    public partial class FrmTimers : Form
    {
        int totalHours = 0;
        int totalMinutes = 0;
        int totalSeconds = 0;

        public FrmTimers()
        {
            InitializeComponent();
            UpdateTimeDisplay();
        }

        public void UpdateTimeDisplay()
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

                // parse out the time to add to total time
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

        private void BtnClearTimers_Click(object sender, EventArgs e)
        {
            // clear the app setting, then the grid and finally update the total time
            Properties.Settings.Default.TimersList.Clear();
            dataGridView1.Rows.Clear();
            UpdateTimeDisplay();
        }

        private void CopyDescriptionContextMenu_Click(object sender, EventArgs e)
        {
            // don't do anything if the grid is empty
            if (dataGridView1.Rows.Count == 0)
            {
                return;
            }
            Clipboard.SetText(dataGridView1.CurrentCell.Value.ToString());
        }

        private void CopyTimeContextMenu_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                return;
            }
            Clipboard.SetText(dataGridView1.CurrentRow.Cells[2].Value.ToString());
        }

        private void FrmTimers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) { Close(); }
        }
    }
}

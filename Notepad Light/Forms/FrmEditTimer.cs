using System;
using Notepad_Light.Helpers;

namespace Notepad_Light.Forms
{
    public partial class FrmEditTimer : Form
    {
        private bool _isAdjustedTime;
        private bool _timeErrorEntered;

        public FrmEditTimer(string fromMainForm)
        {
            InitializeComponent();

            _timeErrorEntered = false;
            _isAdjustedTime = false;
            lblOriginalTime.Text = fromMainForm;

            mskTxtBxNewTime.ValidatingType = typeof(DateTime);
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            mskTxtBxNewTime.TypeValidationCompleted += TypeValidationCompleted;
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
        }

        private void TypeValidationCompleted(object sender, TypeValidationEventArgs e)
        {
            // invalid text warning should only be displayed if the user pressed adjust AND the text box text is not valid
            if (e.IsValidInput || !_isAdjustedTime) return;

            MessageBox.Show("Time must be in the format HH:MM.", "Invalid input",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // reset the time value to 0, otherwise the value will stay with the invalid input
            mskTxtBxNewTime.Text = Strings.zeroEditTime;
            _timeErrorEntered = true;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            _isAdjustedTime = true;
            mskTxtBxNewTime.ValidateText();

            // Set the adjustedlaborproperty with the new time value so the main form can handle the time as needed
            if (!mskTxtBxNewTime.MaskCompleted) return;
            string adjustedtime = mskTxtBxNewTime.Text;
            char[] adjDelim = { ':' };
            string[] adjArray = adjustedtime.Split(adjDelim);
            string adjHours = adjArray.ElementAt(0);
            string adjMinutes = adjArray.ElementAt(1);
            string adjustedtime2 = adjHours + ":" + adjMinutes + ":00";

            // check if this was an error that caused 00:00 to be added
            // if it was an error, reset the value back to the original
            if (_timeErrorEntered)
            {
                adjustedtime2 = lblOriginalTime.Text;
            }

            if (Owner is FrmMain f)
            {
                f.EditedTime = adjustedtime2;
            }

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (Owner is FrmMain f)
            {
                f.EditedTime = lblOriginalTime.Text.TrimStart();
            }

            Close();
        }

        private void mskTxtBxNewTime_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnOk.PerformClick();
            }
        }
    }
}

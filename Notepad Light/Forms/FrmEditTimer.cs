using Notepad_Light.Helpers;

namespace Notepad_Light.Forms
{
    public partial class FrmEditTimer : Form
    {
        public bool _isAdjustedTime;
        public bool _timeErrorEntered;

        public FrmEditTimer(string fromMainForm)
        {
            InitializeComponent();

            _timeErrorEntered = false;
            _isAdjustedTime = false;
            lblOriginalTime.Text = fromMainForm;

            mskTxtBxNewTime.ValidatingType = typeof(DateTime);
            mskTxtBxNewTime.TypeValidationCompleted += TypeValidationCompleted!;
        }

        private void TypeValidationCompleted(object sender, TypeValidationEventArgs e)
        {
            // invalid text warning should only be displayed if the user pressed adjust AND the text box text is not valid
            if (e.IsValidInput || !_isAdjustedTime) return;

            MessageBox.Show("Time must be in the format HH:MM.", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // reset the time value to 0, otherwise the value will stay with the invalid input
            mskTxtBxNewTime.Text = Strings.zeroHRMIN;
            _timeErrorEntered = true;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            _isAdjustedTime = true;
            mskTxtBxNewTime.ValidateText();

            // Set the adjustedlaborproperty with the new time value so the main form can handle the time as needed
            if (!mskTxtBxNewTime.MaskCompleted) return;

            if (Owner is FrmMain f)
            {
                // check if this was an error that caused 00:00 to be added
                // if it was an error, reset the value back to the original
                if (_timeErrorEntered)
                {
                    f.EditedTime = lblOriginalTime.Text;
                }
                else
                {
                    f.EditedTime = App.ConvertTimeValueHRMIN(mskTxtBxNewTime.Text);
                }
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

        private void FrmEditTimer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}

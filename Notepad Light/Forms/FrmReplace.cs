namespace Notepad_Light.Forms
{
    public partial class FrmReplace : Form
    {
        private readonly RichTextBox _rtb;
        private int _searchIndex;

        public FrmReplace(RichTextBox rtb)
        {
            InitializeComponent();
            _rtb = rtb;
        }

        public FrmReplace(RichTextBox rtb, string initialFindText) : this(rtb)
        {
            tbxFind.Text = initialFindText;
        }

        private RichTextBoxFinds BuildFindOptions()
        {
            RichTextBoxFinds options = RichTextBoxFinds.None;
            if (cbxMatchCase.Checked)
                options |= RichTextBoxFinds.MatchCase;
            if (cbxWholeWord.Checked)
                options |= RichTextBoxFinds.WholeWord;
            return options;
        }

        private int FindNext(bool wrapAround)
        {
            if (string.IsNullOrEmpty(tbxFind.Text))
                return -1;

            RichTextBoxFinds options = BuildFindOptions();
            int index = _rtb.Find(tbxFind.Text, _searchIndex, options);

            if (index < 0 && wrapAround && _searchIndex > 0)
            {
                // wrap to beginning
                _searchIndex = 0;
                index = _rtb.Find(tbxFind.Text, _searchIndex, options);
            }

            if (index >= 0)
            {
                _rtb.Select(index, tbxFind.Text.Length);
                _rtb.ScrollToCaret();
                _searchIndex = index + tbxFind.Text.Length;
                UpdateStatus(string.Empty);
            }
            else
            {
                _searchIndex = 0;
                UpdateStatus("No match found.");
            }

            return index;
        }

        private void BtnFindNext_Click(object sender, EventArgs e)
        {
            FindNext(true);
        }

        private void BtnReplace_Click(object sender, EventArgs e)
        {
            // if nothing is currently selected or selection doesn't match, find next first
            if (_rtb.SelectionLength == 0 || !IsSelectionMatch())
            {
                if (FindNext(true) < 0)
                    return;
            }

            _rtb.SelectedText = tbxReplace.Text;
            _rtb.Modified = true;
            FindNext(true);
        }

        private void BtnReplaceAll_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbxFind.Text))
                return;

            RichTextBoxFinds options = BuildFindOptions();
            int count = 0;
            int startPos = 0;

            // suspend layout for performance
            _rtb.SuspendLayout();

            while (startPos < _rtb.TextLength)
            {
                int index = _rtb.Find(tbxFind.Text, startPos, options);
                if (index < 0)
                    break;

                _rtb.Select(index, tbxFind.Text.Length);
                _rtb.SelectedText = tbxReplace.Text;
                startPos = index + tbxReplace.Text.Length;
                count++;
            }

            _rtb.ResumeLayout();

            if (count > 0)
            {
                _rtb.Modified = true;
                UpdateStatus($"{count} replacement{(count == 1 ? "" : "s")} made.");
            }
            else
            {
                UpdateStatus("No match found.");
            }
        }

        private bool IsSelectionMatch()
        {
            if (_rtb.SelectionLength == 0)
                return false;

            StringComparison comparison = cbxMatchCase.Checked
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            return _rtb.SelectedText.Equals(tbxFind.Text, comparison);
        }

        private void UpdateStatus(string message)
        {
            lblStatus.Text = message;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FrmReplace_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) { Close(); }
        }

        private void TbxFind_TextChanged(object sender, EventArgs e)
        {
            bool hasText = tbxFind.Text.Length > 0;
            BtnFindNext.Enabled = hasText;
            BtnReplace.Enabled = hasText;
            BtnReplaceAll.Enabled = hasText;
            _searchIndex = 0;
            UpdateStatus(string.Empty);
        }
    }
}

namespace Notepad_Light.Forms
{
    public partial class FrmReplace : Form
    {
        private List<string> textToSearch = new List<string>();
        public int resultIndex = 0;
        private int searchCount = 0;
        public string replaceText = string.Empty;
        public bool replaceAll = false;
        public string findText = string.Empty;
        public bool formExited = true;
        public int prevLineIndex = 0;

        public FrmReplace(List<string> lines)
        {
            InitializeComponent();
            textToSearch = lines;
        }

        /// <summary>
        /// loop each line and see if any text matches
        /// prevLineIndex is used outside this form in the main form for replace feature
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFindNext_Click(object sender, EventArgs e)
        {
            searchCount = 0;
            resultIndex = 0;
            foreach (string line in textToSearch)
            {
                if (line.Contains(tbxFind.Text) && searchCount != prevLineIndex)
                {
                    BtnReplace.Enabled = true;
                    RtbSearchResults.Text = line;
                    resultIndex = searchCount;
                    prevLineIndex = resultIndex;
                    return;
                }
                searchCount++;
            }
        }

        private void BtnReplace_Click(object sender, EventArgs e)
        {
            if (cbxReplaceAll.Checked)
            {
                replaceAll = true;
            }

            replaceText = tbxReplace.Text;
            findText = tbxFind.Text;
            formExited = false;
            Close();
        }

        private void FrmReplace_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}

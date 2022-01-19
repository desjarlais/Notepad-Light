namespace Notepad_Light.Forms
{
    public partial class FrmOptions : Form
    {
        public FrmOptions()
        {
            InitializeComponent();

            // update options
            ckbUsePasteUI.Checked = Properties.Settings.Default.UsePasteUI;
            cboNewDocFormat.Text = Properties.Settings.Default.NewDocumentFormat;
                        
            // check the file MRU and populate the list
            if (Properties.Settings.Default.FileMRU.Count > 0)
            {
                UpdateMRUListbox();   
            }

            // update the find direction
            switch (Properties.Settings.Default.SearchOption.ToString())
            {
                case "Up": rdoFindDirectionUp.Checked = true; break;
                case "Down": rdoFindDirectionDown.Checked = true; break;
                case "MatchCase": rdoMatchCase.Checked = true; break;
                case "WholeWord": rdoWholeWord.Checked = true; break;
            }
        }

        public void UpdateMRUListbox()
        {
            if (Properties.Settings.Default.FileMRU.Count > 0)
            {
                foreach (var f in Properties.Settings.Default.FileMRU)
                {
                    if (f is not null)
                    {
                        LstMRU.Items.Add(f);
                    }
                }
            }
        }

        private void BtnClearMRU_Click(object sender, EventArgs e)
        {
            // clear all items from the file MRU
            Properties.Settings.Default.FileMRU.Clear();
            UpdateMRUListbox();
            LstMRU.Items.Clear();
            Properties.Settings.Default.Save();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.UsePasteUI = ckbUsePasteUI.Checked;
            Properties.Settings.Default.NewDocumentFormat = cboNewDocFormat.Text;
            Properties.Settings.Default.Save();

            if (rdoFindDirectionUp.Checked == true)
            {
                Properties.Settings.Default.SearchOption = "Up";
            }
            else if (rdoMatchCase.Checked == true)
            {
                Properties.Settings.Default.SearchOption = "MatchCase";
            }
            else if (rdoWholeWord.Checked == true)
            {
                Properties.Settings.Default.SearchOption = "WholeWord";
            }
            else
            {
                Properties.Settings.Default.SearchOption = "Down";
            }

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

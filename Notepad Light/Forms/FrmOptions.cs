using Notepad_Light.Helpers;

namespace Notepad_Light.Forms
{
    public partial class FrmOptions : Form
    {
        public FrmOptions()
        {
            InitializeComponent();

            // update options
            ckbUsePasteUI.Checked = Properties.Settings.Default.UsePasteUI;
                        
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

            // update theme
            if (Properties.Settings.Default.DarkMode)
            {
                rdoDarkMode.Checked = true;
            }
            else
            {
                rdoLightMode.Checked = true;
            }

            // update reverse text
            if (Properties.Settings.Default.ReverseTextColorWithTheme == true)
            {
                ckbReverseTextColor.Checked = true;
            }
            else
            {
                ckbReverseTextColor.Checked = false;
            }

            // update file format dropdown
            if (Properties.Settings.Default.NewFileFormat == Strings.plainText)
            {
                cbxNewFileFormat.SelectedIndex = 0;
            }
            else
            {
                cbxNewFileFormat.SelectedIndex = 1;
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
            Properties.Settings.Default.Save();
            Properties.Settings.Default.AutoSaveInterval = (int)nudAutoSaveInterval.Value;

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

            if (rdoDarkMode.Checked == true)
            {
                Properties.Settings.Default.DarkMode = true;
            }
            else
            {
                Properties.Settings.Default.DarkMode = false;
            }

            if (ckbReverseTextColor.Checked == true)
            {
                Properties.Settings.Default.ReverseTextColorWithTheme = true;
            }
            else
            {
                Properties.Settings.Default.ReverseTextColorWithTheme = false;
            }

            if (cbxNewFileFormat.SelectedIndex == 0)
            {
                Properties.Settings.Default.NewFileFormat = Strings.plainText;
            }
            else
            {
                Properties.Settings.Default.NewFileFormat = Strings.rtf;
            }

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

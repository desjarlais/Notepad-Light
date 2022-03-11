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
                case Strings.findUp: rdoFindDirectionUp.Checked = true; break;
                case Strings.findDown: rdoFindDirectionDown.Checked = true; break;
                case Strings.findMatchCase: rdoMatchCase.Checked = true; break;
                case Strings.findWholeWord: rdoWholeWord.Checked = true; break;
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

            // update file format dropdown
            if (Properties.Settings.Default.NewFileFormat == Strings.plainText)
            {
                cbxNewFileFormat.SelectedIndex = 0;
            }
            else
            {
                cbxNewFileFormat.SelectedIndex = 1;
            }

            // update remove temp files
            if (Properties.Settings.Default.RemoveTempFilesOnExit)
            {
                cbxCleanupTempAppFilesOnExit.Checked = true;
            }
            else
            {
                cbxCleanupTempAppFilesOnExit.Checked = false;
            }

            // update image transparency
            if (Properties.Settings.Default.UseImageTransparency)
            {
                cbxInsertPictureWithTransparency.Checked = true;
            }
            else
            {
                cbxInsertPictureWithTransparency.Checked = false;
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
                Properties.Settings.Default.SearchOption = Strings.findUp;
            }
            else if (rdoMatchCase.Checked == true)
            {
                Properties.Settings.Default.SearchOption = Strings.findMatchCase;
            }
            else if (rdoWholeWord.Checked == true)
            {
                Properties.Settings.Default.SearchOption = Strings.findWholeWord;
            }
            else
            {
                Properties.Settings.Default.SearchOption = Strings.findDown;
            }

            if (rdoDarkMode.Checked == true)
            {
                Properties.Settings.Default.DarkMode = true;
            }
            else
            {
                Properties.Settings.Default.DarkMode = false;
            }

            if (cbxNewFileFormat.SelectedIndex == 0)
            {
                Properties.Settings.Default.NewFileFormat = Strings.plainText;
            }
            else
            {
                Properties.Settings.Default.NewFileFormat = Strings.rtf;
            }

            if (cbxCleanupTempAppFilesOnExit.Checked == true)
            {
                Properties.Settings.Default.RemoveTempFilesOnExit = true;
            }
            else
            {
                Properties.Settings.Default.RemoveTempFilesOnExit = false;
            }

            if (cbxInsertPictureWithTransparency.Checked == true)
            {
                Properties.Settings.Default.UseImageTransparency = true;
            }
            else
            {
                Properties.Settings.Default.UseImageTransparency= false;
            }

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

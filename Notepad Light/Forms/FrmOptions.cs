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
            else if (Properties.Settings.Default.NewFileFormat == Strings.rtf)
            {
                cbxNewFileFormat.SelectedIndex = 1;
            }
            else
            {
                cbxNewFileFormat.SelectedIndex = 2;
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

            // update date/time insert for templates
            if (Properties.Settings.Default.IncludeDateTimeWithTemplates)
            {
                cbxIncludeDateTimeWithTemplates.Checked = true;
            }
            else
            {
                cbxIncludeDateTimeWithTemplates.Checked = false;
            }

            // update rtt paste option
            if (Properties.Settings.Default.PasteRtfUnformatted)
            {
                ckbPasteRtfUnformatted.Checked = true;
            }
            else
            {
                ckbPasteRtfUnformatted.Checked = false;
            }

            // update rtt paste option
            if (Properties.Settings.Default.CheckSpellingAsYouType)
            {
                ckbCheckSpellingAsYouType.Checked = true;
            }
            else
            {
                ckbCheckSpellingAsYouType.Checked = false;
            }

            // populate spell check language dropdown
            PopulateSpellCheckLanguages();

            // update autosave interval
            nudAutoSaveInterval.Value = Properties.Settings.Default.AutoSaveInterval;
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

        /// <summary>
        /// Discovers available dictionary files and populates the spell check language dropdown.
        /// </summary>
        private void PopulateSpellCheckLanguages()
        {
            var languages = SpellCheckService.GetAvailableLanguages();
            string savedLanguage = Properties.Settings.Default.SpellCheckLanguage;
            int selectedIndex = 0;

            for (int i = 0; i < languages.Count; i++)
            {
                cbxSpellCheckLanguage.Items.Add(SpellCheckService.GetLanguageDisplayName(languages[i]));
                if (languages[i].Equals(savedLanguage, StringComparison.OrdinalIgnoreCase))
                {
                    selectedIndex = i;
                }
            }

            // store the language codes for lookup when saving
            cbxSpellCheckLanguage.Tag = languages;

            if (cbxSpellCheckLanguage.Items.Count > 0)
            {
                cbxSpellCheckLanguage.SelectedIndex = selectedIndex;
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
            else if (cbxNewFileFormat.SelectedIndex == 1)
            {
                Properties.Settings.Default.NewFileFormat = Strings.rtf;
            }
            else
            {
                Properties.Settings.Default.NewFileFormat = Strings.markdown;
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
                Properties.Settings.Default.UseImageTransparency = false;
            }

            if (cbxIncludeDateTimeWithTemplates.Checked == true)
            {
                Properties.Settings.Default.IncludeDateTimeWithTemplates = true;
            }
            else
            {
                Properties.Settings.Default.IncludeDateTimeWithTemplates = false;
            }

            if (ckbPasteRtfUnformatted.Checked == true)
            {
                Properties.Settings.Default.PasteRtfUnformatted = true;
            }
            else
            {
                Properties.Settings.Default.PasteRtfUnformatted = false;
            }

            if (ckbClearTimersOnExit.Checked == true)
            {
                Properties.Settings.Default.ClearTimersOnExit = true;
            }
            else
            {
                Properties.Settings.Default.ClearTimersOnExit = false;
            }

            if (ckbCheckSpellingAsYouType.Checked == true)
            {
                Properties.Settings.Default.CheckSpellingAsYouType = true;
            }
            else
            {
                Properties.Settings.Default.CheckSpellingAsYouType = false;
            }

            // save spell check language
            if (cbxSpellCheckLanguage.Tag is List<string> langCodes && cbxSpellCheckLanguage.SelectedIndex >= 0)
            {
                Properties.Settings.Default.SpellCheckLanguage = langCodes[cbxSpellCheckLanguage.SelectedIndex];
            }

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FrmOptions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) { Close(); }
        }
    }
}

using Notepad_Light.Forms;
using Notepad_Light.Helpers;

namespace Notepad_Light
{
    public partial class FrmMain : Form
    {
        // globals
        public string gCurrentFileName = Strings.defaultFileName;
        public bool gChanged = false;
        public bool gRtf = false;
        public int gPrevPageLength = 0;

        public FrmMain()
        {
            InitializeComponent();

            // init file MRU
            if (Properties.Settings.Default.FileMRU.Count > 0)
            {
                int fileMruDeleteIndex = 0;
                int fileMruIndex = 0;
                bool deleteInitValue = false;

                foreach (var f in Properties.Settings.Default.FileMRU)
                {
                    if (f == "#### File MRU ####")
                    {
                        fileMruDeleteIndex = fileMruIndex;
                        deleteInitValue = true;
                    }
                    else
                    {
                        recentToolStripMenuItem.DropDownItems.Add(f);
                        fileMruIndex++;
                    }
                }

                if (deleteInitValue)
                {
                    Properties.Settings.Default.FileMRU.RemoveAt(fileMruDeleteIndex);
                }
            }

            // set initial zoom to 100
            zoomToolStripMenuItem100.Checked = true;
            ApplyZoom(1.0f);

            // set wordwrap
            wordWrapToolStripMenuItem.Checked = rtbPage.WordWrap;

            // set filename
            UpdateFormTitle(gCurrentFileName);

            // enable icons based on default
            UpdateStatusBar();
        }

        #region Functions

        public void UpdateStatusBar()
        {
            if (Properties.Settings.Default.NewDocRtf == true || gCurrentFileName.EndsWith(".rtf"))
            {
                EnableToolbarFormattingIcons();
                toolStripStatusLabelFileType.Text = Strings.rtf;
            }
            else
            {
                DisableToolbarFormattingIcons();
                toolStripStatusLabelFileType.Text = Strings.plainText;
            }
        }

        public void UpdateFormTitle(string docName)
        {
            this.Text = "Notepad Light - " + docName;
            gCurrentFileName = docName;
        }

        public void CreateNewDocument()
        {
            rtbPage.Clear();
            gChanged = false;
            UpdateFormTitle(Strings.defaultFileName);
            UpdateStatusBar();
            ClearToolbarFormattingIcons();
        }

        public void FileNew()
        {
            // clear the textbox
            if (gChanged == false)
            {
                CreateNewDocument();
            }
            else if (gChanged == true)
            {
                DialogResult result = MessageBox.Show("Do you want to save your changes?", "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    FileSaveAs();
                    CreateNewDocument();
                }
                else if (result == DialogResult.No)
                {
                    CreateNewDocument();
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void FileOpen()
        {
            try
            {
                string fUserPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // get the file
                OpenFileDialog ofdFileOpen = new OpenFileDialog
                {
                    Title = "Select File To Open.",
                    Filter = "Text Files | *.txt; *.rtf; ",
                    RestoreDirectory = true,
                    InitialDirectory = fUserPath
                };

                // add the contents to the textbox
                if (ofdFileOpen.ShowDialog() == DialogResult.OK)
                {
                    if (ofdFileOpen.FileName.EndsWith(".txt"))
                    {
                        // setup plain text open
                        rtbPage.LoadFile(ofdFileOpen.FileName, RichTextBoxStreamType.PlainText);
                        DisableToolbarFormattingIcons();
                        gRtf = false;
                        toolStripStatusLabelFileType.Text = Strings.plainText;
                    }
                    else
                    {
                        // setup rtf open
                        rtbPage.LoadFile(ofdFileOpen.FileName, RichTextBoxStreamType.RichText);
                        EnableToolbarFormattingIcons();
                        gRtf = true;
                        toolStripStatusLabelFileType.Text = Strings.rtf;
                    }

                    UpdateMRU(ofdFileOpen.FileName);
                    UpdateFormTitle(ofdFileOpen.FileName);
                    gChanged = false;
                    ClearToolbarFormattingIcons();
                    MoveCursorToFirstLine();
                }
            }
            catch (Exception ex)
            {
                Logger.Log("FileOpen Error = " + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void FileSave()
        {
            try
            {
                // if gChanged is false, no changes to save
                if (gChanged == false)
                {
                    return;
                }

                // if gChanged is true, untitled needs to be save as
                // any other file name do regular save of the content
                if (gCurrentFileName.ToString() == Strings.defaultFileName && gChanged == true)
                {
                    FileSaveAs();
                }
                else
                {
                    if (gCurrentFileName.EndsWith(".txt"))
                        {
                        rtbPage.SaveFile(gCurrentFileName, RichTextBoxStreamType.PlainText);
                    }
                    
                    if (gCurrentFileName.EndsWith(".rtf"))
                    {
                        rtbPage.SaveFile(gCurrentFileName, RichTextBoxStreamType.RichText);
                    }

                    gChanged = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("FileSave Error = " + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void FileSaveAs()
        {
            try
            {
                using (SaveFileDialog sfdSaveAs = new SaveFileDialog())
                {
                    sfdSaveAs.Filter = "Plain Text (*.txt)|*.txt | RTF (*.rtf)|*.rtf";
                    sfdSaveAs.Title = "Save As";

                    if (sfdSaveAs.ShowDialog() == DialogResult.OK && sfdSaveAs.FileName.Length > 0)
                    {
                        rtbPage.SaveFile(sfdSaveAs.FileName);
                        UpdateFormTitle(sfdSaveAs.FileName);
                        UpdateStatusBar();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("FileSaveAs Error :\r\n\r\n" + ex.Message);
            }
        }

        public void Cut()
        {
            rtbPage.Cut();
            gChanged = true;
        }

        public void Copy()
        {
            rtbPage.Copy();
        }

        public void Paste()
        {
            rtbPage.Paste();
            gChanged = true;
        }

        public void Undo()
        {
            rtbPage.Undo();
            gChanged = true;
        }

        public void Redo()
        {
            rtbPage.Redo();
            gChanged = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateToolbarIcons()
        {
            try
            {
                // if multiple selections exist like Ctrl+A, disable all buttons
                if (rtbPage.SelectionFont is null)
                {
                    ClearToolbarFormattingIcons();
                    return;
                }

                // bold
                if (rtbPage.SelectionFont.Bold == true)
                {
                    boldToolStripButton.Checked = true;
                }
                else
                {
                    boldToolStripButton.Checked = false;
                }

                // italic
                if (rtbPage.SelectionFont.Italic == true)
                {
                    italicToolStripButton.Checked = true;
                }
                else
                {
                    italicToolStripButton.Checked = false;
                }

                // underline
                if (rtbPage.SelectionFont.Underline == true)
                {
                    underlineToolStripButton.Checked = true;
                }
                else
                {
                    underlineToolStripButton.Checked = false;
                }

                // strikethrough
                if (rtbPage.SelectionFont.Strikeout == true)
                {
                    strikethroughToolStripButton.Checked = true;
                }
                else
                {
                    strikethroughToolStripButton.Checked = false;
                }

                // bullets
                if (rtbPage.SelectionBullet == true)
                {
                    bulletToolStripButton.Checked = true;
                }
                else
                {
                    bulletToolStripButton.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("UpdateToolbarIcons Error" + ex.Message);
            }
        }

        public void ClearFormatting()
        {
            rtbPage.SelectionBullet = false;
            rtbPage.Font = new Font(Properties.Settings.Default.DefaultFontName, Properties.Settings.Default.DefaultFontSize);
            rtbPage.ForeColor = Color.FromName(Properties.Settings.Default.DefaultFontColorName);
            rtbPage.SelectionIndent = 0;
            gChanged = true;
        }

        public void EnableToolbarFormattingIcons()
        {
            boldToolStripButton.Enabled = true;
            italicToolStripButton.Enabled = true;
            underlineToolStripButton.Enabled = true;
            strikethroughToolStripButton.Enabled = true;
            bulletToolStripButton.Enabled = true;
        }

        public void DisableToolbarFormattingIcons()
        {
            boldToolStripButton.Enabled = false;
            italicToolStripButton.Enabled = false;
            underlineToolStripButton.Enabled = false;
            strikethroughToolStripButton.Enabled = false;
            bulletToolStripButton.Enabled = false;
        }

        public void ClearToolbarFormattingIcons()
        {
            boldToolStripButton.Checked = false;
            italicToolStripButton.Checked = false;
            underlineToolStripButton.Checked = false;
            strikethroughToolStripButton.Checked = false;
            bulletToolStripButton.Checked = false;
        }

        public void EndOfButtonFormatWork()
        {
            rtbPage.Focus();
            UpdateToolbarIcons();
            gChanged = true;
        }

        public void ApplyBold()
        {
            if (rtbPage.SelectionFont.Bold == true)
            {
                rtbPage.SelectionFont = new Font(rtbPage.SelectionFont, rtbPage.SelectionFont.Style & ~FontStyle.Bold);
            }
            else
            {
                rtbPage.SelectionFont = new Font(rtbPage.SelectionFont, rtbPage.SelectionFont.Style | FontStyle.Bold);
            }

            EndOfButtonFormatWork();
        }

        public void ApplyItalic()
        {
            if (rtbPage.SelectionFont.Italic == true)
            {
                rtbPage.SelectionFont = new Font(rtbPage.SelectionFont, rtbPage.SelectionFont.Style & ~FontStyle.Italic);
            }
            else
            {
                rtbPage.SelectionFont = new Font(rtbPage.SelectionFont, rtbPage.SelectionFont.Style | FontStyle.Italic);
            }

            EndOfButtonFormatWork();
        }

        public void ApplyUnderline()
        {
            if (rtbPage.SelectionFont.Underline == true)
            {
                rtbPage.SelectionFont = new Font(rtbPage.SelectionFont, rtbPage.SelectionFont.Style & ~FontStyle.Underline);
            }
            else
            {
                rtbPage.SelectionFont = new Font(rtbPage.SelectionFont, rtbPage.SelectionFont.Style | FontStyle.Underline);
            }

            EndOfButtonFormatWork();
        }

        public void ApplyStrikethrough()
        {
            if (rtbPage.SelectionFont.Strikeout == true)
            {
                rtbPage.SelectionFont = new Font(rtbPage.SelectionFont, rtbPage.SelectionFont.Style & ~FontStyle.Strikeout);
            }
            else
            {
                rtbPage.SelectionFont = new Font(rtbPage.SelectionFont, rtbPage.SelectionFont.Style | FontStyle.Strikeout);
            }

            EndOfButtonFormatWork();
        }

        public void MoveCursorToFirstLine()
        {
            if (Properties.Settings.Default.ClearTextOnInsert)
            {
                rtbPage.ResetText();
            }
            else
            {
                rtbPage.SelectionStart = 0;
                rtbPage.SelectionLength = 0;
            }
        }

        public bool IsFileInMru(string filePath)
        {
            bool result = false;

            foreach (var f in Properties.Settings.Default.FileMRU)
            {
                if (f is not null && f == filePath)
                {
                    result = true;
                }
            }

            return result;
        }

        public void UpdateMRU(string filePath)
        {
            if (Properties.Settings.Default.FileMRU.Count == 5)
            {
                // now move each item down the list
                recentToolStripMenuItem.DropDownItems[4].Text = recentToolStripMenuItem.DropDownItems[3].Text;
                Properties.Settings.Default.FileMRU[4] = Properties.Settings.Default.FileMRU[3];

                recentToolStripMenuItem.DropDownItems[3].Text = recentToolStripMenuItem.DropDownItems[2].Text;
                Properties.Settings.Default.FileMRU[3] = Properties.Settings.Default.FileMRU[2];

                recentToolStripMenuItem.DropDownItems[2].Text = recentToolStripMenuItem.DropDownItems[1].Text;
                Properties.Settings.Default.FileMRU[2] = Properties.Settings.Default.FileMRU[1];

                recentToolStripMenuItem.DropDownItems[1].Text = recentToolStripMenuItem.DropDownItems[0].Text;
                Properties.Settings.Default.FileMRU[1] = Properties.Settings.Default.FileMRU[0];

                recentToolStripMenuItem.DropDownItems[0].Text = filePath;
                Properties.Settings.Default.FileMRU[0] = filePath;
            }
            else
            {
                if (IsFileInMru(filePath) == false)
                {
                    recentToolStripMenuItem.DropDownItems.Add(filePath);
                    Properties.Settings.Default.FileMRU.Add(filePath);
                }
            }

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Open the recent item file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFromRecentHandler(object sender, EventArgs e)
        {
            
        }

        public void UpdateLnColValues()
        {
            // update the line and column values
            int line = rtbPage.GetLineFromCharIndex(rtbPage.SelectionStart);
            int column = rtbPage.SelectionStart - rtbPage.GetFirstCharIndexFromLine(line);

            toolStripStatusLabelLine.Text = line.ToString();
            toolStripStatusLabelColumn.Text = column.ToString();
        }

        public void ResetZoomMenu()
        {
            // reset all values
            zoomToolStripMenuItem100.Checked = false;
            zoomToolStripMenuItem150.Checked = false;
            zoomToolStripMenuItem200.Checked = false;
            zoomToolStripMenuItem250.Checked = false;
            zoomToolStripMenuItem300.Checked = false;
        }

        public void ApplyZoom(float zoomPercentage)
        {
            // set the zoom value
            rtbPage.ZoomFactor = zoomPercentage;
            ResetZoomMenu();

            // set the menu item check mark
            switch (zoomPercentage)
            {
                case 1.0f:
                    zoomToolStripMenuItem100.Checked = true;
                    break;
                case 1.5f:
                    zoomToolStripMenuItem150.Checked = true;
                    break;
                case 2.0f:
                    zoomToolStripMenuItem200.Checked = true;
                    break;
                case 2.5f:
                    zoomToolStripMenuItem250.Checked = true;
                    break;
                case 3.0f:
                    zoomToolStripMenuItem300.Checked = true;
                    break;
                default:
                    break;
            }
        }

        #endregion

        private void rtbPage_SelectionChanged(object sender, EventArgs e)
        {
            UpdateLnColValues();

            // check if content was added
            if (gPrevPageLength != rtbPage.TextLength)
            {
                gChanged = true;
            }

            // now update the prevPageLength
            gPrevPageLength = rtbPage.TextLength;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileNew();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileOpen();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSave();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSaveAs();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbPage.SelectAll();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSettings fOptions = new FrmSettings()
            {
                Owner = this
            };
            fOptions.ShowDialog();

            // coming back from settings, adjust file type
            gRtf = Properties.Settings.Default.NewDocRtf;

            // also need to update the file mru in case it was cleared
            if (Properties.Settings.Default.FileMRU.Count == 0)
            {
                recentToolStripMenuItem.DropDownItems.Clear();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAbout fAbout = new FrmAbout()
            {
                Owner = this
            };
            fAbout.ShowDialog();
        }

        private void errorLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmErrorLog fLog = new FrmErrorLog()
            {
                Owner = this
            };
            fLog.ShowDialog();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            FileNew();
        }

        private void boldToolStripButton_Click(object sender, EventArgs e)
        {
            ApplyBold();
        }

        private void italicToolStripButton_Click(object sender, EventArgs e)
        {
            ApplyItalic();
        }

        private void underlineToolStripButton_Click(object sender, EventArgs e)
        {
            ApplyUnderline();
        }

        private void strikethroughToolStripButton_Click(object sender, EventArgs e)
        {
            ApplyStrikethrough();
        }

        private void undoToolStripButton_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void redoToolStripButton_Click(object sender, EventArgs e)
        {
            Redo();
        }

        private void editFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // setup the initial dialog values from settings
            fontDialog1.ShowColor = true;
            fontDialog1.Font = new Font(Properties.Settings.Default.DefaultFontName, Properties.Settings.Default.DefaultFontSize);
            fontDialog1.Color = Color.FromName(Properties.Settings.Default.DefaultFontColorName);

            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                // update richtextbox control
                rtbPage.Font = fontDialog1.Font;
                rtbPage.ForeColor = fontDialog1.Color;

                // update settings
                Properties.Settings.Default.DefaultFontName = fontDialog1.Font.Name;
                Properties.Settings.Default.DefaultFontSize = Convert.ToInt32(fontDialog1.Font.Size);
                Properties.Settings.Default.DefaultFontColorName = fontDialog1.Color.Name;
            }
        }

        private void clearFormattingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearFormatting();
            EndOfButtonFormatWork();
        }

        private void clearAllTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbPage.ResetText();
            rtbPage.Font = new Font(Properties.Settings.Default.DefaultFontName, Properties.Settings.Default.DefaultFontSize);
        }

        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            App.PlatformSpecificProcessStart("");
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            FileOpen();
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            FileSave();
        }

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            Cut();
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            Paste();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void zoomToolStripMenuItem100_Click(object sender, EventArgs e)
        {
            ApplyZoom(1.0f);
            zoomToolStripMenuItem100.Checked = true;
        }

        private void zoomToolStripMenuItem150_Click(object sender, EventArgs e)
        {
            ApplyZoom(1.5f);
            zoomToolStripMenuItem150.Checked = true;
        }

        private void zoomToolStripMenuItem200_Click(object sender, EventArgs e)
        {
            ApplyZoom(2.0f);
            zoomToolStripMenuItem200.Checked = true;
        }

        private void zoomToolStripMenuItem250_Click(object sender, EventArgs e)
        {
            ApplyZoom(2.5f);
            zoomToolStripMenuItem250.Checked = true;
        }

        private void zoomToolStripMenuItem300_Click(object sender, EventArgs e)
        {
            ApplyZoom(3.0f);
            zoomToolStripMenuItem300.Checked = true;
        }

        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbPage.WordWrap == true)
            {
                rtbPage.WordWrap = false;
                wordWrapToolStripMenuItem.Checked = false;
            }
            else
            {
                rtbPage.WordWrap = true;
                wordWrapToolStripMenuItem.Checked = true;
            }
        }

        private void bulletToolStripButton_Click(object sender, EventArgs e)
        {
            // toggle the bullet and set the indent value
            if (rtbPage.SelectionBullet == true)
            {
                rtbPage.SelectionBullet = false;
                rtbPage.SelectionIndent = 0;
            }
            else if (rtbPage.SelectionBullet == false && Properties.Settings.Default.BulletFirstIndent == true)
            {
                rtbPage.SelectionBullet = true;
                rtbPage.SelectionIndent = 30;
            }
            else
            {
                rtbPage.SelectionBullet = true;
                rtbPage.SelectionIndent = 0;
            }

            EndOfButtonFormatWork();
        }

        private void decreaseIndentToolStripButton_Click(object sender, EventArgs e)
        {
            if (rtbPage.SelectionIndent > 0)
            {
                rtbPage.SelectionIndent -= 30;
            }
        }

        private void increaseIndentToolStripButton_Click(object sender, EventArgs e)
        {
            if (rtbPage.SelectionIndent < 150)
            {
                rtbPage.SelectionIndent += 30;
            }
        }

        private void rtbPage_KeyDown(object sender, KeyEventArgs e)
        {
            // if the line has a bullet and the tab was pressed, indent the line
            // need to suppress the key to prevent the cursor from moving around
            if (e.KeyCode == Keys.Tab && rtbPage.SelectionBullet == true)
            {
                e.SuppressKeyPress = true;
                rtbPage.Select(rtbPage.GetFirstCharIndexOfCurrentLine(), 0);
                increaseIndentToolStripButton.PerformClick();
            }

            // if the user deletes the bullet, we need to remove bullet formatting
            if (e.KeyCode == Keys.Back)
            {
                if (rtbPage.SelectionStart == rtbPage.GetFirstCharIndexOfCurrentLine() && rtbPage.SelectionBullet == true)
                {
                    e.SuppressKeyPress = true;
                    bulletToolStripButton.PerformClick();
                }
            }
        }
    }
}
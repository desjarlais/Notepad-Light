using Notepad_Light.Forms;
using Notepad_Light.Helpers;
using System.Diagnostics;

namespace Notepad_Light
{
    public partial class FrmMain : Form
    {
        // globals
        public string gCurrentFileName = Strings.defaultFileName;
        public bool gChanged = false;
        public bool gRtf = false;
        public int gPrevPageLength = 0;
        private int editedHours, editedMinutes, editedSeconds;
        private Stopwatch gStopwatch;
        private TimeSpan tSpan;

        public FrmMain()
        {
            InitializeComponent();

            // init stopwatch for timer
            gStopwatch = new Stopwatch();

            // init file MRU
            if (Properties.Settings.Default.FileMRU.Count > 0)
            {
                // loop each value and add to recent menu
                int mruCount = 1;
                foreach (var f in Properties.Settings.Default.FileMRU)
                {
                    if (f is not null)
                    {
                        switch (mruCount)
                        {
                            case 1: recentToolStripMenuItem1.Text = f.ToString(); break;
                            case 2: recentToolStripMenuItem2.Text = f.ToString(); break;
                            case 3: recentToolStripMenuItem3.Text = f.ToString(); break;
                            case 4: recentToolStripMenuItem4.Text = f.ToString(); break;
                            case 5: recentToolStripMenuItem5.Text = f.ToString(); break;
                        }
                    }
                    
                    mruCount++;
                }
            }

            // set initial zoom to 100
            zoomToolStripMenuItem100.Checked = true;
            ApplyZoom(1.0f);

            // set wordwrap value, filename in title and enable icons
            wordWrapToolStripMenuItem.Checked = rtbPage.WordWrap;
            UpdateFormTitle(gCurrentFileName);
            UpdateStatusBar();
        }

        #region Functions

        public void UpdateStatusBar()
        {
            if (Properties.Settings.Default.NewDocumentFormat == "RTF" || gCurrentFileName.EndsWith(".rtf"))
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
            
            if (Properties.Settings.Default.NewDocumentFormat == "RTF")
            {
                EnableToolbarFormattingIcons();
            }
            else
            {
                ClearToolbarFormattingIcons();
            }
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
        /// when loading plain text files, we need to disable icons for formatting
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadPlainTextFile(string filePath)
        {
            // setup plain text open
            rtbPage.LoadFile(filePath, RichTextBoxStreamType.PlainText);
            DisableToolbarFormattingIcons();
            gRtf = false;
            toolStripStatusLabelFileType.Text = Strings.plainText;
        }

        /// <summary>
        /// when loading rtf files, enable the formatting buttons
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadRtfFile(string filePath)
        {
            // setup rtf open
            rtbPage.LoadFile(filePath, RichTextBoxStreamType.RichText);
            EnableToolbarFormattingIcons();
            gRtf = true;
            toolStripStatusLabelFileType.Text = Strings.rtf;
        }

        /// <summary>
        /// file open from the MRU
        /// </summary>
        /// <param name="filePath"></param>
        public void FileOpen(string filePath)
        {
            try
            {
                if (filePath.EndsWith(".txt"))
                {
                    LoadPlainTextFile(filePath);
                }
                else
                {
                    LoadRtfFile(filePath);
                }

                UpdateMRU();
                UpdateFormTitle(filePath);
                gChanged = false;
                ClearToolbarFormattingIcons();
                MoveCursorToLocation(0, 0);
            }
            catch (Exception ex)
            {
                Logger.Log("FileOpen Error = " + ex.Message);
            }
        }

        /// <summary>
        /// default file open using file dialog
        /// </summary>
        public void FileOpen()
        {
            try
            {
                OpenFileDialog ofdFileOpen = new OpenFileDialog
                {
                    Title = "Select File To Open.",
                    Filter = "Text Files | *.txt; *.rtf; ",
                    RestoreDirectory = true,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                // add the contents to the textbox
                if (ofdFileOpen.ShowDialog() == DialogResult.OK)
                {
                    // first load the file contents
                    if (ofdFileOpen.FileName.EndsWith(".txt"))
                    {
                        LoadPlainTextFile(ofdFileOpen.FileName);
                    }
                    else
                    {
                        LoadRtfFile(ofdFileOpen.FileName);
                    }

                    // update the title with the file path
                    UpdateFormTitle(ofdFileOpen.FileName);

                    // if the file was opened and is in the mru, we don't want to add it
                    // this will check if it exists and if it does, don't update the mru
                    // otherwise, update mru so we can display it in the ui and add to the settings
                    bool isFileInMru = false;

                    foreach (var f in Properties.Settings.Default.FileMRU)
                    {
                        if (f == gCurrentFileName)
                        {
                            isFileInMru = true;
                        }
                    }

                    if (!isFileInMru)
                    {
                        Properties.Settings.Default.FileMRU.Add(ofdFileOpen.FileName);
                        UpdateMRU();
                    }

                    // lastly, set the saved back to false and update the ui and cursor
                    gChanged = false;
                    ClearToolbarFormattingIcons();
                    MoveCursorToLocation(0, 0);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("FileOpen Error = " + ex.Message);
            }
        }

        /// <summary>
        /// save the current file changes
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
        /// save the file using the dialog
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
                        gChanged = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("FileSaveAs Error :\r\n\r\n" + ex.Message);
            }
        }

        public void OpenRecentFile(string filePath, int buttonIndex)
        {
            if (filePath == "empty")
            {
                return;
            }

            if (File.Exists(filePath))
            {
                FileOpen(filePath);
            }
            else
            {
                // file no longer exists, remove from mru
                RemoveFileFromMRU(filePath, buttonIndex);
            }
        }

        public void ClearRecentMenuItems()
        {
            recentToolStripMenuItem1.Text = "empty";
            recentToolStripMenuItem2.Text = "empty";
            recentToolStripMenuItem3.Text = "empty";
            recentToolStripMenuItem4.Text = "empty";
            recentToolStripMenuItem5.Text = "empty";
        }

        /// <summary>
        /// for files that don't exist, we need to update the ui and settings mru list
        /// </summary>
        /// <param name="path"></param>
        /// <param name="index"></param>
        public void RemoveFileFromMRU(string path, int index)
        {
            int mruIndex = 0;
            int badIndex = 0;

            // check if the non-existent file is in the mru
            if (Properties.Settings.Default.FileMRU.Count > 0)
            {
                foreach (var f in Properties.Settings.Default.FileMRU)
                {
                    if (f == path)
                    {
                        badIndex = mruIndex;
                    }

                    mruIndex++;
                }

                // now that we know where the file is, remove it
                Properties.Settings.Default.FileMRU.RemoveAt(badIndex);
            }

            // clear the menu and add back the remaining files
            ClearRecentMenuItems();
            UpdateMRU();
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
            gChanged = true;

            if (Properties.Settings.Default.UsePasteUI == false)
            {
                rtbPage.Paste();
            }
            else
            {
                // show the form so the user can decide what to paste
                FrmPasteUI pFrm = new FrmPasteUI()
                {
                    Owner = this
                };
                pFrm.ShowDialog();
                
                // paste based on user selection
                switch (pFrm.SelectedPasteOption)
                {
                    case Strings.plainText: rtbPage.SelectedText = Clipboard.GetText(TextDataFormat.Text); break;
                    case Strings.pasteHtml: rtbPage.SelectedText = Clipboard.GetText(TextDataFormat.Html); break;
                    case Strings.rtf: rtbPage.SelectedText = Clipboard.GetText(TextDataFormat.Rtf); break;
                    case Strings.pasteUnicode: rtbPage.SelectedText = Clipboard.GetText(TextDataFormat.UnicodeText); break;
                    case Strings.pasteImage: rtbPage.Paste(); break;
                    default: gChanged = false; break;
                }
            }
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
        /// if there are unsaved changes, we need to ask the user what to do
        /// </summary>
        /// <param name="fromFormClosingEvent"></param>
        public void ExitAppWork(bool fromFormClosingEvent)
        {
            if (gChanged == true)
            {
                DialogResult result = MessageBox.Show("Do you want to save your changes?", "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    FileSave();
                }
            }

            // formclosingevent will fire twice if we call app exit anywhere from form closing
            // only call app.exit if we aren't coming from the event
            if (!fromFormClosingEvent)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// refresh the toolbar icons
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

        /// <summary>
        /// given a position, move the cursor to that location in the richtextbox
        /// </summary>
        /// <param name="startLocation"></param>
        /// <param name="length"></param>
        public void MoveCursorToLocation(int startLocation, int length)
        {
            rtbPage.SelectionStart = startLocation;
            rtbPage.SelectionLength = length;
        }

        /// <summary>
        /// populate the recent menu with the file mru settings list
        /// </summary>
        public void UpdateMRU()
        {
            int index = 1;
            foreach (var f in Properties.Settings.Default.FileMRU)
            {
                switch (index)
                {
                    case 1: recentToolStripMenuItem1.Text = f?.ToString(); break;
                    case 2: recentToolStripMenuItem2.Text = f?.ToString(); break;
                    case 3: recentToolStripMenuItem3.Text = f?.ToString(); break;
                    case 4: recentToolStripMenuItem4.Text = f?.ToString(); break;
                    case 5: recentToolStripMenuItem5.Text = f?.ToString(); break;
                }
                index++;
            }
        }

        /// <summary>
        /// get the current cursor position and update the taskbar with the location
        /// </summary>
        public void UpdateLnColValues()
        {
            int line = rtbPage.GetLineFromCharIndex(rtbPage.SelectionStart);
            int column = rtbPage.SelectionStart - rtbPage.GetFirstCharIndexFromLine(line);

            toolStripStatusLabelLine.Text = line.ToString();
            toolStripStatusLabelColumn.Text = column.ToString();
        }

        public void ClearTimeSpanVariables()
        {
            editedHours = 0;
            editedMinutes = 0;
            editedSeconds = 0;
        }

        /// <summary>
        /// clear any check marks for zoom menu items
        /// </summary>
        public void ResetZoomMenu()
        {
            zoomToolStripMenuItem100.Checked = false;
            zoomToolStripMenuItem150.Checked = false;
            zoomToolStripMenuItem200.Checked = false;
            zoomToolStripMenuItem250.Checked = false;
            zoomToolStripMenuItem300.Checked = false;
        }

        /// <summary>
        /// given a float percentage value, clear the menu
        /// and light up the new value then apply the zoom value
        /// </summary>
        /// <param name="zoomPercentage"></param>
        public void ApplyZoom(float zoomPercentage)
        {
            rtbPage.ZoomFactor = zoomPercentage;
            ResetZoomMenu();
            switch (zoomPercentage)
            {
                case 1.0f: zoomToolStripMenuItem100.Checked = true; break;
                case 1.5f: zoomToolStripMenuItem150.Checked = true; break;
                case 2.0f: zoomToolStripMenuItem200.Checked = true; break;
                case 2.5f: zoomToolStripMenuItem250.Checked = true; break;
                case 3.0f: zoomToolStripMenuItem300.Checked = true; break;
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
            if (Properties.Settings.Default.NewDocumentFormat == "RTF")
            {
                gRtf = true;
            }
            else
            {
                gRtf = false;
            }
            
            // also need to update the file mru in case it was cleared
            if (Properties.Settings.Default.FileMRU.Count == 0)
            {
                ClearRecentMenuItems();
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
            App.PlatformSpecificProcessStart(Strings.githubIssues);
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
            ExitAppWork(true);
        }

        private void zoomToolStripMenuItem100_Click(object sender, EventArgs e)
        {
            ApplyZoom(1.0f);
        }

        private void zoomToolStripMenuItem150_Click(object sender, EventArgs e)
        {
            ApplyZoom(1.5f);
        }

        private void zoomToolStripMenuItem200_Click(object sender, EventArgs e)
        {
            ApplyZoom(2.0f);
        }

        private void zoomToolStripMenuItem250_Click(object sender, EventArgs e)
        {
            ApplyZoom(2.5f);
        }

        private void zoomToolStripMenuItem300_Click(object sender, EventArgs e)
        {
            ApplyZoom(3.0f);
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

        private void recentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (sender is not null)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                OpenRecentFile(sender.ToString(), 1);
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }

        private void recentToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (sender is not null)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                OpenRecentFile(sender.ToString(), 2);
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }

        private void recentToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (sender is not null)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                OpenRecentFile(sender.ToString(), 3);
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }

        private void recentToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (sender is not null)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                OpenRecentFile(sender.ToString(), 4);
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }

        private void recentToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (sender is not null)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                OpenRecentFile(sender.ToString(), 5);
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!gStopwatch.IsRunning)
            {
                return;
            }
            else
            {
                TimeSpan tSpan2 = gStopwatch.Elapsed;
                tSpan = gStopwatch.Elapsed;
                tSpan = tSpan2.Add(new TimeSpan(editedHours, editedMinutes, editedSeconds));
                toolStripLabelTimer.Text = $"{tSpan.Hours:00}:{tSpan.Minutes:00}:{tSpan.Seconds:00}";
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExitAppWork(false);
        }

        private void findToolStripButton_Click(object sender, EventArgs e)
        {
            if (findToolStripTextBox.Text == string.Empty)
            {
                return;
            }
            
            if (findToolStripTextBox.Text.Length > 0)
            {
                int indexToText;
                if (Properties.Settings.Default.SearchOption == "Up")
                {
                    // search up the file and find any word match
                    indexToText = rtbPage.Find(findToolStripTextBox.Text, 0, rtbPage.SelectionStart, RichTextBoxFinds.Reverse);
                }
                else if (Properties.Settings.Default.SearchOption == "Down")
                {
                    // search from the top down and find any word match
                    indexToText = rtbPage.Find(findToolStripTextBox.Text, rtbPage.SelectionStart + 1, RichTextBoxFinds.None);
                }
                else if (Properties.Settings.Default.SearchOption == "MatchCase")
                {
                    // only find words that match the case exactly
                    indexToText = rtbPage.Find(findToolStripTextBox.Text, rtbPage.SelectionStart + 1, RichTextBoxFinds.MatchCase);
                }
                else
                {
                    // only find words that have the entire word in the search textbox
                    indexToText = rtbPage.Find(findToolStripTextBox.Text, rtbPage.SelectionStart + 1, RichTextBoxFinds.WholeWord);
                }

                if (indexToText >= 0)
                {
                    MoveCursorToLocation(indexToText, findToolStripTextBox.Text.Length);
                }
            }
        }

        private void toolStripButtonStartStopTimer_Click(object sender, EventArgs e)
        {
            if (gStopwatch.IsRunning)
            {
                // stop the timer
                gStopwatch.Stop();
                toolStripButtonStartStopTimer.Text = Strings.startTimeText;
                toolStripButtonStartStopTimer.Image = Properties.Resources.StatusRun_16x;
            }
            else
            {
                timer1.Enabled = true;
                gStopwatch.Start();
                toolStripButtonStartStopTimer.Text = Strings.stopTimeText;
                toolStripButtonStartStopTimer.Image = Properties.Resources.Stop_16x;
            }
        }

        private void toolStripButtonResetTimer_Click(object sender, EventArgs e)
        {
            gStopwatch.Reset();
            toolStripLabelTimer.Text = Strings.zeroTimer;
            toolStripButtonStartStopTimer.Image = Properties.Resources.StatusRun_16x;
            ClearTimeSpanVariables();
        }
    }
}
using Notepad_Light.Forms;
using Notepad_Light.Helpers;
using System.Diagnostics;
using System.Reflection;
using System.Drawing.Printing;

namespace Notepad_Light
{
    public partial class FrmMain : Form
    {
        // globals
        public string gCurrentFileName = Strings.defaultFileName;
        public string gTempFilePath = string.Empty;
        public string gPrintString = string.Empty;
        public bool gRtf = false;
        public int gPrevPageLength = 0;
        public int autoSaveInterval, autoSaveTicks;
        private string _EditedTime = string.Empty;
        public string unsavedFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private int editedHours, editedMinutes, editedSeconds, charFrom, ticks;
        private Stopwatch gStopwatch;
        private TimeSpan tSpan;
        public Color clrDarkModeBackground, clrDarkModeTextBackground;

        private const char _semiColonDelim = ':';

        public FrmMain()
        {
            InitializeComponent();

            // initialize stopwatch for timer
            gStopwatch = new Stopwatch();

            // initialize dark mode colors
            clrDarkModeBackground = Color.FromArgb(32, 32, 32);
            clrDarkModeTextBackground = Color.FromArgb(96, 96, 96);

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
                            case 1: RecentToolStripMenuItem1.Text = f.ToString(); break;
                            case 2: RecentToolStripMenuItem2.Text = f.ToString(); break;
                            case 3: RecentToolStripMenuItem3.Text = f.ToString(); break;
                            case 4: RecentToolStripMenuItem4.Text = f.ToString(); break;
                            case 5: RecentToolStripMenuItem5.Text = f.ToString(); break;
                        }
                    }
                    
                    mruCount++;
                }
            }

            // update status bar with app version                
            appVersionToolStripStatusLabel.Text = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

            // set initial zoom to 100 and update menu
            zoomToolStripMenuItem100.Checked = true;
            ApplyZoom(1.0f);

            // set word wrap
            WordWrapToolStripMenuItem.Checked = rtbPage.WordWrap;

            // update title, status, toolbars and autosave intervals
            UpdateFormTitle(gCurrentFileName);
            UpdateStatusBar();
            UpdateToolbarIcons();
            UpdateLnColValues();
            UpdateDocStats();
            UpdateAutoSaveInterval();

            // update theme
            if (Properties.Settings.Default.DarkMode)
            {
                ApplyDarkMode();
            }
            else
            {
                ApplyLightMode();
            }

            // start the autosave timer
            autosaveTimer.Start();

            // make sure log file exists, clear it if it does
            gTempFilePath = Path.GetTempPath() + "\\NotepadLightErrorLog.txt";
            if (!File.Exists(gTempFilePath))
            {
                File.Create(gTempFilePath);
            }
            else
            {
                File.WriteAllText(gTempFilePath, string.Empty);
            }

            // change ui if the default is rtf
            if (Properties.Settings.Default.NewFileFormat == Strings.rtf)
            {
                CreateNewDocument();
            }
        }

        #region Class Properties
        /// <summary>
        /// used for the adjust labor dialog return value
        /// </summary>
        public string EditedTime
        {
            set => _EditedTime = value;
        }
        #endregion

        #region Functions

        public void UpdateAutoSaveInterval()
        {
            autoSaveInterval = Properties.Settings.Default.AutoSaveInterval;
            autoSaveTicks = autoSaveInterval * 60;
        }

        public void UpdateStatusBar()
        {
            if (gRtf || gCurrentFileName.EndsWith(Strings.rtfExt))
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

        /// <summary>
        /// mostly just UI setup work, it doesn't really create a new file/document
        /// </summary>
        public void CreateNewDocument()
        {
            rtbPage.Clear();
            UpdateFormTitle(Strings.defaultFileName);

            // check for file type and update UI accordingly
            if (Properties.Settings.Default.NewFileFormat == Strings.rtf)
            {
                gRtf = true;
                EnableToolbarFormattingIcons();
            }
            else
            {
                gRtf = false;
                ClearToolbarFormattingIcons();
            }
            
            UpdateStatusBar();
        }

        /// <summary>
        /// there is no file->close, so new handles unsaved doc situations
        /// </summary>
        public void FileNew()
        {
            // if there are no unsaved changes, we can just create a new blank document
            if (rtbPage.Modified == false)
            {
                CreateNewDocument();
            }
            else if (rtbPage.Modified == true)
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

            // update the UI after the new doc and unsaved work is done
            UpdateToolbarIcons();
        }

        /// <summary>
        /// load file and update UI
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadPlainTextFile(string filePath)
        {
            rtbPage.LoadFile(filePath, RichTextBoxStreamType.PlainText);
            DisableToolbarFormattingIcons();
            gRtf = false;
            toolStripStatusLabelFileType.Text = Strings.plainText;
        }

        /// <summary>
        /// load rtf and update UI
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadRtfFile(string filePath)
        {
            rtbPage.LoadFile(filePath, RichTextBoxStreamType.RichText);
            EnableToolbarFormattingIcons();
            gRtf = true;
            toolStripStatusLabelFileType.Text = Strings.rtf;
        }

        /// <summary>
        /// write exception details to error log file
        /// </summary>
        /// <param name="output"></param>
        public void WriteErrorLogContent(string output)
        {
            using (StreamWriter sw = new StreamWriter(gTempFilePath + Strings.txtExt, true))
            {
                sw.WriteLine(DateTime.Now + Strings.semiColon + output);
            }
        }

        /// <summary>
        /// file opened from the MRU menu button
        /// </summary>
        /// <param name="filePath"></param>
        public void FileOpen(string filePath)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                if (filePath.EndsWith(Strings.txtExt))
                {
                    LoadPlainTextFile(filePath);
                }
                else
                {
                    LoadRtfFile(filePath);
                }

                // update encoding
                EncodingToolStripStatusLabel.Text = App.GetFileEncoding(filePath);
                ClearToolbarFormattingIcons();
                MoveCursorToLocation(0, 0);
                gPrevPageLength = rtbPage.TextLength;
                UpdateMRU();
                UpdateFormTitle(filePath);
                UpdateDocStats();

                // force both scrollbars weird bug in richtextbox where it doesn't always show scrollbars
                rtbPage.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
                rtbPage.Modified = false;
            }
            catch (Exception ex)
            {
                WriteErrorLogContent("FileOpen Error = " + ex.Message);
            }
            finally
            {
                UpdateToolbarIcons();
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// default file open using file dialog
        /// </summary>
        public void FileOpen()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
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
                    if (ofdFileOpen.FileName.EndsWith(Strings.txtExt))
                    {
                        LoadPlainTextFile(ofdFileOpen.FileName);
                    }
                    else
                    {
                        LoadRtfFile(ofdFileOpen.FileName);
                    }

                    // update the title with the file path
                    UpdateFormTitle(ofdFileOpen.FileName);

                    // update the encoding
                    EncodingToolStripStatusLabel.Text = App.GetFileEncoding(ofdFileOpen.FileName);

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
                    ClearToolbarFormattingIcons();
                    MoveCursorToLocation(0, 0);
                    gPrevPageLength = rtbPage.TextLength;
                    UpdateDocStats();

                    // force both scrollbars, weird bug in richtextbox where it doesn't always show scrollbars
                    rtbPage.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
                    rtbPage.Modified = false;
                }
            }
            catch (Exception ex)
            {
                WriteErrorLogContent("FileOpen Error = " + ex.Message);
            }
            finally
            {
                UpdateToolbarIcons();
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// save the current file changes
        /// </summary>
        public void FileSave()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // if gChanged is false, no changes to save
                if (rtbPage.Modified == false)
                {
                    return;
                }

                // if gChanged is true, untitled needs to be save as
                // any other file name do regular save of the content
                if (gCurrentFileName.ToString() == Strings.defaultFileName && rtbPage.Modified == true)
                {
                    FileSaveAs();
                }
                else
                {
                    if (gCurrentFileName.EndsWith(Strings.txtExt))
                    {
                        rtbPage.SaveFile(gCurrentFileName, RichTextBoxStreamType.PlainText);
                    }
                    
                    if (gCurrentFileName.EndsWith(Strings.rtfExt))
                    {
                        rtbPage.SaveFile(gCurrentFileName, RichTextBoxStreamType.RichText);
                    }

                    rtbPage.Modified = false;
                }

                UpdateToolbarIcons();
            }
            catch (Exception ex)
            {
                WriteErrorLogContent("FileSave Error : " + ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
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
                    sfdSaveAs.AddExtension = true;
                    sfdSaveAs.Title = "Save As";
                    sfdSaveAs.AutoUpgradeEnabled = true;

                    if (sfdSaveAs.ShowDialog() == DialogResult.OK && sfdSaveAs.FileName.Length > 0)
                    {
                        Cursor = Cursors.WaitCursor;
                        if (sfdSaveAs.FilterIndex == 1)
                        {
                            rtbPage.SaveFile(sfdSaveAs.FileName, RichTextBoxStreamType.PlainText);
                        }
                        else
                        {
                            rtbPage.SaveFile(sfdSaveAs.FileName, RichTextBoxStreamType.RichText);
                        }

                        UpdateFormTitle(sfdSaveAs.FileName);
                        rtbPage.Modified = false;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLogContent("FileSaveAs Error :" + ex.Message);
            }
            finally
            {
                UpdateStatusBar();
                UpdateToolbarIcons();
                Cursor = Cursors.Default;
            }
        }

        public void OpenRecentFile(string filePath, int buttonIndex)
        {
            // do nothing if there is no file
            if (filePath == Strings.empty)
            {
                return;
            }

            // if there are unsaved changes, prompt the user before opening
            if (rtbPage.Modified)
            {
                DialogResult result = MessageBox.Show("Do you want to save your changes?", "Save Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    FileSave();
                }
            }

            // if there is a file/path, open it
            if (File.Exists(filePath))
            {
                FileOpen(filePath);
            }
            else
            {
                // if the file no longer exists, remove from mru
                RemoveFileFromMRU(filePath, buttonIndex);
            }

            gPrevPageLength = rtbPage.TextLength;
        }

        public void Print()
        {
            printDialog1.Document = printDocument1;
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                gPrintString = rtbPage.Text;
                printDocument1.Print();
            }
        }

        public void PrintPreview()
        {
            printPreviewDialog1.Document = printDocument1;
            gPrintString = rtbPage.Text;
            printPreviewDialog1.ShowDialog();
        }

        public void PageSetup()
        {
            pageSetupDialog1.Document = printDocument1;
            pageSetupDialog1.ShowDialog();
        }

        public void ClearRecentMenuItems()
        {
            RecentToolStripMenuItem1.Text = Strings.empty;
            RecentToolStripMenuItem2.Text = Strings.empty;
            RecentToolStripMenuItem3.Text = Strings.empty;
            RecentToolStripMenuItem4.Text = Strings.empty;
            RecentToolStripMenuItem5.Text = Strings.empty;
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
                MessageBox.Show("File No Longer Exists, Removing From MRU", "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // clear the menu and add back the remaining files
            ClearRecentMenuItems();
            UpdateMRU();
        }

        public void Cut()
        {
            rtbPage.Cut();
            rtbPage.Modified = true;
        }

        public void Copy()
        {
            rtbPage.Copy();
        }

        public void Paste()
        {
            rtbPage.Modified = true;

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
                    default: rtbPage.Modified = false; break;
                }
            }
        }

        public void Undo()
        {
            rtbPage.Undo();
            rtbPage.Modified = true;
        }

        public void Redo()
        {
            rtbPage.Redo();
            rtbPage.Modified = true;
        }

        /// <summary>
        /// if there are unsaved changes, we need to ask the user what to do
        /// </summary>
        /// <param name="fromFormClosingEvent"></param>
        public void ExitAppWork(bool fromFormClosingEvent)
        {
            if (rtbPage.Modified == true)
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
                    BoldToolStripButton.Checked = true;
                }
                else
                {
                    BoldToolStripButton.Checked = false;
                }

                // italic
                if (rtbPage.SelectionFont.Italic == true)
                {
                    ItalicToolStripButton.Checked = true;
                }
                else
                {
                    ItalicToolStripButton.Checked = false;
                }

                // underline
                if (rtbPage.SelectionFont.Underline == true)
                {
                    UnderlineToolStripButton.Checked = true;
                }
                else
                {
                    UnderlineToolStripButton.Checked = false;
                }

                // strikethrough
                if (rtbPage.SelectionFont.Strikeout == true)
                {
                    StrikethroughToolStripButton.Checked = true;
                }
                else
                {
                    StrikethroughToolStripButton.Checked = false;
                }

                // bullets
                if (rtbPage.SelectionBullet == true)
                {
                    BulletToolStripButton.Checked = true;
                }
                else
                {
                    BulletToolStripButton.Checked = false;
                }

                // alignment
                if (rtbPage.SelectionAlignment == HorizontalAlignment.Left)
                {
                    LeftJustifiedToolStripButton.Checked = true;
                    CenterJustifiedToolStripButton.Checked = false;
                    RightJustifiedToolStripButton.Checked = false;
                }
                else if (rtbPage.SelectionAlignment == HorizontalAlignment.Center)
                {
                    CenterJustifiedToolStripButton.Checked = true;
                    LeftJustifiedToolStripButton.Checked = false;
                    RightJustifiedToolStripButton.Checked = false;
                }
                else if (rtbPage.SelectionAlignment == HorizontalAlignment.Right)
                {
                    RightJustifiedToolStripButton.Checked = true;
                    CenterJustifiedToolStripButton.Checked = false;
                    LeftJustifiedToolStripButton.Checked = false;
                }

                // save icon
                if (rtbPage.Modified)
                {
                    SaveToolStripButton.Enabled = true;
                    SaveToolStripMenuItem.Enabled = true;
                }
                else
                {
                    SaveToolStripButton.Enabled = false;
                    SaveToolStripMenuItem.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                WriteErrorLogContent("UpdateToolbarIcons Error" + ex.Message);
            }
        }

        /// <summary>
        /// gather details for word, char and line count
        /// </summary>
        public void UpdateDocStats()
        {
            int totalWordCount = 0;
            int lineCount;
            foreach (string line in rtbPage.Lines)
            {
                lineCount = line.Split(new char[] { ' ', '.', '?', ',', ':', '\t', '=', ';' }, StringSplitOptions.RemoveEmptyEntries).Length;
                totalWordCount += lineCount;
            }
            
            // update the ui with the values
            WordCountToolStripStatusLabel.Text = totalWordCount.ToString();
            CharacterCountToolStripStatusLabel.Text = rtbPage.Text.Length.ToString();
            LinesToolStripStatusLabel.Text = rtbPage.Lines.Length.ToString();
        }

        public void ClearFormatting()
        {
            rtbPage.SelectionBullet = false;
            rtbPage.SelectionFont = new Font(Properties.Settings.Default.DefaultFontName, Properties.Settings.Default.DefaultFontSize);
            rtbPage.SelectionColor = Color.FromName(Properties.Settings.Default.DefaultFontColorName);
            rtbPage.SelectionIndent = 0;
            rtbPage.SelectionAlignment = HorizontalAlignment.Left;
            rtbPage.Modified = true;
        }

        /// <summary>
        /// if the file is rtf, we can light up the formatting
        /// </summary>
        public void EnableToolbarFormattingIcons()
        {
            BoldToolStripButton.Enabled = true;
            ItalicToolStripButton.Enabled = true;
            UnderlineToolStripButton.Enabled = true;
            StrikethroughToolStripButton.Enabled = true;
            BulletToolStripButton.Enabled = true;
            FontColorToolStripButton.Enabled = true;
            HighlightTextToolStripButton.Enabled = true;
        }

        /// <summary>
        /// plain text files don't need formatting buttons so they can be disabled
        /// </summary>
        public void DisableToolbarFormattingIcons()
        {
            BoldToolStripButton.Enabled = false;
            ItalicToolStripButton.Enabled = false;
            UnderlineToolStripButton.Enabled = false;
            StrikethroughToolStripButton.Enabled = false;
            BulletToolStripButton.Enabled = false;
            FontColorToolStripButton.Enabled = false;
            HighlightTextToolStripButton.Enabled = false;
        }

        /// <summary>
        /// clear all of the formatting icon ui states
        /// </summary>
        public void ClearToolbarFormattingIcons()
        {
            BoldToolStripButton.Checked = false;
            ItalicToolStripButton.Checked = false;
            UnderlineToolStripButton.Checked = false;
            StrikethroughToolStripButton.Checked = false;
            BulletToolStripButton.Checked = false;
        }

        /// <summary>
        /// ui work needed after formatting buttons have been applied/used
        /// </summary>
        public void EndOfButtonFormatWork()
        {
            rtbPage.Focus();
            UpdateToolbarIcons();
            rtbPage.Modified = true;
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
                    case 1: RecentToolStripMenuItem1.Text = f?.ToString(); break;
                    case 2: RecentToolStripMenuItem2.Text = f?.ToString(); break;
                    case 3: RecentToolStripMenuItem3.Text = f?.ToString(); break;
                    case 4: RecentToolStripMenuItem4.Text = f?.ToString(); break;
                    case 5: RecentToolStripMenuItem5.Text = f?.ToString(); break;
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

            line++;
            column++;

            toolStripStatusLabelLine.Text = line.ToString();
            toolStripStatusLabelColumn.Text = column.ToString();
        }

        /// <summary>
        /// reset the timespan variables
        /// </summary>
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

        /// <summary>
        /// change UI to have a black background and white text
        /// </summary>
        public void ApplyDarkMode()
        {
            menuStrip1.BackColor = clrDarkModeBackground;
            toolStrip1.BackColor = clrDarkModeBackground;
            statusStrip1.BackColor = clrDarkModeBackground;
            ChangeControlTextColor(Color.White);
            ChangeSubMenuItemBackColor(clrDarkModeBackground);
            ChangeMenuItemBackColor(clrDarkModeBackground);
            rtbPage.BackColor = clrDarkModeTextBackground;
            ApplyTextColor();
        }

        /// <summary>
        /// change the background to have the default UI
        /// </summary>
        public void ApplyLightMode()
        {
            menuStrip1.BackColor = Color.FromKnownColor(KnownColor.Control);
            toolStrip1.BackColor = Color.FromKnownColor(KnownColor.Control);
            statusStrip1.BackColor = Color.FromKnownColor(KnownColor.Control);
            ChangeControlTextColor(Color.Black);
            ChangeSubMenuItemBackColor(Color.White);
            ChangeMenuItemBackColor(Color.FromKnownColor(KnownColor.Control));
            rtbPage.BackColor = Color.FromKnownColor(KnownColor.Window);
            ApplyTextColor();
        }

        /// <summary>
        /// plain text files can have the text color changed
        /// if the file is rtf, no changes will be made
        /// </summary>
        public void ApplyTextColor()
        {
            // if the file is rtf or the setting says not to reverse the default text color, don't do anything
            if (gRtf)
            {
                return;
            }
            
            // for plain text, check the setting and apply the reverse color for text
            if (Properties.Settings.Default.ReverseTextColorWithTheme)
            {
                if (Properties.Settings.Default.DarkMode)
                {
                    rtbPage.ForeColor = Color.White;
                }
                else
                {
                    rtbPage.ForeColor = Color.Black;
                }
            }
        }

        /// <summary>
        /// change text color of controls and menu items
        /// </summary>
        /// <param name="clr"></param>
        public void ChangeControlTextColor(Color clr)
        {
            // update toolstrip items
            TimerToolStripLabel.ForeColor = clr;
            TimerToolStripLabelOnly.ForeColor = clr;
            StartStopTimerToolStripButton.ForeColor = clr;
            ResetTimerToolStripButton.ForeColor = clr;
            EditTimerToolStripButton.ForeColor = clr;
            FindToolStripButton.ForeColor = clr;
            FindToolStripLabel.ForeColor = clr;
            
            // update status bar items
            toolStripStatusLabelCol.ForeColor = clr;
            toolStripStatusLabelColumn.ForeColor = clr;
            toolStripStatusLabelLn.ForeColor = clr;
            toolStripStatusLabelLine.ForeColor = clr;
            toolStripStatusLabelFType.ForeColor = clr;
            toolStripStatusLabelFileType.ForeColor = clr;
            toolStripStatusLabelAppVer.ForeColor = clr;
            appVersionToolStripStatusLabel.ForeColor = clr;
            EncodingToolStripStatusLabel.ForeColor = clr;
            WordCountToolStripStatusLabel.ForeColor = clr;
            LinesToolStripStatusLabel.ForeColor = clr;
            CharacterCountToolStripStatusLabel.ForeColor = clr;
            toolStripStatusLabelChar.ForeColor = clr;
            toolStripStatusLabelWords.ForeColor = clr;
            toolStripStatusLabelLines.ForeColor = clr;

            // update menu items
            FileToolStripMenuItem.ForeColor = clr;
            NewToolStripMenuItem.ForeColor = clr;
            OpenToolStripMenuItem.ForeColor = clr;
            RecentToolStripMenuItem.ForeColor = clr;
            RecentToolStripMenuItem1.ForeColor = clr;
            RecentToolStripMenuItem2.ForeColor = clr;
            RecentToolStripMenuItem3.ForeColor = clr;
            RecentToolStripMenuItem4.ForeColor = clr;
            RecentToolStripMenuItem5.ForeColor = clr;
            PrintToolStripMenuItem.ForeColor = clr;
            PrintPreviewToolStripMenuItem.ForeColor = clr;
            PageSetupToolStripMenuItem.ForeColor = clr;
            SaveAsToolStripMenuItem.ForeColor = clr;
            SaveToolStripMenuItem.ForeColor = clr;
            OptionsToolStripMenuItem.ForeColor = clr;
            ExitToolStripMenuItem.ForeColor = clr;
            EditToolStripMenuItem.ForeColor = clr;
            UndoToolStripMenuItem.ForeColor = clr;
            RedoToolStripMenuItem.ForeColor = clr;
            CutToolStripMenuItem.ForeColor = clr;
            CopyToolStripMenuItem.ForeColor = clr;
            PasteToolStripMenuItem.ForeColor = clr;
            SelectAllToolStripMenuItem.ForeColor = clr;
            ClearAllTextToolStripMenuItem.ForeColor = clr;
            FindToolStripMenuItem.ForeColor = clr;
            EditFontToolStripMenuItem.ForeColor = clr;
            ClearFormattingToolStripMenuItem.ForeColor = clr;
            BoldToolStripMenuItem.ForeColor = clr;
            ItalicToolStripMenuItem.ForeColor = clr;
            UnderlineToolStripMenuItem.ForeColor = clr;
            StrikethroughToolStripMenuItem.ForeColor = clr;
            WordWrapToolStripMenuItem.ForeColor = clr;
            ZoomToolStripMenuItem.ForeColor = clr;
            zoomToolStripMenuItem100.ForeColor = clr;
            zoomToolStripMenuItem150.ForeColor = clr;
            zoomToolStripMenuItem200.ForeColor = clr;
            zoomToolStripMenuItem250.ForeColor = clr;
            zoomToolStripMenuItem300.ForeColor = clr;
            AboutToolStripMenuItem.ForeColor = clr;
            SubmitFeedbackToolStripMenuItem.ForeColor = clr;
            ReportBugToolStripMenuItem.ForeColor = clr;
            FormatToolStripMenuItem.ForeColor = clr;
            ViewToolStripMenuItem.ForeColor = clr;
            HelpToolStripMenuItem.ForeColor = clr;
        }

        public void ChangeMenuItemBackColor(Color clr)
        {
            FileToolStripMenuItem.BackColor = clr;
            EditToolStripMenuItem.BackColor = clr;
            FormatToolStripMenuItem.BackColor = clr;
            ViewToolStripMenuItem.BackColor = clr;
            HelpToolStripMenuItem.BackColor = clr;
        }

        /// <summary>
        /// change the background color of the sub menu items
        /// </summary>
        /// <param name="clr"></param>
        public void ChangeSubMenuItemBackColor(Color clr)
        {
            NewToolStripMenuItem.BackColor = clr;
            OpenToolStripMenuItem.BackColor = clr;
            RecentToolStripMenuItem.BackColor = clr;
            RecentToolStripMenuItem1.BackColor = clr;
            RecentToolStripMenuItem2.BackColor = clr;
            RecentToolStripMenuItem3.BackColor = clr;
            RecentToolStripMenuItem4.BackColor = clr;
            RecentToolStripMenuItem5.BackColor = clr;
            PrintToolStripMenuItem.BackColor = clr;
            PrintPreviewToolStripMenuItem.BackColor = clr;
            PageSetupToolStripMenuItem.BackColor = clr;
            SaveAsToolStripMenuItem.BackColor = clr;
            SaveToolStripMenuItem.BackColor = clr;
            OptionsToolStripMenuItem.BackColor = clr;
            ExitToolStripMenuItem.BackColor = clr;
            UndoToolStripMenuItem.BackColor = clr;
            RedoToolStripMenuItem.BackColor = clr;
            CutToolStripMenuItem.BackColor = clr;
            CopyToolStripMenuItem.BackColor = clr;
            PasteToolStripMenuItem.BackColor = clr;
            SelectAllToolStripMenuItem.BackColor = clr;
            ClearAllTextToolStripMenuItem.BackColor = clr;
            FindToolStripMenuItem.BackColor = clr;
            EditFontToolStripMenuItem.BackColor = clr;
            ClearFormattingToolStripMenuItem.BackColor = clr;
            BoldToolStripMenuItem.BackColor = clr;
            ItalicToolStripMenuItem.BackColor = clr;
            UnderlineToolStripMenuItem.BackColor = clr;
            StrikethroughToolStripMenuItem.BackColor = clr;
            WordWrapToolStripMenuItem.BackColor = clr;
            ZoomToolStripMenuItem.BackColor = clr;
            zoomToolStripMenuItem100.BackColor = clr;
            zoomToolStripMenuItem150.BackColor = clr;
            zoomToolStripMenuItem200.BackColor = clr;
            zoomToolStripMenuItem250.BackColor = clr;
            zoomToolStripMenuItem300.BackColor = clr;
            AboutToolStripMenuItem.BackColor = clr;
            SubmitFeedbackToolStripMenuItem.BackColor = clr;
            ReportBugToolStripMenuItem.BackColor = clr;
        }

        /// <summary>
        /// known issue in toolstripseparator not using back/fore colors
        /// https://stackoverflow.com/questions/15926377/change-the-backcolor-of-the-toolstripseparator-control
        /// this is for MenuStrip toolstrip separators, but not needed for TooStrip separators
        /// </summary>
        /// <param name="sender">object param from the paint event</param>
        /// <param name="e">eventarg from paint event</param>
        /// <param name="cFill">backcolor for the separator</param>
        /// <param name="cLine">forecolor for the separator</param>
        public void PaintToolStripSeparator(object sender, PaintEventArgs e, Color cFill, Color cLine)
        {
            ToolStripSeparator sep = (ToolStripSeparator)sender;
            e.Graphics.FillRectangle(new SolidBrush(cFill), 0, 0, sep.Width, sep.Height);
            e.Graphics.DrawLine(new Pen(cLine), 30, sep.Height / 2, sep.Width - 4, sep.Height / 2);
        }

        /// <summary>
        /// write file contents
        /// </summary>
        async void BackgroundFileSaveAsync()
        {
            var task = new Task(() => WriteFileContents());
            task.Start();
            await task;
        }

        /// <summary>
        /// depending on the file currently active, save its contents
        /// </summary>
        public void WriteFileContents()
        {
            try
            {
                // if the file is unsaved, write it out to the 
                if (rtbPage.Modified == false)
                {
                    // if no changes were made, nothing to save
                    return;
                }

                if (gCurrentFileName == Strings.defaultFileName)
                {
                    // using rtf for all temp files, if the file is plain text, it will still have the same contents
                    string tempFilePath = unsavedFolderPath + "\\" + gCurrentFileName + Strings.rtf;
                    using (StreamWriter sw = new StreamWriter(tempFilePath, false))
                    {
                        // need to avoid cross thread operation
                        rtbPage.Invoke((MethodInvoker)delegate {
                            rtbPage.SaveFile(tempFilePath);
                            rtbPage.Modified = false;
                            UpdateToolbarIcons();
                        });
                    }
                }
                else
                {
                    // need to avoid cross thread operation
                    rtbPage.Invoke((MethodInvoker)delegate {
                        FileSave();
                    });
                }
            }
            catch (Exception ex)
            {
                WriteErrorLogContent("WriteFileContents Error: " + ex.Message);
            }
        }

        #endregion

        /// <summary>
        /// when the selection changes, we need to update the UI
        /// we also want to update the modified state of the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RtbPage_SelectionChanged(object sender, EventArgs e)
        {
            //// check if content was added
            //if (gPrevPageLength != rtbPage.TextLength)
            //{
            //    rtbPage.Modified = true;
            //}
            
            //// now update the prevPageLength
            //gPrevPageLength = rtbPage.TextLength;

            UpdateLnColValues();
            UpdateToolbarIcons();
            //UpdateDocStats();

        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileNew();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileOpen();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSave();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSaveAs();
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Redo();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cut();
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Paste();
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbPage.SelectAll();
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAbout fAbout = new FrmAbout()
            {
                Owner = this
            };
            fAbout.ShowDialog();
        }

        private void NewToolStripButton_Click(object sender, EventArgs e)
        {
            FileNew();
        }

        private void BoldToolStripButton_Click(object sender, EventArgs e)
        {
            ApplyBold();
        }

        private void ItalicToolStripButton_Click(object sender, EventArgs e)
        {
            ApplyItalic();
        }

        private void UnderlineToolStripButton_Click(object sender, EventArgs e)
        {
            ApplyUnderline();
        }

        private void StrikethroughToolStripButton_Click(object sender, EventArgs e)
        {
            ApplyStrikethrough();
        }

        private void UndoToolStripButton_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void RedoToolStripButton_Click(object sender, EventArgs e)
        {
            Redo();
        }

        /// <summary>
        /// this will apply the font formatting across the entire document/richtextbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // setup the initial dialog values from settings
            fontDialog1.ShowColor = true;
            fontDialog1.Font = new Font(Properties.Settings.Default.DefaultFontName, Properties.Settings.Default.DefaultFontSize);
            fontDialog1.Color = Color.FromName(Properties.Settings.Default.DefaultFontColorName);

            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                // update richtextbox control
                rtbPage.Font = fontDialog1.Font;
                
                // only update color for rtf
                if (gRtf)
                {
                    rtbPage.ForeColor = fontDialog1.Color;
                }
                
                // update settings
                Properties.Settings.Default.DefaultFontName = fontDialog1.Font.Name;
                Properties.Settings.Default.DefaultFontSize = Convert.ToInt32(fontDialog1.Font.Size);
                Properties.Settings.Default.DefaultFontColorName = fontDialog1.Color.Name;
            }
        }

        private void ClearFormattingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearFormatting();
            EndOfButtonFormatWork();
        }

        private void ClearAllTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbPage.ResetText();
            rtbPage.Font = new Font(Properties.Settings.Default.DefaultFontName, Properties.Settings.Default.DefaultFontSize);
            EndOfButtonFormatWork();
        }

        private void HelpToolStripButton_Click(object sender, EventArgs e)
        {
            App.PlatformSpecificProcessStart(Strings.githubIssues);
        }

        private void OpenToolStripButton_Click(object sender, EventArgs e)
        {
            FileOpen();
        }

        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            FileSave();
        }

        private void CutToolStripButton_Click(object sender, EventArgs e)
        {
            Cut();
        }

        private void CopyToolStripButton_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void PasteToolStripButton_Click(object sender, EventArgs e)
        {
            Paste();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
            ExitAppWork(true);
        }

        private void ZoomToolStripMenuItem100_Click(object sender, EventArgs e)
        {
            ApplyZoom(1.0f);
        }

        private void ZoomToolStripMenuItem150_Click(object sender, EventArgs e)
        {
            ApplyZoom(1.5f);
        }

        private void ZoomToolStripMenuItem200_Click(object sender, EventArgs e)
        {
            ApplyZoom(2.0f);
        }

        private void ZoomToolStripMenuItem250_Click(object sender, EventArgs e)
        {
            ApplyZoom(2.5f);
        }

        private void ZoomToolStripMenuItem300_Click(object sender, EventArgs e)
        {
            ApplyZoom(3.0f);
        }

        private void WordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbPage.WordWrap == true)
            {
                rtbPage.WordWrap = false;
                WordWrapToolStripMenuItem.Checked = false;
            }
            else
            {
                rtbPage.WordWrap = true;
                WordWrapToolStripMenuItem.Checked = true;
            }
        }

        private void BulletToolStripButton_Click(object sender, EventArgs e)
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

        private void DecreaseIndentToolStripButton_Click(object sender, EventArgs e)
        {
            if (rtbPage.SelectionIndent > 0)
            {
                rtbPage.SelectionIndent -= 30;
            }
            
            EndOfButtonFormatWork();
        }

        private void IncreaseIndentToolStripButton_Click(object sender, EventArgs e)
        {
            if (rtbPage.SelectionIndent < 150)
            {
                rtbPage.SelectionIndent += 30;
            }

            EndOfButtonFormatWork();
        }

        private void RtbPage_KeyDown(object sender, KeyEventArgs e)
        {
            // if the line has a bullet and the tab was pressed, indent the line
            // need to suppress the key to prevent the cursor from moving around
            if (e.KeyCode == Keys.Tab && rtbPage.SelectionBullet == true)
            {
                e.SuppressKeyPress = true;
                rtbPage.Select(rtbPage.GetFirstCharIndexOfCurrentLine(), 0);
                IncreaseIndentToolStripButton.PerformClick();
                return;
            }

            // if the user deletes the bullet, we need to remove bullet formatting
            if (e.KeyCode == Keys.Back)
            {
                if (rtbPage.SelectionStart == rtbPage.GetFirstCharIndexOfCurrentLine() && rtbPage.SelectionBullet == true)
                {
                    e.SuppressKeyPress = true;
                    BulletToolStripButton.PerformClick();
                }
                // backspace causes selectionchange so the stats update there
            }

            // if delete key is pressed change the saved state
            if (e.KeyCode == Keys.Delete)
            {
                gPrevPageLength = rtbPage.TextLength;
                rtbPage.Modified = true;
                UpdateToolbarIcons();
                UpdateDocStats();
            }
        }

        private void RecentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            OpenRecentFile(sender.ToString(), 1);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        private void RecentToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (sender is not null)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                OpenRecentFile(sender.ToString(), 2);
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }

        private void RecentToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (sender is not null)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                OpenRecentFile(sender.ToString(), 3);
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }

        private void RecentToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (sender is not null)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                OpenRecentFile(sender.ToString(), 4);
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }

        private void RecentToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (sender is not null)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                OpenRecentFile(sender.ToString(), 5);
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
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
                TimerToolStripLabel.Text = $"{tSpan.Hours:00}:{tSpan.Minutes:00}:{tSpan.Seconds:00}";
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExitAppWork(false);
        }

        private void FindToolStripButton_Click(object sender, EventArgs e)
        {
            if (FindToolStripTextBox.Text == string.Empty)
            {
                return;
            }
            else
            {
                int indexToText;
                if (Properties.Settings.Default.SearchOption == "Up")
                {
                    // search up the file and find any word match
                    indexToText = rtbPage.Find(FindToolStripTextBox.Text, 0, rtbPage.SelectionStart, RichTextBoxFinds.Reverse);
                }
                else if (Properties.Settings.Default.SearchOption == "Down")
                {
                    // search from the top down and find any word match
                    indexToText = rtbPage.Find(FindToolStripTextBox.Text, rtbPage.SelectionStart + 1, RichTextBoxFinds.None);
                }
                else if (Properties.Settings.Default.SearchOption == "MatchCase")
                {
                    // only find words that match the case exactly
                    indexToText = rtbPage.Find(FindToolStripTextBox.Text, rtbPage.SelectionStart + 1, RichTextBoxFinds.MatchCase);
                }
                else
                {
                    // only find words that have the entire word in the search textbox
                    indexToText = rtbPage.Find(FindToolStripTextBox.Text, rtbPage.SelectionStart + 1, RichTextBoxFinds.WholeWord);
                }

                if (indexToText >= 0)
                {
                    MoveCursorToLocation(indexToText, FindToolStripTextBox.Text.Length);
                }
            }
        }

        private void FileOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmOptions fOptions = new FrmOptions()
            {
                Owner = this
            };
            fOptions.ShowDialog();

            // also need to update the file mru in case it was cleared
            if (Properties.Settings.Default.FileMRU.Count == 0)
            {
                ClearRecentMenuItems();
            }

            // update the theme
            if (Properties.Settings.Default.DarkMode)
            {
                ApplyDarkMode();
            }
            else
            {
                ApplyLightMode();
            }

            // update the ticks
            UpdateAutoSaveInterval();
        }

        private void SubmitFeedbackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            App.PlatformSpecificProcessStart(Strings.githubDiscussion);
        }

        private void ReportBugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            App.PlatformSpecificProcessStart(Strings.githubIssues);
        }

        private void PrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Print();
        }

        private void PrintPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintPreview();
        }

        private void PageSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PageSetup();
        }

        private void PrintDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // print rtf
                if (gRtf)
                {
                    e.HasMorePages = RtfPrint.Print(rtbPage, ref charFrom, e);
                    return;
                }

                // print plain text
                int charactersOnPage = 0;

                // Sets the value of charactersOnPage to the number of characters 
                // of strToPrint that will fit within the bounds of the page.
                e.Graphics?.MeasureString(gPrintString, rtbPage.SelectionFont, e.MarginBounds.Size, StringFormat.GenericTypographic,
                    out charactersOnPage, out _);

                // Draws the string within the bounds of the page
                e.Graphics?.DrawString(gPrintString, rtbPage.SelectionFont, Brushes.Black, e.MarginBounds, StringFormat.GenericTypographic);

                // Remove the portion of the string that has been printed.
                gPrintString = gPrintString.Substring(charactersOnPage);

                // Check to see if more pages are to be printed.
                e.HasMorePages = (gPrintString.Length > 0);
            }
            catch (Exception ex)
            {
                WriteErrorLogContent("PrintPage Error: " + ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void LeftJustifiedToolStripButton_Click(object sender, EventArgs e)
        {
            rtbPage.SelectionAlignment = HorizontalAlignment.Left;
            EndOfButtonFormatWork();
        }

        private void CenterJustifiedToolStripButton_Click(object sender, EventArgs e)
        {
            rtbPage.SelectionAlignment = HorizontalAlignment.Center;
            EndOfButtonFormatWork();
        }

        private void RightJustifiedToolStripButton_Click(object sender, EventArgs e)
        {
            rtbPage.SelectionAlignment = HorizontalAlignment.Right;
            EndOfButtonFormatWork();
        }

        private void FontColorToolStripButton_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK && colorDialog1.Color != rtbPage.SelectionColor)
            {
                rtbPage.SelectionColor = colorDialog1.Color;
                rtbPage.Modified = true;
            }
        }

        private void FindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbPage.Focus();
            FindToolStripButton.PerformClick();
        }

        private void EditTimerToolStripButton_Click(object sender, EventArgs e)
        {
            FrmEditTimer fEditedTimer = new FrmEditTimer(TimerToolStripLabel.Text)
            {
                Owner = this
            };
            fEditedTimer.ShowDialog(this);
            
            // if the time was adjusted, reset the timer and update with the new time
            if (fEditedTimer._isAdjustedTime)
            {
                ResetTimerToolStripButton.PerformClick();

                string[] dataArray = _EditedTime.Split(_semiColonDelim);
                editedHours = Convert.ToInt32(dataArray.ElementAt(0));
                editedMinutes = Convert.ToInt32(dataArray.ElementAt(1));
                editedSeconds = Convert.ToInt32(dataArray.ElementAt(2));
                TimerToolStripLabel.Text = _EditedTime;
            }
        }

        private void ToolStripSeparator1_Paint(object sender, PaintEventArgs e)
        {
            if (Properties.Settings.Default.DarkMode)
            {
                PaintToolStripSeparator(sender, e, clrDarkModeBackground, Color.White);
            }
            else
            {
                PaintToolStripSeparator(sender, e, Color.White, clrDarkModeBackground);
            }            
        }

        private void ToolStripSeparator3_Paint(object sender, PaintEventArgs e)
        {
            if (Properties.Settings.Default.DarkMode)
            {
                PaintToolStripSeparator(sender, e, clrDarkModeBackground, Color.White);
            }
            else
            {
                PaintToolStripSeparator(sender, e, Color.White, clrDarkModeBackground);
            }
        }

        private void ToolStripSeparator4_Paint(object sender, PaintEventArgs e)
        {
            if (Properties.Settings.Default.DarkMode)
            {
                PaintToolStripSeparator(sender, e, clrDarkModeBackground, Color.White);
            }
            else
            {
                PaintToolStripSeparator(sender, e, Color.White, clrDarkModeBackground);
            }
        }

        private void ToolStripSeparator13_Paint(object sender, PaintEventArgs e)
        {
            if (Properties.Settings.Default.DarkMode)
            {
                PaintToolStripSeparator(sender, e, clrDarkModeBackground, Color.White);
            }
            else
            {
                PaintToolStripSeparator(sender, e, Color.White, clrDarkModeBackground);
            }
        }

        private void ToolStripSeparator10_Paint(object sender, PaintEventArgs e)
        {
            if (Properties.Settings.Default.DarkMode)
            {
                PaintToolStripSeparator(sender, e, clrDarkModeBackground, Color.White);
            }
            else
            {
                PaintToolStripSeparator(sender, e, Color.White, clrDarkModeBackground);
            }
        }

        private void toolStripSeparator14_Paint(object sender, PaintEventArgs e)
        {
            if (Properties.Settings.Default.DarkMode)
            {
                PaintToolStripSeparator(sender, e, clrDarkModeBackground, Color.White);
            }
            else
            {
                PaintToolStripSeparator(sender, e, Color.White, clrDarkModeBackground);
            }
        }

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Cut();
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Paste();
        }

        private void HighlightTextToolStripButton_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK && colorDialog1.Color != rtbPage.SelectionBackColor)
            {
                rtbPage.SelectionBackColor = colorDialog1.Color;
                rtbPage.Modified = true;
            }
        }

        private void rtbPage_TextChanged(object sender, EventArgs e)
        {
            // check if content was added
            if (gPrevPageLength != rtbPage.TextLength)
            {
                rtbPage.Modified = true;
            }

            // now update the prevPageLength
            gPrevPageLength = rtbPage.TextLength;
            UpdateDocStats();
            UpdateToolbarIcons();
        }

        private void selectAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            rtbPage.SelectAll();
        }

        private void toolStripSeparator12_Paint(object sender, PaintEventArgs e)
        {
            if (Properties.Settings.Default.DarkMode)
            {
                PaintToolStripSeparator(sender, e, clrDarkModeBackground, Color.White);
            }
            else
            {
                PaintToolStripSeparator(sender, e, Color.White, clrDarkModeBackground);
            }
        }

        private void autosaveTimer_Tick(object sender, EventArgs e)
        {
            ticks++;

            // using a 10 minute default autosave interval
            if (ticks > autoSaveTicks)
            {
                ticks = 0;
                BackgroundFileSaveAsync();
            }
        }

        private void printDocument1_BeginPrint(object sender, PrintEventArgs e)
        {
            if (gRtf)
            {
                charFrom = 0;
            }
        }

        /// <summary>
        /// toggle the start/stop behavior of the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButtonStartStopTimer_Click(object sender, EventArgs e)
        {
            if (gStopwatch.IsRunning)
            {
                gStopwatch.Stop();
                StartStopTimerToolStripButton.Text = Strings.startTimeText;
                StartStopTimerToolStripButton.Image = Properties.Resources.StatusRun_16x;
            }
            else
            {
                timer1.Enabled = true;
                gStopwatch.Start();
                StartStopTimerToolStripButton.Text = Strings.stopTimeText;
                StartStopTimerToolStripButton.Image = Properties.Resources.Stop_16x;
            }
        }

        /// <summary>
        /// reset the timer and ui back to zero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButtonResetTimer_Click(object sender, EventArgs e)
        {
            gStopwatch.Reset();
            TimerToolStripLabel.Text = Strings.zeroTimer;
            StartStopTimerToolStripButton.Image = Properties.Resources.StatusRun_16x;
            StartStopTimerToolStripButton.Text = Strings.startTimeText;
            ClearTimeSpanVariables();
        }
    }
}
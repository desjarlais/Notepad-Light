using Notepad_Light.Forms;
using Notepad_Light.Helpers;
using System.Diagnostics;
using System.Reflection;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Notepad_Light
{
    public partial class FrmMain : Form
    {
        // P/Invoke declarations
        const int MM_ISOTROPIC = 7;
        const int MM_ANISOTROPIC = 8;

        [DllImport("gdiplus.dll")]
        private static extern uint GdipEmfToWmfBits(IntPtr _hEmf, uint _bufferSize,
            byte[] _buffer, int _mappingMode, EmfToWmfBitsFlags _flags);
        [DllImport("gdi32.dll")]
        private static extern IntPtr SetMetaFileBitsEx(uint _bufferSize,
            byte[] _buffer);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CopyMetaFile(IntPtr hWmf,
            string filename);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteMetaFile(IntPtr hWmf);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteEnhMetaFile(IntPtr hEmf);

        private enum EmfToWmfBitsFlags
        {
            EmfToWmfBitsFlagsDefault = 0x00000000,
            EmfToWmfBitsFlagsEmbedEmf = 0x00000001,
            EmfToWmfBitsFlagsIncludePlaceable = 0x00000002,
            EmfToWmfBitsFlagsNoXORClip = 0x00000004
        };

        // globals
        public string gCurrentFileName = Strings.defaultFileName;
        public string gErrorLog = string.Empty;
        public string gPrintString = string.Empty;
        public bool gRtf = false;
        public int gPrevPageLength = 0;
        public int autoSaveInterval, autoSaveTicks;
        private string _EditedTime = string.Empty;
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

            rtbPage.Modified = false;

            // start the autosave timer
            autosaveTimer.Start();

            // setup log and app files
            gErrorLog = Strings.appFolderFullPath + "NLErrors.txt";
            UpdateTemplateMenu();
            Templates.UpdateTemplatesFromFiles();

            // change ui if the default is rtf
            if (Properties.Settings.Default.NewFileFormat == Strings.rtf)
            {
                CreateNewDocument();
            }

            UpdateToolbarIcons();
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

        public void UpdateTemplateMenu()
        {
            Template1ToolStripMenuItem.Text = Properties.Settings.Default.Template1;
            Template2ToolStripMenuItem.Text = Properties.Settings.Default.Template2;
            Template3ToolStripMenuItem.Text = Properties.Settings.Default.Template3;
            Template4ToolStripMenuItem.Text = Properties.Settings.Default.Template4;
            Template5ToolStripMenuItem.Text = Properties.Settings.Default.Template5;
            FileExistChecks();
        }

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
        /// function to make sure files that are used in the app exist
        /// </summary>
        public void FileExistChecks()
        {
            // create Notepad folder
            if (!Directory.Exists(Strings.appFolderDirectory))
            {
                Directory.CreateDirectory(Strings.appFolderDirectory);
            }

            // create the error temp file when it doesn't exist
            if (!File.Exists(gErrorLog))
            {
                File.Create(gErrorLog);
            }
            else
            {
                // if it does exist, clear it out on startup
                File.WriteAllText(gErrorLog, string.Empty);
            }

            // check the backup templates folder
            if (!Directory.Exists(Strings.appFolderTemplateDir))
            {
                Directory.CreateDirectory(Strings.appFolderTemplateDir);
            }         

            // check the previous templates file
            if (File.Exists(Strings.appFolderDirectory + Strings.pathDivider + Strings.backupTemplateFileName + Strings.txtExt))
            {
                int count = 1;
                foreach (string line in File.ReadLines(Strings.appFolderDirectory + Strings.pathDivider + Strings.backupTemplateFileName + Strings.txtExt))
                {
                    // update the settings and template menu
                    switch (count)
                    {
                        case 1: 
                            Properties.Settings.Default.Template1 = line;
                            Template1ToolStripMenuItem.Text = line;
                            break;
                        case 2: 
                            Properties.Settings.Default.Template2 = line;
                            Template2ToolStripMenuItem.Text = line;
                            break;
                        case 3: 
                            Properties.Settings.Default.Template3 = line;
                            Template3ToolStripMenuItem.Text = line;
                            break;
                        case 4:
                            Properties.Settings.Default.Template4 = line;
                            Template4ToolStripMenuItem.Text = line;
                            break;
                        case 5: 
                            Properties.Settings.Default.Template5 = line;
                            Template5ToolStripMenuItem.Text = line;
                            break;
                        default: break;
                    }
                    count++;
                }
            }
            else
            {
                // template 1
                if (!File.Exists(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template1 + Strings.txtExt))
                {
                    File.Create(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template1 + Strings.txtExt);
                }

                // template 2
                if (!File.Exists(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template2 + Strings.txtExt))
                {
                    File.Create(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template2 + Strings.txtExt);
                }

                // template 3 
                if (!File.Exists(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template3 + Strings.txtExt))
                {
                    File.Create(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template3 + Strings.txtExt);
                }

                // template 4
                if (!File.Exists(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template4 + Strings.txtExt))
                {
                    File.Create(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template4 + Strings.txtExt);
                }

                // template 5
                if (!File.Exists(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template5 + Strings.txtExt))
                {
                    File.Create(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template5 + Strings.txtExt);
                }
            }
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
        /// there is no file->close, new handles unsaved doc situations
        /// </summary>
        public void FileNew()
        {
            // if there are no unsaved changes, we can just create a new blank document
            if (rtbPage.Modified == false)
            {
                CreateNewDocument();
            }
            else
            {
                // if we have changes, prompt the user
                DialogResult result = MessageBox.Show(Strings.saveChangePrompt, Strings.saveChangesTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
            using (StreamWriter sw = new StreamWriter(gErrorLog, true))
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

                EncodingToolStripStatusLabel.Text = App.GetFileEncoding(filePath);
                ClearToolbarFormattingIcons();
                MoveCursorToLocation(0, 0);
                gPrevPageLength = rtbPage.TextLength;
                UpdateMRU();
                UpdateFormTitle(filePath);
                UpdateDocStats();
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
                    if (ofdFileOpen.FileName.EndsWith(Strings.txtExt))
                    {
                        LoadPlainTextFile(ofdFileOpen.FileName);
                    }
                    else
                    {
                        LoadRtfFile(ofdFileOpen.FileName);
                    }

                    UpdateFormTitle(ofdFileOpen.FileName);
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

                    ClearToolbarFormattingIcons();
                    MoveCursorToLocation(0, 0);
                    gPrevPageLength = rtbPage.TextLength;
                    UpdateDocStats();
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

                // if modified is true, untitled needs to be save as
                // any other file name is a regular save
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
                DialogResult result = MessageBox.Show(Strings.saveChangePrompt, Strings.saveChangesTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                MessageBox.Show("File No Longer Exists, Removing From Recent Files", "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            Properties.Settings.Default.Save();

            if (rtbPage.Modified == true)
            {
                DialogResult result = MessageBox.Show(Strings.saveChangePrompt, Strings.saveChangesTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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

            // if enabled, remove all non-app related files or no longer used templates
            if (Properties.Settings.Default.RemoveTempFilesOnExit == true)
            {
                CleanupTemplateFiles();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CleanupTemplateFiles()
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(Strings.appFolderTemplateDir);
                foreach (FileInfo f in dir.GetFiles("*.txt", SearchOption.AllDirectories))
                {
                    if (f.Name == Template1ToolStripMenuItem.Text + Strings.txtExt
                        || f.Name == Template2ToolStripMenuItem.Text + Strings.txtExt
                        || f.Name == Template3ToolStripMenuItem.Text + Strings.txtExt
                        || f.Name == Template4ToolStripMenuItem.Text + Strings.txtExt
                        || f.Name == Template5ToolStripMenuItem.Text + Strings.txtExt)
                    {
                        // leave current templates
                        continue;
                    }
                    else
                    {
                        // delete old templates
                        f.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLogContent("CleanupTempFiles Error: " + ex.Message);
                WriteErrorLogContent("Stack Trace: " + ex.StackTrace);
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
                lineCount = line.Split(new char[] { ' ', '.', '?', ',', ':', '\t', '=', ';', '-' }, StringSplitOptions.RemoveEmptyEntries).Length;
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
            PictureToolStripMenuItem.Enabled = true;
            TableToolStripMenuItem.Enabled = true;
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
            PictureToolStripMenuItem.Enabled = false;
            TableToolStripMenuItem.Enabled = false;
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
            // if the file is rtf no changes need to be made
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
            else
            {
                if (Properties.Settings.Default.DarkMode)
                {
                    rtbPage.ForeColor = Color.Black;
                }
                else
                {
                    rtbPage.ForeColor = Color.White;
                }
            }
        }

        /// <summary>
        /// change text color of controls and menu items
        /// </summary>
        /// <param name="clr"></param>
        public void ChangeControlTextColor(Color clr)
        {
            // update toolstrip buttons
            TimerToolStripLabel.ForeColor = clr;
            TimerToolStripLabelOnly.ForeColor = clr;
            StartStopTimerToolStripButton.ForeColor = clr;
            ResetTimerToolStripButton.ForeColor = clr;
            EditTimerToolStripButton.ForeColor = clr;
            FindToolStripButton.ForeColor = clr;
            FindToolStripLabel.ForeColor = clr;
            
            // update status bar labels
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

            // update File menu
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

            // update Edit menu
            EditToolStripMenuItem.ForeColor = clr;
            UndoToolStripMenuItem.ForeColor = clr;
            RedoToolStripMenuItem.ForeColor = clr;
            CutToolStripMenuItem.ForeColor = clr;
            CopyToolStripMenuItem.ForeColor = clr;
            PasteToolStripMenuItem.ForeColor = clr;
            SelectAllToolStripMenuItem.ForeColor = clr;
            ClearAllTextToolStripMenuItem.ForeColor = clr;
            FindToolStripMenuItem.ForeColor = clr;

            // update Insert menu
            InsertToolStripMenuItem.ForeColor = clr;
            PictureToolStripMenuItem.ForeColor = clr;
            TableToolStripMenuItem.ForeColor = clr;

            // update Formatting menu
            FormatToolStripMenuItem.ForeColor = clr;
            EditFontToolStripMenuItem.ForeColor = clr;
            ClearFormattingToolStripMenuItem.ForeColor = clr;
            BoldToolStripMenuItem.ForeColor = clr;
            ItalicToolStripMenuItem.ForeColor = clr;
            UnderlineToolStripMenuItem.ForeColor = clr;
            StrikethroughToolStripMenuItem.ForeColor = clr;

            // update Templates menu
            TemplatesToolStripMenuItem.ForeColor = clr;
            Template1ToolStripMenuItem.ForeColor = clr;
            Template2ToolStripMenuItem.ForeColor = clr;
            Template3ToolStripMenuItem.ForeColor = clr;
            Template4ToolStripMenuItem.ForeColor = clr;
            Template5ToolStripMenuItem.ForeColor = clr;
            ManageTemplatesToolStripMenuItem.ForeColor = clr;

            // update View menu
            ViewToolStripMenuItem.ForeColor = clr;
            WordWrapToolStripMenuItem.ForeColor = clr;
            ZoomToolStripMenuItem.ForeColor = clr;
            zoomToolStripMenuItem100.ForeColor = clr;
            zoomToolStripMenuItem150.ForeColor = clr;
            zoomToolStripMenuItem200.ForeColor = clr;
            zoomToolStripMenuItem250.ForeColor = clr;
            zoomToolStripMenuItem300.ForeColor = clr;

            // update help menu
            HelpToolStripMenuItem.ForeColor = clr;
            AboutToolStripMenuItem.ForeColor = clr;
            SubmitFeedbackToolStripMenuItem.ForeColor = clr;
            ReportBugToolStripMenuItem.ForeColor = clr;            
        }

        public void ChangeMenuItemBackColor(Color clr)
        {
            FileToolStripMenuItem.BackColor = clr;
            EditToolStripMenuItem.BackColor = clr;
            InsertToolStripMenuItem.BackColor = clr;
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
            // update File menu
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

            // update Edit menu
            UndoToolStripMenuItem.BackColor = clr;
            RedoToolStripMenuItem.BackColor = clr;
            CutToolStripMenuItem.BackColor = clr;
            CopyToolStripMenuItem.BackColor = clr;
            PasteToolStripMenuItem.BackColor = clr;
            SelectAllToolStripMenuItem.BackColor = clr;
            ClearAllTextToolStripMenuItem.BackColor = clr;
            FindToolStripMenuItem.BackColor = clr;
            // update Insert menu
            PictureToolStripMenuItem.BackColor = clr;
            TableToolStripMenuItem.BackColor = clr;

            // update Formatting menu
            EditFontToolStripMenuItem.BackColor = clr;
            ClearFormattingToolStripMenuItem.BackColor = clr;
            BoldToolStripMenuItem.BackColor = clr;
            ItalicToolStripMenuItem.BackColor = clr;
            UnderlineToolStripMenuItem.BackColor = clr;
            StrikethroughToolStripMenuItem.BackColor = clr;

            // update Templates menu
            Template1ToolStripMenuItem.BackColor = clr;
            Template2ToolStripMenuItem.BackColor = clr;
            Template3ToolStripMenuItem.BackColor = clr;
            Template4ToolStripMenuItem.BackColor = clr;
            Template5ToolStripMenuItem.BackColor = clr;
            ManageTemplatesToolStripMenuItem.BackColor = clr;

            // update View menu
            WordWrapToolStripMenuItem.BackColor = clr;
            ZoomToolStripMenuItem.BackColor = clr;
            zoomToolStripMenuItem100.BackColor = clr;
            zoomToolStripMenuItem150.BackColor = clr;
            zoomToolStripMenuItem200.BackColor = clr;
            zoomToolStripMenuItem250.BackColor = clr;
            zoomToolStripMenuItem300.BackColor = clr;

            // update Help menu
            AboutToolStripMenuItem.BackColor = clr;
            SubmitFeedbackToolStripMenuItem.BackColor = clr;
            ReportBugToolStripMenuItem.BackColor = clr;
        }

        private static Image ReplaceTransparency(Image image, Color background)
        {
            Bitmap bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(background);
                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.DrawImage(image, 0, 0, image.Width, image.Height);
            }

            return bitmap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string GetEmbedImageString(Image image)
        {
            Metafile metafile;
            float dpiX; float dpiY;

            using (Graphics g = Graphics.FromImage(image))
            {
                IntPtr hDC = g.GetHdc();
                metafile = new Metafile(hDC, EmfType.EmfOnly);
                g.ReleaseHdc(hDC);
            }

            using (Graphics g = Graphics.FromImage(metafile))
            {
                g.DrawImage(image, 0, 0);
                dpiX = g.DpiX;
                dpiY = g.DpiY;
            }

            IntPtr _hEmf = metafile.GetHenhmetafile();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            uint _bufferSize = GdipEmfToWmfBits(_hEmf, 0, null, MM_ANISOTROPIC,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
            byte[] _buffer = new byte[_bufferSize];
            GdipEmfToWmfBits(_hEmf, _bufferSize, _buffer, MM_ANISOTROPIC,
                                        EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
            IntPtr hmf = SetMetaFileBitsEx(_bufferSize, _buffer);
            string tempfile = Path.GetTempFileName();
            CopyMetaFile(hmf, tempfile);
            DeleteMetaFile(hmf);
            DeleteEnhMetaFile(_hEmf);

            var stream = new MemoryStream();
            byte[] data = File.ReadAllBytes(tempfile);
            int count = data.Length;
            stream.Write(data, 0, count);

            string proto = @"{\rtf1{\pict\wmetafile8\picw" + (int)(((float)image.Width / dpiX) * 2540)
                              + @"\pich" + (int)(((float)image.Height / dpiY) * 2540)
                              + @"\picwgoal" + (int)(((float)image.Width / dpiX) * 1440)
                              + @"\pichgoal" + (int)(((float)image.Height / dpiY) * 1440)
                              + " " + BitConverter.ToString(stream.ToArray()).Replace("-", "")
                              + "}}";
            return proto;
        }

        /// <summary>
        /// check the image for any transparent pixels
        /// looping all pixels can be slow on large images
        /// using every 5 pixels to balance out that perf hit
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static bool ContainsTransparent(Bitmap image)
        {
            // increment every 5 pixels for performance reasons
            for (int y = 0; y < image.Height; y += 5)
            {
                for (int x = 0; x < image.Width; x += 5)
                {
                    if (image.GetPixel(x, y).A != 255)
                    {
                        return true;
                    }
                }
            }
            return false;
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

        public void InsertTemplate(int templateNumber)
        {
            switch (templateNumber)
            {
                case 1: rtbPage.SelectedText = Templates.GetTemplate1().ToString(); break;
                case 2: rtbPage.SelectedText = Templates.GetTemplate2().ToString(); break;
                case 3: rtbPage.SelectedText = Templates.GetTemplate3().ToString(); break;
                case 4: rtbPage.SelectedText = Templates.GetTemplate4().ToString(); break;
                case 5: rtbPage.SelectedText = Templates.GetTemplate5().ToString(); break;
            }
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
                    string tempFilePath = Strings.appFolderDirectory + "\\" + gCurrentFileName;
                    using (StreamWriter sw = new StreamWriter(tempFilePath, false))
                    {
                        // need to avoid cross thread operation
                        rtbPage.Invoke((MethodInvoker)delegate {
                            rtbPage.SaveFile(tempFilePath, RichTextBoxStreamType.RichText);
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
        /// update line/column and toolbars
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RtbPage_SelectionChanged(object sender, EventArgs e)
        {
            UpdateLnColValues();
            UpdateToolbarIcons();
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
            // backspace causes selectionchange so the stats update there
            if (e.KeyCode == Keys.Back)
            {
                if (rtbPage.SelectionStart == rtbPage.GetFirstCharIndexOfCurrentLine() && rtbPage.SelectionBullet == true)
                {
                    e.SuppressKeyPress = true;
                    BulletToolStripButton.PerformClick();
                }
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

            Properties.Settings.Default.Save();
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

        /// <summary>
        /// when text is changed, need to update saved state and ui/stats
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        private void rtbPage_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (e.LinkText is not null)
            {
                App.PlatformSpecificProcessStart(e.LinkText.ToString());
            }
        }

        /// <summary>
        /// allow the user to select a picture to insert into the document/file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.png; *.bmp)|*.jpg; *.jpeg; *.gif; *.png; *.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // get the image from the dialog
                    Image img = Image.FromFile(ofd.FileName);

                    // creating a bitmap to check for transparencies
                    Bitmap bmp = new Bitmap(img);
                    if (ContainsTransparent(bmp))
                    {
                        // depending on the ui theme, apply the same color to the background
                        // TODO: not sure this is really what I want to happen since this changes the image
                        if (Properties.Settings.Default.DarkMode)
                        {
                            img = ReplaceTransparency(Image.FromFile(ofd.FileName), clrDarkModeTextBackground);
                        }
                        else
                        {
                            img = ReplaceTransparency(Image.FromFile(ofd.FileName), Color.White);
                        }
                    }

                    // now convert the image to rtf so it can be displayed in the rtb
                    rtbPage.SelectedRtf = GetEmbedImageString(img);
                    rtbPage.Focus();
                }
            }
        }

        private void Template1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertTemplate(1);
        }

        private void Template2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertTemplate(2);
        }

        private void Template3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertTemplate(3);
        }

        private void Template4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertTemplate(4);
        }

        private void Template5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertTemplate(5);
        }

        private void ManageTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmManageTemplates fTemplates = new FrmManageTemplates()
            {
                Owner = this
            };
            fTemplates.ShowDialog(this);
            Templates.WriteTemplatesToFile();
            UpdateTemplateMenu();
        }

        private void toolStripSeparator18_Paint(object sender, PaintEventArgs e)
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

        /// <summary>
        /// when the richtextbox gets focus, the cursor does not move to the mouse location
        /// using mouse move to accomplish this cursor position behavior
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rtbPage_MouseMove(object sender, MouseEventArgs e)
        {
            rtbPage.Focus();
        }

        private void tableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmInsertTable fTable = new FrmInsertTable()
            {
                Owner = this
            };
            fTable.ShowDialog(this);
            rtbPage.SelectedRtf = App.InsertTable(fTable.fRows, fTable.fCols, 2500);
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

        /// <summary>
        /// increment each second until we hit the autosave threshold
        /// then reset and call the background save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autosaveTimer_Tick(object sender, EventArgs e)
        {
            ticks++;
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
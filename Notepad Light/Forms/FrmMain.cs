using Notepad_Light.Forms;
using Notepad_Light.Helpers;

using System.Diagnostics;
using System.Reflection;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using Markdig;

namespace Notepad_Light
{
    public partial class FrmMain : Form
    {
        // globals
        public string gCurrentFileName = Strings.defaultFileName;
        public static string gErrorLog = string.Empty;
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

            // collapse panel 2 by default, currently only used for markdown files
            splitContainer1.Panel2Collapsed = true;

            // initialize stopwatch for timer
            gStopwatch = new Stopwatch();

            // initialize dark mode colors
            clrDarkModeBackground = Color.FromArgb(32, 32, 32);
            clrDarkModeTextBackground = Color.FromArgb(96, 96, 96);

            // init file MRU
            UpdateMRU();

            // update status bar with app version
            appVersionToolStripStatusLabel.Text = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

            // set initial zoom to 100 and update menu
            ZoomToolStripMenuItem100.Checked = true;
            ApplyZoom(1.0f);

            // set word wrap
            WordWrapToolStripMenuItem.Checked = RtbPage.WordWrap;

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

            RtbPage.Modified = false;

            // start the autosave timer
            autosaveTimer.Start();

            // setup log file
            gErrorLog = Strings.appFolderDirectoryUrl + Strings.errorLogFile;

            // setup templates
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
            SetupAppFiles();
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
            this.Text = Strings.AppTitle + docName;
            gCurrentFileName = docName;
        }

        /// <summary>
        /// function to make sure files that are used in the app exist
        /// </summary>
        public void SetupAppFiles()
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
                    using var myFile = File.Create(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template1 + Strings.txtExt);
                }

                // template 2
                if (!File.Exists(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template2 + Strings.txtExt))
                {
                    using var myFile = File.Create(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template2 + Strings.txtExt);
                }

                // template 3 
                if (!File.Exists(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template3 + Strings.txtExt))
                {
                    using var myFile = File.Create(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template3 + Strings.txtExt);
                }

                // template 4
                if (!File.Exists(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template4 + Strings.txtExt))
                {
                    using var myFile = File.Create(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template4 + Strings.txtExt);
                }

                // template 5
                if (!File.Exists(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template5 + Strings.txtExt))
                {
                    using var myFile = File.Create(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template5 + Strings.txtExt);
                }
            }
        }

        /// <summary>
        /// mostly UI setup work, it doesn't really create a new file/document
        /// </summary>
        public void CreateNewDocument()
        {
            RtbPage.Clear();
            CollapsePanel2();
            ClearFormatting();
            RtbPage.ReadOnly = false;
            readOnlyToolStripStatusLabel.Text = "Editing";
            UpdateFormTitle(Strings.defaultFileName);

            // check for file type and update UI accordingly
            if (Properties.Settings.Default.NewFileFormat == Strings.rtf)
            {
                gRtf = true;
                EnableToolbarFormattingIcons();
                EncodingToolStripStatusLabel.Text = App.GetFileEncoding(RtbPage.Rtf, true);
            }
            else
            {
                gRtf = false;
                ClearToolbarFormattingIcons();
                EncodingToolStripStatusLabel.Text = Encoding.UTF8.EncodingName;
            }

            UpdateStatusBar();
            RtbPage.Modified = false;
        }

        /// <summary>
        /// there is no file->close so file->new handles unsaved doc situations
        /// </summary>
        public void FileNew()
        {
            // if there are no unsaved changes, we can just create a new blank document
            if (RtbPage.Modified == false)
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
            RtbPage.LoadFile(filePath, RichTextBoxStreamType.PlainText);
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
            RtbPage.LoadFile(filePath, RichTextBoxStreamType.RichText);
            EnableToolbarFormattingIcons();
            gRtf = true;
            toolStripStatusLabelFileType.Text = Strings.rtf;
        }

        /// <summary>
        /// load the markdown file and update UI
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadMarkdownFile(string filePath)
        {
            RtbPage.LoadFile(filePath, RichTextBoxStreamType.PlainText);
            DisableToolbarFormattingIcons();
            splitContainer1.Panel2Collapsed = false;
            TaskPaneToolStripMenuItem.Checked = true;
            gRtf = false;
            toolStripStatusLabelFileType.Text = Strings.markdown;
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
                CollapsePanel2();
                CheckForReadOnly(filePath);
                if (filePath.EndsWith(Strings.txtExt))
                {
                    LoadPlainTextFile(filePath);
                    UnloadMarkdown();
                }
                else if (filePath.EndsWith(Strings.rtfExt))
                {
                    LoadRtfFile(filePath);
                    UnloadMarkdown();
                }
                else if (filePath.EndsWith(Strings.mdExt) || filePath.EndsWith(Strings.md2Ext))
                {
                    LoadMarkdownFile(filePath);
                    LoadMarkdownInWebView2();
                }

                EncodingToolStripStatusLabel.Text = App.GetFileEncoding(filePath, false);
                ClearToolbarFormattingIcons();
                MoveCursorToLocation(0, 0);
                gPrevPageLength = RtbPage.TextLength;
                UpdateMRU();
                UpdateFormTitle(filePath);
                UpdateDocStats();
                RtbPage.Modified = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open file - " + ex.Message, "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.WriteErrorLogContent("FileMRUOpen Error = " + ex.Message, gErrorLog);
            }
            finally
            {
                UpdateToolbarIcons();
                Cursor = Cursors.Default;
            }
        }

        public void CheckForReadOnly(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            if (fi.IsReadOnly)
            {
                readOnlyToolStripStatusLabel.Text = "Read-Only";
            }
            else
            {
                readOnlyToolStripStatusLabel.Text = "Editing";
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
                CollapsePanel2();

                // prompt for changes
                SaveChanges();

                // start the file open process
                OpenFileDialog ofdFileOpen = new OpenFileDialog
                {
                    Title = "Open Document",
                    Filter = "Plaint Text Format | *.txt;|" +
                             "Rich Text Format | *.rtf; |" +
                             "Markdown Format | *.md; *.markdown",
                    AutoUpgradeEnabled = true,
                    RestoreDirectory = true,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                // add the contents to the textbox
                if (ofdFileOpen.ShowDialog() == DialogResult.OK)
                {
                    if (ofdFileOpen.FileName.EndsWith(Strings.txtExt))
                    {
                        LoadPlainTextFile(ofdFileOpen.FileName);
                        UnloadMarkdown();
                    }
                    else if (ofdFileOpen.FileName.EndsWith(Strings.rtfExt))
                    {
                        LoadRtfFile(ofdFileOpen.FileName);
                        UnloadMarkdown();
                    }
                    else if (ofdFileOpen.FileName.EndsWith(Strings.mdExt) || ofdFileOpen.FileName.EndsWith(Strings.md2Ext))
                    {
                        LoadMarkdownFile(ofdFileOpen.FileName);
                        LoadMarkdownInWebView2();
                    }

                    UpdateFormTitle(ofdFileOpen.FileName);
                    EncodingToolStripStatusLabel.Text = App.GetFileEncoding(ofdFileOpen.FileName, false);

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
                        if (Properties.Settings.Default.FileMRU.Count > 9)
                        {
                            Properties.Settings.Default.FileMRU.RemoveAt(0);
                        }
                        UpdateMRU();
                    }

                    ClearToolbarFormattingIcons();
                    MoveCursorToLocation(0, 0);
                    gPrevPageLength = RtbPage.TextLength;
                    UpdateDocStats();
                    CheckForReadOnly(ofdFileOpen.FileName);
                    RtbPage.Modified = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open file - " + ex.Message, "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.WriteErrorLogContent("FileOpen Error = " + ex.Message, gErrorLog);
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

                // no changes to save
                if (RtbPage.Modified == false)
                {
                    return;
                }

                // if modified untitled = saveas, existing files only need save
                // if the file is readonly, any changes need to be a new file
                if ((gCurrentFileName.ToString() == Strings.defaultFileName && RtbPage.Modified == true) || readOnlyToolStripStatusLabel.Text == "Read-Only")
                {
                    FileSaveAs();
                }
                else
                {
                    if (gCurrentFileName.EndsWith(Strings.txtExt) || gCurrentFileName.EndsWith(Strings.mdExt) || gCurrentFileName.EndsWith(Strings.md2Ext))
                    {
                        RtbPage.SaveFile(gCurrentFileName, RichTextBoxStreamType.PlainText);
                    }

                    if (gCurrentFileName.EndsWith(Strings.rtfExt))
                    {
                        RtbPage.SaveFile(gCurrentFileName, RichTextBoxStreamType.RichText);
                    }

                    RtbPage.Modified = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save file - " + ex.Message, "File Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.WriteErrorLogContent("FileSave Error : " + ex.Message, gErrorLog);
            }
            finally
            {
                UpdateToolbarIcons();
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
                    sfdSaveAs.Title = "Save As";
                    sfdSaveAs.Filter = "Plain Text Format (*.txt)|*.txt |Markdown Format (*.md)|*.md |Rich Text Format (*.rtf)|*.rtf";

                    // change the file type dropdown based on the selected file format
                    if (gRtf)
                    {
                        sfdSaveAs.FilterIndex = 3;
                    }
                    else
                    {
                        sfdSaveAs.FilterIndex = 1;
                    }

                    // save out the file based on the user selections
                    if (sfdSaveAs.ShowDialog() == DialogResult.OK && sfdSaveAs.FileName.Length > 0)
                    {
                        Cursor = Cursors.WaitCursor;
                        if (sfdSaveAs.FilterIndex == 1 || sfdSaveAs.FilterIndex == 2)
                        {
                            RtbPage.SaveFile(sfdSaveAs.FileName, RichTextBoxStreamType.PlainText);
                        }
                        else
                        {
                            RtbPage.SaveFile(sfdSaveAs.FileName, RichTextBoxStreamType.RichText);
                        }

                        UpdateFormTitle(sfdSaveAs.FileName);
                        RtbPage.Modified = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save file - " + ex.Message, "File Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.WriteErrorLogContent("FileSaveAs Error :" + ex.Message, gErrorLog);
            }
            finally
            {
                UpdateStatusBar();
                UpdateToolbarIcons();
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        ///  if there are unsaved changes, prompt the user before opening
        /// </summary>
        public void SaveChanges()
        {
            if (RtbPage.Modified)
            {
                DialogResult result = MessageBox.Show(Strings.saveChangePrompt, Strings.saveChangesTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    FileSave();
                }
            }
        }

        /// <summary>
        /// if the path is valid, open it or remove it
        /// </summary>
        /// <param name="filePath"></param>
        public void OpenRecentFile(string filePath)
        {
            if (filePath == string.Empty)
            {
                return;
            }

            // if there are unsaved changes, prompt the user before opening
            SaveChanges();

            if (File.Exists(filePath))
            {
                FileOpen(filePath);
            }
            else
            {
                RemoveFileFromMRU(filePath);
            }

            gPrevPageLength = RtbPage.TextLength;
        }

        public void Print()
        {
            printDialog1.Document = printDocument1;
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                gPrintString = RtbPage.Text;
                printDocument1.Print();
            }
        }

        public void PrintPreview()
        {
            printPreviewDialog1.Document = printDocument1;
            gPrintString = RtbPage.Text;
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
            RecentToolStripMenuItem6.Text = Strings.empty;
            RecentToolStripMenuItem7.Text = Strings.empty;
            RecentToolStripMenuItem8.Text = Strings.empty;
            RecentToolStripMenuItem9.Text = Strings.empty;
        }

        /// <summary>
        /// for files that don't exist, we need to update the ui and settings mru list
        /// </summary>
        /// <param name="path"></param>
        /// <param name="index"></param>
        public void RemoveFileFromMRU(string path)
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
                App.WriteErrorLogContent("File MRU Path Removed : " + path, gErrorLog);
                MessageBox.Show("File No Longer Exists, Removing From Recent Files", "Invalid File Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // clear the menu and add back the remaining files
            ClearRecentMenuItems();
            UpdateMRU();
        }

        public void Cut()
        {
            RtbPage.Cut();
            RtbPage.Modified = true;
        }

        public void Copy()
        {
            RtbPage.Copy();
        }

        public void Paste()
        {
            // if the clipboard is empty, do nothing
            if (Clipboard.GetDataObject().GetFormats().Length == 0)
            {
                return;
            }

            RtbPage.Modified = true;

            if (Properties.Settings.Default.UsePasteUI == false)
            {
                // when the clipboard has text and the user doesn't want to use the UI, use that by default
                // otherwise use whatever Paste() selects by default
                DataFormats.Format df = DataFormats.GetFormat(DataFormats.UnicodeText);
                if (RtbPage.CanPaste(df))
                {
                    RtbPage.Paste(df);
                }
                else
                {
                    RtbPage.Paste();
                }
            }
            else
            {
                // show the form so the user can decide what format to paste
                FrmPasteUI pFrm = new FrmPasteUI()
                {
                    Owner = this
                };
                pFrm.ShowDialog();

                // paste based on user selection
                switch (pFrm.SelectedPasteOption)
                {
                    case Strings.pasteCsv: RtbPage.SelectedText = Clipboard.GetText(TextDataFormat.CommaSeparatedValue); break;
                    case Strings.plainText: RtbPage.SelectedText = Clipboard.GetText(TextDataFormat.Text); break;
                    case Strings.pasteHtml: RtbPage.SelectedText = Clipboard.GetText(TextDataFormat.Html); break;
                    case Strings.rtf:
                        if (Properties.Settings.Default.PasteRtfUnformatted)
                        {
                            // paste as unformatted rtf
                            RtbPage.SelectedText = Clipboard.GetData(DataFormats.Rtf).ToString();
                        }
                        else
                        {
                            // paste as formatted rtf
                            byte[] rtfByteArray = Encoding.UTF8.GetBytes(Clipboard.GetText(TextDataFormat.Rtf));
                            MemoryStream rtfStream = new MemoryStream(rtfByteArray);
                            RichTextBox tempRtb = new RichTextBox();
                            tempRtb.LoadFile(rtfStream, RichTextBoxStreamType.RichText);
                            tempRtb.Copy();
                            RtbPage.Paste();
                        }
                        break;
                    case Strings.pasteUnicode: RtbPage.SelectedText = Clipboard.GetText(TextDataFormat.UnicodeText); break;
                    case Strings.pasteImage: RtbPage.Paste(); break;
                    default:
                        RtbPage.Paste();
                        RtbPage.Modified = false;
                        break;
                }
            }
        }

        public void Undo()
        {
            RtbPage.Undo();
            RtbPage.Modified = true;
        }

        public void Redo()
        {
            RtbPage.Redo();
            RtbPage.Modified = true;
        }

        /// <summary>
        /// if there are unsaved changes, we need to ask the user what to do
        /// </summary>
        /// <param name="fromFormClosingEvent">tells the function we came from the FormClosing event</param>
        public void ExitAppWork(bool fromFormClosingEvent)
        {
            Properties.Settings.Default.Save();
            SaveChanges();

            // formclosingevent will fire twice if we call app exit anywhere from form closing
            // only call app.exit if we aren't coming from the event
            if (!fromFormClosingEvent)
            {
                Application.Exit();
            }

            // remove all non-app related files or no longer used templates
            if (Properties.Settings.Default.RemoveTempFilesOnExit == true)
            {
                CleanupTemplateFiles();
            }
        }

        /// <summary>
        /// remove template files no longer being used
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
                App.WriteErrorLogContent("CleanupTempFiles Error: " + ex.Message, gErrorLog);
                App.WriteErrorLogContent("Stack Trace: " + ex.StackTrace, gErrorLog);
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
                if (RtbPage.SelectionFont is null)
                {
                    ClearToolbarFormattingIcons();
                    return;
                }

                // bold
                if (RtbPage.SelectionFont.Bold == true)
                {
                    BoldToolStripButton.Checked = true;
                }
                else
                {
                    BoldToolStripButton.Checked = false;
                }

                // italic
                if (RtbPage.SelectionFont.Italic == true)
                {
                    ItalicToolStripButton.Checked = true;
                }
                else
                {
                    ItalicToolStripButton.Checked = false;
                }

                // underline
                if (RtbPage.SelectionFont.Underline == true)
                {
                    UnderlineToolStripButton.Checked = true;
                }
                else
                {
                    UnderlineToolStripButton.Checked = false;
                }

                // strikethrough
                if (RtbPage.SelectionFont.Strikeout == true)
                {
                    StrikethroughToolStripButton.Checked = true;
                }
                else
                {
                    StrikethroughToolStripButton.Checked = false;
                }

                // bullets
                if (RtbPage.SelectionBullet == true)
                {
                    BulletToolStripButton.Checked = true;
                }
                else
                {
                    BulletToolStripButton.Checked = false;
                }

                // alignment
                if (RtbPage.SelectionAlignment == HorizontalAlignment.Left)
                {
                    LeftJustifiedToolStripButton.Checked = true;
                    CenterJustifiedToolStripButton.Checked = false;
                    RightJustifiedToolStripButton.Checked = false;
                }
                else if (RtbPage.SelectionAlignment == HorizontalAlignment.Center)
                {
                    CenterJustifiedToolStripButton.Checked = true;
                    LeftJustifiedToolStripButton.Checked = false;
                    RightJustifiedToolStripButton.Checked = false;
                }
                else if (RtbPage.SelectionAlignment == HorizontalAlignment.Right)
                {
                    RightJustifiedToolStripButton.Checked = true;
                    CenterJustifiedToolStripButton.Checked = false;
                    LeftJustifiedToolStripButton.Checked = false;
                }

                // save icon
                if (RtbPage.Modified)
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
                App.WriteErrorLogContent("UpdateToolbarIcons Error" + ex.Message, gErrorLog);
            }
        }

        /// <summary>
        /// gather details for number of word, char and lines
        /// </summary>
        public void UpdateDocStats()
        {
            int totalWordCount = 0;
            int lineCount;
            foreach (string line in RtbPage.Lines)
            {
                char[] delimiters = { ' ', '.', '?', ',', ':', '\t', ';', '-', '!', '\'', '=', '|',
                    '&', '@', '#', '*', '%', '~', '(', ')', '/', '+', '[', ']', '{', '}', '<', '>', '$', '^' };
                lineCount = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
                totalWordCount += lineCount;
            }

            // update the statusbar
            WordCountToolStripStatusLabel.Text = totalWordCount.ToString();
            CharacterCountToolStripStatusLabel.Text = RtbPage.Text.Length.ToString();
            LinesToolStripStatusLabel.Text = RtbPage.Lines.Length.ToString();
        }

        /// <summary>
        /// set font to the default values
        /// </summary>
        public void ClearFormatting()
        {
            RtbPage.SelectionBullet = false;
            RtbPage.SelectionFont = new Font("Segoe UI", 9);
            RtbPage.SelectionColor = Color.Black;
            RtbPage.SelectionIndent = 0;
            RtbPage.SelectionAlignment = HorizontalAlignment.Left;
            RtbPage.Modified = true;
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
            RightJustifiedToolStripButton.Enabled = true;
            CenterJustifiedToolStripButton.Enabled = true;
            LeftJustifiedToolStripButton.Enabled = true;
            DecreaseIndentToolStripButton.Enabled = true;
            IncreaseIndentToolStripButton.Enabled = true;
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
            RightJustifiedToolStripButton.Enabled = false;
            CenterJustifiedToolStripButton.Enabled = false;
            LeftJustifiedToolStripButton.Enabled = false;
            DecreaseIndentToolStripButton.Enabled = false;
            IncreaseIndentToolStripButton.Enabled = false;
        }

        /// <summary>
        /// clear the icon states that are based on selection
        /// </summary>
        public void ClearToolbarFormattingIcons()
        {
            BoldToolStripButton.Checked = false;
            ItalicToolStripButton.Checked = false;
            UnderlineToolStripButton.Checked = false;
            StrikethroughToolStripButton.Checked = false;
            BulletToolStripButton.Checked = false;
            RightJustifiedToolStripButton.Checked = false;
            CenterJustifiedToolStripButton.Checked = false;
            LeftJustifiedToolStripButton.Checked = false;
        }

        /// <summary>
        /// ui work needed after formatting buttons have been applied/used
        /// </summary>
        public void EndOfButtonFormatWork()
        {
            RtbPage.Focus();
            UpdateToolbarIcons();
            RtbPage.Modified = true;
        }

        public void ApplyBold()
        {
            if (RtbPage.SelectionFont.Bold == true)
            {
                RtbPage.SelectionFont = new Font(RtbPage.SelectionFont, RtbPage.SelectionFont.Style & ~FontStyle.Bold);
            }
            else
            {
                RtbPage.SelectionFont = new Font(RtbPage.SelectionFont, RtbPage.SelectionFont.Style | FontStyle.Bold);
            }

            EndOfButtonFormatWork();
        }

        public void ApplyItalic()
        {
            if (RtbPage.SelectionFont.Italic == true)
            {
                RtbPage.SelectionFont = new Font(RtbPage.SelectionFont, RtbPage.SelectionFont.Style & ~FontStyle.Italic);
            }
            else
            {
                RtbPage.SelectionFont = new Font(RtbPage.SelectionFont, RtbPage.SelectionFont.Style | FontStyle.Italic);
            }

            EndOfButtonFormatWork();
        }

        public void ApplyUnderline()
        {
            if (RtbPage.SelectionFont.Underline == true)
            {
                RtbPage.SelectionFont = new Font(RtbPage.SelectionFont, RtbPage.SelectionFont.Style & ~FontStyle.Underline);
            }
            else
            {
                RtbPage.SelectionFont = new Font(RtbPage.SelectionFont, RtbPage.SelectionFont.Style | FontStyle.Underline);
            }

            EndOfButtonFormatWork();
        }

        public void ApplyStrikethrough()
        {
            if (RtbPage.SelectionFont.Strikeout == true)
            {
                RtbPage.SelectionFont = new Font(RtbPage.SelectionFont, RtbPage.SelectionFont.Style & ~FontStyle.Strikeout);
            }
            else
            {
                RtbPage.SelectionFont = new Font(RtbPage.SelectionFont, RtbPage.SelectionFont.Style | FontStyle.Strikeout);
            }

            EndOfButtonFormatWork();
        }

        /// <summary>
        /// move the cursor to specified location in the richtextbox
        /// </summary>
        /// <param name="startLocation"></param>
        /// <param name="length"></param>
        public void MoveCursorToLocation(int startLocation, int length)
        {
            RtbPage.SelectionStart = startLocation;
            RtbPage.SelectionLength = length;
        }

        /// <summary>
        /// populate the recent menu with the file mru settings list
        /// </summary>
        public void UpdateMRU()
        {
            try
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
                        case 6: RecentToolStripMenuItem6.Text = f?.ToString(); break;
                        case 7: RecentToolStripMenuItem7.Text = f?.ToString(); break;
                        case 8: RecentToolStripMenuItem8.Text = f?.ToString(); break;
                        case 9: RecentToolStripMenuItem9.Text = f?.ToString(); break;
                    }
                    index++;
                }
            }
            catch (Exception ex)
            {
                // log the error and do not update mru
                App.WriteErrorLogContent("UpdateMRU Error: " + ex.Message, gErrorLog);
            }
        }

        /// <summary>
        /// update the font into in the toolbar
        /// </summary>
        public void UpdateFontInformation()
        {
            try
            {
                fontToolStripStatusLabel.Text = "Font: " + RtbPage.SelectionFont.Name + " Size: " + RtbPage.SelectionFont.Size + " pt";
            }
            catch (Exception ex)
            {
                App.WriteErrorLogContent("UpdateFontInformation Error: " + ex.Message, gErrorLog);
            }
        }

        /// <summary>
        /// get the current cursor position and update the statusbar with the location
        /// </summary>
        public void UpdateLnColValues()
        {
            int line = RtbPage.GetLineFromCharIndex(RtbPage.SelectionStart);
            int column = RtbPage.SelectionStart - RtbPage.GetFirstCharIndexFromLine(line);

            line++;
            column++;

            toolStripStatusLabelLine.Text = line.ToString();
            toolStripStatusLabelColumn.Text = column.ToString();
        }

        public void InsertDateTime()
        {
            RtbPage.SelectedText = DateTime.Now.ToString() + "\r\n";
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
        /// given a float percentage value, clear the menu
        /// and light up the new value then apply the zoom value
        /// </summary>
        /// <param name="zoomPercentage"></param>
        public void ApplyZoom(float zoomPercentage)
        {
            RtbPage.ZoomFactor = zoomPercentage;

            // reset the check marks
            ZoomToolStripMenuItem100.Checked = false;
            ZoomToolStripMenuItem150.Checked = false;
            ZoomToolStripMenuItem200.Checked = false;
            ZoomToolStripMenuItem250.Checked = false;
            ZoomToolStripMenuItem300.Checked = false;

            // mark the menu item based on the zoom %
            switch (zoomPercentage)
            {
                case 1.0f: ZoomToolStripMenuItem100.Checked = true; break;
                case 1.5f: ZoomToolStripMenuItem150.Checked = true; break;
                case 2.0f: ZoomToolStripMenuItem200.Checked = true; break;
                case 2.5f: ZoomToolStripMenuItem250.Checked = true; break;
                case 3.0f: ZoomToolStripMenuItem300.Checked = true; break;
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
            RtbPage.BackColor = clrDarkModeTextBackground;

            ChangeControlTextColor(Color.White);
            ChangeSubMenuItemBackColor(clrDarkModeBackground);
            ChangeMenuItemBackColor(clrDarkModeBackground);
        }

        /// <summary>
        /// change the background to have the default UI
        /// </summary>
        public void ApplyLightMode()
        {
            menuStrip1.BackColor = Color.FromKnownColor(KnownColor.Control);
            toolStrip1.BackColor = Color.FromKnownColor(KnownColor.Control);
            statusStrip1.BackColor = Color.FromKnownColor(KnownColor.Control);
            RtbPage.BackColor = Color.FromKnownColor(KnownColor.Window);

            ChangeControlTextColor(Color.Black);
            ChangeSubMenuItemBackColor(Color.White);
            ChangeMenuItemBackColor(Color.FromKnownColor(KnownColor.Control));
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
            SearchToolStripLabel.ForeColor = clr;

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
            toolStripStatusLabelFileEncoding.ForeColor = clr;
            fontToolStripStatusLabel.ForeColor = clr;

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
            RecentToolStripMenuItem6.ForeColor = clr;
            RecentToolStripMenuItem7.ForeColor = clr;
            RecentToolStripMenuItem8.ForeColor = clr;
            RecentToolStripMenuItem9.ForeColor = clr;
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
            ReplaceToolStripMenuItem.ForeColor = clr;

            // update Insert menu
            InsertToolStripMenuItem.ForeColor = clr;
            PictureToolStripMenuItem.ForeColor = clr;
            TableToolStripMenuItem.ForeColor = clr;
            dateTimeToolStripMenuItem.ForeColor = clr;

            // update Formatting menu
            FormatToolStripMenuItem.ForeColor = clr;
            EditFontToolStripMenuItem.ForeColor = clr;
            ClearFormattingToolStripMenuItem.ForeColor = clr;

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
            ZoomToolStripMenuItem100.ForeColor = clr;
            ZoomToolStripMenuItem150.ForeColor = clr;
            ZoomToolStripMenuItem200.ForeColor = clr;
            ZoomToolStripMenuItem250.ForeColor = clr;
            ZoomToolStripMenuItem300.ForeColor = clr;
            TaskPaneToolStripMenuItem.ForeColor = clr;

            // update Help menu
            HelpToolStripMenuItem.ForeColor = clr;
            AboutToolStripMenuItem.ForeColor = clr;
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
            RecentToolStripMenuItem6.BackColor = clr;
            RecentToolStripMenuItem7.BackColor = clr;
            RecentToolStripMenuItem8.BackColor = clr;
            RecentToolStripMenuItem9.BackColor = clr;
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
            ReplaceToolStripMenuItem.BackColor = clr;

            // update Insert menu
            PictureToolStripMenuItem.BackColor = clr;
            TableToolStripMenuItem.BackColor = clr;
            dateTimeToolStripMenuItem.BackColor = clr;

            // update Formatting menu
            EditFontToolStripMenuItem.BackColor = clr;
            ClearFormattingToolStripMenuItem.BackColor = clr;

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
            ZoomToolStripMenuItem100.BackColor = clr;
            ZoomToolStripMenuItem150.BackColor = clr;
            ZoomToolStripMenuItem200.BackColor = clr;
            ZoomToolStripMenuItem250.BackColor = clr;
            ZoomToolStripMenuItem300.BackColor = clr;
            TaskPaneToolStripMenuItem.BackColor = clr;

            // update Help menu
            AboutToolStripMenuItem.BackColor = clr;
            ReportBugToolStripMenuItem.BackColor = clr;
        }

        /// <summary>
        /// change the image transparency to the color passed into background
        /// </summary>
        /// <param name="image"></param>
        /// <param name="background"></param>
        /// <returns></returns>
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
        /// check the image for any transparent pixels
        /// looping all pixels can be slow on large images
        /// using every 5 pixels to balance out the perf hit
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static bool ContainsTransparentPixel(Bitmap image)
        {
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
        /// search bing for the selected text and display it in the panel
        /// </summary>
        private async void WebView2Navigate()
        {
            await webView2Md.EnsureCoreWebView2Async();
            string searchUrl = "https://www.bing.com/search?q=" + RtbPage.SelectedText;
            webView2Md.Source = new Uri(searchUrl);
        }

        /// <summary>
        /// function to replace text, Find button needs to be clicked before this will work
        /// </summary>
        public void ReplaceText()
        {
            if (RtbPage.SelectedText == string.Empty)
            {
                return;
            }

            FrmReplace fReplace = new FrmReplace(ActiveForm)
            {
                Owner = this
            };
            fReplace.ShowDialog(this);

            // user cancelled the dialog scenario or no matches were found
            if (fReplace.formExited)
            {
                return;
            }

            // single replace
            RtbPage.SelectedText = RtbPage.SelectedText.Replace(RtbPage.SelectedText, fReplace.replaceText);
        }

        /// <summary>
        /// handle the user clicking the Find button
        /// </summary>
        public void FindText()
        {
            if (FindTextBox.Text == string.Empty)
            {
                return;
            }
            else
            {
                // if the cursor is at the end of the textbox, change start position to 0
                if (RtbPage.SelectionStart == RtbPage.Text.Length)
                {
                    MoveCursorToLocation(0, 0);
                }

                int indexToText;
                if (Properties.Settings.Default.SearchOption == Strings.findUp)
                {
                    // search up the file and find any word match
                    indexToText = RtbPage.Find(FindTextBox.Text, 0, RtbPage.SelectionStart, RichTextBoxFinds.Reverse);
                }
                else if (Properties.Settings.Default.SearchOption == Strings.findDown)
                {
                    // search from the top down and find any word match
                    indexToText = RtbPage.Find(FindTextBox.Text, RtbPage.SelectionStart + 1, RichTextBoxFinds.None);
                }
                else if (Properties.Settings.Default.SearchOption == Strings.findMatchCase)
                {
                    // only find words that match the case exactly
                    indexToText = RtbPage.Find(FindTextBox.Text, RtbPage.SelectionStart + 1, RichTextBoxFinds.MatchCase);
                }
                else
                {
                    // only find words that have the entire word in the search textbox
                    indexToText = RtbPage.Find(FindTextBox.Text, RtbPage.SelectionStart + 1, RichTextBoxFinds.WholeWord);
                }

                // move to the location of the find result
                if (indexToText >= 0)
                {
                    MoveCursorToLocation(indexToText, FindTextBox.Text.Length);
                }

                // end of the document, restart at the beginning
                if (indexToText == -1)
                {
                    // only move if something was found
                    if (RtbPage.SelectionStart != 0)
                    {
                        MoveCursorToLocation(0, 0);
                        FindText();
                    }
                }
            }
        }

        public void InsertTemplate(int templateNumber)
        {
            if (Properties.Settings.Default.IncludeDateTimeWithTemplates)
            {
                InsertDateTime();
            }

            switch (templateNumber)
            {
                case 1: RtbPage.SelectedText = Templates.GetTemplate1().ToString(); break;
                case 2: RtbPage.SelectedText = Templates.GetTemplate2().ToString(); break;
                case 3: RtbPage.SelectedText = Templates.GetTemplate3().ToString(); break;
                case 4: RtbPage.SelectedText = Templates.GetTemplate4().ToString(); break;
                case 5: RtbPage.SelectedText = Templates.GetTemplate5().ToString(); break;
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
                // if the doc is not modified, nothing to do
                RtbPage.Invoke((MethodInvoker)delegate { if (RtbPage.Modified == false) { return; } });

                // save the existing changes to the file
                if (gCurrentFileName != Strings.defaultFileName)
                {
                    // need to avoid cross thread operation
                    RtbPage.Invoke((MethodInvoker)delegate { FileSave(); });
                }
            }
            catch (Exception ex)
            {
                App.WriteErrorLogContent("WriteFileContents Error: " + ex.Message, gErrorLog);
                App.WriteErrorLogContent("Stack: " + ex.StackTrace, gErrorLog);
            }
        }

        /// <summary>
        /// collapse panel 2 when not needed
        /// </summary>
        public void CollapsePanel2()
        {
            if (splitContainer1.Panel2Collapsed == false)
            {
                splitContainer1.Panel2Collapsed = true;
                TaskPaneToolStripMenuItem.Checked = false;
            }
        }

        private async void UnloadMarkdown()
        {
            // initialize the webview
            await webView2Md.EnsureCoreWebView2Async();
            string? html = Markdown.ToHtml(string.Empty);
            webView2Md.NavigateToString(html);
        }

        /// <summary>
        /// initialize the webview2 control and load the html
        /// </summary>
        /// <param name="html"></param>
        private async void LoadMarkdownInWebView2()
        {
            // initialize the webview
            await webView2Md.EnsureCoreWebView2Async();

            // render the html content of the markdown text
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            string? html = Markdown.ToHtml(RtbPage.Text, pipeline);
            webView2Md.NavigateToString(html);
        }

        #endregion

        #region Events

        /// <summary>
        /// known issue in toolstripseparator not using back/fore colors
        /// https://stackoverflow.com/questions/15926377/change-the-backcolor-of-the-toolstripseparator-control
        /// this is for MenuStrip toolstrip separators, but not needed for ToolStrip separators
        /// </summary>
        /// <param name="sender">object param from the paint event</param>
        /// <param name="e">eventarg from paint event</param>
        /// <param name="cFill">backcolor for the separator</param>
        /// <param name="cLine">forecolor for the separator</param>
        public void PaintToolStripSeparator(object sender, PaintEventArgs e)
        {
            ToolStripSeparator sep = (ToolStripSeparator)sender;
            if (Properties.Settings.Default.DarkMode)
            {
                e.Graphics.FillRectangle(new SolidBrush(clrDarkModeBackground), 0, 0, sep.Width, sep.Height);
                e.Graphics.DrawLine(new Pen(Color.White), 30, sep.Height / 2, sep.Width - 4, sep.Height / 2);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, sep.Width, sep.Height);
                e.Graphics.DrawLine(new Pen(clrDarkModeBackground), 30, sep.Height / 2, sep.Width - 4, sep.Height / 2);
            }
        }

        /// <summary>
        /// update line/column and toolbars
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RtbPage_SelectionChanged(object sender, EventArgs e)
        {
            UpdateLnColValues();
            UpdateToolbarIcons();
            UpdateFontInformation();
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
            RtbPage.SelectAll();
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
            if (gRtf)
            {
                fontDialog1.ShowColor = true;
                fontDialog1.ShowEffects = true;
            }
            else
            {
                fontDialog1.ShowColor = false;
                fontDialog1.ShowEffects = false;
            }

            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                // rtf will allow font and color changes
                if (gRtf)
                {
                    RtbPage.SelectionFont = fontDialog1.Font;
                    RtbPage.SelectionColor = fontDialog1.Color;
                }
                else
                {
                    // for plain text, only change font
                    RtbPage.Font = fontDialog1.Font;
                }
            }
        }

        private void ClearFormattingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearFormatting();
            EndOfButtonFormatWork();
        }

        private void ClearAllTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RtbPage.ResetText();
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
            if (RtbPage.WordWrap == true)
            {
                RtbPage.WordWrap = false;
                WordWrapToolStripMenuItem.Checked = false;
            }
            else
            {
                RtbPage.WordWrap = true;
                WordWrapToolStripMenuItem.Checked = true;
            }
        }

        private void BulletToolStripButton_Click(object sender, EventArgs e)
        {
            if (RtbPage.SelectionBullet == true)
            {
                // this is the toggling off scenario, set bullet to false and move everything back to the beginning of the line
                RtbPage.SelectionBullet = false;
                RtbPage.SelectionIndent = 0;
            }
            else if (RtbPage.SelectionBullet == false && Properties.Settings.Default.BulletFirstIndent == true)
            {
                // this is a toggle on scenario to use an indented bullet
                RtbPage.SelectionBullet = true;
                RtbPage.SelectionIndent = 30;
            }
            else
            {
                // this is a toggle on sceanrio to add the bullet with no indent
                RtbPage.SelectionBullet = true;
                RtbPage.SelectionIndent = 0;
            }

            // todo: find a way to account for existing indents and move the bullet accordingly
            // see https://github.com/desjarlais/Notepad-Light/issues/10

            EndOfButtonFormatWork();
        }

        private void DecreaseIndentToolStripButton_Click(object sender, EventArgs e)
        {
            if (RtbPage.SelectionIndent > 0)
            {
                RtbPage.SelectionIndent -= 30;
            }

            EndOfButtonFormatWork();
        }

        private void IncreaseIndentToolStripButton_Click(object sender, EventArgs e)
        {
            if (RtbPage.SelectionIndent < 150)
            {
                RtbPage.SelectionIndent += 30;
            }

            EndOfButtonFormatWork();
        }

        private void RtbPage_KeyDown(object sender, KeyEventArgs e)
        {
            // if the line has a bullet and the tab was pressed, indent the line
            // need to suppress the key to prevent the cursor from moving around
            // tab will cause selectionchange so the ui/stats update there
            if (e.KeyCode == Keys.Tab && RtbPage.SelectionBullet == true)
            {
                e.SuppressKeyPress = true;
                RtbPage.Select(RtbPage.GetFirstCharIndexOfCurrentLine(), 0);
                IncreaseIndentToolStripButton.PerformClick();
                return;
            }

            // if the user deletes the bullet, we need to remove bullet formatting
            // backspace causes selectionchange so the statusbar updates there
            if (e.KeyCode == Keys.Back)
            {
                if (RtbPage.SelectionStart == RtbPage.GetFirstCharIndexOfCurrentLine() && RtbPage.SelectionBullet == true)
                {
                    e.SuppressKeyPress = true;
                    BulletToolStripButton.PerformClick();
                }
            }

            // if delete key is pressed change the saved state and update ui/stats
            if (e.KeyCode == Keys.Delete)
            {
                gPrevPageLength = RtbPage.TextLength;
                RtbPage.Modified = true;
                UpdateToolbarIcons();
                UpdateDocStats();
            }
        }

        private void RecentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (sender is not null) { OpenRecentFile(sender.ToString()!); }
        }

        private void RecentToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (sender is not null) { OpenRecentFile(sender.ToString()!); }
        }

        private void RecentToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (sender is not null) { OpenRecentFile(sender.ToString()!); }
        }

        private void RecentToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (sender is not null) { OpenRecentFile(sender.ToString()!); }
        }

        private void RecentToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (sender is not null) { OpenRecentFile(sender.ToString()!); }
        }

        private void RecentToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            if (sender is not null) { OpenRecentFile(sender.ToString()!); }
        }

        private void RecentToolStripMenuItem7_Click(object sender, EventArgs e)
        {
            if (sender is not null) { OpenRecentFile(sender.ToString()!); }
        }

        private void RecentToolStripMenuItem8_Click(object sender, EventArgs e)
        {
            if (sender is not null) { OpenRecentFile(sender.ToString()!); }
        }

        private void RecentToolStripMenuItem9_Click(object sender, EventArgs e)
        {
            if (sender is not null) { OpenRecentFile(sender.ToString()!); }
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
            FindText();
        }

        private void FileOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmOptions fOptions = new FrmOptions()
            {
                Owner = this
            };
            fOptions.ShowDialog();

            // update the file mru
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

            // update the autosave ticks
            UpdateAutoSaveInterval();
            Properties.Settings.Default.Save();
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
                    e.HasMorePages = Win32.Print(RtbPage, ref charFrom, e);
                    return;
                }

                // print plain text
                int charactersOnPage = 0;

                // Sets the value of charactersOnPage to the number of characters 
                // of strToPrint that will fit within the bounds of the page.
                e.Graphics?.MeasureString(gPrintString, RtbPage.SelectionFont, e.MarginBounds.Size, StringFormat.GenericTypographic,
                    out charactersOnPage, out _);

                // Draws the string within the bounds of the page
                e.Graphics?.DrawString(gPrintString, RtbPage.SelectionFont, Brushes.Black, e.MarginBounds, StringFormat.GenericTypographic);

                // Remove the portion of the string that has been printed.
                gPrintString = gPrintString.Substring(charactersOnPage);

                // Check to see if more pages are to be printed.
                e.HasMorePages = (gPrintString.Length > 0);
            }
            catch (Exception ex)
            {
                App.WriteErrorLogContent("PrintPage Error: " + ex.Message, gErrorLog);
                App.WriteErrorLogContent("Stack: " + ex.StackTrace, gErrorLog);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void LeftJustifiedToolStripButton_Click(object sender, EventArgs e)
        {
            RtbPage.SelectionAlignment = HorizontalAlignment.Left;
            EndOfButtonFormatWork();
        }

        private void CenterJustifiedToolStripButton_Click(object sender, EventArgs e)
        {
            RtbPage.SelectionAlignment = HorizontalAlignment.Center;
            EndOfButtonFormatWork();
        }

        private void RightJustifiedToolStripButton_Click(object sender, EventArgs e)
        {
            RtbPage.SelectionAlignment = HorizontalAlignment.Right;
            EndOfButtonFormatWork();
        }

        private void FontColorToolStripButton_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK && colorDialog1.Color != RtbPage.SelectionColor)
            {
                RtbPage.SelectionColor = colorDialog1.Color;
                RtbPage.Modified = true;
            }
        }

        private void FindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RtbPage.Focus();
            FindText();
        }

        private void EditTimerToolStripButton_Click(object sender, EventArgs e)
        {
            FrmEditTimer fEditedTimer = new FrmEditTimer(TimerToolStripLabel.Text)
            {
                Owner = this
            };
            fEditedTimer.ShowDialog(this);

            // if the time was adjusted, reset the timer and update the display
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
            PaintToolStripSeparator(sender, e);
        }

        private void ToolStripSeparator3_Paint(object sender, PaintEventArgs e)
        {
            PaintToolStripSeparator(sender, e);
        }

        private void ToolStripSeparator4_Paint(object sender, PaintEventArgs e)
        {
            PaintToolStripSeparator(sender, e);
        }

        private void ToolStripSeparator13_Paint(object sender, PaintEventArgs e)
        {
            PaintToolStripSeparator(sender, e);
        }

        private void ToolStripSeparator14_Paint(object sender, PaintEventArgs e)
        {
            PaintToolStripSeparator(sender, e);
        }

        private void CutContextMenu_Click(object sender, EventArgs e)
        {
            Cut();
        }

        private void CopyContextMenu_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void PasteContextMenu_Click(object sender, EventArgs e)
        {
            Paste();
        }

        private void HighlightTextToolStripButton_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK && colorDialog1.Color != RtbPage.SelectionBackColor)
            {
                RtbPage.SelectionBackColor = colorDialog1.Color;
                RtbPage.Modified = true;
            }
        }

        /// <summary>
        /// when text is changed, need to update saved state and ui/stats
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RtbPage_TextChanged(object sender, EventArgs e)
        {
            // check if content was added
            if (gPrevPageLength != RtbPage.TextLength)
            {
                RtbPage.Modified = true;
            }

            // update the prevPageLength
            gPrevPageLength = RtbPage.TextLength;
            UpdateDocStats();
            UpdateToolbarIcons();

            // for markdown files, update the panel
            if (splitContainer1.Panel2Collapsed == false && toolStripStatusLabelFileType.Text == "Markdown")
            {
                LoadMarkdownInWebView2();
            }
        }

        private void RtbPage_LinkClicked(object sender, LinkClickedEventArgs e)
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
                ofd.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.png; *.bmp; *.wmf; *.emf)|*.jpg; *.jpeg; *.gif; *.png; *.bmp; *.wmf; *.emf";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // get the image from the dialog
                    Image img = Image.FromFile(ofd.FileName);

                    // creating a bitmap to check for transparencies
                    Bitmap bmp = new Bitmap(img);
                    if (ContainsTransparentPixel(bmp))
                    {
                        // depending on the ui theme, apply the same color to the background
                        if (Properties.Settings.Default.DarkMode && Properties.Settings.Default.UseImageTransparency == true)
                        {
                            img = ReplaceTransparency(Image.FromFile(ofd.FileName), clrDarkModeTextBackground);
                        }
                        else
                        {
                            img = ReplaceTransparency(Image.FromFile(ofd.FileName), Color.White);
                        }
                    }

                    // convert the image to rtf so it can be displayed in the rtb
                    RtbPage.SelectedRtf = Rtf.InsertPicture(img, gErrorLog);
                    RtbPage.Focus();
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

        private void ToolStripSeparator18_Paint(object sender, PaintEventArgs e)
        {
            PaintToolStripSeparator(sender, e);
        }

        /// <summary>
        /// when the richtextbox gets focus, the cursor does not move to the mouse location
        /// using mouse move to accomplish this cursor position behavior
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RtbPage_MouseMove(object sender, MouseEventArgs e)
        {
            RtbPage.Focus();
        }

        private void TableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmInsertTable fTable = new FrmInsertTable()
            {
                Owner = this
            };
            fTable.ShowDialog(this);

            // if the user cancelled the form, row and col should be 0 and nothing needs to happen
            // otherwise, the value will be between 1 and 10, so call inserttable
            if (fTable.fRows > 0 && fTable.fCols > 0)
            {
                RtbPage.SelectedRtf = Rtf.InsertTable(fTable.fRows, fTable.fCols, 2500);
            }
        }

        private void DateTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertDateTime();
        }

        private void ReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReplaceText();
        }

        private void SearchContextMenu_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = false;
            WebView2Navigate();
        }

        private void BtnClosePanel2_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = true;
            TaskPaneToolStripMenuItem.Checked = false;
        }

        private void TaskPaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
            TaskPaneToolStripMenuItem.Checked = !TaskPaneToolStripMenuItem.Checked;
        }

        private void SelectAllContextMenu_Click(object sender, EventArgs e)
        {
            RtbPage.SelectAll();
        }

        private void ToolStripSeparator12_Paint(object sender, PaintEventArgs e)
        {
            PaintToolStripSeparator(sender, e);
        }

        /// <summary>
        /// increment each second until we hit the autosave threshold
        /// then reset and call the background save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutosaveTimer_Tick(object sender, EventArgs e)
        {
            ticks++;
            if (ticks > autoSaveTicks)
            {
                ticks = 0;
                BackgroundFileSaveAsync();
            }
        }

        private void PrintDocument1_BeginPrint(object sender, PrintEventArgs e)
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

        private void ReplaceToolStripButton_Click(object sender, EventArgs e)
        {
            ReplaceText();
        }

        #endregion
    }
}
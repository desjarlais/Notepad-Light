namespace Notepad_Light.Helpers
{
    public static class Strings
    {
        // general strings
        public const string semiColon = " : ";
        public const string semiColonNoSpaces = ":";
        public const string defaultFileName = "Untitled";
        public const string backupTemplateFileName = "TemplateBackup";
        public const string rtf = "Rtf";
        public const string plainText = "Text";
        public const string pasteHtml = "Html";
        public const string pasteUnicode = "UnicodeText";
        public const string pasteImage = "Image";
        public const string pasteCsv = "CSV";
        public const string startTimeText = "Start";
        public const string stopTimeText = "Stop";
        public const string zeroTimer = "00:00:00";
        public const string zeroHRMIN = "00:00";
        public const string zeroSEC = "00";
        public const string empty = "empty";
        public const string rtfExt = ".rtf";
        public const string txtExt = ".txt";
        public const string saveChangePrompt = "Do you want to save your changes?";
        public const string saveChangesTitle = "Save Changes";
        public const string replaceText = "Replace Text";
        public const string textReplaced = "Text Replaced";
        public const string pathDivider = "\\";
        public const string AppTitle = "Notepad Light - ";
        public const string findUp = "Up";
        public const string findDown = "Down";
        public const string findMatchCase = "MatchCase";
        public const string findWholeWord = "WholeWord";
        public const string errorLogFile = "NLErrors.txt";

        // hyperlinks
        public const string mainWebsite = "https://github.com/desjarlais/Notepad-Light";
        public const string githubIssues = "https://github.com/desjarlais/Notepad-Light/issues";
        public const string githubDiscussion = "https://github.com/desjarlais/Notepad-Light/discussions";

        // file paths
        public static string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string appFolderDirectory = localAppData + "\\BrandeSoft\\Notepad Light";
        public static string appFolderDirectoryUrl = localAppData + "\\BrandeSoft\\NLErrors.txt";
        public static string appFolderTemplateDir = localAppData + "\\BrandeSoft\\Notepad Light\\Templates";
        public static string appFolderTemplatesFullPath = appFolderTemplateDir + pathDivider;
        public static string appFolderFullPath = appFolderDirectory + pathDivider;

        // rtf tags
        public const string rtfStart = @"{\rtf1";
        public const string paragraph = @"\pard";
        public const string rtfTableRowStart = @"\trowd";
        public const string rtfTableCell = @"\cellx";
        public const string rtfEnd = "}";
    }
}

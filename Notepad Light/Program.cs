namespace Notepad_Light
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // migrate user settings from a previous version after an upgrade
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            Application.EnableVisualStyles();
            ApplicationConfiguration.Initialize();
            Application.Run(new FrmMain());
        }
    }
}
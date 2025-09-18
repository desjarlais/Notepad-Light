using Notepad_Light.Helpers;
using System.Reflection;

namespace Notepad_Light.Forms
{
    public partial class FrmAbout : Form
    {
        public FrmAbout(string logFilePath)
        {
            InitializeComponent();
            lblVersion.Text = "Version: " + Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
            TxbSysInfo.Text = Win32.osDetails().ToString();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            App.PlatformSpecificProcessStart(Strings.mainWebsite);
        }

        private void BtnOpenErrorLog_Click(object sender, EventArgs e)
        {
            App.PlatformSpecificProcessStart(Strings.appFolderDirectoryUrl);
        }

        private void FrmAbout_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) { Close(); }
        }
    }
}

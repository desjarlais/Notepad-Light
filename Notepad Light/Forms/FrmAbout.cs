using Notepad_Light.Helpers;
using System.Reflection;

namespace Notepad_Light.Forms
{
    public partial class FrmAbout : Form
    {
        public FrmAbout()
        {
            InitializeComponent();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            lblVersion.Text = "Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
            App.PlatformSpecificProcessStart(Path.GetTempPath() + "\\NotepadLightErrorLog" + Strings.txtExt);            
        }
    }
}

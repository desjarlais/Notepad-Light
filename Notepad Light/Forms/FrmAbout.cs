using Notepad_Light.Helpers;
using System.Reflection;
using System.Text;

namespace Notepad_Light.Forms
{
    public partial class FrmAbout : Form
    {
        public FrmAbout()
        {
            InitializeComponent();
            lblVersion.Text = "Version: " + Assembly.GetExecutingAssembly().GetName().Version!.ToString();

            Win32.SYSTEM_INFO info = new Win32.SYSTEM_INFO();
            Win32.GetSystemInfo(ref info);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Processor Architecture = " + ConvertProcArchitecture(info.wProcessorArchitecture));
            sb.AppendLine("Number of processors = " + info.dwNumberOfProcessors);
            sb.AppendLine("Page Size = " + info.dwPageSize);
            sb.AppendLine();
            sb.AppendLine("OS Details:");
            sb.AppendLine("-----------");
            OperatingSystem os = Environment.OSVersion;

            sb.AppendLine("OS Version = " + os.Version);
            sb.AppendLine("OS Platform = " + ConvertPlatform((int)os.Platform));
            sb.AppendLine("OS Service Pack = " + os.ServicePack);
            sb.AppendLine("OS Version String = " + os.VersionString);

            Version ver = os.Version;
            sb.AppendLine("Major Version = " + ver.Major);
            sb.AppendLine("Major Revision = " + ver.MajorRevision);
            sb.AppendLine("Minor Version = " + ver.Minor);
            sb.AppendLine("Minor Revision = " + ver.MinorRevision);
            sb.AppendLine("Build = " + ver.Build);

            TxbSysInfo.Text = sb.ToString();
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
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        public string ConvertPlatform(int val)
        {
            switch (val)
            {
                case 0: return "Windows S Win32";
                case 1: return "Windows Win32";
                case 2: return "Win32NT";
                case 3: return "WinCE";
                case 4: return "Unix";
                case 5: return "Xbox";
                case 6: return "MacOSX";
                default: return "Other";
            }
        }

        public string ConvertProcArchitecture(int val)
        {
            switch (val) 
            {
                case 0: return "Intel x86";
                case 5: return "ARM";
                case 6: return "Intel Itanium-based";
                case 9: return "AMD/Intel x64";
                case 12: return "ARM64";
                default: return "unknown";
            }
        }
    }
}

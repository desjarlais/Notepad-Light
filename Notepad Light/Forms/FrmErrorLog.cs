using Notepad_Light.Helpers;
using System.Text;

namespace Notepad_Light.Forms
{
    public partial class FrmErrorLog : Form
    {
        public FrmErrorLog()
        {
            InitializeComponent();
            foreach (var s in Properties.Settings.Default.ErrorLog)
            {
                if (s != null)
                {
                    LstOutput.Items.Add(s.ToString());
                }
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                if (LstOutput.Items.Count <= 0)
                {
                    return;
                }

                StringBuilder buffer = new StringBuilder();
                foreach (object t in LstOutput.Items)
                {
                    buffer.Append(t);
                    buffer.Append('\n');
                }

                Clipboard.SetText(buffer.ToString());
            }
            catch (Exception ex)
            {
                Logger.Log("Error Form Copy Error : " + ex.Message);
            }
        }
    }
}

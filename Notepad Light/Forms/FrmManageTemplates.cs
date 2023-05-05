using Notepad_Light.Helpers;
using System.Text;

namespace Notepad_Light.Forms
{
    public partial class FrmManageTemplates : Form
    {
        public FrmManageTemplates()
        {
            InitializeComponent();

            // init combo box
            tbxTemplate1.Text = Properties.Settings.Default.Template1;
            tbxTemplate2.Text = Properties.Settings.Default.Template2;
            tbxTemplate3.Text = Properties.Settings.Default.Template3;
            tbxTemplate4.Text = Properties.Settings.Default.Template4;
            tbxTemplate5.Text = Properties.Settings.Default.Template5;
            
            rdoTemplate1.Checked = true;

            if (rdoTemplate1.Checked)
            {
                tbTemplateText.Text = Templates.GetTemplate1().ToString();
            }
            else if (rdoTemplate2.Checked)
            {
                tbTemplateText.Text = Templates.GetTemplate2().ToString();
            }
            else if (rdoTemplate3.Checked)
            {
                tbTemplateText.Text = Templates.GetTemplate3().ToString();
            }
            else if (rdoTemplate4.Checked)
            {
                tbTemplateText.Text = Templates.GetTemplate4().ToString();
            }
            else
            {
                tbTemplateText.Text = Templates.GetTemplate5().ToString();
            }
        }

        private void BtnSaveTemplates_Click(object sender, EventArgs e)
        {
            // don't allow empty template names
            if (tbxTemplate1.Text.Trim() == string.Empty ||
                tbxTemplate2.Text.Trim() == string.Empty ||
                tbxTemplate3.Text.Trim() == string.Empty ||
                tbxTemplate4.Text.Trim() == string.Empty ||
                tbxTemplate5.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Template Name Can't Be Empty", "Invalid Template Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // save any renamed templates
            Properties.Settings.Default.Template1 = tbxTemplate1.Text;
            Properties.Settings.Default.Template2 = tbxTemplate2.Text;
            Properties.Settings.Default.Template3 = tbxTemplate3.Text;
            Properties.Settings.Default.Template4 = tbxTemplate4.Text;
            Properties.Settings.Default.Template5 = tbxTemplate5.Text;

            // update the backup text file with these names, first clear the file
            File.WriteAllText(Strings.appFolderDirectory + Strings.pathDivider + Strings.backupTemplateFileName + Strings.txtExt, string.Empty);
            
            // get the current list of names
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Properties.Settings.Default.Template1);
            sb.AppendLine(Properties.Settings.Default.Template2);
            sb.AppendLine(Properties.Settings.Default.Template3);
            sb.AppendLine(Properties.Settings.Default.Template4);
            sb.AppendLine(Properties.Settings.Default.Template5);

            // push those names to the file
            File.WriteAllText(Strings.appFolderDirectory + Strings.pathDivider + Strings.backupTemplateFileName + Strings.txtExt, sb.ToString());

            // update the current templates
            if (rdoTemplate1.Checked)
            {
                Templates.SetTemplate1(tbTemplateText.Text);
            }
            else if (rdoTemplate2.Checked)
            {
                Templates.SetTemplate2(tbTemplateText.Text);
            }
            else if (rdoTemplate3.Checked)
            {
                Templates.SetTemplate3(tbTemplateText.Text);
            }
            else if (rdoTemplate4.Checked)
            {
                Templates.SetTemplate4(tbTemplateText.Text);
            }
            else
            {
                Templates.SetTemplate5(tbTemplateText.Text);
            }
        }

        private void rdoTemplate1_CheckedChanged(object sender, EventArgs e)
        {
            // turn on 1
            tbxTemplate1.Enabled = true;
            tbTemplateText.Clear();
            tbTemplateText.Text = Templates.GetTemplate1().ToString();

            // disable the rest
            tbxTemplate2.Enabled = false;
            tbxTemplate3.Enabled = false;
            tbxTemplate4.Enabled = false;
            tbxTemplate5.Enabled = false;
        }

        private void rdoTemplate2_CheckedChanged(object sender, EventArgs e)
        {
            tbxTemplate2.Enabled = true;
            tbxTemplate1.Enabled = false;
            tbxTemplate3.Enabled = false;
            tbxTemplate4.Enabled = false;
            tbxTemplate5.Enabled = false;
            tbTemplateText.Clear();
            tbTemplateText.Text = Templates.GetTemplate2().ToString();
        }

        private void rdoTemplate3_CheckedChanged(object sender, EventArgs e)
        {
            tbxTemplate3.Enabled = true;
            tbxTemplate1.Enabled = false;
            tbxTemplate2.Enabled = false;
            tbxTemplate4.Enabled = false;
            tbxTemplate5.Enabled = false;
            tbTemplateText.Clear();
            tbTemplateText.Text = Templates.GetTemplate3().ToString();
        }

        private void rdoTemplate4_CheckedChanged(object sender, EventArgs e)
        {
            tbxTemplate4.Enabled = true;
            tbxTemplate1.Enabled = false;
            tbxTemplate2.Enabled = false;
            tbxTemplate3.Enabled = false;
            tbxTemplate5.Enabled = false;
            tbTemplateText.Clear();
            tbTemplateText.Text = Templates.GetTemplate4().ToString();
        }

        private void rdoTemplate5_CheckedChanged(object sender, EventArgs e)
        {
            tbxTemplate5.Enabled = true;
            tbxTemplate1.Enabled = false;
            tbxTemplate2.Enabled = false;
            tbxTemplate3.Enabled = false;
            tbxTemplate4.Enabled = false;
            tbTemplateText.Clear();
            tbTemplateText.Text = Templates.GetTemplate5().ToString();
        }

        private void FrmManageTemplates_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) { Close(); }
        }
    }
}

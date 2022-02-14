using Notepad_Light.Helpers;

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
            // save any renamed templates
            Properties.Settings.Default.Template1 = tbxTemplate1.Text;
            Properties.Settings.Default.Template2 = tbxTemplate2.Text;
            Properties.Settings.Default.Template3 = tbxTemplate3.Text;
            Properties.Settings.Default.Template4 = tbxTemplate4.Text;
            Properties.Settings.Default.Template5 = tbxTemplate5.Text;
            
            // save current template
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

        private void BtnCloseForm_Click(object sender, EventArgs e)
        {
            Close();
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
    }
}

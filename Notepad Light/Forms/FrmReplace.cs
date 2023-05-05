namespace Notepad_Light.Forms
{
    public partial class FrmReplace : Form
    {
        public string replaceText = string.Empty;
        public bool replaceAll = false;
        public bool formExited = true;

        public FrmReplace(Form mForm)
        {
            InitializeComponent();
        }

        private void BtnReplace_Click(object sender, EventArgs e)
        {
            if (cbxReplaceAll.Checked)
            {
                replaceAll = true;
            }

            replaceText = tbxReplace.Text;
            formExited = false;
            Close();
        }

        private void FrmReplace_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) { Close(); }
        }
    }
}

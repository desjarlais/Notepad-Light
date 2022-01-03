namespace Notepad_Light.Forms
{
    public partial class FrmSettings : Form
    {
        public FrmSettings()
        {
            InitializeComponent();

            // update options
            ckbUsePasteUI.Checked = Properties.Settings.Default.UsePasteUI;
            ckbUseRtf.Checked = Properties.Settings.Default.NewDocRtf;
            
            // check the file MRU and populate the list
            if (Properties.Settings.Default.FileMRU.Count > 0)
            {
                UpdateMRUListbox();   
            }
        }

        public void UpdateMRUListbox()
        {
            if (Properties.Settings.Default.FileMRU.Count > 0)
            {
                foreach (var f in Properties.Settings.Default.FileMRU)
                {
                    if (f is not null)
                    {
                        LstMRU.Items.Add(f);
                    }
                }
            }
        }

        private void BtnClearMRU_Click(object sender, EventArgs e)
        {
            // clear all items from the file MRU
            Properties.Settings.Default.FileMRU.Clear();
            UpdateMRUListbox();
            LstMRU.Items.Clear();
            Properties.Settings.Default.Save();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.UsePasteUI = ckbUsePasteUI.Checked;
            Properties.Settings.Default.NewDocRtf = ckbUseRtf.Checked;
            Properties.Settings.Default.Save();
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

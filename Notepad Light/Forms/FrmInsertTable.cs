namespace Notepad_Light.Forms
{
    public partial class FrmInsertTable : Form
    {
        public int fCols = 0;
        public int fRows = 0;

        public FrmInsertTable()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            fCols = (int)nudColumns.Value;
            fRows = (int)nudRows.Value;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FrmInsertTable_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) { Close(); }
        }
    }
}

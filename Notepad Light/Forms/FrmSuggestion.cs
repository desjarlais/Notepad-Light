using Notepad_Light.NetSpeller;

namespace Notepad_Light.Forms
{
    public partial class FrmSuggestion : Form
    {
        Spelling gSpell;

        public FrmSuggestion(Spelling spell, RichTextBox rtb)
        {
            InitializeComponent();
            gSpell = spell;
            RtbTextToCheck = rtb;
        }

        private void BtnIgnore_Click(object sender, EventArgs e)
        {
            gSpell.Text = RtbTextToCheck.SelectedText;
            gSpell.IgnoreWord();
            gSpell.SpellCheck();
        }

        private void BtnIgnoreAll_Click(object sender, EventArgs e)
        {
            gSpell.Text = RtbTextToCheck.SelectedText;
            gSpell.IgnoreAllWord();
            gSpell.SpellCheck();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            gSpell.Dictionary.Add(RtbTextToCheck.SelectedText);
            gSpell.SpellCheck();
        }

        private void BtnReplace_Click(object sender, EventArgs e)
        {
            gSpell.Text = RtbTextToCheck.SelectedText;
            gSpell.ReplaceWord(LstSuggestions.SelectedItem.ToString());
            gSpell.SpellCheck();
        }

        private void BtnReplaceAll_Click(object sender, EventArgs e)
        {
            if (LstSuggestions.SelectedItem != null)
            {
                gSpell.Text = RtbTextToCheck.SelectedText;
                gSpell.ReplaceAllWord(LstSuggestions.SelectedItem.ToString());
                gSpell.SpellCheck();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

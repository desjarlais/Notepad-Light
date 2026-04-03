using Notepad_Light.Helpers;

namespace Notepad_Light.Forms
{
    public partial class FrmSpellCheck : Form
    {
        private readonly SpellCheckService _spellCheckService;
        public SpellCheckAction UserAction { get; private set; } = SpellCheckAction.Cancel;
        public string ReplacementWord { get; private set; } = string.Empty;

        public FrmSpellCheck(string misspelledWord, List<string> suggestions, SpellCheckService spellCheckService)
        {
            InitializeComponent();
            _spellCheckService = spellCheckService;

            lblMisspelledWord.Text = misspelledWord;
            txtReplaceWith.Text = suggestions.Count > 0 ? suggestions[0] : misspelledWord;

            lstSuggestions.Items.Clear();
            foreach (var suggestion in suggestions)
            {
                lstSuggestions.Items.Add(suggestion);
            }

            if (lstSuggestions.Items.Count > 0)
            {
                lstSuggestions.SelectedIndex = 0;
            }
        }

        private void lstSuggestions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSuggestions.SelectedItem != null)
            {
                txtReplaceWith.Text = lstSuggestions.SelectedItem.ToString();
            }
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            UserAction = SpellCheckAction.Ignore;
            Close();
        }

        private void btnIgnoreAll_Click(object sender, EventArgs e)
        {
            _spellCheckService.IgnoreAllWord(lblMisspelledWord.Text);
            UserAction = SpellCheckAction.IgnoreAll;
            Close();
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            ReplacementWord = txtReplaceWith.Text;
            UserAction = SpellCheckAction.Replace;
            Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _spellCheckService.AddWord(lblMisspelledWord.Text);
            UserAction = SpellCheckAction.Add;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UserAction = SpellCheckAction.Cancel;
            Close();
        }
    }
}

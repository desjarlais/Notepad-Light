using Notepad_Light.Helpers;

namespace Notepad_Light.Forms
{
    public partial class FrmSuggestion : Form
    {
        private readonly SpellCheckService _spellCheckService;
        private readonly RichTextBox _sourceRtb;
        private List<MisspelledWord> _misspelledWords = [];
        private int _currentIndex;

        public bool TextModified { get; private set; }

        public FrmSuggestion(SpellCheckService spellCheckService, RichTextBox rtb)
        {
            InitializeComponent();
            _spellCheckService = spellCheckService;
            _sourceRtb = rtb;
            RtbTextToCheck.Text = rtb.Text;
            RtbTextToCheck.ReadOnly = true;
            nudMaxSuggestions.Value = 10;
            LstSuggestions.SelectedIndexChanged += LstSuggestions_SelectedIndexChanged;
            Load += FrmSuggestion_Load;
        }

        private void FrmSuggestion_Load(object? sender, EventArgs e)
        {
            FindAndNavigate();
        }

        private List<MisspelledWord> GetFilteredMisspelledWords()
        {
            var words = _spellCheckService.FindMisspelledWords(RtbTextToCheck.Text);

            if (ChkIgnoreDigits.Checked)
                words.RemoveAll(w => w.Word.Any(char.IsDigit));
            if (ChkIgnoreUpper.Checked)
                words.RemoveAll(w => w.Word.Any(char.IsLetter) && w.Word.Equals(w.Word.ToUpperInvariant(), StringComparison.Ordinal));
            if (ChkIgnoreHtml.Checked)
                words.RemoveAll(w => IsInsideHtmlTag(RtbTextToCheck.Text, w.StartIndex));

            return words;
        }

        private static bool IsInsideHtmlTag(string text, int position)
        {
            for (int i = position - 1; i >= 0; i--)
            {
                if (text[i] == '>') return false;
                if (text[i] == '<') return true;
            }
            return false;
        }

        private void FindAndNavigate()
        {
            _misspelledWords = GetFilteredMisspelledWords();
            _currentIndex = 0;
            ShowCurrentWord();
        }

        private void ShowCurrentWord()
        {
            if (_currentIndex >= _misspelledWords.Count)
            {
                ClearHighlights();
                LstSuggestions.Items.Clear();
                TxReplaceWith.Text = string.Empty;
                toolStripStatusLabel1.Text = $"Word: 0 of 0";
                toolStripStatusLabel3.Text = "Index: 0";
                toolStripStatusLabel5.Text = string.Empty;
                MessageBox.Show("Spell check complete.", "Spell Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                return;
            }

            var current = _misspelledWords[_currentIndex];

            ClearHighlights();
            RtbTextToCheck.Select(current.StartIndex, current.Length);
            RtbTextToCheck.SelectionBackColor = Color.Yellow;
            RtbTextToCheck.ScrollToCaret();

            int maxSuggestions = Math.Max(1, (int)nudMaxSuggestions.Value);
            var suggestions = _spellCheckService.Suggest(current.Word, maxSuggestions).ToList();
            LstSuggestions.Items.Clear();
            foreach (var s in suggestions)
                LstSuggestions.Items.Add(s);

            TxReplaceWith.Text = suggestions.Count > 0 ? suggestions[0] : current.Word;
            if (LstSuggestions.Items.Count > 0)
                LstSuggestions.SelectedIndex = 0;

            toolStripStatusLabel1.Text = $"Word: {_currentIndex + 1} of {_misspelledWords.Count}";
            toolStripStatusLabel3.Text = $"Index: {current.StartIndex}";
            toolStripStatusLabel5.Text = current.Word;
        }

        private void ClearHighlights()
        {
            int savedStart = RtbTextToCheck.SelectionStart;
            int savedLength = RtbTextToCheck.SelectionLength;
            RtbTextToCheck.SelectAll();
            RtbTextToCheck.SelectionBackColor = RtbTextToCheck.BackColor;
            RtbTextToCheck.Select(savedStart, savedLength);
        }

        private void LstSuggestions_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (LstSuggestions.SelectedItem != null)
                TxReplaceWith.Text = LstSuggestions.SelectedItem.ToString();
        }

        private void BtnIgnore_Click(object sender, EventArgs e)
        {
            _currentIndex++;
            ShowCurrentWord();
        }

        private void BtnIgnoreAll_Click(object sender, EventArgs e)
        {
            if (_currentIndex < _misspelledWords.Count)
            {
                _spellCheckService.IgnoreAllWord(_misspelledWords[_currentIndex].Word);
                FindAndNavigate();
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (_currentIndex < _misspelledWords.Count)
            {
                _spellCheckService.AddWord(_misspelledWords[_currentIndex].Word);
                FindAndNavigate();
            }
        }

        private void BtnReplace_Click(object sender, EventArgs e)
        {
            if (_currentIndex < _misspelledWords.Count && !string.IsNullOrEmpty(TxReplaceWith.Text))
            {
                var current = _misspelledWords[_currentIndex];
                _sourceRtb.Select(current.StartIndex, current.Length);
                _sourceRtb.SelectedText = TxReplaceWith.Text;
                TextModified = true;
                RtbTextToCheck.Text = _sourceRtb.Text;
                FindAndNavigate();
            }
        }

        private void BtnReplaceAll_Click(object sender, EventArgs e)
        {
            if (_currentIndex < _misspelledWords.Count && !string.IsNullOrEmpty(TxReplaceWith.Text))
            {
                string original = _misspelledWords[_currentIndex].Word;
                string replacement = TxReplaceWith.Text;

                // replace all matching misspelled instances backwards to preserve indices
                var matchingWords = _misspelledWords
                    .Where(w => w.Word.Equals(original, StringComparison.Ordinal))
                    .OrderByDescending(w => w.StartIndex)
                    .ToList();

                foreach (var word in matchingWords)
                {
                    _sourceRtb.Select(word.StartIndex, word.Length);
                    _sourceRtb.SelectedText = replacement;
                }

                TextModified = true;
                RtbTextToCheck.Text = _sourceRtb.Text;
                FindAndNavigate();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

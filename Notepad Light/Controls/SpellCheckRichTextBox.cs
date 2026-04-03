using System.ComponentModel;
using Notepad_Light.Helpers;
using static Notepad_Light.Helpers.Win32;

namespace Notepad_Light.Controls
{
    /// <summary>
    /// A RichTextBox that draws red wavy underlines beneath misspelled words.
    /// Spell checking is debounced so it runs after the user pauses typing.
    /// </summary>
    public class SpellCheckRichTextBox : RichTextBox
    {
        private const int WM_VSCROLL = 0x0115;
        private const int WM_HSCROLL = 0x0114;
        private const int WM_MOUSEWHEEL = 0x020A;

        private SpellCheckService? _spellCheckService;
        private readonly System.Windows.Forms.Timer _debounceTimer;
        private List<MisspelledWord> _misspelledWords = [];
        private bool _spellCheckEnabled = true;

        /// <summary>
        /// The spell check service used to detect misspelled words.
        /// Squiggles are drawn only when this is set and SpellCheckEnabled is true.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpellCheckService? SpellCheckService
        {
            get => _spellCheckService;
            set
            {
                _spellCheckService = value;
                RequestSpellCheck();
            }
        }

        /// <summary>
        /// Gets or sets whether real-time spell checking (red squiggles) is enabled.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SpellCheckEnabled
        {
            get => _spellCheckEnabled;
            set
            {
                _spellCheckEnabled = value;
                if (!value)
                {
                    _misspelledWords.Clear();
                    Invalidate();
                }
                else
                {
                    RequestSpellCheck();
                }
            }
        }

        public SpellCheckRichTextBox()
        {
            _debounceTimer = new System.Windows.Forms.Timer { Interval = 500 };
            _debounceTimer.Tick += DebounceTimer_Tick;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            RequestSpellCheck();
        }

        private void RequestSpellCheck()
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        private void DebounceTimer_Tick(object? sender, EventArgs e)
        {
            _debounceTimer.Stop();
            UpdateMisspelledWords();
            Invalidate();
        }

        private void UpdateMisspelledWords()
        {
            if (_spellCheckService == null || !_spellCheckEnabled || TextLength == 0)
            {
                _misspelledWords.Clear();
                return;
            }

            try
            {
                _misspelledWords = _spellCheckService.FindMisspelledWords(Text);

                // don't flag the word the user is currently typing
                int caret = SelectionStart;
                _misspelledWords.RemoveAll(w =>
                    caret > w.StartIndex && caret <= w.StartIndex + w.Length);
            }
            catch
            {
                _misspelledWords.Clear();
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT && _misspelledWords.Count > 0 && _spellCheckEnabled)
            {
                using var g = CreateGraphics();
                DrawAllSquiggles(g);
            }
            else if (m.Msg is WM_VSCROLL or WM_HSCROLL or WM_MOUSEWHEEL)
            {
                if (_misspelledWords.Count > 0 && _spellCheckEnabled)
                {
                    Invalidate();
                }
            }
        }

        private void DrawAllSquiggles(Graphics g)
        {
            int clientHeight = ClientSize.Height;
            int clientWidth = ClientSize.Width;
            int lineHeight = Font.Height;

            using var pen = new Pen(Color.Red, 1);

            foreach (var word in _misspelledWords)
            {
                Point startPos = GetPositionFromCharIndex(word.StartIndex);

                // skip words above or below the visible area
                if (startPos.Y + lineHeight < 0 || startPos.Y > clientHeight)
                    continue;

                int endCharIndex = word.StartIndex + word.Length;
                Point endPos = GetPositionFromCharIndex(
                    endCharIndex < TextLength ? endCharIndex : endCharIndex - 1);

                int baselineY = startPos.Y + lineHeight - 1;

                if (startPos.Y == endPos.Y)
                {
                    // single-line word
                    int endX;
                    if (endCharIndex < TextLength)
                    {
                        endX = GetPositionFromCharIndex(endCharIndex).X;
                    }
                    else
                    {
                        Size charSize = TextRenderer.MeasureText(
                            g, word.Word[^1].ToString(), Font, Size.Empty,
                            TextFormatFlags.NoPadding);
                        endX = endPos.X + charSize.Width;
                    }

                    int startX = Math.Max(startPos.X, 0);
                    endX = Math.Min(endX, clientWidth);

                    if (startX < endX)
                        DrawSquiggle(g, pen, startX, endX, baselineY);
                }
                else
                {
                    // word wraps across lines — draw first line segment
                    DrawSquiggle(g, pen, Math.Max(startPos.X, 0), clientWidth, baselineY);

                    // draw last line segment
                    int endBaselineY = endPos.Y + lineHeight - 1;
                    if (endBaselineY >= 0 && endBaselineY <= clientHeight)
                    {
                        int endX;
                        if (endCharIndex < TextLength)
                        {
                            endX = GetPositionFromCharIndex(endCharIndex).X;
                        }
                        else
                        {
                            Size charSize = TextRenderer.MeasureText(
                                g, word.Word[^1].ToString(), Font, Size.Empty,
                                TextFormatFlags.NoPadding);
                            endX = endPos.X + charSize.Width;
                        }

                        DrawSquiggle(g, pen, 0, Math.Min(endX, clientWidth), endBaselineY);
                    }
                }
            }
        }

        private static void DrawSquiggle(Graphics g, Pen pen, int startX, int endX, int y)
        {
            if (endX <= startX) return;

            const int squiggleStep = 2;
            var points = new List<Point>((endX - startX) / squiggleStep + 2);
            bool up = true;

            for (int x = startX; x <= endX; x += squiggleStep)
            {
                points.Add(new Point(x, up ? y : y + squiggleStep));
                up = !up;
            }

            if (points.Count > 1)
                g.DrawLines(pen, points.ToArray());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _debounceTimer.Stop();
                _debounceTimer.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

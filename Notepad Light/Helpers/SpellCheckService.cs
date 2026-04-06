using WeCantSpell.Hunspell;

namespace Notepad_Light.Helpers
{
    /// <summary>
    /// Provides spell checking functionality using WeCantSpell.Hunspell.
    /// Lazily loads the dictionary on first use and caches the WordList instance.
    /// </summary>
    public sealed class SpellCheckService : IDisposable
    {
        private WordList? _wordList;
        private readonly HashSet<string> _ignoredWords = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _customWords = new(StringComparer.OrdinalIgnoreCase);
        private string _dicPath;
        private string _affPath;
        private string _language;
        private bool _disposed;

        // known language code to display name mapping
        private static readonly Dictionary<string, string> LanguageDisplayNames = new(StringComparer.OrdinalIgnoreCase)
        {
            ["en_US"] = "English (US)",
            ["en_GB"] = "English (UK)",
            ["es_ES"] = "Spanish",
            ["fr_FR"] = "French",
            ["de_DE"] = "German",
            ["it_IT"] = "Italian",
            ["pt_BR"] = "Portuguese (Brazil)"
        };

        public SpellCheckService(string language)
        {
            string baseDir = AppContext.BaseDirectory;
            _language = language;
            _dicPath = Path.Combine(baseDir, "Dictionaries", language + ".dic");
            _affPath = Path.Combine(baseDir, "Dictionaries", language + ".aff");
        }

        /// <summary>
        /// Gets the current language code (e.g. "en_US").
        /// </summary>
        public string Language => _language;

        /// <summary>
        /// Changes the active dictionary language and reloads on next use.
        /// </summary>
        public void ChangeLanguage(string language)
        {
            string baseDir = AppContext.BaseDirectory;
            _language = language;
            _dicPath = Path.Combine(baseDir, "Dictionaries", language + ".dic");
            _affPath = Path.Combine(baseDir, "Dictionaries", language + ".aff");
            _wordList = null;
        }

        /// <summary>
        /// Returns the list of available language codes by scanning the Dictionaries folder
        /// for matching .dic/.aff file pairs.
        /// </summary>
        public static List<string> GetAvailableLanguages()
        {
            var languages = new List<string>();
            string dictDir = Path.Combine(AppContext.BaseDirectory, "Dictionaries");
            if (!Directory.Exists(dictDir)) return languages;

            foreach (string dicFile in Directory.GetFiles(dictDir, "*.dic"))
            {
                string langCode = Path.GetFileNameWithoutExtension(dicFile);
                string affFile = Path.Combine(dictDir, langCode + ".aff");
                if (File.Exists(affFile))
                {
                    languages.Add(langCode);
                }
            }

            languages.Sort();
            return languages;
        }

        /// <summary>
        /// Returns a friendly display name for a language code.
        /// Falls back to the code itself if no mapping exists.
        /// </summary>
        public static string GetLanguageDisplayName(string languageCode)
        {
            return LanguageDisplayNames.TryGetValue(languageCode, out string? name) ? name : languageCode;
        }

        /// <summary>
        /// Ensures the Hunspell dictionary is loaded.
        /// </summary>
        private void EnsureLoaded()
        {
            if (_wordList != null) return;

            if (!File.Exists(_dicPath) || !File.Exists(_affPath))
            {
                throw new FileNotFoundException(
                    $"Dictionary files not found. Expected: {_dicPath} and {_affPath}");
            }

            _wordList = WordList.CreateFromFiles(_dicPath, _affPath);
        }

        /// <summary>
        /// Checks if a word is spelled correctly.
        /// </summary>
        public bool Check(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) return true;
            if (_ignoredWords.Contains(word)) return true;
            if (_customWords.Contains(word)) return true;

            EnsureLoaded();
            return _wordList!.Check(word);
        }

        /// <summary>
        /// Returns spelling suggestions for a misspelled word.
        /// </summary>
        public IEnumerable<string> Suggest(string word, int maxSuggestions = 10)
        {
            if (string.IsNullOrWhiteSpace(word)) return [];

            EnsureLoaded();
            return _wordList!.Suggest(word).Take(maxSuggestions);
        }

        /// <summary>
        /// Ignores a word for the current session only.
        /// </summary>
        public void IgnoreWord(string word)
        {
            if (!string.IsNullOrWhiteSpace(word))
                _ignoredWords.Add(word);
        }

        /// <summary>
        /// Ignores all occurrences of a word for the current session.
        /// Same as IgnoreWord since the ignore list is checked globally.
        /// </summary>
        public void IgnoreAllWord(string word)
        {
            IgnoreWord(word);
        }

        /// <summary>
        /// Adds a word to the custom dictionary (session only).
        /// </summary>
        public void AddWord(string word)
        {
            if (!string.IsNullOrWhiteSpace(word))
                _customWords.Add(word);
        }

        /// <summary>
        /// Finds all misspelled words in the given text and returns their positions.
        /// </summary>
        public List<MisspelledWord> FindMisspelledWords(string text)
        {
            var results = new List<MisspelledWord>();
            if (string.IsNullOrEmpty(text)) return results;

            EnsureLoaded();

            int i = 0;
            int len = text.Length;

            while (i < len)
            {
                // skip non-word characters
                if (!IsWordChar(text[i]))
                {
                    i++;
                    continue;
                }

                // find word boundaries
                int wordStart = i;
                while (i < len && IsWordChar(text[i]))
                {
                    i++;
                }

                string word = text.Substring(wordStart, i - wordStart);

                // skip words that are all digits or single characters
                if (word.Length <= 1 || IsAllDigits(word))
                    continue;

                if (!Check(word))
                {
                    results.Add(new MisspelledWord(word, wordStart, word.Length));
                }
            }

            return results;
        }

        private static bool IsWordChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '\'' || c == '\u2019'; // include apostrophe
        }

        private static bool IsAllDigits(string word)
        {
            foreach (char c in word)
            {
                if (!char.IsDigit(c)) return false;
            }
            return true;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _wordList = null;
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Represents a misspelled word with its position in the text.
    /// </summary>
    public readonly struct MisspelledWord
    {
        public string Word { get; }
        public int StartIndex { get; }
        public int Length { get; }

        public MisspelledWord(string word, int startIndex, int length)
        {
            Word = word;
            StartIndex = startIndex;
            Length = length;
        }
    }
}

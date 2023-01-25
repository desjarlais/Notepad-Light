using System.Text;

namespace Notepad_Light.Forms
{
    public partial class FrmMarkdownViewer : Form
    {
        public enum mdCharType { Text, Heading1, Heading2 };
        public mdCharType currentMarkdownType = mdCharType.Text;

        public FrmMarkdownViewer(string mdText)
        {
            InitializeComponent();

            Dictionary<string, string> lines = new Dictionary<string, string>();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < mdText.Length; i++) 
            {
                sb.Append(mdText[i]);

                // check for control words
                switch (mdText[i])
                {
                    case '#': 
                        currentMarkdownType = mdCharType.Heading1;
                        break;
                }

                // end current line processing
                if (mdText[i] == '\n')
                {
                    lines.Add(sb.ToString(), "EOL");
                    sb.Clear();
                }
            }
        }

        /// <summary>
        /// Create a font style based on the type of markdown
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public Font CreateFont(mdCharType cFontToCreate)
        {
            Font fFont = new Font("Times New Roman", 12.0f);
            
            switch (cFontToCreate)
            {
                case mdCharType.Heading1: fFont = new Font("Times New Roman", 16.0f, FontStyle.Italic); break;
                case mdCharType.Heading2: fFont = new Font("Times New Roman", 14.0f, FontStyle.Italic); break;
                case mdCharType.Text: fFont = new Font("Times New Roman", 12.0f); break;
            }
            return fFont;
        }
    }
}

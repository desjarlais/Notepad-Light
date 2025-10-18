using System.Text;

namespace Notepad_Light.Helpers
{
    public static class UTF8
    {
        /// <summary>
        /// Save the content to a file in UTF-8 encoding.
        /// </summary>
        public static void SaveRtfUtf8(RichTextBox rtb, string filePath)
        {
            if (rtb == null) throw new ArgumentNullException(nameof(rtb));
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.Write(rtb.Rtf);
            }
        }

        public static void SaveTextUtf8(RichTextBox rtb, string filePath)
        {
            if (rtb == null) throw new ArgumentNullException(nameof(rtb));
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.Write(rtb.Text);
            }
        }

        /// <summary>
        /// Load UTF-8 encoded plain text into a RichTextBox.
        /// </summary>
        public static void LoadTextUtf8(RichTextBox rtb, string filePath)
        {
            if (rtb == null) throw new ArgumentNullException(nameof(rtb));
            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                rtb.Text = reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Load UTF-8 encoded RTF content into a RichTextBox.
        /// </summary>
        public static void LoadRtfUtf8(RichTextBox rtb, string filePath)
        {
            if (rtb == null) throw new ArgumentNullException(nameof(rtb));
            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                rtb.Rtf = reader.ReadToEnd();
            }
        }
    }
}

using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Notepad_Light.Helpers
{
    public class App
    {
        /// <summary>
        /// helper to get descriptive hresult info
        /// </summary>
        /// <param name="hr">hresult to be converted</param>
        /// <returns></returns>
        public static string MessageFromHResult(int hr)
        {
            return Marshal.GetExceptionForHR(hr)!.Message;
        }

        /// <summary>
        /// given a file, return the encoding
        /// </summary>
        /// <param name="filePath">file to check encoding</param>
        /// <returns></returns>
        public static string GetFileEncoding(string filePath)
        {
            // rtf is most likely going to be ansi
            // todo, add a better check for keywords in rtf
            if (filePath.EndsWith(Strings.rtfExt))
            {
                // until I get the parser going, just pass 0 and return ansi
                return GetCodePage(0);
            }

            // read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom);
            }

            // determine the encoding based on the BOM
            // supported encoding for .net5+ (utf8, utf16, utf32, utf32BE, unicodeFFFE, usascii, iso88591)
            if (bom[0] == 0x00 && bom[1] == 0x00 && bom[2] == 0x00 && bom[3] == 0x00) return Encoding.UTF8.EncodingName; // UTF8 or ANSI
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8.EncodingName; // UTF-8 with BOM
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode.EncodingName; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.Unicode.EncodingName; //UTF-16BE
            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32.EncodingName; //UTF-32LE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32.EncodingName;  //UTF-32BE
            return Encoding.UTF8.EncodingName;
        }

        public static void PlatformSpecificProcessStart(string url)
        {
            // known issue in .NET Core https://github.com/dotnet/corefx/issues/10361
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    MessageBox.Show("Unable to open web site.", "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static string ConvertTimeValueHRMIN(string input)
        {
            char[] adjDelim = { ':' };
            string[] adjArray = input.Split(adjDelim);
            string adjHours = adjArray.ElementAt(0);
            string adjMinutes = adjArray.ElementAt(1);
            return adjHours + Strings.semiColonNoSpaces + adjMinutes + Strings.semiColonNoSpaces + Strings.zeroSEC;
        }

        public static string ConvertTimeValueHRMINSEC(string input)
        {
            char[] adjDelim = { ':' };
            string[] adjArray = input.Split(adjDelim);
            string adjHours = adjArray.ElementAt(0);
            string adjMinutes = adjArray.ElementAt(1);
            string adjSeconds = adjArray.ElementAt(2);
            return adjHours + Strings.semiColonNoSpaces + adjMinutes + Strings.semiColonNoSpaces + adjSeconds;
        }

        public static Stream SwapClipboardAudio(Stream replacementAudioStream)
        {
            Stream? returnAudioStream = null;
            if (Clipboard.ContainsAudio())
            {
                returnAudioStream = Clipboard.GetAudioStream();
                Clipboard.SetAudio(replacementAudioStream);
            }
            return returnAudioStream!;
        }

        public static StringCollection SwapClipboardFileDropList(StringCollection replacementList)
        {
            StringCollection? returnList = null;
            if (Clipboard.ContainsFileDropList())
            {
                returnList = Clipboard.GetFileDropList();
                Clipboard.SetFileDropList(replacementList);
            }
            return returnList!;
        }

        public static Image SwapClipboardImage(Image replacementImage)
        {
            Image? returnImage = null;
            if (Clipboard.ContainsImage())
            {
                returnImage = Clipboard.GetImage();
                Clipboard.SetImage(replacementImage);
            }
            return returnImage!;
        }

        /// <summary>
        /// create an rtf table
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static string InsertTable(int rows, int cols, int width)
        {
            StringBuilder sb = new StringBuilder();

            // rtf start
            sb.Append(@"{\rtf1 ");

            int cellWidth;

            // start row
            sb.Append(@"\trowd");

            for (int i = 0; i < rows; i++)
            {
                sb.Append(@"\trowd");
                for (int j = 0; j < cols; j++)
                {
                    cellWidth = (j + 1) * width;
                    sb.Append(@"\cellx" + cellWidth.ToString());
                }

                sb.Append(@"\intbl \cell \row");
            }

            sb.Append(@"\pard");
            sb.Append('}');

            return sb.ToString();
        }

        public static string GetCharacterSet(string rtfCharSet)
        {
            switch (rtfCharSet)
            {
                case "mac":
                    return "Apple Macintosh";
                case "pc":
                    return "IBM PC code page 437";
                case "pca":
                    return "IBM PC code page 850";
                case "ansicpgN":
                    return "Unicode -> ANSI converstion";
                case "fbidis":
                    return "Active single font";
                default:
                    return "ANSI";
            }
        }

        public static string GetCodePage(int codepage)
        {
            switch (codepage)
            {
                case 437:
                    return "United States IBM";
                case 708:
                    return "Arabic (ASMO 708)";
                case 709:
                    return "Arabic (ASMO 449+, BCON V4)";
                case 710:
                    return "Arabic (transparent Arabic)";
                case 711:
                    return "Arabic (Nafitha Enhanced)";
                case 720:
                    return "Arabic (transparent ASMO)";
                case 819:
                    return "Windows 3.1 (United States and Western Europe)";
                case 850:
                    return "IBM multilingual";
                case 852:
                case 1250:
                    return "Eastern European";
                case 860:
                    return "Portuguese";
                case 862:
                case 1255:
                    return "Hebrew";
                case 863:
                    return "French Canadian";
                case 864:
                case 1256:
                    return "Arabic";
                case 865:
                    return "Norwegian";
                case 866:
                    return "Soviet Union";
                case 874:
                    return "Thai";
                case 932:
                    return "Japanese";
                case 936:
                    return "Simplified Chinese";
                case 949:
                    return "Korean";
                case 950:
                    return "Traditional Chinese";
                case 1251:
                    return "Cyrillic";
                case 1252:
                    return "Western European";
                case 1253:
                    return "Greek";
                case 1254:
                    return "Turkish";
                case 1257:
                    return "Baltic";
                case 1258:
                    return "Vietnamese";
                case 1361:
                    return "Johab";
                case 10000:
                    return "MAC Roman";
                case 10001:
                    return "MAC Japan";
                case 10004:
                    return "MAC Arabic";
                case 10005:
                    return "MAC Hebrew";
                case 10006:
                    return "MAC Greek";
                case 10007:
                    return "MAC Cyrillic";
                case 10029:
                    return "MAC Latin2";
                case 10081:
                    return "MAC Turkish";
                case 57002:
                    return "Devanagari";
                case 57003:
                    return "Bengali";
                case 57004:
                    return "Tamil";
                case 57005:
                    return "Telugu";
                case 57006:
                    return "Assamese";
                case 57007:
                    return "Oriya";
                case 57008:
                    return "Kannada";
                case 57009:
                    return "Malayalam";
                case 57010:
                    return "Gujarati";
                case 57011:
                    return "Punjabi";
                default:
                    return "ANSI";
            }
        }
    }
}

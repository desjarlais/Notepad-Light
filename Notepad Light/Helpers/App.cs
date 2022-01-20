using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Notepad_Light.Helpers
{
    public class App
    {
        /// <summary>
        /// given a file, return the encoding
        /// </summary>
        /// <param name="filePath">file to check encoding</param>
        /// <returns></returns>
        public static string GetFileEncoding(string filePath)
        {
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

        //public static Stream SwapClipboardAudio(Stream replacementAudioStream)
        //{
        //    Stream? returnAudioStream = null;
        //    if (Clipboard.ContainsAudio())
        //    {
        //        returnAudioStream = Clipboard.GetAudioStream();
        //        Clipboard.SetAudio(replacementAudioStream);
        //    }
        //    return returnAudioStream;
        //}

        //public static StringCollection SwapClipboardFileDropList(StringCollection replacementList)
        //{
        //    StringCollection? returnList = null;
        //    if (Clipboard.ContainsFileDropList())
        //    {
        //        returnList = Clipboard.GetFileDropList();
        //        Clipboard.SetFileDropList(replacementList);
        //    }
        //    return returnList;
        //}

        //public static Image SwapClipboardImage(Image replacementImage)
        //{
        //    Image? returnImage = null;
        //    if (Clipboard.ContainsImage())
        //    {
        //        returnImage = Clipboard.GetImage();
        //        Clipboard.SetImage(replacementImage);
        //    }
        //    return returnImage;
        //}
    }
}

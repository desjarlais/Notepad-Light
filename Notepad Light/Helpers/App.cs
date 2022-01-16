using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Notepad_Light.Helpers
{
    public class App
    {
        public enum supportedEncoding 
        {
            // list of .net 5+ supported encodings
            utf16 = 1200,
            unicodeFFFE = 1201,
            utf32 = 12000,
            utf32BE = 12001,
            usascii = 20127,
            iso88591 = 28591,
            utf8 = 65001
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

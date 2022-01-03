using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Notepad_Light.Helpers
{
    public class App
    {
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

        //public static string SwapClipboardHtmlText(String replacementHtmlText)
        //{
        //    string? returnHtmlText = null;
        //    if (Clipboard.ContainsText(TextDataFormat.Html))
        //    {
        //        returnHtmlText = Clipboard.GetText(TextDataFormat.Html);
        //        Clipboard.SetText(replacementHtmlText, TextDataFormat.Html);
        //    }
        //    return returnHtmlText;
        //}
    }
}

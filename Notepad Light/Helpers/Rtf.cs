using System.Drawing.Imaging;
using System.Globalization;
using System.Text;

namespace Notepad_Light.Helpers
{
    public class Rtf
    {
        #region RtfSamples

        #endregion

        #region CreateRtfContentFunctions
        /// <summary>
        /// create an rtf table based on the args
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static string InsertTable(int rows, int cols, int width)
        {
            StringBuilder sb = new StringBuilder();

            // rtf start
            sb.Append(Strings.rtfStart);

            int cellWidth;

            // start row
            sb.Append(Strings.rtfTableRowStart);

            for (int i = 0; i < rows; i++)
            {
                sb.Append(Strings.rtfTableRowStart);
                for (int j = 0; j < cols; j++)
                {
                    cellWidth = (j + 1) * width;
                    sb.Append(@"\cellx" + cellWidth.ToString());
                }

                sb.Append(@"\intbl\cell\row");
            }

            sb.Append(Strings.rtfParagraph);
            sb.Append(Strings.rtfEnd);

            return sb.ToString();
        }

        /// <summary>
        /// convert an image to rtf from passed in Image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string InsertPicture(Image image, string errorFilePath)
        {
            Metafile metafile;
            float dpiX; float dpiY;

            using (Graphics g = Graphics.FromImage(image))
            {
                IntPtr hDC = g.GetHdc();
                metafile = new Metafile(hDC, EmfType.EmfOnly);
                g.ReleaseHdc(hDC);
            }

            using (Graphics g = Graphics.FromImage(metafile))
            {
                g.DrawImage(image, 0, 0);
                dpiX = g.DpiX;
                dpiY = g.DpiY;
            }

            IntPtr _hEmf = metafile.GetHenhmetafile();
            uint _bufferSize = Win32.GdipEmfToWmfBits(_hEmf, 0, null!, Win32.MM_ANISOTROPIC, Win32.EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
            byte[] _buffer = new byte[_bufferSize];
            uint hresult = Win32.GdipEmfToWmfBits(_hEmf, _bufferSize, _buffer, Win32.MM_ANISOTROPIC, Win32.EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
            App.WriteErrorLogContent(errorFilePath, "GdipEmfToWmfBits hr = " + hresult.ToString());
            IntPtr hmf = Win32.SetMetaFileBitsEx(_bufferSize, _buffer);
            string tempfile = Path.GetTempFileName();

            Win32.CopyMetaFile(hmf, tempfile);
            Win32.DeleteMetaFile(hmf);
            Win32.DeleteEnhMetaFile(_hEmf);

            var stream = new MemoryStream();
            byte[] data = File.ReadAllBytes(tempfile);
            int count = data.Length;
            stream.Write(data, 0, count);

            string rtfImage = @"{\rtf1{\pict\wmetafile8\picw" + (int)((image.Width / dpiX) * 2540)
                                + @"\pich" + (int)((image.Height / dpiY) * 2540)
                                + @"\picwgoal" + (int)((image.Width / dpiX) * 1440)
                                + @"\pichgoal" + (int)((image.Height / dpiY) * 1440)
                                + " " + BitConverter.ToString(stream.ToArray()).Replace("-", "")
                                + "}}";
            return rtfImage;
        }

        #endregion
    }
}

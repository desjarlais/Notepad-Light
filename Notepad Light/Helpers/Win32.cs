using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace Notepad_Light.Helpers
{
    public static class Win32
    {
        /// <summary>
        /// Prints text in <box>, starting at <charFrom>
        /// </summary>
        /// <param name="box"></param>
        /// <param name="charFrom"></param>
        /// <param name="e"></param>
        /// <returns>True if more pages needed</returns>
        /// <exception cref="ApplicationException"></exception>
        public static bool Print(RichTextBox box, ref int charFrom, PrintPageEventArgs e)
        {
            FORMATRANGE fmtRange;

            // Allocate device context for output device
            IntPtr hdc = e.Graphics!.GetHdc();
            fmtRange.hdc = hdc;
            fmtRange.hdcTarget = hdc;

            // Set printable area, converted from 0.01" to twips
            fmtRange.rc.Top = Convert.ToInt32(e.MarginBounds.Top * Hundredth2Twips);
            fmtRange.rc.Bottom = Convert.ToInt32(e.MarginBounds.Bottom * Hundredth2Twips);
            fmtRange.rc.Left = Convert.ToInt32(e.MarginBounds.Left * Hundredth2Twips);
            fmtRange.rc.Right = Convert.ToInt32(e.MarginBounds.Right * Hundredth2Twips);

            // Set page area, converted from 0.01" to twips
            fmtRange.rcPage.Top = Convert.ToInt32(e.PageBounds.Top * Hundredth2Twips);
            fmtRange.rcPage.Bottom = Convert.ToInt32(e.PageBounds.Bottom * Hundredth2Twips);
            fmtRange.rcPage.Left = Convert.ToInt32(e.PageBounds.Left * Hundredth2Twips);
            fmtRange.rcPage.Right = Convert.ToInt32(e.PageBounds.Right * Hundredth2Twips);

            // Set character range to print
            fmtRange.chrg.cpMin = charFrom;
            fmtRange.chrg.cpMax = box.TextLength;

            // Marshal to unmanaged memory
            IntPtr hdlRange = Marshal.AllocCoTaskMem(Marshal.SizeOf(fmtRange));
            Marshal.StructureToPtr(fmtRange, hdlRange, false);

            // Send RichTextBox the EM_FORMATRANGE message to print the text
            IntPtr res = SendMessage(box.Handle, EM_FORMATRANGE, (IntPtr)1, hdlRange);
            int err = Marshal.GetLastWin32Error();

            // Release resources
            Marshal.FreeCoTaskMem(hdlRange);
            e.Graphics.ReleaseHdc(hdc);

            // Throw exception on error so we don't endlessly print an empty page
            if (res == IntPtr.Zero) throw new ApplicationException(string.Format("Rtf Printing failed, error code={0}", err));

            // Update <charFrom> to next character to print, return <true> if more pages needed
            charFrom = res.ToInt32();

            return charFrom < box.TextLength;
        }

        // P/Invoke declarations
        public enum EmfToWmfBitsFlags
        {
            EmfToWmfBitsFlagsDefault = 0x00000000, // Specifies the default conversion.
            EmfToWmfBitsFlagsEmbedEmf = 0x00000001, // Specifies that the source EMF metafile is embedded as a comment in the resulting WMF metafile.
            EmfToWmfBitsFlagsIncludePlaceable = 0x00000002, // Specifies that the resulting WMF metafile is in the placeable metafile format; that is, it has the additional 22-byte header required by a placeable metafile.
            EmfToWmfBitsFlagsNoXORClip = 0x00000004 // Specifies that the clipping region is stored in the metafile in the traditional way. If you do not set this flag, the Metafile::EmfToWmfBits method applies an optimization that stores the clipping region as a path and simulates clipping by using the XOR operator.
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct CHARRANGE
        {
            internal int cpMin;
            internal int cpMax;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct FORMATRANGE
        {
            internal IntPtr hdc;
            internal IntPtr hdcTarget;
            internal RECT rc;
            internal RECT rcPage;
            internal CHARRANGE chrg;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEM_INFO
        {
            internal ushort wProcessorArchitecture;
            internal ushort wReserved;
            internal uint dwPageSize;
            internal IntPtr lpMinimumApplicationAddress;
            internal IntPtr lpMaximumApplicationAddress;
            internal IntPtr dwActiveProcessorMask;
            internal uint dwNumberOfProcessors;
            internal uint dwProcessorType;
            internal uint dwAllocationGranularity;
            internal ushort wProcessorLevel;
            internal ushort wProcessorRevision;
        }

        public const int WM_USER = 0x0400;
        public const int EM_FORMATRANGE = WM_USER + 57;
        public const int Hundredth2Twips = 20 * 72 / 100;
        
        public const int MM_ISOTROPIC = 7;
        public const int MM_ANISOTROPIC = 8;

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern void GetSystemInfo(ref SYSTEM_INFO Info);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);                
        [DllImport("gdiplus.dll")]
        internal static extern uint GdipEmfToWmfBits(IntPtr HEmf, uint bufferSize, byte[] buffer, int mappingMode, EmfToWmfBitsFlags flags);
        [DllImport("gdi32.dll")]
        internal static extern IntPtr SetMetaFileBitsEx(uint bufferSize, byte[] buffer);
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CopyMetaFile(IntPtr hWmf, string filename);
        [DllImport("gdi32.dll")]
        internal static extern bool DeleteMetaFile(IntPtr hWmf);
        [DllImport("gdi32.dll")]
        internal static extern bool DeleteEnhMetaFile(IntPtr hEmf);
    }
}

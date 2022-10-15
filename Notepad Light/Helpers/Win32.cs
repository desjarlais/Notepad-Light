using System.Runtime.InteropServices;

namespace Notepad_Light.Helpers
{
    public static class Win32
    {
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

        public const int WM_USER = 0x0400;
        public const int EM_FORMATRANGE = WM_USER + 57;
        public const int Hundredth2Twips = 20 * 72 / 100;
        
        public const int MM_ISOTROPIC = 7;
        public const int MM_ANISOTROPIC = 8;

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

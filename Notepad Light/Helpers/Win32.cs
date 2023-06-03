using System.Drawing.Printing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace Notepad_Light.Helpers
{
    public static class Win32
    {
        /// <summary>
        /// used to get machine details
        /// </summary>
        /// <returns></returns>
        public static StringBuilder osDetails()
        {
            SYSTEM_INFO info = new SYSTEM_INFO();
            MEMORYSTATUSEX mStatus = new MEMORYSTATUSEX();
            GetSystemInfo(ref info);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("App Details:");
            sb.AppendLine("-----------");
            sb.AppendLine("Version: Notepad Light v" + Assembly.GetExecutingAssembly().GetName().Version!.ToString());
            sb.AppendLine("Runtime: " + Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName);
            sb.AppendLine();
            sb.AppendLine("Machine Details:");
            sb.AppendLine("-----------");
            sb.AppendLine("Processor Architecture = " + ConvertProcArchitecture(info.wProcessorArchitecture));
            sb.AppendLine("Number of processors = " + info.dwNumberOfProcessors);
            sb.AppendLine("Page Size = " + info.dwPageSize);

            if (GlobalMemoryStatusEx(mStatus))
            {
                sb.AppendLine("Physical Memory = " + (mStatus.ullTotalPhys / 1024 / 1024 / 1024) + " GB");
                sb.AppendLine("Working Set Memory = " + App.SizeSuffix(Environment.WorkingSet));
            }            
            sb.AppendLine();
            sb.AppendLine("OS Details:");
            sb.AppendLine("-----------");
            OperatingSystem os = Environment.OSVersion;
            sb.AppendLine("OS Version = " + os.VersionString);
            sb.AppendLine("OS Platform = " + ConvertPlatform((int)os.Platform));

            if (os.ServicePack != string.Empty)
            {
                sb.AppendLine("OS Service Pack = " + os.ServicePack);
            }

            return sb;
        }

        public static string ConvertPlatform(int val)
        {
            switch (val)
            {
                case 0: return "Windows S Win32";
                case 1: return "Windows Win32";
                case 2: return "Win32NT";
                case 3: return "WinCE";
                case 4: return "Unix";
                case 5: return "Xbox";
                case 6: return "MacOSX";
                default: return "Other";
            }
        }

        public static string ConvertProcArchitecture(int val)
        {
            switch (val)
            {
                case 0: return "Intel x86";
                case 5: return "ARM";
                case 6: return "Intel Itanium-based";
                case 9: return "AMD/Intel x64";
                case 12: return "ARM64";
                default: return "unknown";
            }
        }

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

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator Point(POINT p)
            {
                return new Point(p.X, p.Y);
            }

            public static implicit operator POINT(Point p)
            {
                return new POINT(p.X, p.Y);
            }

            public override string ToString()
            {
                return $"X: {X}, Y: {Y}";
            }
        }

        public const int WM_USER = 0x0400;
        public const int EM_GETSCROLLPOS = WM_USER + 221;
        public const int EM_SETSCROLLPOS = WM_USER + 222;
        public const int EM_FORMATRANGE = WM_USER + 57;
        public const int Hundredth2Twips = 20 * 72 / 100;
        
        public const int MM_ISOTROPIC = 7;
        public const int MM_ANISOTROPIC = 8;

        /// <summary>
        /// contains information about the current state of both physical and virtual memory, including extended memory
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MEMORYSTATUSEX
        {
            /// <summary>
            /// Size of the structure, in bytes. You must set this member before calling GlobalMemoryStatusEx.
            /// </summary>
            public uint dwLength;

            /// <summary>
            /// Number between 0 and 100 that specifies the approximate percentage of physical memory that is in use (0 indicates no memory use and 100 indicates full memory use).
            /// </summary>
            public uint dwMemoryLoad;

            /// <summary>
            /// Total size of physical memory, in bytes.
            /// </summary>
            public ulong ullTotalPhys;

            /// <summary>
            /// Size of physical memory available, in bytes.
            /// </summary>
            public ulong ullAvailPhys;

            /// <summary>
            /// Size of the committed memory limit, in bytes. This is physical memory plus the size of the page file, minus a small overhead.
            /// </summary>
            public ulong ullTotalPageFile;

            /// <summary>
            /// Size of available memory to commit, in bytes. The limit is ullTotalPageFile.
            /// </summary>
            public ulong ullAvailPageFile;

            /// <summary>
            /// Total size of the user mode portion of the virtual address space of the calling process, in bytes.
            /// </summary>
            public ulong ullTotalVirtual;

            /// <summary>
            /// Size of unreserved and uncommitted memory in the user mode portion of the virtual address space of the calling process, in bytes.
            /// </summary>
            public ulong ullAvailVirtual;

            /// <summary>
            /// Size of unreserved and uncommitted memory in the extended portion of the virtual address space of the calling process, in bytes.
            /// </summary>
            public ulong ullAvailExtendedVirtual;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:MEMORYSTATUSEX"/> class.
            /// </summary>
            public MEMORYSTATUSEX()
            {
                dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

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
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
    }
}

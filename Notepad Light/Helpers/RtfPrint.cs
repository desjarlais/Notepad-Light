using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace Notepad_Light.Helpers
{
    class RtfPrint
    {
        // P/Invoke declarations
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
        private const int WM_USER = 0x0400;
        private const int EM_FORMATRANGE = WM_USER + 57;
        private const int Hundredth2Twips = 20 * 72 / 100;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        public static bool Print(RichTextBox box, ref int charFrom, PrintPageEventArgs e)
        {
            // Prints text in <box>, starting at <charFrom>.  Returns <true> if more pages are needed
            FORMATRANGE fmtRange;
            // Allocate device context for output device
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            IntPtr hdc = e.Graphics.GetHdc();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
    }
}

using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace Notepad_Light.Helpers
{
    class RtfPrint
    {
        public static bool Print(RichTextBox box, ref int charFrom, PrintPageEventArgs e)
        {
            // Prints text in <box>, starting at <charFrom>.  Returns <true> if more pages are needed
            Win32.FORMATRANGE fmtRange;
            // Allocate device context for output device
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            IntPtr hdc = e.Graphics.GetHdc();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            fmtRange.hdc = hdc;
            fmtRange.hdcTarget = hdc;

            // Set printable area, converted from 0.01" to twips
            fmtRange.rc.Top = Convert.ToInt32(e.MarginBounds.Top * Win32.Hundredth2Twips);
            fmtRange.rc.Bottom = Convert.ToInt32(e.MarginBounds.Bottom * Win32.Hundredth2Twips);
            fmtRange.rc.Left = Convert.ToInt32(e.MarginBounds.Left * Win32.Hundredth2Twips);
            fmtRange.rc.Right = Convert.ToInt32(e.MarginBounds.Right * Win32.Hundredth2Twips);

            // Set page area, converted from 0.01" to twips
            fmtRange.rcPage.Top = Convert.ToInt32(e.PageBounds.Top * Win32.Hundredth2Twips);
            fmtRange.rcPage.Bottom = Convert.ToInt32(e.PageBounds.Bottom * Win32.Hundredth2Twips);
            fmtRange.rcPage.Left = Convert.ToInt32(e.PageBounds.Left * Win32.Hundredth2Twips);
            fmtRange.rcPage.Right = Convert.ToInt32(e.PageBounds.Right * Win32.Hundredth2Twips);

            // Set character range to print
            fmtRange.chrg.cpMin = charFrom;
            fmtRange.chrg.cpMax = box.TextLength;

            // Marshal to unmanaged memory
            IntPtr hdlRange = Marshal.AllocCoTaskMem(Marshal.SizeOf(fmtRange));
            Marshal.StructureToPtr(fmtRange, hdlRange, false);

            // Send RichTextBox the EM_FORMATRANGE message to print the text
            IntPtr res = Win32.SendMessage(box.Handle, Win32.EM_FORMATRANGE, (IntPtr)1, hdlRange);
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

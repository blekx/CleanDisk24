using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace CleanDisk24
{
    public static class MouseHooking
    {
        #region Mouse Dll
        //https://stackoverflow.com/questions/4226740/how-do-i-get-the-current-mouse-screen-coordinates-in-wpf
        //https://stackoverflow.com/questions/935599/how-to-center-a-wpf-app-on-screen?rq=1

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }
        #endregion
    }
}
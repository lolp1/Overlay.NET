using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Overlay.NET.Common
{
    public static class WindowExtensions
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        public static void MakeWindowTransparent(this Window wnd)
        {
            var hwnd = new WindowInteropHelper(wnd).Handle;
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);
    }
}
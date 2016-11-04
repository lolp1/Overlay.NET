using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Overlay.NET.Common
{
    public static class WindowExtensions
    {
        const int GwlExstyle = -20;
        const int WsExTransparent = 0x00000020;

        public static void MakeWindowTransparent(this Window wnd)
        {
            var hwnd = new WindowInteropHelper(wnd).Handle;
            var extendedStyle = GetWindowLong(hwnd, GwlExstyle);
            SetWindowLong(hwnd, GwlExstyle, extendedStyle | WsExTransparent);
        }

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);
    }
}
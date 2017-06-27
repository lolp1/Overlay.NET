using System;
using System.Runtime.InteropServices;

namespace Overlay.NET.Common {
    internal class Native {
        [DllImport("user32.dll")]
        public static extern IntPtr CreateWindowEx(
            uint dwExStyle,
            string lpClassName,
            string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        [DllImport("user32.dll")]
        public static extern int DestroyWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy,
            uint uFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int index);

        [DllImport("user32.dll")]
        public static extern int ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hwnd, out Rect lpRect);

        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdc, ref Point point, ref Point size,
            IntPtr hdcSrc, IntPtr pptSrc, int crKey, IntPtr blend, uint flags);

        [DllImport("dwmapi.dll")]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref RawMargin pMargins);

        [DllImport("dwmapi.dll")]
        public static extern int DwmEnableBlurBehindWindow(IntPtr hWnd, ref DWM_BLURBEHIND pBlurBehind);

        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
            => IntPtr.Size == 8 ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        // This static method is required because Win32 does not support
        // GetWindowLongPtr directly
        public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex) => IntPtr.Size == 8 ? GetWindowLongPtr64(hWnd, nIndex) : GetWindowLongPtr32(hWnd, nIndex);

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RawMargin {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_BLURBEHIND
        {
            public uint dwFlags;
            public int fEnable;
            public uint hRgnBlur;
            public int fTransitionOnMaximized;
        }
    }
}
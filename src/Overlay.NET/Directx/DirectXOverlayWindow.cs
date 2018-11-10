using System;
using System.Threading;
using System.Threading.Tasks;
using Overlay.NET.Common;

namespace Overlay.NET.Directx {
    public class DirectXOverlayWindow {
        /// <summary>
        ///     Gets a value indicating whether this instance is disposing.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is disposing; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposing { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether [parent window exists].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [parent window exists]; otherwise, <c>false</c>.
        /// </value>
        public bool ParentWindowExists { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is top most.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is top most; otherwise, <c>false</c>.
        /// </value>
        public bool IsTopMost { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible { get; private set; }

        /// <summary>
        ///     Gets the x.
        /// </summary>
        /// <value>
        ///     The x.
        /// </value>
        public int X { get; private set; }

        /// <summary>
        ///     Gets the y.
        /// </summary>
        /// <value>
        ///     The y.
        /// </value>
        public int Y { get; private set; }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public int Width { get; private set; }

        /// <summary>
        ///     Gets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public int Height { get; private set; }

        /// <summary>
        ///     Gets the handle.
        /// </summary>
        /// <value>
        ///     The handle.
        /// </value>
        public IntPtr Handle { get; private set; }

        /// <summary>
        ///     Gets the parent window.
        /// </summary>
        /// <value>
        ///     The parent window.
        /// </value>
        public IntPtr ParentWindow { get; }

        /// <summary>
        ///     The margin
        /// </summary>
        private Native.RawMargin _margin;

        /// <summary>
        ///     The graphics
        /// </summary>
        public Direct2DRenderer Graphics;

        /// <summary>
        ///     Makes a transparent Fullscreen window
        /// </summary>
        /// <param name="limitFps">VSync</param>
        /// <exception cref="Exception">Could not create OverlayWindow</exception>
        public DirectXOverlayWindow(bool limitFps = true) {
            IsDisposing = false;
            IsVisible = true;
            IsTopMost = true;

            ParentWindowExists = false;

            X = 0;
            Y = 0;
            Width = Native.GetSystemMetrics(WindowConstants.SmCxScreen);
            Height = Native.GetSystemMetrics(WindowConstants.SmCyScreen);

            ParentWindow = IntPtr.Zero;

            if (!CreateWindow()) {
                throw new Exception("Could not create OverlayWindow");
            }

            Graphics = new Direct2DRenderer(Handle, limitFps);

            SetBounds(X, Y, Width, Height);
        }

        /// <summary>
        ///     Makes a transparent window which adjust it's size and position to fit the parent window
        /// </summary>
        /// <param name="parent">HWND/Handle of a window</param>
        /// <param name="limitFps">VSync</param>
        /// <exception cref="Exception">
        ///     The handle of the parent window isn't valid
        ///     or
        ///     Could not create OverlayWindow
        /// </exception>
        public DirectXOverlayWindow(IntPtr parent, bool limitFps = true) {
            if (parent == IntPtr.Zero) {
                throw new Exception("The handle of the parent window isn't valid");
            }

            Native.Rect bounds;
            Native.GetWindowRect(parent, out bounds);

            IsDisposing = false;
            IsVisible = true;
            IsTopMost = true;

            ParentWindowExists = true;

            X = bounds.Left;
            Y = bounds.Top;

            Width = bounds.Right - bounds.Left;
            Height = bounds.Bottom - bounds.Top;

            ParentWindow = parent;

            if (!CreateWindow()) {
                throw new Exception("Could not create OverlayWindow");
            }

            Graphics = new Direct2DRenderer(Handle, limitFps);

            SetBounds(X, Y, Width, Height);
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="DirectXOverlayWindow" /> class.
        /// </summary>
        ~DirectXOverlayWindow() {
            Dispose();
        }

        /// <summary>
        ///     Clean up used ressources and destroy window
        /// </summary>
        public void Dispose() {
            IsDisposing = true;
            Graphics.Dispose();
            Native.DestroyWindow(Handle);
        }

        /// <summary>
        ///     Creates a window with the information's stored in this class
        /// </summary>
        /// <returns>
        ///     true on success
        /// </returns>
        private bool CreateWindow() {
            Handle = Native.CreateWindowEx(
                WindowConstants.WindowExStyleDx,
                WindowConstants.DesktopClass,
                "",
                WindowConstants.WindowStyleDx,
                X,
                Y,
                Width,
                Height,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero);

            if (Handle == IntPtr.Zero) {
                return false;
            }

            Native.SetLayeredWindowAttributes(Handle, 0, 255, WindowConstants.LwaAlpha);

            ExtendFrameIntoClient();

            return true;
        }

        /// <summary>
        /// Resize and set new position if the parent window's bounds change
        /// </summary>
        public void Update() {
            if (!ParentWindowExists)
                return;

            Native.Rect bounds;
            Native.GetWindowRect(ParentWindow, out bounds);

            if (X != bounds.Left || Y != bounds.Top || Width != bounds.Right - bounds.Left ||
                Height != bounds.Bottom - bounds.Top) {
                SetBounds(bounds.Left, bounds.Top, bounds.Right - bounds.Left, bounds.Bottom - bounds.Top);
            }
        }

        /// <summary>
        ///     Extends the frame into client.
        /// </summary>
        private void ExtendFrameIntoClient() {
            _margin.cxLeftWidth = X;
            _margin.cxRightWidth = Width;
            _margin.cyBottomHeight = Height;
            _margin.cyTopHeight = Y;
            Native.DwmExtendFrameIntoClientArea(Handle, ref _margin);
        }

        /// <summary>
        ///     Sets the position.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void SetPos(int x, int y) {
            X = x;
            Y = y;

            Native.SetWindowPos(Handle, WindowConstants.HwndTopmost, X, Y, Width, Height, 0);

            ExtendFrameIntoClient();
        }

        /// <summary>
        ///     Sets the size.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void SetSize(int width, int height) {
            Width = width;
            Height = height;

            Native.SetWindowPos(Handle, WindowConstants.HwndTopmost, X, Y, Width, Height, 0);

            Graphics.AutoResize(Width, Height);

            ExtendFrameIntoClient();
        }

        /// <summary>
        ///     Sets the bounds.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void SetBounds(int x, int y, int width, int height) {
            X = x;
            Y = y;
            Width = width;
            Height = height;

            Native.SetWindowPos(Handle, WindowConstants.HwndTopmost, X, Y, Width, Height, 0);

            Graphics?.AutoResize(Width, Height);

            ExtendFrameIntoClient();
        }

        /// <summary>
        ///     Shows this instance.
        /// </summary>
        public void Show() {
            if (IsVisible) {
                return;
            }

            Native.ShowWindow(Handle, WindowConstants.SwShow);
            IsVisible = true;

            ExtendFrameIntoClient();
        }

        /// <summary>
        ///     Hides this instance.
        /// </summary>
        public void Hide() {
            if (!IsVisible) {
                return;
            }

            Native.ShowWindow(Handle, WindowConstants.SwHide);
            IsVisible = false;
        }
    }
}
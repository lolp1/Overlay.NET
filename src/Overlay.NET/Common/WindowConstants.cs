namespace Overlay.NET.Common {
    /// <summary>
    /// </summary>
    internal class WindowConstants {
        /// <summary>
        ///     The sm cx screen
        /// </summary>
        public const int SmCxScreen = 0;

        /// <summary>
        ///     The sm cy screen
        /// </summary>
        public const int SmCyScreen = 1;

        /// <summary>
        ///     The desktop class
        /// </summary>
        public const string DesktopClass = "Static"; // System Class for a static control

        /// <summary>
        ///     The window style dx
        /// </summary>
        public const uint WindowStyleDx = 0x8000000 //WS_DISABLED
                                          | 0x10000000 //WS_VISIBLE
                                          | 0x80000000; //WS_POPUP

        /// <summary>
        ///     The window ex style dx
        /// </summary>
        public const uint WindowExStyleDx = 0x8000000 //WS_EX_NOACTIVATE
                                            | 0x80000 //WS_EX_LAYERED
                                            | 0x80 //WS_EX_TOOLWINDOW -> Not in taskbar
                                            | 0x8 //WS_EX_TOPMOST
                                            | 0x20; //WS_EX_TRANSPARENT

        /// <summary>
        ///     The layered window attribute alpha (LWA_ALPHA)
        /// </summary>
        public const int LwaAlpha = 0x00000002;

        /// <summary>
        ///     The HWND notopmost
        /// </summary>
        public const int HwndNotopmost = -2;

        /// <summary>
        ///     The HWND topmost
        /// </summary>
        public const int HwndTopmost = -1;

        /// <summary>
        ///     The HWND top
        /// </summary>
        public const int HwndTop = 0;

        /// <summary>
        ///     The HWND bottom
        /// </summary>
        public const int HwndBottom = 1;

        /// <summary>
        ///     The SWP flags show
        /// </summary>
        public const uint SwpFlagsShow = 0x40;

        /// <summary>
        ///     The SWP flags hide
        /// </summary>
        public const uint SwpFlagsHide = 0x80;

        /// <summary>
        ///     The sw show
        /// </summary>
        public const uint SwShow = 5;

        /// <summary>
        ///     The sw hide
        /// </summary>
        public const uint SwHide = 0;
    }
}
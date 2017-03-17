namespace Overlay.NET.Wpf {
    /// <summary>
    ///     Abstract overlay that uses a transparent WPF window to do its rendering.
    /// </summary>
    /// <seealso cref="Overlay.NET.OverlayPlugin" />
    public abstract class WpfOverlayPlugin : OverlayPlugin {
        /// <summary>
        ///     Gets or sets the overlay window.
        /// </summary>
        /// <value>
        ///     The overlay window.
        /// </value>
        public OverlayWindow OverlayWindow { get; protected set; }
    }
}
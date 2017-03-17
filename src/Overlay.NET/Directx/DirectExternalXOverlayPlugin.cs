namespace Overlay.NET.Directx {
    /// <summary>
    /// </summary>
    /// <seealso cref="Overlay.NET.OverlayPlugin" />
    public abstract class DirectXOverlayPlugin : OverlayPlugin {
        /// <summary>
        ///     Gets or sets the overlay window.
        /// </summary>
        /// <value>
        ///     The overlay window.
        /// </value>
        public DirectXOverlayWindow OverlayWindow { get; protected set; }
    }
}
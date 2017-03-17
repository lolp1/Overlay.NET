using SharpDX.DirectWrite;

namespace Overlay.NET.Directx {
    /// <summary>
    /// </summary>
    internal class TextLayoutBuffer {
        /// <summary>
        ///     The text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     The text layout
        /// </summary>
        public TextLayout TextLayout { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextLayoutBuffer" /> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="layout">The layout.</param>
        public TextLayoutBuffer(string text, TextLayout layout) {
            Text = text;
            TextLayout = layout;
            TextLayout.TextAlignment = TextAlignment.Leading;
            TextLayout.WordWrapping = WordWrapping.NoWrap;
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose() {
            TextLayout.Dispose();
            Text = null;
        }
    }
}
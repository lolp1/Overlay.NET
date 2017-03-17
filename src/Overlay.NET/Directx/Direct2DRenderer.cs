using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Overlay.NET.Common;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Color = System.Drawing.Color;
using Factory = SharpDX.DirectWrite.Factory;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;

namespace Overlay.NET.Directx {
    /// <summary>
    /// </summary>
    public class Direct2DRenderer {
        /// <summary>
        ///     Gets the size of the buffer brush.
        /// </summary>
        /// <value>
        ///     The size of the buffer brush.
        /// </value>
        public int BufferBrushSize { get; private set; }

        /// <summary>
        ///     Gets the size of the buffer font.
        /// </summary>
        /// <value>
        ///     The size of the buffer font.
        /// </value>
        public int BufferFontSize { get; private set; }

        /// <summary>
        ///     Gets the size of the buffer layout.
        /// </summary>
        /// <value>
        ///     The size of the buffer layout.
        /// </value>
        public int BufferLayoutSize { get; private set; }

        //transparent background color
        /// <summary>
        ///     The GDI transparent
        /// </summary>
        private static Color _gdiTransparent = Color.Transparent;

        /// <summary>
        ///     The transparent
        /// </summary>
        private static readonly RawColor4 Transparent = new RawColor4(_gdiTransparent.R, _gdiTransparent.G, _gdiTransparent.B,
            _gdiTransparent.A);

        //direct x vars
        /// <summary>
        ///     The device
        /// </summary>
        private readonly WindowRenderTarget _device;

        /// <summary>
        ///     The factory
        /// </summary>
        private readonly SharpDX.Direct2D1.Factory _factory;

        /// <summary>
        ///     The font factory
        /// </summary>
        private readonly Factory _fontFactory;

        /// <summary>
        ///     The brush container
        /// </summary>
        private List<SolidColorBrush> _brushContainer = new List<SolidColorBrush>(32);

        //thread safe resizing
        /// <summary>
        ///     The do resize
        /// </summary>
        private bool _doResize;

        /// <summary>
        ///     The font container
        /// </summary>
        private List<TextFormat> _fontContainer = new List<TextFormat>(32);

        /// <summary>
        ///     The layout container
        /// </summary>
        private List<TextLayoutBuffer> _layoutContainer = new List<TextLayoutBuffer>(32);

        /// <summary>
        ///     The resize x
        /// </summary>
        private int _resizeX;

        /// <summary>
        ///     The resize y
        /// </summary>
        private int _resizeY;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Direct2DRenderer" /> class.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="limitFps">if set to <c>true</c> [limit FPS].</param>
        public Direct2DRenderer(IntPtr hwnd, bool limitFps) {
            _factory = new SharpDX.Direct2D1.Factory();

            _fontFactory = new Factory();

            Native.Rect bounds;
            Native.GetWindowRect(hwnd, out bounds);

            var targetProperties = new HwndRenderTargetProperties {
                Hwnd = hwnd,
                PixelSize = new Size2(bounds.Right - bounds.Left, bounds.Bottom - bounds.Top),
                PresentOptions = limitFps ? PresentOptions.None : PresentOptions.Immediately
            };

            var prop = new RenderTargetProperties(RenderTargetType.Hardware,
                new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied), 0, 0, RenderTargetUsage.None,
                FeatureLevel.Level_DEFAULT);

            _device = new WindowRenderTarget(_factory, prop, targetProperties) {
                TextAntialiasMode = TextAntialiasMode.Aliased,
                AntialiasMode = AntialiasMode.Aliased
            };
        }

        /// <summary>
        ///     Do not call if you use OverlayWindow class
        /// </summary>
        public void Dispose() {
            DeleteBrushContainer();
            DeleteFontContainer();
            DeleteLayoutContainer();

            _brushContainer = null;
            _fontContainer = null;
            _layoutContainer = null;

            _fontFactory.Dispose();
            _factory.Dispose();
            _device.Dispose();
        }

        /// <summary>
        ///     tells renderer to resize when possible
        /// </summary>
        /// <param name="x">Width</param>
        /// <param name="y">Height</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AutoResize(int x, int y) {
            _doResize = true;
            _resizeX = x;
            _resizeY = y;
        }

        /// <summary>
        ///     Call this after EndScene if you created brushes within a loop
        /// </summary>
        public void DeleteBrushContainer() {
            BufferBrushSize = _brushContainer.Count;
            foreach (var solidColorBrush in _brushContainer) {
                solidColorBrush.Dispose();
            }
            _brushContainer = new List<SolidColorBrush>(BufferBrushSize);
        }

        /// <summary>
        ///     Call this after EndScene if you created fonts within a loop
        /// </summary>
        public void DeleteFontContainer() {
            BufferFontSize = _fontContainer.Count;
            foreach (var textFormat in _fontContainer) {
                textFormat.Dispose();
            }
            _fontContainer = new List<TextFormat>(BufferFontSize);
        }

        /// <summary>
        ///     Call this after EndScene if you changed your text's font or have problems with huge memory usage
        /// </summary>
        public void DeleteLayoutContainer() {
            BufferLayoutSize = _layoutContainer.Count;
            foreach (var layoutBuffer in _layoutContainer) {
                layoutBuffer.Dispose();
            }
            _layoutContainer = new List<TextLayoutBuffer>(BufferLayoutSize);
        }

        /// <summary>
        ///     Creates a new SolidColorBrush
        /// </summary>
        /// <param name="color">0x7FFFFFF Premultiplied alpha color</param>
        /// <returns>
        ///     int Brush identifier
        /// </returns>
        public int CreateBrush(int color) {
            _brushContainer.Add(new SolidColorBrush(_device,
                new RawColor4((color >> 16) & 255L, (color >> 8) & 255L, (byte) color & 255L, (color >> 24) & 255L)));
            return _brushContainer.Count - 1;
        }

        /// <summary>
        ///     Creates a new SolidColorBrush. Make sure you applied an alpha value
        /// </summary>
        /// <param name="color">System.Drawing.Color struct</param>
        /// <returns>
        ///     int Brush identifier
        /// </returns>
        public int CreateBrush(Color color) {
            if (color.A == 0) {
                color = Color.FromArgb(255, color);
            }

            _brushContainer.Add(new SolidColorBrush(_device, new RawColor4(color.R, color.G, color.B, color.A / 255.0f)));
            return _brushContainer.Count - 1;
        }

        /// <summary>
        ///     Creates a new Font
        /// </summary>
        /// <param name="fontFamilyName">i.e. Arial</param>
        /// <param name="size">size in units</param>
        /// <param name="bold">print bold text</param>
        /// <param name="italic">print italic text</param>
        /// <returns></returns>
        public int CreateFont(string fontFamilyName, float size, bool bold = false, bool italic = false) {
            _fontContainer.Add(new TextFormat(_fontFactory, fontFamilyName, bold ? FontWeight.Bold : FontWeight.Normal,
                italic ? FontStyle.Italic : FontStyle.Normal, size));
            return _fontContainer.Count - 1;
        }

        /// <summary>
        ///     Do your drawing after this
        /// </summary>
        public void BeginScene() {
            if (_doResize) {
                _device.Resize(new Size2(_resizeX, _resizeY));

                _doResize = false;
            }
            _device.BeginDraw();
        }

        /// <summary>
        ///     Present frame. Do not draw after this.
        /// </summary>
        public void EndScene() {
            _device.EndDraw();
            if (!_doResize) {
                return;
            }
            _device.Resize(new Size2(_resizeX, _resizeY));

            _doResize = false;
        }

        /// <summary>
        ///     Clears the frame
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearScene() => _device.Clear(Transparent);

        /// <summary>
        ///     Draws the line.
        /// </summary>
        /// <param name="startX">The start x.</param>
        /// <param name="startY">The start y.</param>
        /// <param name="endX">The end x.</param>
        /// <param name="endY">The end y.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawLine(int startX, int startY, int endX, int endY, float stroke, int brush) => _device.DrawLine(new RawVector2(startX, startY), new RawVector2(endX, endY), _brushContainer[brush], stroke);

        /// <summary>
        ///     Draws the rectangle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawRectangle(int x, int y, int width, int height, float stroke, int brush) => _device.DrawRectangle(new RawRectangleF(x, y, x + width, y + height), _brushContainer[brush], stroke);

        /// <summary>
        ///     Draws the circle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawCircle(int x, int y, int radius, float stroke, int brush) => _device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius, radius), _brushContainer[brush], stroke);

        /// <summary>
        ///     Draws the box2 d.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="interiorBrush">The interior brush.</param>
        public void DrawBox2D(int x, int y, int width, int height, float stroke, int brush, int interiorBrush) {
            _device.DrawRectangle(new RawRectangleF(x, y, x + width, y + height), _brushContainer[brush], stroke);
            _device.FillRectangle(new RawRectangleF(x + stroke, y + stroke, x + width - stroke, y + height - stroke),
                _brushContainer[interiorBrush]);
        }

        /// <summary>
        ///     Draws the box3 d.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="length">The length.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="interiorBrush">The interior brush.</param>
        public void DrawBox3D(int x, int y, int width, int height, int length, float stroke, int brush,
            int interiorBrush) {
            var first = new RawRectangleF(x, y, x + width, y + height);
            var second = new RawRectangleF(x + length, y - length, first.Right + length, first.Bottom - length);

            var lineStart = new RawVector2(x, y);
            var lineEnd = new RawVector2(second.Left, second.Top);

            _device.DrawRectangle(first, _brushContainer[brush], stroke);
            _device.DrawRectangle(second, _brushContainer[brush], stroke);

            _device.FillRectangle(first, _brushContainer[interiorBrush]);
            _device.FillRectangle(second, _brushContainer[interiorBrush]);

            _device.DrawLine(lineStart, lineEnd, _brushContainer[brush], stroke);

            lineStart.X += width;
            lineEnd.X = lineStart.X + length;

            _device.DrawLine(lineStart, lineEnd, _brushContainer[brush], stroke);

            lineStart.Y += height;
            lineEnd.Y += height;

            _device.DrawLine(lineStart, lineEnd, _brushContainer[brush], stroke);

            lineStart.X -= width;
            lineEnd.X -= width;

            _device.DrawLine(lineStart, lineEnd, _brushContainer[brush], stroke);
        }

        /// <summary>
        ///     Draws the rectangle3 d.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="length">The length.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        public void DrawRectangle3D(int x, int y, int width, int height, int length, float stroke, int brush) {
            var first = new RawRectangleF(x, y, x + width, y + height);
            var second = new RawRectangleF(x + length, y - length, first.Right + length, first.Bottom - length);

            var lineStart = new RawVector2(x, y);
            var lineEnd = new RawVector2(second.Left, second.Top);

            _device.DrawRectangle(first, _brushContainer[brush], stroke);
            _device.DrawRectangle(second, _brushContainer[brush], stroke);

            _device.DrawLine(lineStart, lineEnd, _brushContainer[brush], stroke);

            lineStart.X += width;
            lineEnd.X = lineStart.X + length;

            _device.DrawLine(lineStart, lineEnd, _brushContainer[brush], stroke);

            lineStart.Y += height;
            lineEnd.Y += height;

            _device.DrawLine(lineStart, lineEnd, _brushContainer[brush], stroke);

            lineStart.X -= width;
            lineEnd.X -= width;

            _device.DrawLine(lineStart, lineEnd, _brushContainer[brush], stroke);
        }

        /// <summary>
        ///     Draws the plus.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="length">The length.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        public void DrawPlus(int x, int y, int length, float stroke, int brush) {
            var first = new RawVector2(x - length, y);
            var second = new RawVector2(x + length, y);

            var third = new RawVector2(x, y - length);
            var fourth = new RawVector2(x, y + length);

            _device.DrawLine(first, second, _brushContainer[brush], stroke);
            _device.DrawLine(third, fourth, _brushContainer[brush], stroke);
        }

        /// <summary>
        ///     Draws the edge.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="length">The length.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        public void DrawEdge(int x, int y, int width, int height, int length, float stroke, int brush) //geht
        {
            var first = new RawVector2(x, y);
            var second = new RawVector2(x, y + length);
            var third = new RawVector2(x + length, y);

            _device.DrawLine(first, second, _brushContainer[brush], stroke);
            _device.DrawLine(first, third, _brushContainer[brush], stroke);

            first.Y += height;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X + length;

            _device.DrawLine(first, second, _brushContainer[brush], stroke);
            _device.DrawLine(first, third, _brushContainer[brush], stroke);

            first.X = x + width;
            first.Y = y;
            second.X = first.X - length;
            second.Y = first.Y;
            third.X = first.X;
            third.Y = first.Y + length;

            _device.DrawLine(first, second, _brushContainer[brush], stroke);
            _device.DrawLine(first, third, _brushContainer[brush], stroke);

            first.Y += height;
            second.X += length;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X - length;

            _device.DrawLine(first, second, _brushContainer[brush], stroke);
            _device.DrawLine(first, third, _brushContainer[brush], stroke);
        }

        /// <summary>
        ///     Draws the bar h.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="value">The value.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="interiorBrush">The interior brush.</param>
        public void DrawBarH(int x, int y, int width, int height, float value, float stroke, int brush,
            int interiorBrush) {
            var first = new RawRectangleF(x, y, x + width, y + height);

            _device.DrawRectangle(first, _brushContainer[brush], stroke);

            if (Math.Abs(value) < 0) {
                return;
            }

            first.Top += height - height / 100.0f * value;

            _device.FillRectangle(first, _brushContainer[interiorBrush]);
        }

        /// <summary>
        ///     Draws the bar v.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="value">The value.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="interiorBrush">The interior brush.</param>
        public void DrawBarV(int x, int y, int width, int height, float value, float stroke, int brush,
            int interiorBrush) {
            var first = new RawRectangleF(x, y, x + width, y + height);

            _device.DrawRectangle(first, _brushContainer[brush], stroke);

            if (Math.Abs(value) < 0) {
                return;
            }

            first.Right -= width - width / 100.0f * value;

            _device.FillRectangle(first, _brushContainer[interiorBrush]);
        }

        /// <summary>
        ///     Fills the rectangle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="brush">The brush.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillRectangle(int x, int y, int width, int height, int brush) => _device.FillRectangle(new RawRectangleF(x, y, x + width, y + height), _brushContainer[brush]);

        /// <summary>
        ///     Fills the circle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="brush">The brush.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillCircle(int x, int y, int radius, int brush) => _device.FillEllipse(new Ellipse(new RawVector2(x, y), radius, radius), _brushContainer[brush]);

        /// <summary>
        ///     Bordereds the line.
        /// </summary>
        /// <param name="startX">The start x.</param>
        /// <param name="startY">The start y.</param>
        /// <param name="endX">The end x.</param>
        /// <param name="endY">The end y.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="borderBrush">The border brush.</param>
        public void BorderedLine(int startX, int startY, int endX, int endY, float stroke, int brush, int borderBrush) {
            _device.DrawLine(new RawVector2(startX, startY), new RawVector2(endX, endY), _brushContainer[brush], stroke);

            _device.DrawLine(new RawVector2(startX, startY - stroke), new RawVector2(endX, endY - stroke),
                _brushContainer[borderBrush], stroke);
            _device.DrawLine(new RawVector2(startX, startY + stroke), new RawVector2(endX, endY + stroke),
                _brushContainer[borderBrush], stroke);

            _device.DrawLine(new RawVector2(startX - stroke / 2, startY - stroke * 1.5f),
                new RawVector2(startX - stroke / 2, startY + stroke * 1.5f), _brushContainer[borderBrush], stroke);
            _device.DrawLine(new RawVector2(endX - stroke / 2, endY - stroke * 1.5f),
                new RawVector2(endX - stroke / 2, endY + stroke * 1.5f), _brushContainer[borderBrush], stroke);
        }

        /// <summary>
        ///     Bordereds the rectangle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="borderStroke">The border stroke.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="borderBrush">The border brush.</param>
        public void BorderedRectangle(int x, int y, int width, int height, float stroke, float borderStroke, int brush,
            int borderBrush) {
            _device.DrawRectangle(
                new RawRectangleF(x - (stroke - borderStroke), y - (stroke - borderStroke),
                    x + width + stroke - borderStroke, y + height + stroke - borderStroke), _brushContainer[borderBrush],
                borderStroke);

            _device.DrawRectangle(new RawRectangleF(x, y, x + width, y + height), _brushContainer[brush], stroke);

            _device.DrawRectangle(
                new RawRectangleF(x + (stroke - borderStroke), y + (stroke - borderStroke),
                    x + width - stroke + borderStroke, y + height - stroke + borderStroke), _brushContainer[borderBrush],
                borderStroke);
        }

        /// <summary>
        ///     Bordereds the circle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="borderBrush">The border brush.</param>
        public void BorderedCircle(int x, int y, int radius, float stroke, int brush, int borderBrush) {
            _device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius + stroke, radius + stroke),
                _brushContainer[borderBrush], stroke);

            _device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius, radius), _brushContainer[brush], stroke);

            _device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius - stroke, radius - stroke),
                _brushContainer[borderBrush], stroke);
        }

        /// <summary>
        ///     Do not buffer text if you draw i.e. FPS. Use buffer for player names, rank....
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="bufferText">if set to <c>true</c> [buffer text].</param>
        public void DrawText(string text, int font, int brush, int x, int y, bool bufferText = true) {
            if (bufferText) {
                var bufferPos = -1;

                for (var i = 0; i < _layoutContainer.Count; i++) {
                    if (_layoutContainer[i].Text.Length != text.Length || _layoutContainer[i].Text != text) {
                        continue;
                    }
                    bufferPos = i;
                    break;
                }

                if (bufferPos == -1) {
                    _layoutContainer.Add(new TextLayoutBuffer(text,
                        new TextLayout(_fontFactory, text, _fontContainer[font], float.MaxValue, float.MaxValue)));
                    bufferPos = _layoutContainer.Count - 1;
                }

                _device.DrawTextLayout(new RawVector2(x, y), _layoutContainer[bufferPos].TextLayout,
                    _brushContainer[brush], DrawTextOptions.NoSnap);
            }
            else {
                var layout = new TextLayout(_fontFactory, text, _fontContainer[font], float.MaxValue, float.MaxValue);
                _device.DrawTextLayout(new RawVector2(x, y), layout, _brushContainer[brush]);
                layout.Dispose();
            }
        }
    }
}
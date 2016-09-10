using System;
using System.Windows;
using Overlay.NET.Common;
using Process.NET.Windows;

namespace Overlay.NET.Wpf
{
    /// <summary>
    ///     Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {
        private readonly IWindow _targetWindow;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OverlayWindow" /> class.
        /// </summary>
        /// <param name="targetWindow">The window.</param>
        public OverlayWindow(IWindow targetWindow)
        {
            _targetWindow = targetWindow;
            InitializeComponent();
        }

        /// <summary>
        ///     Updates this instance.
        /// </summary>
        public void Update()
        {
            Width = _targetWindow.Width;
            Height = _targetWindow.Height;
            Left = _targetWindow.X;
            Top = _targetWindow.Y;
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Window.SourceInitialized" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            // We need to do this in order to allow shapes
            // drawn on the canvas to be click-through. 
            // There is no other way to do this.
            // Source: https://social.msdn.microsoft.com/Forums/en-US/c32889d3-effa-4b82-b179-76489ffa9f7d/how-to-clicking-throughpassing-shapesellipserectangle?forum=wpf
            this.MakeWindowTransparent();
        }

        /// <summary>
        ///     Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Add(UIElement element)
        {
            OverlayGrid.Children.Add(element);
        }

        /// <summary>
        ///     Removes the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Remove(UIElement element)
        {
            OverlayGrid.Children.Remove(element);
        }

        /// <summary>
        ///     Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="index">The index.</param>
        public void Add(UIElement element, int index)
        {
            OverlayGrid.Children[index] = element;
        }

        /// <summary>
        ///     Removes the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void Remove(int index)
        {
            OverlayGrid.Children.RemoveAt(index);
        }
    }
}
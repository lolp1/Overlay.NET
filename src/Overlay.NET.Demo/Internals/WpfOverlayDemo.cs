using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Overlay.NET.Common;
using Overlay.NET.Wpf;
using Process.NET.Windows;

namespace Overlay.NET.Demo.Internals
{
    [RegisterPlugin("WpfOverlayDemo-1", "Jacob Kemple", "WpfOverlayDemo", "0.0", "A basic demo of the WPF overlay.")]
    public class WpfOverlayDemo : WpfOverlayPlugin
    {
// Used to limit update rates via timestamps 
        // This way we can avoid thread issues with wanting to delay updates
        private readonly TickEngine _tickEngine = new TickEngine();
        private Ellipse _ellipse;

        private bool _isSetup;
        private Line _line;
        private Polygon _polygon;

        // Shapes used in the demo
        private Rectangle _rectangle;

        public ISettings<WpfDemoOverlaySettings> Settings = new SerializableSettings<WpfDemoOverlaySettings>();


        public override void Enable()
        {
            _tickEngine.IsTicking = true;
            base.Enable();
        }

        public override void Disable()
        {
            _tickEngine.IsTicking = false;
            base.Disable();
        }

        public override void Initialize(IWindow targetWindow)
        {
            // Set target window by calling the base method
            base.Initialize(targetWindow);

            OverlayWindow = new OverlayWindow(targetWindow);

            Settings.Load();

            // For demo, show how to use settings
            var current = Settings.Current;
            var type = GetType();

            current.Author = GetAuthor(type);
            current.Description = GetDescription(type);
            current.Identifier = GetIdentifier(type);
            current.Name = GetName(type);
            current.Version = GetVersion(type);

            // File is made from above info
            Settings.Save();

            // Set up update interval and register events for the tick engine.
            _tickEngine.Interval = Settings.Current.UpdateRate.Milliseconds();
            _tickEngine.PreTick += OnPreTick;
            _tickEngine.Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs eventArgs)
        {
            // Only want to set them up once.
            if (!_isSetup)
            {
                SetUp();
                _isSetup = true;
            }

            // This will only be true if the target window is active
            // (or very recently has been, depends on your update rate)
            if (OverlayWindow.IsVisible)
            {
                OverlayWindow.Update();
            }
        }

        private void OnPreTick(object sender, EventArgs eventArgs)
        {
            // Ensure window is shown or hidden correctly prior to updating
            if (!TargetWindow.IsActivated && OverlayWindow.IsVisible)
            {
                OverlayWindow.Hide();
                return;
            }

            if (TargetWindow.IsActivated && !OverlayWindow.IsVisible)
            {
                OverlayWindow.Show();
            }
        }


        public override void Update()
        {
            // Raises the events only when the given interval has
            // passed since the last event, so it is okay to call every frame
            _tickEngine.Pulse();
        }

        // Clear objects
        public override void Dispose()
        {
            if (IsEnabled)
            {
                Disable();
            }

            OverlayWindow?.Hide();
            OverlayWindow?.Close();
            OverlayWindow = null;
            _tickEngine.Stop();
            Settings.Save();
            base.Dispose();
        }

        // Random shapes.. thanks Julian ^_^
        private void SetUp()
        {
            _polygon = new Polygon
            {
                Points = new PointCollection(5)
                {
                    new Point(100, 150),
                    new Point(120, 130),
                    new Point(140, 150),
                    new Point(140, 200),
                    new Point(100, 200)
                },
                Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 255)),
                Fill =
                    new RadialGradientBrush(
                        Color.FromRgb(255, 255, 0),
                        Color.FromRgb(255, 0, 255))
            };

            OverlayWindow.Add(_polygon);

            // Create a line
            _line = new Line
            {
                X1 = 100,
                X2 = 300,
                Y1 = 200,
                Y2 = 200,
                Stroke = new SolidColorBrush(Color.FromRgb(0, 255, 0)),
                StrokeThickness = 2
            };

            OverlayWindow.Add(_line);

            // Create an ellipse (circle)
            _ellipse = new Ellipse
            {
                Width = 15,
                Height = 15,
                Margin = new Thickness(300, 300, 0, 0),
                Stroke =
                    new SolidColorBrush(Color.FromRgb(0, 255, 255))
            };

            OverlayWindow.Add(_ellipse);

            // Create a rectangle
            _rectangle = new Rectangle
            {
                RadiusX = 2,
                RadiusY = 2,
                Width = 50,
                Height = 100,
                Margin = new Thickness(400, 400, 0, 0),
                Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0)),
                Fill =
                    new SolidColorBrush(Color.FromArgb(100, 255, 255,
                        255))
            };

            OverlayWindow.Add(_rectangle);
        }
    }
}
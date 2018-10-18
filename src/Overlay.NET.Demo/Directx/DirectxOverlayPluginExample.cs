using System;
using System.Diagnostics;
using System.Drawing;
using Overlay.NET.Common;
using Overlay.NET.Demo.Internals;
using Overlay.NET.Directx;
using Process.NET.Windows;

namespace Overlay.NET.Demo.Directx {
    [RegisterPlugin("DirectXverlayDemo-1", "Jacob Kemple", "DirectXOverlayDemo", "0.0",
        "A basic demo of the DirectXoverlay.")]
    public class DirectxOverlayPluginExample : DirectXOverlayPlugin {
        private readonly TickEngine _tickEngine = new TickEngine();
        public readonly ISettings<DemoOverlaySettings> Settings = new SerializableSettings<DemoOverlaySettings>();
        private int _displayFps;
        private int _font;
        private int _hugeFont;
        private int _i;
        private int _interiorBrush;
        private int _redBrush;
        private int _redOpacityBrush;
        private float _rotation;
        private Stopwatch _watch;

        public override void Initialize(IWindow targetWindow) {
            // Set target window by calling the base method
            base.Initialize(targetWindow);

            // For demo, show how to use settings
            var current = Settings.Current;
            var type = GetType();

            if (current.UpdateRate == 0)
                current.UpdateRate = 1000 / 60;

            current.Author = GetAuthor(type);
            current.Description = GetDescription(type);
            current.Identifier = GetIdentifier(type);
            current.Name = GetName(type);
            current.Version = GetVersion(type);

            // File is made from above info
            Settings.Save();
            Settings.Load();
            Console.Title = @"OverlayExample";

            OverlayWindow = new DirectXOverlayWindow(targetWindow.Handle, false);
            _watch = Stopwatch.StartNew();

            _redBrush = OverlayWindow.Graphics.CreateBrush(0x7FFF0000);
            _redOpacityBrush = OverlayWindow.Graphics.CreateBrush(Color.FromArgb(80, 255, 0, 0));
            _interiorBrush = OverlayWindow.Graphics.CreateBrush(0x7FFFFF00);

            _font = OverlayWindow.Graphics.CreateFont("Arial", 20);
            _hugeFont = OverlayWindow.Graphics.CreateFont("Arial", 50, true);

            _rotation = 0.0f;
            _displayFps = 0;
            _i = 0;
            // Set up update interval and register events for the tick engine.

            _tickEngine.PreTick += OnPreTick;
            _tickEngine.Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e) {
            if (!OverlayWindow.IsVisible) {
                return;
            }

            OverlayWindow.Update();
            InternalRender();
        }

        private void OnPreTick(object sender, EventArgs e) {
            var targetWindowIsActivated = TargetWindow.IsActivated;
            if (!targetWindowIsActivated && OverlayWindow.IsVisible) {
                _watch.Stop();
                ClearScreen();
                OverlayWindow.Hide();
            }
            else if (targetWindowIsActivated && !OverlayWindow.IsVisible) {
                OverlayWindow.Show();
            }
        }

        // ReSharper disable once RedundantOverriddenMember
        public override void Enable() {
            _tickEngine.Interval = Settings.Current.UpdateRate.Milliseconds();
            _tickEngine.IsTicking = true;
            base.Enable();
        }

        // ReSharper disable once RedundantOverriddenMember
        public override void Disable() {
            _tickEngine.IsTicking = false;
            base.Disable();
        }

        public override void Update() => _tickEngine.Pulse();

        protected void InternalRender() {
            if (!_watch.IsRunning) {
                _watch.Start();
            }

            OverlayWindow.Graphics.BeginScene();
            OverlayWindow.Graphics.ClearScene();

            //first row
            OverlayWindow.Graphics.DrawText("DrawBarH", _font, _redBrush, 50, 40);
            OverlayWindow.Graphics.DrawBarH(50, 70, 20, 100, 80, 2, _redBrush, _interiorBrush);

            OverlayWindow.Graphics.DrawText("DrawBarV", _font, _redBrush, 200, 40);
            OverlayWindow.Graphics.DrawBarV(200, 120, 100, 20, 80, 2, _redBrush, _interiorBrush);

            OverlayWindow.Graphics.DrawText("DrawBox2D", _font, _redBrush, 350, 40);
            OverlayWindow.Graphics.DrawBox2D(350, 70, 50, 100, 2, _redBrush, _redOpacityBrush);

            OverlayWindow.Graphics.DrawText("DrawBox3D", _font, _redBrush, 500, 40);
            OverlayWindow.Graphics.DrawBox3D(500, 80, 50, 100, 10, 2, _redBrush, _redOpacityBrush);

            OverlayWindow.Graphics.DrawText("DrawCircle3D", _font, _redBrush, 650, 40);
            OverlayWindow.Graphics.DrawCircle(700, 120, 35, 2, _redBrush);

            OverlayWindow.Graphics.DrawText("DrawEdge", _font, _redBrush, 800, 40);
            OverlayWindow.Graphics.DrawEdge(800, 70, 50, 100, 10, 2, _redBrush);

            OverlayWindow.Graphics.DrawText("DrawLine", _font, _redBrush, 950, 40);
            OverlayWindow.Graphics.DrawLine(950, 70, 1000, 200, 2, _redBrush);

            //second row
            OverlayWindow.Graphics.DrawText("DrawPlus", _font, _redBrush, 50, 250);
            OverlayWindow.Graphics.DrawPlus(70, 300, 15, 2, _redBrush);

            OverlayWindow.Graphics.DrawText("DrawRectangle", _font, _redBrush, 200, 250);
            OverlayWindow.Graphics.DrawRectangle(200, 300, 50, 100, 2, _redBrush);

            OverlayWindow.Graphics.DrawText("DrawRectangle3D", _font, _redBrush, 350, 250);
            OverlayWindow.Graphics.DrawRectangle3D(350, 320, 50, 100, 10, 2, _redBrush);

            OverlayWindow.Graphics.DrawText("FillCircle", _font, _redBrush, 800, 250);
            OverlayWindow.Graphics.FillCircle(850, 350, 50, _redBrush);

            OverlayWindow.Graphics.DrawText("FillRectangle", _font, _redBrush, 950, 250);
            OverlayWindow.Graphics.FillRectangle(950, 300, 50, 100, _redBrush);

            _rotation += 0.03f; //related to speed

            if (_rotation > 50.0f) //size of the swastika
            {
                _rotation = -50.0f;
            }

            if (_watch.ElapsedMilliseconds > 1000) {
                _i = _displayFps;
                _displayFps = 0;
                _watch.Restart();
            }

            else {
                _displayFps++;
            }

            OverlayWindow.Graphics.DrawText("FPS: " + _i, _hugeFont, _redBrush, 400, 600, false);

            OverlayWindow.Graphics.EndScene();
        }

        public override void Dispose() {
            OverlayWindow.Dispose();
            base.Dispose();
        }

        private void ClearScreen() {
            OverlayWindow.Graphics.BeginScene();
            OverlayWindow.Graphics.ClearScene();
            OverlayWindow.Graphics.EndScene();
        }
    }
}
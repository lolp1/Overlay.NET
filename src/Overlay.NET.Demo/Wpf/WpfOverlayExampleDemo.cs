using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Overlay.NET.Common;
using Process.NET;
using Process.NET.Memory;

namespace Overlay.NET.Demo.Wpf {
    /// <summary>
    /// </summary>
    public class WpfOverlayExampleDemo {
        /// <summary>
        ///     The overlay
        /// </summary>
        private static OverlayPlugin _overlay;

        /// <summary>
        ///     The process sharp
        /// </summary>
        private static ProcessSharp _processSharp;

        /// <summary>
        ///     The work
        /// </summary>
        private static bool _work;

        /// <summary>
        ///     Starts the demo.
        /// </summary>
        public void StartDemo() {
            Log.Debug(@"Please type the process name of the window you want to attach to, e.g 'notepad.");
            Log.Debug("Note: If there is more than one process found, the first will be used.");

            // Set up objects/overlay
            var processName = Console.ReadLine();
            var process = System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault();
            if (process == null) {
                Log.Warn($"No process by the name of {processName} was found.");
                Log.Warn("Please open one or use a different name and restart the demo.");
                Console.ReadLine();
                return;
            }

            _processSharp = new ProcessSharp(process, MemoryType.Remote);
            _overlay = new WpfOverlayDemoExample();

            var wpfOverlay = (WpfOverlayDemoExample) _overlay;

            // This is done to focus on the fact the Init method
            // is overriden in the wpf overlay demo in order to set the
            // wpf overlay window instance
            wpfOverlay.Initialize(_processSharp.WindowFactory.MainWindow);
            wpfOverlay.Enable();

            _work = true;

            // Log some info about the overlay.
            Log.Debug("Starting update loop (open the process you specified and drag around)");
            Log.Debug("Update rate: " + wpfOverlay.Settings.Current.UpdateRate.Milliseconds());

            var info = wpfOverlay.Settings.Current;

            Log.Debug($"Author: {info.Author}");
            Log.Debug($"Description: {info.Description}");
            Log.Debug($"Name: {info.Name}");
            Log.Debug($"Identifier: {info.Identifier}");
            Log.Debug($"Version: {info.Version}");

            Log.Info("Note: Settings are saved to a settings folder in your main app folder.");

            Log.Info("Give your window focus to enable the overlay (and unfocus to disable..)");

            Log.Info("Close the console to end the demo.");

            wpfOverlay.OverlayWindow.Draw += OnDraw;

            // Do work
            while (_work) {
                _overlay.Update();
            }

            Log.Debug("Demo complete.");
        }

        private static void OnDraw(object sender, DrawingContext context) {
            // Draw a formatted text string into the DrawingContext.
            context.DrawText(
                new FormattedText("Click Me!", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                    new Typeface("Verdana"), 36, Brushes.BlueViolet), new Point(200, 116));

            context.DrawLine(new Pen(Brushes.Blue, 10), new Point(100, 100), new Point(10, 10));
        }
    }
}
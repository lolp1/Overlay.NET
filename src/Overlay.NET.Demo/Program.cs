using System;
using Overlay.NET.Common;
using Overlay.NET.Demo.Directx;
using Overlay.NET.Demo.Internals;
using Overlay.NET.Demo.Wpf;
using Process.NET;

namespace Overlay.NET.Demo {
    /// <summary>
    /// </summary>
    public static class Program {
        /// <summary>
        ///     Defines the entry point of the application.
        /// </summary>
        [STAThread]
        public static void Main() {
            Log.Register("Console", new ConsoleLog());
            Log.Debug("Enter 1 to run WPF overlay demo");
            Log.Debug("Enter 2 to run DirectX overlay demo");

            var result = Console.ReadLine();
            int oneOrTwo;

            var parsed = int.TryParse(result, out oneOrTwo);

            if (!parsed) {
                Log.Debug("Unable to read your input, make sure it consist of '1' or '2' only and try again");
                return;
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (oneOrTwo) {
                case 1:
                    var wpfDemo = new WpfOverlayExampleDemo();
                    wpfDemo.StartDemo();
                    Log.WriteLine("Demo running..");
                    break;
                case 2:
                    var directXDemo = new DirectXOverlayDemo();
                    directXDemo.StartDemo();
                    Log.WriteLine("Demo running..");
                    break;
            }

            Console.ReadLine();
        }
    }
}
using System;
using System.Linq;
using Overlay.NET.Common;
using Process.NET;
using Process.NET.Memory;

namespace Overlay.NET.Demo.Directx
{
    public class DirectXOverlayDemo
    {
        DirectxOverlayPluginExample _directXoverlayPluginExample;
        ProcessSharp _processSharp;

        public void StartDemo()
        {
            Log.Debug(@"Please type the process name of the window you want to attach to, e.g 'notepad.");
            Log.Debug("Note: If there is more than one process found, the first will be used.");

            var processName = Console.ReadLine();

            var process = System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault();
            if (process == null)
            {
                Log.Warn($"No process by the name of {processName} was found.");
                Log.Warn("Please open one or use a different name and restart the demo.");
                Console.ReadLine();
                return;
            }

            _directXoverlayPluginExample = new DirectxOverlayPluginExample();
            _processSharp = new ProcessSharp(process, MemoryType.Remote);

            _directXoverlayPluginExample.Initialize(_processSharp.WindowFactory.MainWindow);
            _directXoverlayPluginExample.Enable();
            Log.Info("Close the console to end the demo.");

            while (true)
            {
                _directXoverlayPluginExample.Update();
            }

            Log.Info("Close the console to end the demo.");
            Log.Debug("Demo complete.");
        }
    }
}
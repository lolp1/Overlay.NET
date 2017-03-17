using System;
using System.Diagnostics;
using System.IO;

namespace Overlay.NET.Common {
    public sealed class FileLogger : ILogger {
        public string Path { get; set; }

        public bool IsDisposed { get; set; }
        public bool MustBeDisposed { get; set; } = true;

        public void WriteLine(string entry) {
            try {
                _stream.WriteLine(entry);
            }

            catch (Exception e) {
                Trace.WriteLine(e.Message);
            }
        }

        public static readonly FileLogger Instance = new FileLogger();

        private readonly StreamWriter _stream;

        public FileLogger(RelativeFile file) {
            Path = file.GetFullPath();
            _stream = new StreamWriter(Path, true) {AutoFlush = true};
        }

        public FileLogger(string file) {
            Path = file;
            _stream = new StreamWriter(Path, true) {AutoFlush = true};
        }

        private FileLogger() {
            var file = new RelativeFile {
                Name = "AppLog",
                Extension = ".txt",
                SubDirectory = "Logs",
                TimeStampFile = true,
                UseDetailedTimeStamp = false
            };

            Path = file.GetFullPath();
            _stream = new StreamWriter(Path, true) {AutoFlush = true};
        }

        public void Dispose() {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;
            _stream.Flush();
            _stream.Close();
        }
    }
}
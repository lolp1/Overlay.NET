using System;
using System.IO;
using System.Linq;

namespace Overlay.NET.Common {
    /// <summary>
    ///     Class representing a relative file on the device's storage.
    /// </summary>
    public class RelativeFile {
        /// <summary>
        ///     Gets the base directory.
        /// </summary>
        /// <value>
        ///     The base directory.
        /// </value>
        public string Directory {
            get {
                if (string.IsNullOrEmpty(_directory)) {
                    _directory = AppDomain.CurrentDomain.BaseDirectory;
                }

                return _directory;
            }
            set { _directory = value; }
        }

        /// <summary>
        ///     Gets or sets the folder.
        /// </summary>
        /// <value>
        ///     The folder.
        /// </value>
        public string SubDirectory { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the extension.
        /// </summary>
        /// <value>
        ///     The extension.
        /// </value>
        public string Extension { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [use detailed time stamp].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [use detailed time stamp]; otherwise, <c>false</c>.
        /// </value>
        public bool UseDetailedTimeStamp { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [time stamp file].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [time stamp file]; otherwise, <c>false</c>.
        /// </value>
        public bool TimeStampFile { get; set; }

        private string _directory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RelativeFile" /> class.
        /// </summary>
        /// <param name="directory">The base directory.</param>
        public RelativeFile(string directory) {
            _directory = directory;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RelativeFile" /> class.
        /// </summary>
        public RelativeFile() : this(AppDomain.CurrentDomain.BaseDirectory) {}

        /// <summary>
        ///     Creates the a text file in a your app domains base directory, or if a subdirectory is set, in a sub directory
        ///     relative to your app folder.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="relativeSubDirectoryName">The name of the relative sub directory, if any.</param>
        /// <param name="timeStampFile">if set to <c>true</c> [the file will be time stamped]</param>
        /// <param name="useDetailedTimeStamp">if set to <c>true</c> [the time stamp will be more detailed, if one is used].</param>
        /// <returns>
        ///     <see cref="RelativeFile" />
        /// </returns>
        public static RelativeFile JsonFile(string fileName, string relativeSubDirectoryName = "",
            bool timeStampFile = false, bool useDetailedTimeStamp = true) => new RelativeFile {
            Name = fileName,
            Extension = ".json",
            SubDirectory = relativeSubDirectoryName,
            TimeStampFile = timeStampFile,
            UseDetailedTimeStamp = useDetailedTimeStamp
        };

        /// <summary>
        ///     Creates the a text file in a your app domains base directory, or if a subdirectory is set, in a sub directory
        ///     relative to your app folder.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="relativeSubDirectoryName">The name of the relative sub directory, if any.</param>
        /// <param name="timeStampFile">if set to <c>true</c> [the file will be time stamped]</param>
        /// <param name="useDetailedTimeStamp">if set to <c>true</c> [the time stamp will be more detailed, if one is used].</param>
        /// <returns>
        ///     <see cref="RelativeFile" />
        /// </returns>
        public static RelativeFile XmlFile(string fileName, string relativeSubDirectoryName = "",
            bool timeStampFile = false, bool useDetailedTimeStamp = true) => new RelativeFile {
            Name = fileName,
            Extension = ".xml",
            SubDirectory = relativeSubDirectoryName,
            TimeStampFile = timeStampFile,
            UseDetailedTimeStamp = useDetailedTimeStamp
        };

        /// <summary>
        ///     Gets the full path.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">@The file detail can not be blank or null</exception>
        public string GetFullPath() {
            var checks = new[] {
                Directory, Name, Extension
            };

            foreach (var fileDetail in checks.Where(string.IsNullOrEmpty)) {
                throw new ArgumentException($@"The file detail can not be blank or null {fileDetail}", fileDetail);
            }

            if (!Extension.StartsWith(".")) {
                Extension = "." + Extension;
            }

            if (TimeStampFile) {
                Name = UseDetailedTimeStamp
                    ? $"{Name}-{DateTime.Now:yyyy-MM-dd_hh-mm-ss}"
                    : $"{Name}-{DateTime.Now:yyyy-MM-dd}";
            }

            var path = Directory;

            var combineDirectory = !string.IsNullOrEmpty(SubDirectory);

            if (combineDirectory) {
                path = Path.Combine(Directory, SubDirectory);
            }

            if (!System.IO.Directory.Exists(path)) {
                System.IO.Directory.CreateDirectory(path);
            }

            return combineDirectory
                ? Path.Combine(Directory, SubDirectory, Name + Extension)
                : Path.Combine(Directory, Name + Extension);
        }
    }
}
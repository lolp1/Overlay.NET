using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Overlay.NET.Common {
    public class SerializableSettings<T> : ISettings<T> where T : new() {
        /// <summary>
        ///     Gets or sets the file.
        /// </summary>
        /// <value>
        ///     The file.
        /// </value>
        public RelativeFile SettingsFile {
            get { return _file ?? (_file = RelativeFile.XmlFile(typeof(T).Name, "Settings")); }
            set { _file = value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [backup on save].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [backup on save]; otherwise, <c>false</c>.
        /// </value>
        public bool BackupOnSave { get; set; } = true;

        /// <summary>
        ///     Gets or sets the path.
        /// </summary>
        /// <value>
        ///     The path.
        /// </value>
        public string Path {
            get {
                if (string.IsNullOrEmpty(_path)) {
                    _path = SettingsFile.GetFullPath();
                }

                return _path;
            }
            protected set { _path = value; }
        }

        /// <summary>
        ///     Gets or sets the name of the settings.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name {
            get {
                if (string.IsNullOrEmpty(_name)) {
                    _name = typeof(T).Name;
                }

                return _name;
            }
            set { _name = value; }
        }

        /// <summary>
        ///     Gets or sets the encoding.
        /// </summary>
        /// <value>
        ///     The encoding.
        /// </value>
        public Encoding Encoding {
            get { return _encoding ?? (_encoding = Encoding.UTF8); }
            set { _encoding = value; }
        }

        /// <summary>
        ///     Gets or sets the current settings instance.
        /// </summary>
        /// <value>
        ///     The current.
        /// </value>
        public T Current {
            get {
                if (_currentExist) {
                    return _current;
                }
                _current = new T();
                _currentExist = true;

                return _current;
            }

            set {
                _current = value;
                _currentExist = true;
            }
        }

        /// <summary>
        ///     Gets or sets the serializer.
        /// </summary>
        /// <value>
        ///     The serializer.
        /// </value>
        protected ISerializer Serializer {
            get {
                if (_serializer != null) {
                    return _serializer;
                }
                _serializer = new XmlSerializerEx();
                SettingsFile.Extension = ".xml";
                SettingsFile.SubDirectory = "XmlSettings";

                return _serializer;
            }
            set { _serializer = value; }
        }

        /// <summary>
        ///     Saves the settings to the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        public void Save(string file = "") {
            if (string.IsNullOrEmpty(file)) {
                file = Path;
            }

            if (File.Exists(file) && BackupOnSave) {
                BackupSettings(file);
            }

            Serializer.ExportToFile(Current, file, Encoding.UTF8);
        }

        /// <summary>
        ///     Loads the settings from the specified location.
        /// </summary>
        /// <param name="location">
        ///     The location of the file./param>
        ///     <exception cref="System.ArgumentNullException"></exception>
        /// </param>
        public void Load(string location = "") {
            if (string.IsNullOrEmpty(location)) {
                location = Path;
            }

            if (!File.Exists(location)) {
                Save(location);
            }

            var settings = Serializer.ImportFromFile<T>(location, Encoding);

            if (settings == null) {
                throw new ArgumentNullException(nameof(settings));
            }

            Current = settings;

            RaiseOnSettingsChanged(location);
        }

        private T _current;
        private bool _currentExist;
        private Encoding _encoding;
        private RelativeFile _file;
        private string _name;
        private string _path;
        private ISerializer _serializer;

        /// <summary>
        ///     Backups the settings if one already exist.
        /// </summary>
        /// <param name="target">The target to back up settings to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected void BackupSettings(string target = "") {
            if (string.IsNullOrEmpty(target)) {
                target = Path;
            }

            var extension = System.IO.Path.GetExtension(target);

            if (extension == null) {
                throw new ArgumentNullException(Path + " does not contain an extension.");
            }

            var time = DateTime.Now;

            var backup = target.Replace(extension,
                $"-{time.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture)}{extension}");

            Serializer.ExportToFile(Current, backup, Encoding);
        }

        /// <summary>
        ///     Occurs when [on settings changed].
        /// </summary>
        public event EventHandler<string> OnSettingsChanged;

        /// <summary>
        ///     Raises the on settings changed.
        /// </summary>
        /// <param name="e">The e.</param>
        protected virtual void RaiseOnSettingsChanged(string e) => OnSettingsChanged?.Invoke(this, e);
    }
}
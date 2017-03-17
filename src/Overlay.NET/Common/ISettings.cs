namespace Overlay.NET.Common {
    /// <summary>
    ///     Interface that defines a way to load and save settings.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISettings<out T> {
        /// <summary>
        ///     Gets the current settings instance
        /// </summary>
        /// <value>
        ///     The current settings.
        /// </value>
        T Current { get; }

        /// <summary>
        ///     Saves the settings to the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        void Save(string target = "");

        /// <summary>
        ///     Loads the settings from the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        void Load(string location = "");
    }
}
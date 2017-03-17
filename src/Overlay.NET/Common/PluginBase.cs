using System;
using System.Linq;

namespace Overlay.NET.Common {
    /// <summary>
    ///     The abstract base class for plugins.
    /// </summary>
    public abstract class PluginBase {
        /// <summary>
        ///     Gets the plugin's Author, or empty string if it fails.
        /// </summary>
        /// <param name="type">The plugin's Type.</param>
        /// <returns>The plugin's Author, or empty string if it fails.</returns>
        public static string GetAuthor(Type type) {
            var attribute = GetAttribute(type);
            return attribute == null ? string.Empty : attribute.Author ?? string.Empty;
        }

        /// <summary>
        ///     Gets the plugin's Description, or empty string if it fails.
        /// </summary>
        /// <param name="type">The plugin's Type.</param>
        /// <returns>The plugin's Description, or empty string if it fails.</returns>
        public static string GetDescription(Type type) {
            var attribute = GetAttribute(type);
            return attribute == null ? string.Empty : attribute.Description ?? string.Empty;
        }

        /// <summary>
        ///     Gets the identifier for the plugin, or empty string if it fails.
        /// </summary>
        /// <param name="type">The plugin's Type.</param>
        /// <returns>The identifier for the plugin, or empty string if it fails.</returns>
        public static string GetIdentifier(Type type) {
            var attribute = GetAttribute(type);
            return attribute == null ? string.Empty : attribute.Identifier ?? string.Empty;
        }

        /// <summary>
        ///     Gets the plugin's Name, or empty string if it fails.
        /// </summary>
        /// <param name="type">The plugin's Type.</param>
        /// <returns>The plugin's Name, or empty string if it fails.</returns>
        public static string GetName(Type type) {
            var attribute = GetAttribute(type);
            return attribute == null ? string.Empty : attribute.Name ?? string.Empty;
        }

        /// <summary>
        ///     Gets the plugin's Version, or empty string if it fails.
        /// </summary>
        /// <param name="type">The plugin's Type.</param>
        /// <returns>The plugin's Version, or empty string if it fails.</returns>
        public static string GetVersion(Type type) {
            var attribute = GetAttribute(type);
            return attribute == null ? string.Empty : attribute.Version ?? string.Empty;
        }

        /// <summary>
        ///     Gets whether the Type is a registered plugin or not.
        /// </summary>
        /// <param name="type">The plugin's Type.</param>
        /// <returns>Whether the Type is a registered plugin or not.</returns>
        public static bool IsRegisteredPlugin(Type type) => GetAttribute(type) != null;

        /// <summary>
        ///     Gets the plugin's RegisteredPluginAttribute, or null if it fails.
        /// </summary>
        /// <param name="type">The plugin's Type.</param>
        /// <returns>The plugin's RegisteredPluginAttribute, or null if it fails.</returns>
        private static RegisterPluginAttribute GetAttribute(Type type) => (RegisterPluginAttribute)
            type.GetCustomAttributes(typeof(RegisterPluginAttribute), false).FirstOrDefault();

        /// <summary>
        ///     Marks a PluginBase derived class as to be loaded and contains some optional information about the plugin.
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, Inherited = false)]
        protected sealed class RegisterPluginAttribute : Attribute {
            /// <summary>
            ///     Gets the plugin's Author.
            /// </summary>
            public string Author { get; }

            /// <summary>
            ///     Gets the plugin's Description.
            /// </summary>
            public string Description { get; }

            /// <summary>
            ///     Gets an identifier for the plugin.
            /// </summary>
            public string Identifier { get; }

            /// <summary>
            ///     Gets the plugin's Name.
            /// </summary>
            public string Name { get; }

            /// <summary>
            ///     Gets the plugin's Version.
            /// </summary>
            public string Version { get; }

            /// <summary>
            ///     Creates a new instance of the <see cref="SharpPlugins.PluginBase.RegisterPluginAttribute" /> class
            ///     with the identifier, given optional information about the plugin to mark it as to be loaded.
            /// </summary>
            /// <param name="identifier">The identifier for the plugin.</param>
            /// <param name="author">The plugin's Author.</param>
            /// <param name="name">The plugin's Name.</param>
            /// <param name="version">The plugin's Version.</param>
            /// <param name="description">The plugin's Description.</param>
            public RegisterPluginAttribute(string identifier, string author = "", string name = "", string version = "",
                string description = "") {
                Identifier = identifier;
                Author = author;
                Name = name;
                Version = version;
                Description = description;
            }
        }
    }
}
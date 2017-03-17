using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Overlay.NET.Common {
    /// <summary>
    ///     Contains static methods for loading plugin types from .dll files.
    /// </summary>
    public static class PluginLoader {
        /// <summary>
        ///     Creates instances of all the given types that are instanciable plugins.
        /// </summary>
        /// <typeparam name="TPlugin">The PluginBase derived plugin type.</typeparam>
        /// <param name="types">The types to create instances from.</param>
        /// <returns>Instances of all the types that are instanciable plugins.</returns>
        public static IEnumerable<TPlugin> InstanciatePlugins<TPlugin>(IEnumerable<Type> types)
            where TPlugin : PluginBase => InstanciatePlugins<TPlugin>(types, new object[0]);

        /// <summary>
        ///     Creates instances of all the given Types that are instanciable plugins using the given arguments for the
        ///     constructor.
        /// </summary>
        /// <typeparam name="TPlugin">The PluginBase derived plugin type.</typeparam>
        /// <param name="types">The Types to create instances from.</param>
        /// <param name="args">The arguments for the constructor.</param>
        /// <returns>Instances of all the types that are instanciable plugins.</returns>
        public static IEnumerable<TPlugin> InstanciatePlugins<TPlugin>(IEnumerable<Type> types, params object[] args)
            where TPlugin : PluginBase => from type in types
                                          where IsInstanciablePlugin(type)
                                          select (TPlugin) Activator.CreateInstance(type, args);

        /// <summary>
        ///     Determines whether the given Type is an instanciable plugin or not.
        /// </summary>
        /// <param name="type">The Type to check.</param>
        /// <returns>Whether the given Type is an instanciable plugin or not.</returns>
        public static bool IsInstanciablePlugin(Type type) => PluginBase.IsRegisteredPlugin(type) && !type.IsAbstract;

        /// <summary>
        ///     Takes file paths and checks if each exists, and loads .dll files to find the instanciable plugin types deriving
        ///     from TPlugin.
        /// </summary>
        /// <typeparam name="TPlugin">The Type that the plugins have to be derived from.</typeparam>
        /// <param name="pluginFiles">The paths to the .dll files to check.</param>
        /// <returns>The Types of the found instanciable plugins.</returns>
        public static IEnumerable<Type> LoadPluginsFromFiles<TPlugin>(IEnumerable<string> pluginFiles)
            where TPlugin : PluginBase => from pluginFile in pluginFiles
                                          let extension = Path.GetExtension(pluginFile)
                                          where
                                          extension != null && File.Exists(pluginFile) &&
                                          extension.Equals(".dll", StringComparison.InvariantCultureIgnoreCase)
                                          let assembly = Assembly.LoadFrom(pluginFile)
                                          from type in assembly.GetExportedTypes()
                                          where typeof(TPlugin).IsAssignableFrom(type) && IsInstanciablePlugin(type)
                                          select type;

        /// <summary>
        ///     Takes directory paths and checks if each exists, and loads .dll files, including from subfolders,
        ///     to find the instanciable plugin types deriving from TPlugin.
        /// </summary>
        /// <typeparam name="TPlugin">The Type that the plugins have to be derived from.</typeparam>
        /// <param name="pluginFolders">The paths to the folders to check.</param>
        /// <returns>The Types of the found instanciable plugins.</returns>
        public static IEnumerable<Type> LoadPluginsFromFolders<TPlugin>(IEnumerable<string> pluginFolders)
            where TPlugin : PluginBase => from pluginFolder in pluginFolders
                                          where Directory.Exists(pluginFolder)
                                          let files = Directory.EnumerateFiles(pluginFolder, "*.dll", SearchOption.AllDirectories)
                                          let types = LoadPluginsFromFiles<TPlugin>(files)
                                          from type in types
                                          select type;
    }
}
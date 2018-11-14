namespace CodeChange.Toolkit.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Provides methods for loading plug-in assemblies in the AppDomain
    /// </summary>
    public class PluginAssemblyLoader
    {
        /// <summary>
        /// Defines the name of the plug-ins folder
        /// </summary>
        public const string PluginsFolderName = "Plugins";

        /// <summary>
        /// Loads all plug-ins into the current AppDomain
        /// </summary>
        /// <returns>A collection of plug-ins that were loaded</returns>
        public virtual IEnumerable<Assembly> LoadPluginAssemblies()
        {
            var path = GetPluginPath();

            if (Directory.Exists(path))
            {
                var loader = new GenericPluginLoader<IPlugin>();
                var assemblies = loader.LoadAssemblies(path);

                return assemblies;
            }
            else
            {
                return new List<Assembly>();
            }
        }

        /// <summary>
        /// Gets the path to the plug-ins directory
        /// </summary>
        /// <returns>The path to the plug-in directory</returns>
        private string GetPluginPath()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var location = assembly.GetName().CodeBase;
            var path = Path.GetDirectoryName(location);

            path += "\\" + PluginAssemblyLoader.PluginsFolderName;

            if (path.StartsWith("file:\\"))
            {
                path = path.Replace("file:\\", String.Empty);
            }

            return path;
        }
    }
}

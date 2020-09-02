namespace CodeChange.Toolkit.Plugins
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a contract for a repository that manages a collection of plug-ins
    /// </summary>
    public interface IPluginRepository
    {
        /// <summary>
        /// Determines if a plug-in exists with the name specified
        /// </summary>
        /// <param name="pluginName">The plug-in name</param>
        /// <returns>True, if the plug-in exists; otherwise false</returns>
        bool PluginExists(string pluginName);

        /// <summary>
        /// Gets a plug-in matching the name specified
        /// </summary>
        /// <param name="pluginName">The plug-in name</param>
        /// <returns>The matching plug-in</returns>
        IPlugin GetPlugin(string pluginName);

        /// <summary>
        /// Gets a plug-in of a specific type matching the name specified
        /// </summary>
        /// <typeparam name="T">The plug-in type</typeparam>
        /// <param name="pluginName">The plug-in name</param>
        /// <returns>The matching plug-in</returns>
        T GetPlugin<T>(string pluginName) where T : IPlugin;

        /// <summary>
        /// Gets a collection of all plug-ins in the repository
        /// </summary>
        /// <returns>A collection of matching plug-ins</returns>
        IEnumerable<IPlugin> GetAllPlugins();

        /// <summary>
        /// Gets a collection of plug-ins of the type specified in the repository
        /// </summary>
        /// <typeparam name="T">The plug-in type</typeparam>
        /// <returns>A collection of matching plug-ins</returns>
        IEnumerable<T> GetPlugins<T>() where T : IPlugin;
    }
}

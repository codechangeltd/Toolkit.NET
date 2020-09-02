namespace CodeChange.Toolkit.Domain.Plugins
{
    using CodeChange.Toolkit.Plugins;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a contract for a repository that manages installed plug-ins
    /// </summary>
    public interface IInstalledPluginRepository
    {
        /// <summary>
        /// Adds a installed plug-in to the repository
        /// </summary>
        /// <param name="plugin">The installed plug-in to add</param>
        void AddInstallation(InstalledPlugin plugin);

        /// <summary>
        /// Determines if a plug-in has already been installed
        /// </summary>
        /// <param name="pluginName">The plug-in name</param>
        /// <returns>True, if the plug-in has already been installed; otherwise false</returns>
        bool IsPluginInstalled(string pluginName);

        /// <summary>
        /// Gets a single installed plug-in
        /// </summary>
        /// <param name="pluginName">The name of the plug-in</param>
        /// <returns>The matching installed plug-in</returns>
        InstalledPlugin GetInstallation(string pluginName);

        /// <summary>
        /// Gets a collection of all installed plug-ins
        /// </summary>
        /// <returns>A collection of matching installed plug-ins</returns>
        IEnumerable<InstalledPlugin> GetAllInstallations();

        /// <summary>
        /// Gets a collection of installed plug-ins for the type specified
        /// </summary>
        /// <typeparam name="T">The plug-in type</typeparam>
        /// <returns>A collection of matching installed plug-ins</returns>
        IEnumerable<InstalledPlugin> GetInstallations<T>() where T : IPlugin;

        /// <summary>
        /// Gets a collection of enabled installed plug-ins
        /// </summary>
        /// <returns>A collection of matching installed plug-ins</returns>
        IEnumerable<InstalledPlugin> GetEnabledInstallations();

        /// <summary>
        /// Gets a collection of disabled installed plug-ins
        /// </summary>
        /// <returns>A collection of matching installed plug-ins</returns>
        IEnumerable<InstalledPlugin> GetDisabledInstallations();

        /// <summary>
        /// Updates a single installed plug-in
        /// </summary>
        /// <param name="plugin">The installed plug-in to update</param>
        void UpdateInstallation(InstalledPlugin plugin);

        /// <summary>
        /// Removes a single installed plug-in from the repository
        /// </summary>
        /// <param name="pluginName">The plug-in name</param>
        void RemoveInstallation(string pluginName);
    }
}

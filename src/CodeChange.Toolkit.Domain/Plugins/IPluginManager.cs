namespace CodeChange.Toolkit.Domain.Plugins
{
    using CodeChange.Toolkit.Plugins;

    /// <summary>
    /// Defines a contract for a domain service that manages plug-in installations
    /// </summary>
    public interface IPluginManager
    {
        /// <summary>
        /// Automatically installs any new plug-ins that are found
        /// </summary>
        void AutoInstallPlugins();
        
        /// <summary>
        /// Initialises the plug-in specified
        /// </summary>
        /// <param name="pluginName">The plug-in name</param>
        void InitialisePlugin
        (
            string pluginName
        );

        /// <summary>
        /// Initialises all plug-ins
        /// </summary>
        void InitialiseAllPlugins();

        /// <summary>
        /// Initialises all plug-ins of the type specified
        /// </summary>
        /// <typeparam name="T">The plug-in type</typeparam>
        void InitialisePlugins<T>()
            where T: IPlugin;

        /// <summary>
        /// Disables the plug-in specified
        /// </summary>
        /// <param name="pluginName">The plug-in name</param>
        void DisablePlugin
        (
            string pluginName
        );

        /// <summary>
        /// Enables the plug-in specified
        /// </summary>
        /// <param name="pluginName">The plug-in name</param>
        void EnablePlugin
        (
            string pluginName
        );

        /// <summary>
        /// Uninstalls the plug-in specified
        /// </summary>
        /// <param name="pluginName">The plug-in name</param>
        void UninstallPlugin
        (
            string pluginName
        );
    }
}

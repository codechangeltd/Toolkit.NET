namespace CodeChange.Toolkit.Plugins
{
    using System;

    /// <summary>
    /// Defines a contract a single plug-in
    /// </summary>
    public interface IPlugin : IDisposable
    {
        /// <summary>
        /// Gets the name of the plug-in
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the plug-in
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the name of the plug-ins author
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Gets the plug-in version number
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Installs the plug-in ready for first time use
        /// </summary>
        void Install();

        /// <summary>
        /// Upgrades the plug-in from the version specified to the latest version
        /// </summary>
        /// <param name="fromVersion">The version to upgrade from</param>
        void Upgrade
        (
            Version fromVersion
        );

        /// <summary>
        /// Uninstalls the plug-in and cleans up any redundant data
        /// </summary>
        void Uninstall();

        /// <summary>
        /// Initialises the plug-in
        /// </summary>
        void Initialise();
    }
}

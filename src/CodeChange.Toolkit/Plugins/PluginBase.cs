namespace CodeChange.Toolkit.Plugins
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// Represents a base class for all plug-in implementations
    /// </summary>
    public abstract class PluginBase : IPlugin
    {
        private FileVersionInfo _versionInfo;

        /// <summary>
        /// Gets a default name for the plug-in
        /// </summary>
        public virtual string Name
        {
            get
            {
                return this.GetType().Name.Replace
                (
                    "Plugin",
                    String.Empty
                );
            }
        }

        /// <summary>
        /// Gets the description of the plug-in
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the name of the plug-ins author
        /// </summary>
        public virtual string Author
        {
            get
            {
                if (_versionInfo == null)
                {
                    _versionInfo = FileVersionInfo.GetVersionInfo
                    (
                        Assembly.GetExecutingAssembly().Location
                    );
                }

                return _versionInfo.CompanyName;
            }
        }

        /// <summary>
        /// Gets the plug-in version number
        /// </summary>
        public virtual Version Version
        {
            get
            {
                var assembly = this.GetType().Assembly;
                
                var fvi = FileVersionInfo.GetVersionInfo
                (
                    assembly.Location
                );
                
                return new Version
                (
                    fvi.FileVersion
                );
            }
        }

        /// <summary>
        /// Installs the plug-in ready for first time use
        /// </summary>
        public virtual void Install() { }

        /// <summary>
        /// Upgrades the plug-in from the version specified to the latest version
        /// </summary>
        /// <param name="fromVersion">The version to upgrade from</param>
        public virtual void Upgrade
            (
                Version fromVersion
            )
        {
            Validate.IsNotNull(fromVersion);

            if (this.Version.Equals(fromVersion))
            {
                throw new InvalidOperationException
                (
                    "The plug-in cannot be upgraded to the same version."
                );
            }

            PerformUpgrade(fromVersion);
        }

        /// <summary>
        /// Performs the upgrade from the version specified to the current version
        /// </summary>
        /// <param name="fromVersion">The version to upgrade from</param>
        /// <remarks>
        /// The default implementation runs the Uninstall() and then Install() methods.
        /// Some plug-ins may require custom upgrade logic.
        /// </remarks>
        protected virtual void PerformUpgrade
            (
                Version fromVersion
            )
        {
            Validate.IsNotNull(fromVersion);

            Uninstall();
            Install();
        }

        /// <summary>
        /// Uninstalls the plug-in and cleans up any redundant data
        /// </summary>
        public virtual void Uninstall() { }

        /// <summary>
        /// Initialises the plug-in
        /// </summary>
        public virtual void Initialise() { }

        /// <summary>
        /// Disposes the plug-in by 
        /// </summary>
        public virtual void Dispose() { }
    }
}

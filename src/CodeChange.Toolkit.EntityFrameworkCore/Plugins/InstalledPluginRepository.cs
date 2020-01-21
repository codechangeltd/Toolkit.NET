namespace CodeChange.Toolkit.EntityFrameworkCore.Plugins
{
    using CodeChange.Toolkit.Domain.Aggregate;
    using CodeChange.Toolkit.Domain.Plugins;
    using CodeChange.Toolkit.Plugins;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents an Entity Framework installed plugins repository
    /// </summary>
    public sealed class InstalledPluginRepository
        : RepositoryBase<InstalledPlugin>, IInstalledPluginRepository
    {
        /// <summary>
        /// Constructs the repository with a database context instance
        /// </summary>
        /// <param name="context">The database context instance</param>
        public InstalledPluginRepository
            (
                DbContext context
            )
            : base(context)
        { }

        /// <summary>
        /// Determines if a plug-in has already been installed
        /// </summary>
        /// <param name="pluginName">The plug-in name</param>
        /// <returns>True, if the plug-in has already been installed; otherwise false</returns>
        public bool IsPluginInstalled
            (
                string pluginName
            )
        {
            Validate.IsNotEmpty(pluginName);

            return this.GetAll().Any
            (
                m => m.PluginName == pluginName
            );
        }

        /// <summary>
        /// Adds a installed plug-in to the repository
        /// </summary>
        /// <param name="plugin">The installed plug-in to add</param>
        public void AddInstallation
            (
                InstalledPlugin plugin
            )
        {
            Validate.IsNotNull(plugin);

            var installed = IsPluginInstalled
            (
                plugin.PluginName
            );

            if (installed)
            {
                var name = plugin.PluginName;

                throw new InvalidOperationException
                (
                    $"A plug-in named '{name}' has already been installed."
                );
            }

            this.AddEntity(plugin);
        }

        /// <summary>
        /// Gets a single installed plug-in from the repository
        /// </summary>
        /// <param name="pluginName">The name of the plug-in</param>
        /// <returns>The matching installed plug-in</returns>
        public InstalledPlugin GetInstallation
            (
                string pluginName
            )
        {
            Validate.IsNotEmpty(pluginName);

            var plugin = this.GetAll().FirstOrDefault
            (
                m => m.PluginName == pluginName
            );

            if (plugin == null)
            {
                throw new EntityNotFoundException
                (
                    pluginName,
                    $"No installed plug-in was found with the name specified."
                );
            }

            return plugin;
        }

        /// <summary>
        /// Gets a collection of all installed plug-ins
        /// </summary>
        /// <returns>A collection of matching installed plug-ins</returns>
        public IEnumerable<InstalledPlugin> GetAllInstallations()
        {
            return this.GetAll().OrderBy
            (
                a => a.PluginName
            );
        }

        /// <summary>
        /// Gets a collection of installed plug-ins for the type specified
        /// </summary>
        /// <typeparam name="T">The plug-in type</typeparam>
        /// <returns>A collection of matching installed plug-ins</returns>
        public IEnumerable<InstalledPlugin> GetInstallations<T>()
            where T : IPlugin
        {
            var typeName = typeof(T).Name;

            var query = this.GetAll().Where
            (
                m => m.PluginInterfaceTypeName == typeName
            );

            return query.OrderBy(a => a.PluginName);
        }

        /// <summary>
        /// Gets a collection of enabled installed plug-ins
        /// </summary>
        /// <returns>A collection of matching installed plug-ins</returns>
        public IEnumerable<InstalledPlugin> GetEnabledInstallations()
        {
            var query = this.GetAll().Where
            (
                m => false == m.Disabled
            );

            return query.OrderBy(a => a.PluginName);
        }

        /// <summary>
        /// Gets a collection of disabled installed plug-ins
        /// </summary>
        /// <returns>A collection of matching installed plug-ins</returns>
        public IEnumerable<InstalledPlugin> GetDisabledInstallations()
        {
            var plugins = this.GetAll().Where
            (
                m => m.Disabled
            );

            return plugins.OrderBy(a => a.PluginName);
        }

        /// <summary>
        /// Updates a single installed plug-in
        /// </summary>
        /// <param name="plugin">The installed plug-in to update</param>
        public void UpdateInstallation
            (
                InstalledPlugin plugin
            )
        {
            this.UpdateEntity(plugin);
        }

        /// <summary>
        /// Removes a single installed plug-in from the repository
        /// </summary>
        /// <param name="pluginName">The plug-in name</param>
        public void RemoveInstallation
            (
                string pluginName
            )
        {
            var plugin = GetInstallation(pluginName);

            this.RemoveEntity(plugin);
        }
    }
}

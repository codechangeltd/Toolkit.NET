namespace CodeChange.Toolkit.EntityFrameworkCore.Plugins
{
    using CodeChange.Toolkit.Domain.Aggregate;
    using CodeChange.Toolkit.Domain.Plugins;
    using CodeChange.Toolkit.Plugins;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class InstalledPluginRepository : RepositoryBase<InstalledPlugin>, IInstalledPluginRepository
    {
        public InstalledPluginRepository(DbContext context)
            : base(context)
        { }

        public bool IsPluginInstalled(string pluginName)
        {
            Validate.IsNotEmpty(pluginName);

            return GetAll().Any(_ => _.PluginName.Equals(pluginName, StringComparison.OrdinalIgnoreCase));
        }

        public void AddInstallation(InstalledPlugin plugin)
        {
            Validate.IsNotNull(plugin);

            var installed = IsPluginInstalled(plugin.PluginName);

            if (installed)
            {
                var name = plugin.PluginName;

                throw new InvalidOperationException
                (
                    $"A plug-in named '{name}' has already been installed."
                );
            }

            AddEntity(plugin);
        }

        public InstalledPlugin GetInstallation(string pluginName)
        {
            Validate.IsNotEmpty(pluginName);

            var plugin = GetAll()
                .FirstOrDefault(_ => _.PluginName.Equals(pluginName, StringComparison.OrdinalIgnoreCase));

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

        public IEnumerable<InstalledPlugin> GetAllInstallations()
        {
            return GetAll().OrderBy(_ => _.PluginName);
        }

        public IEnumerable<InstalledPlugin> GetInstallations<T>() where T : IPlugin
        {
            var typeName = typeof(T).Name;

            return GetAll()
                .Where(_ => _.PluginInterfaceTypeName == typeName)
                .OrderBy(_ => _.PluginName);
        }

        public IEnumerable<InstalledPlugin> GetEnabledInstallations()
        {
            return GetAll().Where(_ => false == _.Disabled).OrderBy(_ => _.PluginName);
        }

        public IEnumerable<InstalledPlugin> GetDisabledInstallations()
        {
            return GetAll().Where(_ => _.Disabled).OrderBy(_ => _.PluginName);
        }

        public void UpdateInstallation(InstalledPlugin plugin)
        {
            var result = UpdateEntity(plugin);

            if (result.IsFailure)
            {
                throw new InvalidOperationException("A duplicate plugin key was found.");
            }
        }

        public void RemoveInstallation(string pluginName)
        {
            var plugin = GetInstallation(pluginName);

            RemoveEntity(plugin);
        }
    }
}

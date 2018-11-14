namespace CodeChange.Toolkit.Domain.Plugins
{
    using CodeChange.Toolkit.Plugins;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a standard implementation of a plugin registrar
    /// </summary>
    public class PluginManager : IPluginManager
    {
        private IPluginRepository _pluginRepository;
        private IInstalledPluginRepository _installationRepository;

        /// <summary>
        /// Constructs the service with required dependencies
        /// </summary>
        /// <param name="pluginRepository">The plug-in repository</param>
        /// <param name="installationRepository">The installed plug-in repository</param>
        public PluginManager
            (
                IPluginRepository pluginRepository,
                IInstalledPluginRepository installationRepository
            )
        {
            Validate.IsNotNull(pluginRepository);
            Validate.IsNotNull(installationRepository);

            _pluginRepository = pluginRepository;
            _installationRepository = installationRepository;
        }

        /// <summary>
        /// Automatically installs any new plug-ins that are found
        /// </summary>
        public void AutoInstallPlugins()
        {
            var namesOfPluginsInstalled = new List<string>();
            var allPlugins = _pluginRepository.GetAllPlugins();

            foreach (var plugin in allPlugins.ToList())
            {
                var pluginName = plugin.Name;

                var nameFound = namesOfPluginsInstalled.Contains
                (
                    pluginName
                );

                if (false == nameFound)
                {
                    var isRegistered = _installationRepository.IsPluginInstalled
                    (
                        pluginName
                    );

                    if (isRegistered)
                    {
                        var installation = _installationRepository.GetInstallation
                        (
                            pluginName
                        );

                        if (installation.CompareVersions(plugin) != 0)
                        {
                            var fromVersion = new Version
                            (
                                installation.PluginVersion
                            );

                            plugin.Upgrade(fromVersion);
                            installation.UpdatePluginDetails(plugin);

                            _installationRepository.UpdateInstallation
                            (
                                installation
                            );
                        }
                    }
                    else
                    {
                        var installedPlugin = InstalledPlugin.CreateInstalledPlugin
                        (
                            plugin
                        );

                        plugin.Install();

                        _installationRepository.AddInstallation
                        (
                            installedPlugin
                        );

                        namesOfPluginsInstalled.Add
                        (
                            pluginName
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Initialises the plug-in specified
        /// </summary>
        /// <param name="pluginName">The plug-in name</param>
        public void InitialisePlugin
            (
                string pluginName
            )
        {
            Validate.IsNotEmpty(pluginName);

            var isRegistered = _installationRepository.IsPluginInstalled
            (
                pluginName
            );

            if (false == isRegistered)
            {
                throw new InvalidOperationException
                (
                    $"The plug-in '{pluginName}' has not been registered."
                );
            }

            var installation = _installationRepository.GetInstallation
            (
                pluginName
            );

            if (installation.Disabled)
            {
                throw new InvalidOperationException
                (
                    $"The plug-in '{pluginName}' is disabled and cannot be initialised."
                );
            }

            var plugin = _pluginRepository.GetPlugin
            (
                pluginName
            );

            plugin.Initialise();
        }

        /// <summary>
        /// Initialises all plug-ins
        /// </summary>
        public void InitialiseAllPlugins()
        {
            var plugins = _pluginRepository.GetAllPlugins();

            InitialisePlugins(plugins);
        }

        /// <summary>
        /// Initialises all plug-ins of the type specified
        /// </summary>
        /// <typeparam name="T">The plug-in type</typeparam>
        public void InitialisePlugins<T>()
            where T : IPlugin
        {
            var plugins = _pluginRepository.GetPlugins<T>();

            InitialisePlugins
            (
                (IEnumerable<IPlugin>)plugins
            );
        }

        /// <summary>
        /// Initialises the plug-in collection specified
        /// </summary>
        /// <param name="plugins">The plug-ins to initialise</param>
        private void InitialisePlugins
            (
                IEnumerable<IPlugin> plugins
            )
        {
            Validate.IsNotNull(plugins);

            foreach (var plugin in plugins.ToList())
            {
                var installation = _installationRepository.GetInstallation
                (
                    plugin.Name
                );

                if (false == installation.Disabled)
                {
                    plugin.Initialise();
                }
            }
        }

        /// <summary>
        /// Disables the plug-in specified
        /// </summary>
        /// <param name="pluginName">The plug-in name</param>
        public void DisablePlugin
            (
                string pluginName
            )
        {
            Validate.IsNotEmpty(pluginName);

            var installedPlugin = _installationRepository.GetInstallation
            (
                pluginName
            );

            installedPlugin.Disable();

            _installationRepository.UpdateInstallation
            (
                installedPlugin
            );
        }

        /// <summary>
        /// Enables the plug-in specified
        /// </summary>
        /// <param name="pluginName">The plug-in name</param>
        public void EnablePlugin
            (
                string pluginName
            )
        {
            Validate.IsNotEmpty(pluginName);

            var installedPlugin = _installationRepository.GetInstallation
            (
                pluginName
            );

            installedPlugin.Enable();

            _installationRepository.UpdateInstallation
            (
                installedPlugin
            );
        }

        /// <summary>
        /// Uninstalls the plug-in specified
        /// </summary>
        /// <param name="pluginName">The plug-in name</param>
        public void UninstallPlugin
            (
                string pluginName
            )
        {
            Validate.IsNotEmpty(pluginName);

            var plugin = _pluginRepository.GetPlugin
            (
                pluginName
            );

            plugin.Uninstall();

            _installationRepository.RemoveInstallation
            (
                pluginName
            );
        }
    }
}

namespace CodeChange.Toolkit.Domain.Plugins
{
    /// <summary>
    /// Represents a domain event for when a plugin is uninstalled
    /// </summary>
    public class PluginUninstalledEvent : PluginDomainEventBase
    {
        public PluginUninstalledEvent
            (
                InstalledPlugin plugin
            )
            : base(plugin)
        { }

        /// <summary>
        /// Provides a custom description that represents the domain event
        /// </summary>
        /// <returns>A string that represents the domain event</returns>
        public override string ToString()
        {
            return $"The plug-in '{this.Plugin.PluginName}' was uninstalled.";
        }
    }
}

namespace CodeChange.Toolkit.Domain.Plugins
{
    /// <summary>
    /// Represents a domain event for when a plugin is uninstalled
    /// </summary>
    public class PluginUninstalledEvent : PluginDomainEventBase
    {
        public PluginUninstalledEvent(InstalledPlugin plugin)
            : base(plugin)
        { }

        public override string ToString()
        {
            return $"The plug-in '{this.Plugin.PluginName}' was uninstalled.";
        }
    }
}

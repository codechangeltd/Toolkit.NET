namespace CodeChange.Toolkit.Domain.Plugins
{
    /// <summary>
    /// Represents a domain event for when a plug-in is installed
    /// </summary>
    public class PluginInstalledEvent : PluginDomainEventBase
    {
        public PluginInstalledEvent(InstalledPlugin plugin)
            : base(plugin)
        { }

        public override string ToString()
        {
            return $"The plug-in '{this.Plugin.PluginName}' was installed.";
        }
    }
}

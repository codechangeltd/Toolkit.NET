namespace CodeChange.Toolkit.Domain.Plugins
{
    /// <summary>
    /// Represents a domain event for when a plug-in is enabled
    /// </summary>
    public class PluginEnabledEvent : PluginDomainEventBase
    {
        public PluginEnabledEvent(InstalledPlugin plugin)
            : base(plugin)
        { }

        public override string ToString()
        {
            return $"The plug-in '{this.Plugin.PluginName}' was enabled.";
        }
    }
}

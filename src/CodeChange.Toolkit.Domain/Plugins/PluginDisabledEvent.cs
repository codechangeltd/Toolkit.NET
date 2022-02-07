namespace CodeChange.Toolkit.Domain.Plugins
{
    /// <summary>
    /// Represents a domain event for when a plug-in is disabled
    /// </summary>
    public class PluginDisabledEvent : PluginDomainEventBase
    {
        public PluginDisabledEvent(InstalledPlugin plugin)
            : base(plugin)
        { }

        public override string ToString()
        {
            return $"The plug-in '{this.Plugin.PluginName}' was disabled.";
        }
    }
}

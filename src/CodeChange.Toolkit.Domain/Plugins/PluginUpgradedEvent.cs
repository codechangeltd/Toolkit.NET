namespace CodeChange.Toolkit.Domain.Plugins
{
    /// <summary>
    /// Represents a domain event for when a plug-in is installed
    /// </summary>
    public class PluginUpgradedEvent : PluginDomainEventBase
    {
        public PluginUpgradedEvent(InstalledPlugin plugin)
            : base(plugin)
        { }

        /// <summary>
        /// Provides a custom description that represents the domain event
        /// </summary>
        /// <returns>A string that represents the domain event</returns>
        public override string ToString()
        {
            var name = this.Plugin.PluginName;
            var version = this.Plugin.PluginVersion;

            return $"The plug-in '{name}' was upgraded to version {version}.";
        }
    }
}

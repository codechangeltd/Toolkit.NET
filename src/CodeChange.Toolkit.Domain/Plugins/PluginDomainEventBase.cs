namespace CodeChange.Toolkit.Domain.Plugins
{
    using CodeChange.Toolkit.Domain.Events;
    using System;

    /// <summary>
    /// Represents a base class for all plug-in domain event implementations
    /// </summary>
    public abstract class PluginDomainEventBase : IDomainEvent
    {
        public PluginDomainEventBase(InstalledPlugin plugin)
        {
            Validate.IsNotNull(plugin);

            this.Plugin = plugin;
        }

        public InstalledPlugin Plugin { get; private set; }
    }
}

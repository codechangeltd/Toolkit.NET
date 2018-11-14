namespace CodeChange.Toolkit.Domain.Plugins
{
    using CodeChange.Toolkit.Domain.Events;
    using System;

    /// <summary>
    /// Represents a base class for all plug-in domain event implementations
    /// </summary>
    public abstract class PluginDomainEventBase : IDomainEvent
    {
        /// <summary>
        /// Constructs a plug-in domain event with a installed plug-in
        /// </summary>
        /// <param name="plug-in">The plug-in reference</param>
        public PluginDomainEventBase
            (
                InstalledPlugin plugin
            )
        {
            Validate.IsNotNull(plugin);

            this.Plugin = plugin;
        }

        /// <summary>
        /// Gets a reference to the plug-in that was assigned to the domain event
        /// </summary>
        public InstalledPlugin Plugin { get; private set; }
    }
}

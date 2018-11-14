namespace CodeChange.Toolkit.EF6.Events
{
    using CodeChange.Toolkit.Domain.Events;

    /// <summary>
    /// Represents an entity type database schema configuration for an activity log
    /// </summary>
    public class DomainEventLogConfiguration
        : AggregateEntityTypeConfiguration<DomainEventLog>
    { }
}

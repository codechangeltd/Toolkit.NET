namespace CodeChange.Toolkit.EntityFrameworkCore.Events
{
    using CodeChange.Toolkit.Domain.Events;

    /// <summary>
    /// Represents an entity type database schema configuration for an activity log
    /// </summary>
    public class DomainEventLogConfiguration
        : AggregateEntityTypeConfiguration<DomainEventLog>
    { }
}

namespace CodeChange.Toolkit.EF6.Events
{
    using CodeChange.Toolkit.Domain.Events;
    
    /// <summary>
    /// Represents an entity type database schema configuration for an domain event log detail
    /// </summary>
    public class DomainEventLogDetailConfiguration
        : AggregateEntityTypeConfiguration<DomainEventLogDetail>
    {
        public DomainEventLogDetailConfiguration()
        {
            this.HasKey
            (
                m => new
                {
                    m.ID,
                    m.LogId
                }
            );

            this.HasRequired(m => m.Log)
                .WithMany(m => m.Details)
                .HasForeignKey(m => new { m.LogId })
                .WillCascadeOnDelete();
        }
    }
}

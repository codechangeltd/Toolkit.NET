namespace CodeChange.Toolkit.EntityFrameworkCore.Events
{
    using CodeChange.Toolkit.Domain.Events;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Represents an entity type database schema configuration for an domain event log detail
    /// </summary>
    public class DomainEventLogDetailConfiguration
        : AggregateEntityTypeConfiguration<DomainEventLogDetail>
    {
        public DomainEventLogDetailConfiguration()
            : base()
        {
            this.DisableKeyAutoBuild = true;
        }

        protected override void ApplyCustomConfiguration
            (
                EntityTypeBuilder<DomainEventLogDetail> builder
            )
        {
            builder.HasKey
            (
                m => new
                {
                    m.ID,
                    m.LogId
                }
            );

            builder.HasOne(m => m.Log)
                .WithMany(m => m.Details)
                .HasForeignKey(m => new { m.LogId })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

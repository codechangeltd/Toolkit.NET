namespace CodeChange.Toolkit.EntityFrameworkCore.Events
{
    using CodeChange.Toolkit.Domain.Events;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Represents an entity type database schema configuration for an domain event log detail
    /// </summary>
    public class DomainEventLogDetailConfiguration : AggregateEntityTypeConfiguration<DomainEventLogDetail>
    {
        public DomainEventLogDetailConfiguration() : base()
        {
            this.DisableKeyAutoBuild = true;
        }

        protected override void ApplyCustomConfiguration(EntityTypeBuilder<DomainEventLogDetail> builder)
        {
            builder.HasKey(_ => new { _.ID, _.LogId });

            builder.HasOne(_ => _.Log)
                .WithMany(_ => _.Details)
                .HasForeignKey(_ => new { _.LogId })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

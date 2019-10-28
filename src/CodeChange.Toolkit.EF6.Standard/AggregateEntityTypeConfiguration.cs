namespace CodeChange.Toolkit.EF6
{
    using CodeChange.Toolkit.Domain.Aggregate;
    using Humanizer;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.ModelConfiguration;

    /// <summary>
    /// Represents a base entity configuration for all aggregate entities
    /// </summary>
    /// <typeparam name="TEntity">The entity type to configure</typeparam>
    public class AggregateEntityTypeConfiguration<TEntity> : EntityTypeConfiguration<TEntity>
        where TEntity : class, IAggregateEntity
    {
        /// <summary>
        /// Constructs a new aggregate entity type configuration with standard configuration
        /// </summary>
        public AggregateEntityTypeConfiguration()
            : base()
        {
            this.Property(m => m.ID).HasDatabaseGeneratedOption
            (
                DatabaseGeneratedOption.Identity
            );

            var indexAttribute = new IndexAttribute()
            {
                IsUnique = true
            };

            // Apply a maximum length and an index to the lookup key
            this.Property(m => m.LookupKey)
                .HasMaxLength(150)
                .HasColumnAnnotation
                (
                    "Index",
                    new IndexAnnotation(indexAttribute)
                );

            this.Map
            (
                m =>
                {
                    m.ToTable
                    (
                        typeof(TEntity).Name.Pluralize()
                    );
                }
            );
        }
    }
}

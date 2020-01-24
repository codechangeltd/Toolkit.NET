namespace CodeChange.Toolkit.EntityFrameworkCore
{
    using CodeChange.Toolkit.Domain.Aggregate;
    using Humanizer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// A base entity configuration for all aggregate entities
    /// </summary>
    /// <typeparam name="TEntity">The entity type to configure</typeparam>
    public abstract class AggregateEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : class, IAggregateEntity
    {
        protected bool DisableKeyAutoBuild { get; set; }

        public virtual void Configure
            (
                EntityTypeBuilder<TEntity> builder
            )
        {
            ApplyCustomConfiguration(builder);

            if (false == this.DisableKeyAutoBuild)
            {
                builder.HasKey(m => m.ID);
            }

            builder.HasIndex(m => m.ID);
            builder.HasIndex(m => m.LookupKey);

            builder.ToTable
            (
                typeof(TEntity).Name.Pluralize()
            );
        }

        protected virtual void ApplyCustomConfiguration
            (
                EntityTypeBuilder<TEntity> builder
            )
        { }
    }
}

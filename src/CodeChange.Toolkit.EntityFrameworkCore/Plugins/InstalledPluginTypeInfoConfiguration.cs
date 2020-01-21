namespace CodeChange.Toolkit.EntityFrameworkCore.Plugins
{
    using CodeChange.Toolkit.Domain.Plugins;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Represents a database schema configuration for an installed plug-in type infos
    /// </summary>
    public class InstalledPluginTypeInfoConfiguration
        : AggregateEntityTypeConfiguration<InstalledPluginTypeInfo>
    {
        public InstalledPluginTypeInfoConfiguration()
            : base()
        {
            this.DisableKeyAutoBuild = true;
        }

        protected override void ApplyCustomConfiguration
            (
                EntityTypeBuilder<InstalledPluginTypeInfo> builder
            )
        {
            builder.HasKey
            (
                m => new
                {
                    m.ID,
                    m.InstalledPluginId
                }
            );

            builder.HasOne(m => m.InstalledPlugin)
                .WithMany(m => m.AssemblyTypes)
                .HasForeignKey(m => new { m.InstalledPluginId })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

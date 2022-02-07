namespace CodeChange.Toolkit.EntityFrameworkCore.Plugins
{
    using CodeChange.Toolkit.Domain.Plugins;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Represents a database schema configuration for an installed plug-in type infos
    /// </summary>
    public class InstalledPluginTypeInfoConfiguration : AggregateEntityTypeConfiguration<InstalledPluginTypeInfo>
    {
        public InstalledPluginTypeInfoConfiguration() : base()
        {
            this.DisableKeyAutoBuild = true;
        }

        protected override void ApplyCustomConfiguration(EntityTypeBuilder<InstalledPluginTypeInfo> builder)
        {
            builder.HasKey(x => new { x.ID, x.InstalledPluginId });

            builder.HasOne(x => x.InstalledPlugin)
                .WithMany(x => x.AssemblyTypes)
                .HasForeignKey(x => new { x.InstalledPluginId })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

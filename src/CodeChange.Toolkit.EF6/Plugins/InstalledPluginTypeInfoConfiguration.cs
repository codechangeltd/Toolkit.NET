namespace CodeChange.Toolkit.EF6.Plugins
{
    using CodeChange.Toolkit.Domain.Plugins;

    /// <summary>
    /// Represents a database schema configuration for an installed plug-in type infos
    /// </summary>
    public class InstalledPluginTypeInfoConfiguration
        : AggregateEntityTypeConfiguration<InstalledPluginTypeInfo>
    {
        public InstalledPluginTypeInfoConfiguration()
            : base()
        {
            this.HasKey(m => new
            {
                m.ID,
                m.InstalledPluginId
            });

            this.HasRequired(m => m.InstalledPlugin)
                .WithMany(m => m.AssemblyTypes)
                .HasForeignKey(m => new { m.InstalledPluginId })
                .WillCascadeOnDelete();
        }
    }
}

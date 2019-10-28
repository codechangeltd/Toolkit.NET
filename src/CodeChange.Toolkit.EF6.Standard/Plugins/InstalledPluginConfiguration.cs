namespace CodeChange.Toolkit.EF6.Plugins
{
    using CodeChange.Toolkit.Domain.Plugins;

    /// <summary>
    /// Represents a database schema configuration for an installed plug-in
    /// </summary>
    public class InstalledPluginConfiguration
        : AggregateEntityTypeConfiguration<InstalledPlugin>
    { }
}

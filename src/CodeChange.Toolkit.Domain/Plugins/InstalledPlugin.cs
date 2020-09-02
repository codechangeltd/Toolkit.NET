namespace CodeChange.Toolkit.Domain.Plugins
{
    using CodeChange.Toolkit.Domain.Aggregate;
    using CodeChange.Toolkit.Domain.Events;
    using CodeChange.Toolkit.Plugins;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;

    /// <summary>
    /// Represents an aggregate entity for an installed plug-in
    /// </summary>
    public class InstalledPlugin : IAggregateRoot
	{
		/// <summary>
		/// Default protected parameter less constructor for the ORM to initialise the entity
		/// </summary>
		protected InstalledPlugin()
        {
            this.UnpublishedEvents = new List<IDomainEvent>();
        }

        /// <summary>
        /// Constructs a installed plug-in with the plug-in specified
        /// </summary>
        /// <param name="plugin">The plug-in</param>
        protected InstalledPlugin(IPlugin plugin)
		{
			Validate.IsNotNull(plugin);

			this.LookupKey = new EntityKeyGenerator().GenerateKey();
			this.UnpublishedEvents = new List<IDomainEvent>();
			this.DateCreated = DateTime.UtcNow;
			this.DateModified = DateTime.UtcNow;
			this.AssemblyTypes = new Collection<InstalledPluginTypeInfo>();

			SetPluginDetails(plugin);

			this.UnpublishedEvents.Add
			(
				new PluginInstalledEvent(this)
			);
		}

		/// <summary>
		/// Creates a new installed plug-in with the plug-in specified
		/// </summary>
		/// <param name="plugin">The plug-in</param>
		/// <returns>The installed plug-in that was created</returns>
		public static InstalledPlugin CreateInstalledPlugin(IPlugin plugin)
		{
			return new InstalledPlugin(plugin);
		}

		/// <summary>
		/// A database auto generated ID value, used internally for persistence
		/// </summary>
		public long ID { get; protected set; }

		/// <summary>
		/// A lookup key value for the entity, this must be unique for each entity of the same type
		/// </summary>
		public string LookupKey { get; protected set; }

		/// <summary>
		/// Gets the aggregate entities unique key value
		/// </summary>
		/// <returns>The key value</returns>
		public virtual string GetKeyValue()
		{
			return this.LookupKey;
		}

		/// <summary>
		/// Gets or sets the date the aggregate was created
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the date the aggregate was last modified
		/// </summary>
		public DateTime DateModified { get; set; }

		/// <summary>
		/// Gets a list of unpublished domain events
		/// </summary>
		public IList<IDomainEvent> UnpublishedEvents { get; protected set; }

		/// <summary>
		/// Forces the aggregate root to destroy itself (similar to Dispose)
		/// </summary>
		/// <remarks>
		/// This allows the object to clean up any related data or perform any tasks that
		/// need to be completed before the object is destroyed (and deleted).
		/// </remarks>
		public virtual void Destroy()
		{
            this.UnpublishedEvents.Add(new PluginUninstalledEvent(this));
		}

		/// <summary>
		/// Sets the plug-in details
		/// </summary>
		/// <param name="plugin">The plug-in</param>
		protected void SetPluginDetails(IPlugin plugin)
		{
			Validate.IsNotNull(plugin);

			if (String.IsNullOrEmpty(plugin.Name))
			{
				throw new InvalidOperationException
                (
                    "The plug-in name cannot be null or empty."
                );
			}

			var pluginType = plugin.GetType();
			var interfaceType = plugin.GetPluginInterfaceType();

			this.PluginName = plugin.Name;
			this.PluginDescription = plugin.Description;
			this.PluginTypeName = pluginType.Name;
			this.PluginInterfaceTypeName = interfaceType.Name;
			this.PluginVersion = plugin.Version.ToString();

			var assembly = Assembly.GetAssembly(plugin.GetType());
			var assemblyName = assembly.GetName();
			var assemblyTypes = assembly.GetLoadableTypes();
			var assemblyPath = assembly.GetFullPath();

			this.AssemblySimpleName = assemblyName.Name;
			this.AssemblyFullName = assemblyName.FullName;
			this.AssemblyLocation = assemblyPath;
			this.AssemblyTypes.Clear();

			foreach (var type in assemblyTypes)
			{
				var info = new InstalledPluginTypeInfo(this, type);

				this.AssemblyTypes.Add(info);
			}
		}

		/// <summary>
		/// Updates the installed plug-in details using the plugin specified
		/// </summary>
		/// <param name="plugin">The plug-in</param>
		internal void UpdatePluginDetails(IPlugin plugin)
		{
			Validate.IsNotNull(plugin);

			SetPluginDetails(plugin);

            this.UnpublishedEvents.Add(new PluginUpgradedEvent(this));
		}

		/// <summary>
		/// Gets the name of the plug-in
		/// </summary>
		public string PluginName { get; protected set; }
		
		/// <summary>
		/// Gets a description of the plug-in
		/// </summary>
		public string PluginDescription { get; protected set; }

		/// <summary>
		/// Gets the type name of the plug-in
		/// </summary>
		public string PluginTypeName { get; protected set; }

		/// <summary>
		/// Gets the plug-ins interface type name
		/// </summary>
		public string PluginInterfaceTypeName { get; protected set; }

		/// <summary>
		/// Gets the version of the plug-in
		/// </summary>
		public string PluginVersion { get; protected set; }

		/// <summary>
		/// Compares the installed plugin version with the plugin specified
		/// </summary>
		/// <param name="plugin">The plugin</param>
		/// <returns>An indication of their relative values</returns>
		/// <remarks>
		/// A return value of zero indicates that the versions are the same
		/// A return value greater than zero indicates that the plugin is newer
		/// A return value less than zero indicates that the plugin is older
		/// </remarks>
		internal int CompareVersions(IPlugin plugin)
		{
			Validate.IsNotNull(plugin);

			var installedVersion = new Version(this.PluginVersion);

			return installedVersion.CompareTo(plugin.Version);
		}

		/// <summary>
		/// Gets the simple name of the assembly where the plug-in resides
		/// </summary>
		public string AssemblySimpleName { get; protected set; }

		/// <summary>
		/// Gets the full name of the assembly where the plug-in resides
		/// </summary>
		public string AssemblyFullName { get; protected set; }

		/// <summary>
		/// Gets the location of the assembly where the plug-in resides
		/// </summary>
		public string AssemblyLocation { get; protected set; }

		/// <summary>
		/// Gets a collection of types for the plug-ins assembly
		/// </summary>
		public virtual ICollection<InstalledPluginTypeInfo> AssemblyTypes { get; protected set; }

		/// <summary>
		/// Gets a flag used to indicate if the plug-in has been disabled
		/// </summary>
		public bool Disabled { get; protected set; }

		/// <summary>
		/// Gets the date and time the plug-in was disabled
		/// </summary>
		public DateTime? DateDisabled { get; protected set; }

		/// <summary>
		/// Disables the installed plug-in
		/// </summary>
		public void Disable()
		{
			if (this.Disabled)
			{
				throw new InvalidOperationException
				(
                    $"The plug-in '{this.PluginName}' has already been disabled."
				);
			}

			this.Disabled = true;
			this.DateDisabled = DateTime.UtcNow;

            this.UnpublishedEvents.Add(new PluginDisabledEvent(this));
		}

		/// <summary>
		/// Enables the installed plug-in
		/// </summary>
		public void Enable()
		{
			if (false == this.Disabled)
			{
				throw new InvalidOperationException
				(
                    $"The plug-in '{this.PluginName}' has already been enabled."
                );
			}

			this.Disabled = false;
			this.DateDisabled = null;

            this.UnpublishedEvents.Add(new PluginEnabledEvent(this));
		}
	}
}

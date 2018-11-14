namespace CodeChange.Toolkit.Domain.Plugins
{
    using CodeChange.Toolkit.Domain.Aggregate;
    using System;

	/// <summary>
	/// Represents a installed plug-in type information entity
	/// </summary>
	public class InstalledPluginTypeInfo : IAggregateEntity
	{
		/// <summary>
		/// Default protected parameter less constructor for the ORM to initialise the entity
		/// </summary>
		protected InstalledPluginTypeInfo()
			: base()
		{ }

		/// <summary>
		/// Constructs the type information with the registered plugin and a type
		/// </summary>
		/// <param name="installedPlugin">The installed plug-in</param>
		/// <param name="type">The class type</param>
		protected internal InstalledPluginTypeInfo
			(
				InstalledPlugin installedPlugin,
				Type type
			)
		{
			Validate.IsNotNull(installedPlugin);
			Validate.IsNotNull(type);

			this.LookupKey = new EntityKeyGenerator().GenerateKey();
			this.InstalledPlugin = installedPlugin;
			this.TypeName = type.Name;
			this.TypeFullName = type.FullName;

			if (type.BaseType != null)
			{
				this.BaseTypeName = type.BaseType.Name;
				this.BaseTypeFullName = type.BaseType.FullName;
			}
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
		/// Gets the installed plug-in reference
		/// </summary>
		public InstalledPlugin InstalledPlugin { get; protected set; }

		/// <summary>
		/// Gets the ID of the installed plug-in
		/// </summary>
		public long InstalledPluginId { get; protected set; }

		/// <summary>
		/// Gets the types name
		/// </summary>
		public string TypeName { get; protected set; }

		/// <summary>
		/// Gets the types full name
		/// </summary>
		public string TypeFullName { get; protected set; }

		/// <summary>
		/// Gets the base type name
		/// </summary>
		public string BaseTypeName { get; protected set; }

		/// <summary>
		/// Gets the base types full name
		/// </summary>
		public string BaseTypeFullName { get; protected set; }
	}
}

namespace System.Data.Entity
{
    using CodeChange.Toolkit.Domain.Aggregate;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Reflection;

    public static class DbContextExtensions
    {
        /// <summary>
        /// Reads all date time values as UTC kind for every entity that is materialized
        /// </summary>
        /// <param name="context">The DbContext</param>
        /// <remarks>
        /// Adapted from http://stackoverflow.com/a/11683020
        /// </remarks>
        public static void ReadAllDateTimeValuesAsUtc(this DbContext context)
        {
            Validate.IsNotNull(context);

            var objectContext = ((IObjectContextAdapter)context).ObjectContext;

            objectContext.ObjectMaterialized += ReadAllDateTimeValuesAsUtc;
        }

        /// <summary>
        /// Uses reflection to read all date time properties and set the kind to UTC
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The object materialized event arguments</param>
        private static void ReadAllDateTimeValuesAsUtc(object sender, ObjectMaterializedEventArgs e)
        {
            var properties = e.Entity
                .GetType()
                .GetProperties()
                .Where(_ => _.PropertyType == typeof(DateTime) || _.PropertyType == typeof(DateTime?));

            properties.ToList().ForEach(_ => SpecifyUtcKind(_, e.Entity));
        }

        /// <summary>
        /// Specifies that the kind for a specified date time property is set to UTC
        /// </summary>
        /// <param name="property">The property to update</param>
        /// <param name="value">The value to set</param>
        private static void SpecifyUtcKind(PropertyInfo property, object value)
        {
            Validate.IsNotNull(property);

            var datetime = property.GetValue(value, null);

            if (property.PropertyType == typeof(DateTime))
            {
                datetime = DateTime.SpecifyKind((DateTime)datetime, DateTimeKind.Utc);
            }
            else if (property.PropertyType == typeof(DateTime?))
            {
                var nullable = (DateTime?)datetime;

                if (false == nullable.HasValue)
                {
                    return;
                }

                datetime = (DateTime?)DateTime.SpecifyKind(nullable.Value, DateTimeKind.Utc);
            }
            else
            {
                return;
            }

            property.SetValue(value, datetime, null);
        }

        /// <summary>
        /// Gets all aggregates that are pending saving
        /// </summary>
        /// <param name="context">The DbContext</param>
        /// <returns>A collection of aggregate roots</returns>
        public static IEnumerable<IAggregateRoot> GetPendingAggregates(this DbContext context)
        {
            Validate.IsNotNull(context);

            var entries = context.ChangeTracker
                .Entries()
                .Where(_ => _.Entity.GetType().ImplementsInterface(typeof(IAggregateRoot)));

            var aggregates = new List<IAggregateRoot>();

            foreach (var entry in entries.ToList())
            {
                aggregates.Add((IAggregateRoot)entry.Entity);
            }

            return aggregates;
        }
    }
}

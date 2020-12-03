namespace CodeChange.Toolkit.EntityFrameworkCore
{
    using CodeChange.Toolkit.Domain.Aggregate;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides various extension methods for the Entity Framework DbContext
    /// </summary>
    public static class DbContextExtensions
    {
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

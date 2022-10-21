namespace CodeChange.Toolkit.EntityFrameworkCore
{
    using CodeChange.Toolkit.Domain.Aggregate;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class DbContextExtensions
    {
        /// <summary>
        /// Gets all aggregates that are pending saving
        /// </summary>
        /// <param name="context">The DbContext</param>
        /// <returns>A collection of aggregate roots</returns>
        public static IAggregateRoot[] GetPendingAggregates(this DbContext context)
        {
            var entries = context.ChangeTracker
                .Entries()
                .Where(x => x.Entity.GetType().ImplementsInterface(typeof(IAggregateRoot)))
                .Where(x => x.State != EntityState.Detached && x.State != EntityState.Unchanged);

            var aggregates = new List<IAggregateRoot>();

            foreach (var entry in entries.ToList())
            {
                aggregates.Add((IAggregateRoot)entry.Entity);
            }

            return aggregates.ToArray();
        }

        /// <summary>
        /// Gets all aggregates that are pending saving from multiple database contexts
        /// </summary>
        /// <param name="contexts">The DbContext collection</param>
        /// <returns>A collection of aggregate roots</returns>
        public static IAggregateRoot[] GetPendingAggregates(this IEnumerable<DbContext> contexts)
        {
            var aggregates = new List<IAggregateRoot>();

            foreach (var context in contexts)
            {
                aggregates.AddRange(GetPendingAggregates(context));
            }

            return aggregates.ToArray();
        }
    }
}

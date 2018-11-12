namespace CodeChange.Toolkit.Domain.Aggregate
{
    using CodeChange.Toolkit.Domain.Events;
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Represents various extension methods for aggregate roots
    /// </summary>
    public static class AggregateRootExtensions
    {
        /// <summary>
        /// Gets a collection of pre-transaction domain events
        /// </summary>
        /// <param name="aggregate">The aggregate root</param>
        /// <returns>A collection of matching domain events</returns>
        public static IList<IDomainEvent> GetPreTransactionEvents
            (
                this IAggregateRoot aggregate
            )
        {
            Validate.IsNotNull(aggregate);

            var events = new List<IDomainEvent>();

            if (aggregate.UnpublishedEvents != null)
            {
                foreach (var @event in aggregate.UnpublishedEvents)
                {
                    var preTransaction = @event.IsPreTransaction();
                    
                    if (preTransaction)
                    {
                        events.Add(@event);
                    }
                }
            }

            return events;
        }

        /// <summary>
        /// Gets a collection of post-transaction domain events
        /// </summary>
        /// <param name="aggregate">The aggregate root</param>
        /// <returns>A collection of matching domain events</returns>
        public static IList<IDomainEvent> GetPostTransactionEvents
            (
                this IAggregateRoot aggregate
            )
        {
            Validate.IsNotNull(aggregate);

            var events = new List<IDomainEvent>();

            if (aggregate.UnpublishedEvents != null)
            {
                foreach (var @event in aggregate.UnpublishedEvents)
                {
                    var postTransaction = @event.IsPostTransaction();

                    if (postTransaction)
                    {
                        events.Add(@event);
                    }
                }
            }

            return events;
        }
    }
}

namespace CodeChange.Toolkit.Domain.Events
{
    using System;

    /// <summary>
    /// Represents a domain event queue item
    /// </summary>
    public sealed class EventQueueItem
    {
        /// <summary>
        /// Constructs the event queue item with dependencies
        /// </summary>
        /// <param name="aggregateKey">The aggregate roots key</param>
        /// <param name="aggregateType">The aggregate root type</param>
        /// <param name="event">The domain event</param>
        public EventQueueItem
            (
                string aggregateKey,
                Type aggregateType,
                IDomainEvent @event
            )
        {
            Validate.IsNotEmpty(aggregateKey);
            Validate.IsNotNull(aggregateType);
            Validate.IsNotNull(@event);

            this.AggregateKey = aggregateKey;
            this.AggregateType = aggregateType;
            this.Event = @event;
        }

        /// <summary>
        /// Gets the aggregate roots key
        /// </summary>
        public string AggregateKey { get; private set; }

        /// <summary>
        /// Gets the aggregate root type
        /// </summary>
        public Type AggregateType { get; private set; }

        /// <summary>
        /// Gets the domain event
        /// </summary>
        public IDomainEvent Event { get; private set; }
    }
}

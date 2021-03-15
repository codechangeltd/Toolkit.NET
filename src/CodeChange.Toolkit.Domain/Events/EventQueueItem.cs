namespace CodeChange.Toolkit.Domain.Events
{
    using System;

    /// <summary>
    /// Represents a domain event queue item
    /// </summary>
    public sealed class EventQueueItem
    {
        public EventQueueItem(string aggregateKey, Type aggregateType, IDomainEvent @event)
        {
            Validate.IsNotEmpty(aggregateKey);
            Validate.IsNotNull(aggregateType);
            Validate.IsNotNull(@event);

            this.AggregateKey = aggregateKey;
            this.AggregateType = aggregateType;
            this.Event = @event;
        }

        /// <summary>
        /// Gets the key of the aggregate root where the event came from
        /// </summary>
        public string AggregateKey { get; private set; }

        /// <summary>
        /// Gets the type of the aggregate root where the event came from
        /// </summary>
        public Type AggregateType { get; private set; }

        /// <summary>
        /// Gets the domain event that is being queued
        /// </summary>
        public IDomainEvent Event { get; private set; }

        public override bool Equals(object obj)
        {
            if (this == null && obj == null)
            {
                return true;
            }
            else if (this == null || obj == null || obj.GetType() != typeof(EventQueueItem))
            {
                return false;
            }
            else
            {
                var item = (EventQueueItem)obj;

                return item.AggregateKey.Equals(this.AggregateKey)
                    && item.AggregateType.Equals(this.AggregateType)
                    && item.Event.GenerateHashCode() == this.Event.GenerateHashCode();
            }
        }

        public override int GetHashCode()
        {
            return this.GenerateHashCode();
        }

        public override string ToString()
        {
            return this.Event.ToString();
        }
    }
}

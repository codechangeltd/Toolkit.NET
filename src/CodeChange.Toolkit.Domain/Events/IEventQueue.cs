namespace CodeChange.Toolkit.Domain.Events
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a contract for a domain event queue
    /// </summary>
    public interface IEventQueue : IEnumerable<EventQueueItem>
    {
        /// <summary>
        /// Adds a domain event to the event queue
        /// </summary>
        /// <param name="aggregateKey">The aggregate roots key</param>
        /// <param name="aggregateType">The aggregate root type</param>
        /// <param name="event">The domain event</param>
        void Add(string aggregateKey, Type aggregateType, IDomainEvent @event);

        /// <summary>
        /// Gets the next domain event in the queue
        /// </summary>
        /// <returns>The queued item</returns>
        EventQueueItem GetNext();

        /// <summary>
        /// Determines if the event queue is empty
        /// </summary>
        /// <returns>True, if the queue is empty; otherwise false</returns>
        bool IsEmpty();
    }
}

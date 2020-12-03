namespace CodeChange.Toolkit.Domain.Events
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The default implementation of a domain event queue
    /// </summary>
    public sealed class EventQueue : IEventQueue
    {
        private readonly List<EventQueueItem> _items;

        /// <summary>
        /// Constructs the event queue with a new collection
        /// </summary>
        public EventQueue()
        {
            _items = new List<EventQueueItem>();
        }

        /// <summary>
        /// Adds a domain event to the event queue
        /// </summary>
        /// <param name="aggregateKey">The aggregate roots key</param>
        /// <param name="aggregateType">The aggregate root type</param>
        /// <param name="event">The domain event</param>
        public void Add(string aggregateKey, Type aggregateType, IDomainEvent @event)
        {
            Validate.IsNotEmpty(aggregateKey);
            Validate.IsNotNull(aggregateType);
            Validate.IsNotNull(@event);

            var item = new EventQueueItem(aggregateKey, aggregateType, @event);

            _items.Add(item);
        }

        /// <summary>
        /// Gets the next domain event in the queue
        /// </summary>
        /// <returns>The queued item</returns>
        public EventQueueItem GetNext()
        {
            if (_items.Count == 0)
            {
                throw new InvalidOperationException
                (
                    "There are no items left in the queue."
                );
            }
            else
            {
                var nextItem = _items.First();

                _items.RemoveAt(0);

                return nextItem;
            }
        }

        /// <summary>
        /// Determines if the event queue is empty
        /// </summary>
        /// <returns>True, if the queue is empty; otherwise false</returns>
        public bool IsEmpty() => (_items.Count == 0);

        /// <summary>
        /// Gets an enumerator for the event queue items
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<EventQueueItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for the event queue
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}

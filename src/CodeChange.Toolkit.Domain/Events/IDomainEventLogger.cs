namespace CodeChange.Toolkit.Domain.Events
{
    using System;

    /// <summary>
    /// Defines a contract for a domain event logger that is responsible for logging domain events
    /// </summary>
    public interface IDomainEventLogger
    {
        /// <summary>
        /// Logs the domain event specified
        /// </summary>
        /// <param name="event">The domain event</param>
        void LogEvent
        (
            IDomainEvent @event
        );

        /// <summary>
        /// Logs the domain event specified
        /// </summary>
        /// <param name="aggregateKey">The aggregate key</param>
        /// <param name="aggregateType">The aggregate type</param>
        /// <param name="event">The domain event</param>
        void LogEvent
        (
            string aggregateKey,
            Type aggregateType,
            IDomainEvent @event
        );
    }
}

namespace CodeChange.Toolkit.Domain.Aggregate
{
    using CodeChange.Toolkit.Domain.Events;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the interface for all domain aggregate root implementations
    /// </summary>
    public interface IAggregateRoot : IAggregateEntity, ILoggingInfo
    {
        /// <summary>
        /// Gets a list of unpublished domain events
        /// </summary>
        IList<IDomainEvent> UnpublishedEvents { get; }

        /// <summary>
        /// Forces the aggregate root to destroy itself (similar to Dispose)
        /// </summary>
        /// <remarks>
        /// This allows the object to clean up any related data or perform any tasks that
        /// need to be completed before the object is destroyed (and deleted).
        /// </remarks>
        void Destroy();
    }
}

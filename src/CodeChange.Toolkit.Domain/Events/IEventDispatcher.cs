namespace CodeChange.Toolkit.Domain.Events
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a contract for a domain event dispatcher
    /// </summary>
    public interface IEventDispatcher
    {
        /// <summary>
        /// Dispatches a domain event of the type specified
        /// </summary>
        /// <typeparam name="T">The domain event type</typeparam>
        /// <param name="event">The event to dispatch</param>
        /// <param name="preTransaction">True, if pre-transaction handlers required</param>
        [Obsolete]
        void Dispatch<T>(T @event, bool preTransaction = false) where T : IDomainEvent;

        /// <summary>
        /// Dispatches a domain event
        /// </summary>
        /// <param name="event">The event to dispatch</param>
        /// <param name="preTransaction">True, if pre-transaction handlers required</param>
        [Obsolete]
        void Dispatch(IDomainEvent @event, bool preTransaction = false);

        /// <summary>
        /// Asynchronously dispatches a domain event of the type specified
        /// </summary>
        /// <typeparam name="T">The domain event type</typeparam>
        /// <param name="event">The event to dispatch</param>
        /// <param name="preTransaction">True, if pre-transaction handlers required</param>
        /// <returns>The dispatch task</returns>
        Task DispatchAsync<T>(T @event, bool preTransaction = false) where T : IDomainEvent;

        /// <summary>
        /// Asynchronously dispatches a domain event
        /// </summary>
        /// <param name="event">The event to dispatch</param>
        /// <param name="preTransaction">True, if pre-transaction handlers required</param>
        /// <returns>The dispatch task</returns>
        Task DispatchAsync(IDomainEvent @event, bool preTransaction = false);
    }
}

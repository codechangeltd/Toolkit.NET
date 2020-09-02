namespace CodeChange.Toolkit.Domain.Events
{
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
        void Dispatch<T>(T @event, bool preTransaction = false) where T : IDomainEvent;

        /// <summary>
        /// Dispatches a domain event
        /// </summary>
        /// <param name="event">The event to dispatch</param>
        /// <param name="preTransaction">True, if pre-transaction handlers required</param>
        void Dispatch(IDomainEvent @event, bool preTransaction = false);
    }
}

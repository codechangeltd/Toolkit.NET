namespace CodeChange.Toolkit.Domain.Events
{
    /// <summary>
    /// Interface for implementations that handle domain events
    /// </summary>
    /// <typeparam name="T">The domain event type</typeparam>
    public interface IDomainEventHandler<T> 
        where T : IDomainEvent
    {
        /// <summary>
        /// Handles the domain event implementation
        /// </summary>
        /// <param name="domainEvent">The domain event handler</param>
        void Handle
        (
            T domainEvent
        );
    }
}

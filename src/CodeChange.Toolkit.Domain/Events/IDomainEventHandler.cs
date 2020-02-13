namespace CodeChange.Toolkit.Domain.Events
{
    using CodeChange.Toolkit.Domain.Messages;

    /// <summary>
    /// Defines a contract for a domain event handler in a domain orientated architecture
    /// </summary>
    /// <typeparam name="T">The domain event type to handle</typeparam>
    public interface IDomainEventHandler<in T> : IHandler<T>
        where T : IDomainEvent
    { }
}

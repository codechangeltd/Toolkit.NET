namespace CodeChange.Toolkit.Domain.Events
{
    using CodeChange.Toolkit.Domain.Messages;

    /// <summary>
    /// Defines a contract for a event within domain orientated architectures
    /// </summary>
    public interface IDomainEvent : IMessage { }
}

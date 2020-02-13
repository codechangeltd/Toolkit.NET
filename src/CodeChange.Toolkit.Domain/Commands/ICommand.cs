namespace CodeChange.Toolkit.Domain.Commands
{
    using CodeChange.Toolkit.Domain.Messages;
    
    /// <summary>
    /// Defines a contract for a command in a CQRS architecture
    /// </summary>
    public interface ICommand : IMessage { }
}

namespace CodeChange.Toolkit.Domain.Commands
{
    using CodeChange.Toolkit.Domain.Messages;

    /// <summary>
    /// Defines a contract for a command handler in a CQRS architecture
    /// </summary>
    /// <typeparam name="T">The command type to handle</typeparam>
    public interface ICommandHandler<in T> : IHandler<T> 
        where T : ICommand
    { }
}

namespace CodeChange.Toolkit.Domain.Commands
{
    using MediatR;

    /// <summary>
    /// Defines a contract for a command handler in a CQRS architecture
    /// </summary>
    /// <typeparam name="TCommand">The command type to handle</typeparam>
    /// <typeparam name="TResponse">The response type returned</typeparam>
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    { }
}

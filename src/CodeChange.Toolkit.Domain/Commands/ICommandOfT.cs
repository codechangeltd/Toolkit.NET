namespace CodeChange.Toolkit.Domain.Commands
{
    using CodeChange.Toolkit.Domain.Messages;
    using MediatR;

    /// <summary>
    /// Defines a contract for a command in a CQRS architecture
    /// </summary>
    /// <typeparam name="TResponse">The response type returned by the command</typeparam>
    public interface ICommand<TResponse> : IMessage, IRequest<TResponse> { }
}

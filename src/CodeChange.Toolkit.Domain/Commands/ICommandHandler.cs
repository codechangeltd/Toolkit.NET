﻿namespace CodeChange.Toolkit.Domain.Commands
{
    using CodeChange.Toolkit.Domain.Messages;
    using MediatR;

    /// <summary>
    /// Defines a contract for a command handler in a CQRS architecture
    /// </summary>
    /// <typeparam name="TCommand">The command type to handle</typeparam>
    public interface ICommandHandler<in TCommand> : IHandler<TCommand>, IRequestHandler<TCommand, Result>
        where TCommand : ICommand
    { }
}

namespace CodeChange.Toolkit.Domain.Commands
{
    /// <summary>
    /// Defines a contract for a command in a CQRS architecture
    /// </summary>
    public interface ICommand : ICommand<Result> { }
}

namespace CodeChange.Toolkit.Domain.Messages
{
    using CSharpFunctionalExtensions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a contract for a handler in a domain orientated architecture
    /// </summary>
    /// <typeparam name="T">The message type to handle</typeparam>
    public interface IHandler<in T> where T : IMessage
    {
        /// <summary>
        /// Handles the message by executing orchestration logic
        /// </summary>
        /// <param name="message">The message to handle</param>
        /// <param name="cancellationToken">The cancellation token (optional)</param>
        /// <returns>The result of the message</returns>
        Task<Result> Handle
        (
            T message,
            CancellationToken cancellationToken = default
        );
    }
}

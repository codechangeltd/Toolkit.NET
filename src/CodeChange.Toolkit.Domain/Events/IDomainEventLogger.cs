﻿namespace CodeChange.Toolkit.Domain.Events
{
    using CSharpFunctionalExtensions;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a contract for a domain event logger that is responsible for logging domain events
    /// </summary>
    public interface IDomainEventLogger
    {
        /// <summary>
        /// Logs the domain event specified
        /// </summary>
        /// <param name="event">The domain event</param>
        /// <returns>The result of the operation</returns>
        Result LogEvent(IDomainEvent @event);

        /// <summary>
        /// Asynchronously logs the domain event specified
        /// </summary>
        /// <param name="event">The domain event</param>
        /// <param name="cancellationToken">The cancellation token (optional)</param>
        /// <returns>The result of the operation</returns>
        Task<Result> LogEventAsync(IDomainEvent @event, CancellationToken cancellationToken = default);

        /// <summary>
        /// Logs the domain event specified
        /// </summary>
        /// <param name="aggregateKey">The aggregate key</param>
        /// <param name="aggregateType">The aggregate type</param>
        /// <param name="event">The domain event</param>
        /// <returns>The result of the operation</returns>
        Result LogEvent(string aggregateKey, Type aggregateType, IDomainEvent @event);

        /// <summary>
        /// Asynchronously logs the domain event specified
        /// </summary>
        /// <param name="aggregateKey">The aggregate key</param>
        /// <param name="aggregateType">The aggregate type</param>
        /// <param name="event">The domain event</param>
        /// <param name="cancellationToken">The cancellation token (optional)</param>
        /// <returns>The result of the operation</returns>
        Task<Result> LogEventAsync(string aggregateKey, Type aggregateType, IDomainEvent @event, CancellationToken cancellationToken = default);
    }
}

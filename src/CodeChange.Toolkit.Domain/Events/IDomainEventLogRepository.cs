namespace CodeChange.Toolkit.Domain.Events
{
    using CSharpFunctionalExtensions;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a contract for a repository that manages domain event logs
    /// </summary>
    public interface IDomainEventLogRepository
    {
        /// <summary>
        /// Adds a single domain event log
        /// </summary>
        /// <param name="log">The event log to add</param>
        /// <returns>The result of the operation</returns>
        Result AddLog(DomainEventLog log);

        /// <summary>
        /// Asynchronously adds a single domain event log
        /// </summary>
        /// <param name="log">The event log to add</param>
        /// <param name="cancellationToken">The cancellation token (optional)</param>
        /// <returns>The result of the operation</returns>
        Task<Result> AddLogAsync(DomainEventLog log, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a single domain event log from the repository
        /// </summary>
        /// <param name="key">The log key</param>
        /// <returns>The domain event log</returns>
        Maybe<DomainEventLog> GetLog(string key);

        /// <summary>
        /// Asynchronously gets a single domain event log from the repository
        /// </summary>
        /// <param name="key">The log key</param>
        /// <param name="cancellationToken">The cancellation token (optional)</param>
        /// <returns>The domain event log</returns>
        Task<Maybe<DomainEventLog>> GetLogAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets domain event logs for a date range
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>A collection of domain event logs</returns>
        IEnumerable<DomainEventLog> GetLogs(DateTime startDate, DateTime? endDate);

        /// <summary>
        /// Asynchronously gets domain event logs for a date range
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>A collection of domain event logs</returns>
        Task<IEnumerable<DomainEventLog>> GetLogsAsync(DateTime startDate, DateTime? endDate, CancellationToken cancellationToken = default);
    }
}

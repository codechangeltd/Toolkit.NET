namespace CodeChange.Toolkit.Domain.Events
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a contract for a repository that manages domain event logs
    /// </summary>
    public interface IDomainEventLogRepository
    {
        /// <summary>
        /// Adds a single domain event log
        /// </summary>
        /// <param name="log">The event log to add</param>
        void AddLog
        (
            DomainEventLog log
        );

        /// <summary>
        /// Gets a single domain event log from the repository
        /// </summary>
        /// <param name="key">The log key</param>
        /// <returns>The domain event log</returns>
        DomainEventLog GetLog
        (
            string key
        );

        /// <summary>
        /// Gets a all domain event logs in the repository
        /// </summary>
        /// <returns>A collection of domain event logs</returns>
        IEnumerable<DomainEventLog> GetAllLogs();

        /// <summary>
        /// Gets domain event logs for a date range
        /// </summary>
        /// <returns>A collection of domain event logs</returns>
        IEnumerable<DomainEventLog> GetLogs
        (
            DateTime startDate,
            DateTime? endDate
        );
    }
}

namespace CodeChange.Toolkit.EntityFrameworkCore.Events
{
    using CodeChange.Toolkit.Domain.Events;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents an EF Core implementation for the domain event log repository
    /// </summary>
    public sealed class DomainEventLogRepository
        : RepositoryBase<DomainEventLog>, IDomainEventLogRepository
    {
        /// <summary>
        /// Constructs the repository with a database context instance
        /// </summary>
        /// <param name="context">The database context instance</param>
        public DomainEventLogRepository
            (
                DbContext context
            )
            : base(context)
        { }

        /// <summary>
        /// Adds a single domain event log
        /// </summary>
        /// <param name="log">The event log to add</param>
        public void AddLog
            (
                DomainEventLog log
            )
        {
            Validate.IsNotNull(log);

            this.AddEntity(log);
        }

        /// <summary>
        /// Gets a single domain event log from the repository
        /// </summary>
        /// <param name="key">The log key</param>
        /// <returns>The domain event log</returns>
        public DomainEventLog GetLog
            (
                string key
            )
        {
            Validate.IsNotEmpty(key);

            return this.GetEntityByLookupKey
            (
                key,
                true
            );
        }

        /// <summary>
        /// Gets a all domain event logs in the repository
        /// </summary>
        /// <returns>A collection of domain event logs</returns>
        public IEnumerable<DomainEventLog> GetAllLogs()
        {
            return this.GetAll().OrderByDescending
            (
                a => a.DateCreated
            );
        }

        /// <summary>
        /// Gets domain event logs for a date range
        /// </summary>
        /// <returns>A collection of domain event logs</returns>
        public IEnumerable<DomainEventLog> GetLogs
            (
                DateTime startDate,
                DateTime? endDate
            )
        {
            var logs = this.GetAll().Where
            (
                m => m.DateCreated >= startDate
            );

            if (endDate.HasValue)
            {
                logs = logs.Where
                (
                    m => m.DateCreated <= endDate.Value
                );
            }

            return logs.OrderByDescending
            (
                a => a.DateCreated
            );
        }
    }
}

namespace CodeChange.Toolkit.EntityFrameworkCore.Events
{
    using CodeChange.Toolkit.Domain.Events;
    using Microsoft.EntityFrameworkCore;
    using System;

    /// <summary>
    /// Represents an EF implementation of a domain event logger
    /// </summary>
    public sealed class DomainEventLogger : IDomainEventLogger
    {
        private readonly IDomainEventLogRepository _logRepository;
        private readonly DbContext _dbContext;

        /// <summary>
        /// Constructs the domain event logger with required dependencies
        /// </summary>
        /// <param name="logRepository">The event log repository</param>
        /// <param name="dbContext">The DB context</param>
        public DomainEventLogger
            (
                IDomainEventLogRepository logRepository,
                DbContext dbContext
            )
        {
            Validate.IsNotNull(logRepository);
            Validate.IsNotNull(dbContext);

            _logRepository = logRepository;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Logs the domain event specified
        /// </summary>
        /// <param name="event">The domain event</param>
        public void LogEvent
            (
                IDomainEvent @event
            )
        {
            Validate.IsNotNull(@event);

            var log = DomainEventLog.CreateLog
            (
                @event
            );

            _logRepository.AddLog(log);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Logs the domain event specified
        /// </summary>
        /// <param name="aggregateKey">The aggregate key</param>
        /// <param name="aggregateType">The aggregate type</param>
        /// <param name="event">The domain event</param>
        public void LogEvent
            (
                string aggregateKey,
                Type aggregateType,
                IDomainEvent @event
            )
        {
            Validate.IsNotEmpty(aggregateKey);
            Validate.IsNotNull(aggregateType);
            Validate.IsNotNull(@event);

            var log = DomainEventLog.CreateLog
            (
                aggregateKey,
                aggregateType,
                @event
            );

            _logRepository.AddLog(log);
            _dbContext.SaveChanges();
        }
    }
}

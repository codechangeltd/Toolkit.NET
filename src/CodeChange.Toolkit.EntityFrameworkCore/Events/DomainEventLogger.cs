namespace CodeChange.Toolkit.EntityFrameworkCore.Events
{
    using CodeChange.Toolkit.Domain.Events;
    using Microsoft.EntityFrameworkCore;
    using System;

    /// <summary>
    /// Represents an EF Core implementation of a domain event logger
    /// </summary>
    public sealed class DomainEventLogger : IDomainEventLogger
    {
        private readonly IDomainEventLogRepository _logRepository;
        private readonly DbContext _dbContext;

        public DomainEventLogger(IDomainEventLogRepository logRepository, DbContext dbContext)
        {
            Validate.IsNotNull(logRepository);
            Validate.IsNotNull(dbContext);

            _logRepository = logRepository;
            _dbContext = dbContext;
        }

        public void LogEvent(IDomainEvent @event)
        {
            Validate.IsNotNull(@event);

            var log = DomainEventLog.CreateLog(@event);

            _logRepository.AddLog(log);
            _dbContext.SaveChanges();
        }

        public void LogEvent(string aggregateKey, Type aggregateType, IDomainEvent @event)
        {
            Validate.IsNotEmpty(aggregateKey);
            Validate.IsNotNull(aggregateType);
            Validate.IsNotNull(@event);

            var log = DomainEventLog.CreateLog(aggregateKey, aggregateType, @event);

            _logRepository.AddLog(log);
            _dbContext.SaveChanges();
        }
    }
}

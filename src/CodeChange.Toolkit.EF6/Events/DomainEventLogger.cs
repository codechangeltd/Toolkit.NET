namespace CodeChange.Toolkit.EF6.Events
{
    using CodeChange.Toolkit.Domain.Events;
    using System;
    using System.Data.Entity;

    /// <summary>
    /// Represents an EF6 implementation of a domain event logger
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

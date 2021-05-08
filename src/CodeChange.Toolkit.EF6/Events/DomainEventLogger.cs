namespace CodeChange.Toolkit.EF6.Events
{
    using CodeChange.Toolkit.Domain.Events;
    using CSharpFunctionalExtensions;
    using System;
    using System.Data.Entity;
    using System.Threading;
    using System.Threading.Tasks;

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

        public Result LogEvent(IDomainEvent @event)
        {
            var log = DomainEventLog.CreateLog(@event);

            return _logRepository.AddLog(log).Tap(() => _dbContext.SaveChanges());
        }

        public Result LogEvent(string aggregateKey, Type aggregateType, IDomainEvent @event)
        {
            var log = DomainEventLog.CreateLog(aggregateKey, aggregateType, @event);

            return _logRepository.AddLog(log).Tap(() => _dbContext.SaveChanges());
        }

        public Task<Result> LogEventAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result> LogEventAsync(string aggregateKey, Type aggregateType, IDomainEvent @event, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}

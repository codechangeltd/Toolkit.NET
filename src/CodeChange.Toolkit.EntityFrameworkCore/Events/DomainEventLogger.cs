namespace CodeChange.Toolkit.EntityFrameworkCore.Events
{
    using CodeChange.Toolkit.Domain.Events;
    using CSharpFunctionalExtensions;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

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

        public Result LogEvent(IDomainEvent @event)
        {
            var log = DomainEventLog.CreateLog(@event);

            return _logRepository.AddLog(log).Tap(() => _dbContext.SaveChanges());
        }

        public async Task<Result> LogEventAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
        {
            var log = DomainEventLog.CreateLog(@event);
            var addResult = await _logRepository.AddLogAsync(log).ConfigureAwait(false);

            return await addResult.Tap(async () => await _dbContext.SaveChangesAsync());
        }

        public Result LogEvent(string aggregateKey, Type aggregateType, IDomainEvent @event)
        {
            var log = DomainEventLog.CreateLog(aggregateKey, aggregateType, @event);

            return _logRepository.AddLog(log).Tap(() => _dbContext.SaveChanges());
        }

        public async Task<Result> LogEventAsync(string aggregateKey, Type aggregateType, IDomainEvent @event, CancellationToken cancellationToken = default)
        {
            var log = DomainEventLog.CreateLog(aggregateKey, aggregateType, @event);
            var addResult = await _logRepository.AddLogAsync(log).ConfigureAwait(false);

            return await addResult.Tap(async () => await _dbContext.SaveChangesAsync());
        }
    }
}

namespace CodeChange.Toolkit.EF6.Events
{
    using CodeChange.Toolkit.Domain.Events;
    using CSharpFunctionalExtensions;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an EF implementation for the domain event log repository
    /// </summary>
    public sealed class DomainEventLogRepository : RepositoryBase<DomainEventLog>, IDomainEventLogRepository
    {
        public DomainEventLogRepository(DbContext context)
            : base(context)
        { }

        public void AddLog(DomainEventLog log)
        {
            AddEntity(log);
        }

        public DomainEventLog GetLog(string key)
        {
            return GetEntityByLookupKey(key, true);
        }

        public IEnumerable<DomainEventLog> GetLogs(DateTime startDate, DateTime? endDate)
        {
            var logs = GetAll().Where(_ => _.DateCreated >= startDate);

            if (endDate.HasValue)
            {
                logs = logs.Where(_ => _.DateCreated <= endDate.Value);
            }

            return logs.OrderByDescending(_ => _.DateCreated);
        }

        Result IDomainEventLogRepository.AddLog(DomainEventLog log)
        {
            throw new NotImplementedException();
        }

        public Task<Result> AddLogAsync(DomainEventLog log, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        Maybe<DomainEventLog> IDomainEventLogRepository.GetLog(string key)
        {
            throw new NotImplementedException();
        }

        public Task<Maybe<DomainEventLog>> GetLogAsync(string key, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DomainEventLog>> GetLogsAsync(DateTime startDate, DateTime? endDate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}

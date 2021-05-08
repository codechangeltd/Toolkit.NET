namespace CodeChange.Toolkit.EntityFrameworkCore.Events
{
    using CodeChange.Toolkit.Domain.Events;
    using CSharpFunctionalExtensions;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an EF Core implementation for the domain event log repository
    /// </summary>
    public sealed class DomainEventLogRepository : RepositoryBase<DomainEventLog>, IDomainEventLogRepository
    {
        public DomainEventLogRepository(DbContext context)
            : base(context)
        { }

        public Result AddLog(DomainEventLog log)
        {
            return AddEntity(log);
        }

        public async Task<Result> AddLogAsync(DomainEventLog log, CancellationToken cancellationToken = default)
        {
            return await AddEntityAsync(log, cancellationToken).ConfigureAwait(false);
        }

        public Maybe<DomainEventLog> GetLog(string key)
        {
            return GetEntity(key, true);
        }

        public async Task<Maybe<DomainEventLog>> GetLogAsync(string key, CancellationToken cancellationToken = default)
        {
            return await GetEntityAsync(key, true, cancellationToken).ConfigureAwait(false);
        }

        public IEnumerable<DomainEventLog> GetLogs(DateTime startDate, DateTime? endDate)
        {
            return GenerateLogQuery(startDate, endDate);
        }

        public async Task<IEnumerable<DomainEventLog>> GetLogsAsync(DateTime startDate, DateTime? endDate, CancellationToken cancellationToken = default)
        {
            return await GenerateLogQuery(startDate, endDate).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        private IQueryable<DomainEventLog> GenerateLogQuery(DateTime startDate, DateTime? endDate)
        {
            var query = GetAll().Where(_ => _.DateCreated >= startDate);

            if (endDate.HasValue)
            {
                query = query.Where(_ => _.DateCreated <= endDate.Value);
            }

            return query.OrderByDescending(_ => _.DateCreated);
        }
    }
}

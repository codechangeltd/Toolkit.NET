namespace CodeChange.Toolkit.EntityFrameworkCore
{
    using CodeChange.Toolkit.Domain.Events;
    using CodeChange.Toolkit.Persistence;
    using Microsoft.EntityFrameworkCore;
    using Nito.AsyncEx.Synchronous;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An Entity Framework Core unit of work implementation
    /// </summary>
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IDomainEventLogger _eventLogger;

        public UnitOfWork(DbContext context, IEventDispatcher eventDispatcher, IDomainEventLogger eventLogger)
        {
            Validate.IsNotNull(context);
            Validate.IsNotNull(eventDispatcher);
            Validate.IsNotNull(eventLogger);

            _context = context;
            _eventDispatcher = eventDispatcher;
            _eventLogger = eventLogger;
        }

        public void RefreshAll()
        {
            foreach (var entity in _context.ChangeTracker.Entries())
            {
                entity.Reload();
            }
        }

        public async Task RefreshAllAsync(CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task>();

            foreach (var entity in _context.ChangeTracker.Entries())
            {
                tasks.Add(entity.ReloadAsync(cancellationToken));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public int SaveChanges()
        {
            return SaveChangesAsync(_context).WaitAndUnwrapException();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await SaveChangesAsync(_context, cancellationToken).ConfigureAwait(false);
        }

        private async Task<int> SaveChangesAsync
            (
                DbContext context,
                CancellationToken cancellationToken = default
            )
        {
            Validate.IsNotNull(context);

            var success = false;
            var aggregates = _context.GetPendingAggregates().ToArray();
            var preEventQueue = EventQueueFactory.CreatePreTransactionEventQueue(aggregates);
            var postEventQueue = EventQueueFactory.CreatePostTransactionEventQueue(aggregates);
            var rows = default(int);

            ProcessEventQueue(preEventQueue, true);

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    rows = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    transaction.Commit();
                    success = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    if (ex is DBConcurrencyException)
                    {
                        // NOTE:
                        // For concurrency exceptions we want to ensure the 
                        // entities are not cached in the context so we can 
                        // stop the same error being raised indefinitely.

                        await RefreshAllAsync(cancellationToken).ConfigureAwait(false);
                    }

                    throw;
                }
            }

            if (success)
            {
                foreach (var aggregate in aggregates)
                {
                    if (aggregate.UnpublishedEvents != null)
                    {
                        aggregate.UnpublishedEvents.Clear();
                    }
                }

                ProcessEventQueue(postEventQueue);
            }

            return rows;
        }

        /// <summary>
        /// Processes an event queue by dispatching the events
        /// </summary>
        /// <param name="preTransaction">True, if pre-transaction handlers required</param>
        /// <param name="queue">The event queue to process</param>
        private void ProcessEventQueue(IEventQueue queue, bool preTransaction = false)
        {
            while (false == queue.IsEmpty())
            {
                var nextItem = queue.GetNext();

                _eventDispatcher.Dispatch(nextItem.Event, preTransaction);

                // We don't want to log pre-transaction events
                if (false == preTransaction)
                {
                    _eventLogger.LogEvent
                    (
                        nextItem.AggregateKey,
                        nextItem.AggregateType,
                        nextItem.Event
                    );
                }
            }
        }

        public void Dispose()
        {
            // NOTE:
            // This was disabled after having issues with the DbContext being disposed early.
            // See this Stack Overflow answer for more: http://stackoverflow.com/a/35587389

            // In a nutshell, Autofac would create an instance of a repository and DbContext
            // per lifetime scope but a new instance of IUnitOfWork for different threads 
            // in the same lifetime scope. At some point the DbContext gets disposed but when 
            // a new instance of IUnitOfWork is instantiated it uses the old instance of the
            // repository and DbContext that was created for the current lifetime scope 
            // (which has already been disposed).

            //_context.Dispose();
        }
    }
}

namespace CodeChange.Toolkit.EntityFrameworkCore
{
    using CodeChange.Toolkit.Domain.Aggregate;
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

        /// <summary>
        /// Constructs the repository with an Entity Framework database context instance
        /// </summary>
        /// <param name="context">The database context instance</param>
        /// <param name="eventDispatcher">The domain event dispatcher</param>
        /// <param name="eventLogger">The domain event logger</param>
        public UnitOfWork
            (
                DbContext context,
                IEventDispatcher eventDispatcher,
                IDomainEventLogger eventLogger
            )
        {
            Validate.IsNotNull(context);
            Validate.IsNotNull(eventDispatcher);
            Validate.IsNotNull(eventLogger);

            _context = context;
            _eventDispatcher = eventDispatcher;
            _eventLogger = eventLogger;
        }

        /// <summary>
        /// Refreshes all objects being tracked with data from the data source
        /// </summary>
        public void RefreshAll()
        {
            foreach (var entity in _context.ChangeTracker.Entries())
            {
                entity.Reload();
            }
        }

        /// <summary>
        /// Asynchronously refreshes all objects being tracked with data from the data source
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        public async Task RefreshAllAsync
            (
                CancellationToken cancellationToken = default
            )
        {
            var tasks = new List<Task>();

            foreach (var entity in _context.ChangeTracker.Entries())
            {
                tasks.Add
                (
                    entity.ReloadAsync(cancellationToken)
                );
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        /// <summary>
        /// Saves all changes made in this context to the underlying database
        /// </summary>
        /// <returns>The number of objects written to the underlying database</returns>
        public int SaveChanges()
        {
            return SaveChangesAsync(_context).WaitAndUnwrapException();
        }

        /// <summary>
        /// Asynchronously saves all changes made in unit to the underlying database
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The number of objects written to the underlying database</returns>
        public async Task<int> SaveChangesAsync
            (
                CancellationToken cancellationToken = default
            )
        {
            var saveTask = SaveChangesAsync
            (
                _context,
                cancellationToken
            );

            return await saveTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database
        /// </summary>
        /// <param name="context">The DB context</param>
        /// <returns>The number of objects written to the underlying database</returns>
        private async Task<int> SaveChangesAsync
            (
                DbContext context,
                CancellationToken cancellationToken = default
            )
        {
            Validate.IsNotNull(context);

            var success = false;
            var preEventQueue = GenerateEventQueue(true);
            var postEventQueue = GenerateEventQueue();
            var aggregates = GetPendingAggregates();
            var rows = default(int);

            ProcessEventQueue
            (
                preEventQueue,
                true
            );

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var saveTask = context.SaveChangesAsync
                    (
                        cancellationToken
                    );

                    rows = await saveTask.ConfigureAwait(false);
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

                        var refreshTask = RefreshAllAsync(cancellationToken);

                        await refreshTask.ConfigureAwait(false);
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
        private void ProcessEventQueue
            (
                IEventQueue queue,
                bool preTransaction = false
            )
        {
            while (false == queue.IsEmpty())
            {
                var nextItem = queue.GetNext();

                _eventDispatcher.Dispatch
                (
                    nextItem.Event,
                    preTransaction
                );

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

        /// <summary>
        /// Generates a queue of unpublished domain events
        /// </summary>
        /// <param name="preTransaction">True, if pre-transaction events are required</param>
        /// <returns>A collection of domain events</returns>
        private IEventQueue GenerateEventQueue
            (
                bool preTransaction = false
            )
        {
            var aggregates = GetPendingAggregates();
            var queue = new EventQueue();

            foreach (var aggregate in aggregates)
            {
                var aggregateKey = aggregate.GetKeyValue();
                var aggregateType = aggregate.GetType();
                var nextEvents = default(IList<IDomainEvent>);

                if (preTransaction)
                {
                    nextEvents = aggregate.GetPreTransactionEvents();
                }
                else
                {
                    nextEvents = aggregate.GetPostTransactionEvents();
                }

                if (nextEvents != null && nextEvents.Any())
                {
                    foreach (var @event in nextEvents)
                    {
                        queue.Add
                        (
                            aggregateKey,
                            aggregateType,
                            @event
                        );
                    }
                }
            }

            return queue;
        }

        /// <summary>
        /// Gets all aggregates that are pending saving
        /// </summary>
        /// <returns>A collection of aggregate roots</returns>
        private IEnumerable<IAggregateRoot> GetPendingAggregates()
        {
            var changeTracker = _context.ChangeTracker;
            
            var entries = changeTracker.Entries().Where
            (
                x => x.Entity.GetType().ImplementsInterface
                (
                    typeof(IAggregateRoot)
                )
            );

            var aggregates = new List<IAggregateRoot>();

            foreach (var entry in entries.ToList())
            {
                aggregates.Add
                (
                    (IAggregateRoot)entry.Entity
                );
            }

            return aggregates;
        }

        /// <summary>
        /// Forces the database context dispose method to run
        /// </summary>
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

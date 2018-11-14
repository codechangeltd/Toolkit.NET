﻿namespace CodeChange.Toolkit.EF6
{
    using CodeChange.Toolkit.Domain.Aggregate;
    using CodeChange.Toolkit.Domain.Events;
    using CodeChange.Toolkit.Persistence;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Core;
    using System.Data.Entity.SqlServer;
    using System.Linq;

    /// <summary>
    /// Represents an Entity Framework unit of work implementation
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
        /// Saves all changes made in this context to the underlying database
        /// </summary>
        /// <returns>The number of objects written to the underlying database</returns>
        public int SaveChanges()
        {
            return SaveChanges
            (
                _context
            );
        }

        /// <summary>
        /// Saves all changes made in this context to the underlying database
        /// </summary>
        /// <param name="context">The DB context</param>
        /// <returns>The number of objects written to the underlying database</returns>
        private int SaveChanges
            (
                DbContext context
            )
        {
            Validate.IsNotNull(context);

            var success = false;
            var preEventQueue = GenerateEventQueue(true);
            var postEventQueue = GenerateEventQueue();
            var rows = default(int);

            ProcessEventQueue
            (
                preEventQueue,
                true
            );

            var executionStrategy = new SqlAzureExecutionStrategy();

            AzureEfConfiguration.SuspendExecutionStrategy = true;

            executionStrategy.Execute
            (
                () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            rows = context.SaveChanges();
                            transaction.Commit();

                            success = true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            if (ex is DBConcurrencyException
                                || ex is OptimisticConcurrencyException)
                            {
                                // NOTE:
                                // For concurrency exceptions we want to ensure the 
                                // entities are not cached in the context so we can 
                                // stop the same error being raised indefinitely.

                                RefreshAll();
                            }

                            throw;
                        }
                    }
                }
            );

            AzureEfConfiguration.SuspendExecutionStrategy = false;

            if (success)
            {
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
        /// Generates a new queue of unpublished domain events
        /// </summary>
        /// <param name="preTransaction">True, if pre-transaction events are required</param>
        /// <returns>A collection of domain events</returns>
        private IEventQueue GenerateEventQueue
            (
                bool preTransaction = false
            )
        {
            var changeTracker = _context.ChangeTracker;
            var queue = new EventQueue();

            // Find all pending entities of the type TEntity so we can move them
            var entries = changeTracker.Entries().Where
            (
                x => x.Entity.GetType().ImplementsInterface
                (
                    typeof(IAggregateRoot)
                )
            );

            foreach (var entry in entries.ToList())
            {
                var entity = (IAggregateRoot)entry.Entity;
                var aggregateKey = entity.GetKeyValue();
                var aggregateType = entity.GetType();
                var nextEvents = default(IList<IDomainEvent>);

                if (preTransaction)
                {
                    nextEvents = entity.GetPreTransactionEvents();
                }
                else
                {
                    nextEvents = entity.GetPostTransactionEvents();
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

                    entity.UnpublishedEvents.Clear();
                }
            }

            return queue;
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
﻿namespace CodeChange.Toolkit.EntityFrameworkCore;

using Nito.AsyncEx.Synchronous;
using System.Data;

/// <summary>
/// Represents an Entity Framework Core Unit of Work implementation
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext[] _contexts;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IDomainEventLogger _eventLogger;

    public UnitOfWork(DbContext context, IEventDispatcher eventDispatcher, IDomainEventLogger eventLogger)
    {
        _contexts = new DbContext[1] { context };
        _eventDispatcher = eventDispatcher;
        _eventLogger = eventLogger;
    }

    public UnitOfWork(IEventDispatcher eventDispatcher, IDomainEventLogger eventLogger, params DbContext[] contexts)
    {
        // NOTE:
        // Ensure the connection string is the same withing each context,
        // because we cannot currently handle distributed transactions.

        if (contexts.Length < 1)
        {
            throw new ArgumentException("At least one DbContext is required.");
        }

        var firstConnectionString = contexts.First().Database.GetConnectionString();
        var allHaveSameConnection = contexts.All(x => x.Database.GetConnectionString() == firstConnectionString);

        if (false == allHaveSameConnection)
        {
            throw new ArgumentException("The connection string must be the same for each DbContext.");
        }

        _eventDispatcher = eventDispatcher;
        _eventLogger = eventLogger;
        _contexts = contexts;
    }

    public void RefreshAll()
    {
        foreach (var context in _contexts)
        {
            foreach (var entity in context.ChangeTracker.Entries())
            {
                entity.Reload();
            }
        }
    }

    public async Task RefreshAllAsync(CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>();

        foreach (var context in _contexts)
        {
            foreach (var entity in context.ChangeTracker.Entries())
            {
                tasks.Add(entity.ReloadAsync(cancellationToken));
            }
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public int SaveChanges()
    {
        return SaveChangesAsync(_contexts).WaitAndUnwrapException();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await SaveChangesAsync(_contexts, cancellationToken).ConfigureAwait(false);
    }

    private async Task<int> SaveChangesAsync(DbContext[] contexts, CancellationToken cancellationToken = default)
    {
        var success = false;
        var rows = default(int);

        var aggregates = contexts.GetPendingAggregates();

        await ProcessPreTransactionEvents().ConfigureAwait(false);

        foreach (var context in contexts)
        {
            await context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken).ConfigureAwait(false);
        }

        try
        {
            foreach (var context in contexts)
            {
                rows += await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            foreach (var context in contexts)
            {
                await context.Database.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);

                success = true;
            }
        }
        catch (Exception ex)
        {
            foreach (var context in contexts)
            {
                await context.Database.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);

                if (ex is DBConcurrencyException)
                {
                    // NOTE:
                    // For concurrency exceptions we want to ensure the 
                    // entities are not cached in the context so we can 
                    // stop the same error being raised indefinitely.

                    await RefreshAllAsync(cancellationToken).ConfigureAwait(false);
                }
            }

            throw;
        }

        if (success)
        {
            await ProcessPostTransactionEvents().ConfigureAwait(false);
        }

        async Task ProcessPreTransactionEvents()
        {
            IEventQueue CreateQueue()
            {
                return EventQueueFactory.CreatePreTransactionEventQueue(aggregates);
            }

            var eventQueue = CreateQueue();

            while (false == eventQueue.IsEmpty())
            {
                var preProcessItems = eventQueue.ToList();

                await ProcessEventQueue(eventQueue, true).ConfigureAwait(false);

                aggregates = contexts.GetPendingAggregates();
                eventQueue = CreateQueue().Remove(preProcessItems);
            }
        }

        async Task ProcessPostTransactionEvents()
        {
            var eventQueue = EventQueueFactory.CreatePostTransactionEventQueue(aggregates);

            foreach (var aggregate in aggregates)
            {
                aggregate.UnpublishedEvents?.Clear();
            }

            await ProcessEventQueue(eventQueue).ConfigureAwait(false);
        }

        return rows;
    }

    /// <summary>
    /// Asynchronously processes an event queue by dispatching the events
    /// </summary>
    /// <param name="preTransaction">True, if pre-transaction handlers required</param>
    /// <param name="queue">The event queue to process</param>
    private async Task ProcessEventQueue(IEventQueue queue, bool preTransaction = false)
    {
        while (false == queue.IsEmpty())
        {
            var nextItem = queue.GetNext();
            var @event = nextItem.Event;

            await _eventDispatcher.DispatchAsync(@event, preTransaction).ConfigureAwait(false);

            // We don't want to log pre-transaction events
            if (false == preTransaction)
            {
                var key = nextItem.AggregateKey;
                var type = nextItem.AggregateType;

                await _eventLogger.LogEventAsync(key, type, @event).ConfigureAwait(false);
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

        GC.SuppressFinalize(this);
    }
}

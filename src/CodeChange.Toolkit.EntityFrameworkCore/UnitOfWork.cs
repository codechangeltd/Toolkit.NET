namespace CodeChange.Toolkit.EntityFrameworkCore;

using Nito.AsyncEx.Synchronous;
using System.Data;
using System.Data.SqlClient;

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

        var connectionString = contexts.First().Database.GetConnectionString();

        using (var connection = new SqlConnection(connectionString))
        using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
        {
            try
            {
                foreach (var context in contexts)
                {
                    await context.Database.UseTransactionAsync(transaction, cancellationToken).ConfigureAwait(false);
                    rows += await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }

                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

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
                if (aggregate.UnpublishedEvents != null)
                {
                    aggregate.UnpublishedEvents.Clear();
                }
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
        var queueTasks = new List<Task>();

        while (false == queue.IsEmpty())
        {
            var nextItem = queue.GetNext();
            var dispatchTask = _eventDispatcher.DispatchAsync(nextItem.Event, preTransaction);

            queueTasks.Add(dispatchTask);

            // We don't want to log pre-transaction events
            if (false == preTransaction)
            {
                var logTask = _eventLogger.LogEventAsync
                (
                    nextItem.AggregateKey,
                    nextItem.AggregateType,
                    nextItem.Event
                );

                queueTasks.Add(logTask);
            }
        }

        await Task.WhenAll(queueTasks).ConfigureAwait(false);
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

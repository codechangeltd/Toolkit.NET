namespace CodeChange.Toolkit.EntityFrameworkCore;

using Nito.AsyncEx.Synchronous;
using Paginator;
using Paginator.Async;
using Paginator.Async.EntityFrameworkCore;

/// <summary>
/// A base class for Entity Framework Core repositories
/// </summary>
/// <typeparam name="TRoot">The aggregate root type</typeparam>
public abstract class RepositoryBase<TRoot> where TRoot : class, IAggregateRoot
{
    private readonly DbSet<TRoot> _readSet;
    private readonly DbSet<TRoot> _writeSet;
    private List<PropertyInfo>? _navigationProperties;

    public RepositoryBase(DbContext context)
    {
        _readSet = context.Set<TRoot>();
        _writeSet = context.Set<TRoot>();

        ReadContext = context;
        WriteContext = context;
    }

    public RepositoryBase(DbContext readContext, DbContext writeContext)
    {
        _readSet = readContext.Set<TRoot>();
        _writeSet = writeContext.Set<TRoot>();

        ReadContext = readContext;
        WriteContext = writeContext;
    }

    /// <summary>
    /// The read-only database context
    /// </summary>
    public DbContext ReadContext { get; }

    /// <summary>
    /// The write-only database context
    /// </summary>
    public DbContext WriteContext { get; }

    /// <summary>
    /// Gets a list of navigation properties found for the aggregate type
    /// </summary>
    protected List<PropertyInfo> NavigationProperties
    {
        get
        {
            _navigationProperties ??= GetNavigationProperties();

            return _navigationProperties;
        }
    }

    /// <summary>
    /// Uses reflection to get a list of navigation properties for the entity specified
    /// </summary>
    /// <returns>A list of matching navigation property</returns>
    protected List<PropertyInfo> GetNavigationProperties()
    {
        var entityType = typeof(TRoot);

        var properties = ReadContext.Model
            .GetEntityTypes()
            .Where(x => x.ClrType == entityType)
            .SelectMany(x => x.GetNavigations().Select(nav => nav.PropertyInfo));

        return properties.Where(info => info != null).ToList()!;
    }

    /// <summary>
    /// Default implementation for the dispose method
    /// </summary>
    /// <remarks>
    /// The database context dispose should be handled in the unit of work
    /// </remarks>
    public void Dispose() { }

    /// <summary>
    /// Adds a new entity to the set in the database context
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <returns>The result of the operation</returns>
    protected virtual Result AddEntity(TRoot entity) => AddEntityAsync(entity).WaitAndUnwrapException();

    /// <summary>
    /// Asynchronously adds a new entity to the set in the database context
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The result of the operation</returns>
    protected virtual async Task<Result> AddEntityAsync(TRoot entity, CancellationToken cancellationToken = default)
    {
        var key = entity.Key;
        var hasBeenUsed = await KeyUsedAsync(key, cancellationToken).ConfigureAwait(false);

        if (false == hasBeenUsed)
        {
            await _writeSet.AddAsync(entity, cancellationToken);

            return Result.Success();
        }
        else
        {
            return Result.Failure($"An item with the key '{key}' has already been added.");
        }
    }

    /// <summary>
    /// Adds or updates an entity in the repository
    /// </summary>
    /// <param name="entity">The entity to add or update</param>
    /// <returns>The result of the operation</returns>
    protected virtual Result AddOrUpdateEntity(TRoot entity)
    {
        return AddOrUpdateEntityAsync(entity).WaitAndUnwrapException();
    }

    /// <summary>
    /// Asynchronously adds or updates an entity in the repository
    /// </summary>
    /// <param name="entity">The entity to add or update</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The result of the operation</returns>
    protected virtual async Task<Result> AddOrUpdateEntityAsync(TRoot entity, CancellationToken cancellationToken = default)
    {
        var context = WriteContext;
        var attached = _writeSet.Local.Any(x => x.Key == entity.Key);

        if (attached)
        {
            var entry = context.Entry(entity);

            if (entry.State != EntityState.Added)
            {
                entry.State = EntityState.Modified;
            }

            return Result.Success();
        }
        else
        {
            var untrackedSet = _writeSet.AsNoTracking();
            var countTask = untrackedSet.CountAsync(x => x.Key == entity.Key, cancellationToken);

            var usedCount = await countTask.ConfigureAwait(false);

            if (usedCount == 0)
            {
                return await AddEntityAsync(entity, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await UpdateEntityAsync(entity, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Determines if the key specified has already been used by another entity
    /// </summary>
    /// <param name="key">The key value to check</param>
    /// <returns>True, if the key has already been used; otherwise false</returns>
    protected virtual bool KeyUsed(string key)
    {
        return KeyUsedAsync(key).WaitAndUnwrapException();
    }

    /// <summary>
    /// Asynchronously determines if the key specified has already been used by another entity
    /// </summary>
    /// <param name="key">The key value to check</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True, if the key has already been used; otherwise false</returns>
    protected virtual async Task<bool> KeyUsedAsync(string key, CancellationToken cancellationToken = default)
    {
        if (String.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        var attached = _writeSet.Local.Any(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

        if (attached)
        {
            return true;
        }

        return await ExistsAsync(key, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a single entity from the database context using the key value
    /// </summary>
    /// <param name="key">The entities key value</param>
    /// <param name="useEagerLoading">If true, eager loading is applied to the entity</param>
    /// <returns>The matching entity wrapped into a Maybe</returns>
    protected virtual Maybe<TRoot?> GetEntity(string key, bool useEagerLoading = false)
    {
        return GetEntityAsync(key).WaitAndUnwrapException();
    }

    /// <summary>
    /// Asynchronously gets a single entity from the database context using the key value
    /// </summary>
    /// <param name="key">The entities key value</param>
    /// <param name="useEagerLoading">If true, eager loading is applied to the entity</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The matching entity wrapped into a Maybe</returns>
    protected virtual async Task<Maybe<TRoot?>> GetEntityAsync
        (
            string key,
            bool useEagerLoading = false,
            CancellationToken cancellationToken = default
        )
    {
        return await GetEntityAsync(x => x.Key == key, useEagerLoading, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously gets a single entity from the database context using the key value
    /// </summary>
    /// <param name="predicate">The search predicate</param>
    /// <param name="useEagerLoading">If true, eager loading is applied to the entity</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The matching entity wrapped into a Maybe</returns>
    private async Task<Maybe<TRoot?>> GetEntityAsync
        (
            Expression<Func<TRoot, bool>> predicate,
            bool useEagerLoading = false,
            CancellationToken cancellationToken = default
        )
    {
        var findTask = FindFirstOrDefaultAsync(predicate, useEagerLoading, cancellationToken);
        var entity = await findTask.ConfigureAwait(false);

        if (entity == default(TRoot))
        {
            // FALLBACK: if we can't find the entity in the database 
            // then try to find it in the change trackers added entries.
            var tracker = WriteContext.ChangeTracker;

            var addedEntities = tracker.Entries<TRoot>()
                .Where(entry => entry.State == EntityState.Added)
                .Select(entry => entry.Entity);

            entity = addedEntities.FirstOrDefault(predicate.Compile());
        }

        return entity;
    }

    /// <summary>
    /// Gets a collection of all entities from the database context
    /// </summary>
    /// <param name="useEagerLoading">If true, eager loading is applied to the query</param>
    /// <returns>A collection of all aggregate root entities in the database</returns>
    protected virtual IQueryable<TRoot> GetAll(bool useEagerLoading = false)
    {
        var query = (IQueryable<TRoot>)_readSet;

        if (useEagerLoading)
        {
            query = ApplyEagerLoading(query);
        }

        return query;
    }

    /// <summary>
    /// Finds all entities in the database matching a predicate
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="useEagerLoading">If true, eager loading is applied to the query</param>
    /// <returns>A query for finding the entities</returns>
    protected virtual IQueryable<TRoot> FindAll(Expression<Func<TRoot, bool>> predicate, bool useEagerLoading = false)
    {
        var query = _readSet.Where(predicate);

        if (useEagerLoading)
        {
            query = ApplyEagerLoading(query);
        }

        return query;
    }

    /// <summary>
    /// Paginates a query synchronously
    /// </summary>
    /// <param name="query">The query to paginate</param>
    /// <param name="pageSize">The maximum page size</param>
    /// <returns>A paged collection</returns>
    protected virtual IPagedCollection<TRoot> Paginate(IQueryable<TRoot> query, int pageSize)
    {
        return new PagedCollection<TRoot>(query, pageSize);
    }

    /// <summary>
    /// Paginates a query for asynchronous operations
    /// </summary>
    /// <param name="query">The query to paginate</param>
    /// <param name="pageSize">The maximum page size</param>
    /// <returns>An asynchronous paged collection</returns>
    protected virtual IAsyncPagedCollection<TRoot> PaginateAsync(IQueryable<TRoot> query, int pageSize)
    {
        return new EfCoreAsyncPagedCollection<TRoot>(query, pageSize);
    }

    /// <summary>
    /// Finds the first entity in the database matching a predicate
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="useEagerLoading">If true, eager loading is applied to the query</param>
    /// <returns>The matching entity, or the default value if not found</returns>
    protected virtual TRoot? FindFirstOrDefault(Expression<Func<TRoot, bool>> predicate, bool useEagerLoading = false)
    {
        return FindAll(predicate).FirstOrDefault();
    }

    /// <summary>
    /// Asynchronously finds the first entity in the database matching a predicate
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="useEagerLoading">If true, eager loading is applied to the query</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The matching entity, or the default value if not found</returns>
    protected virtual async Task<TRoot?> FindFirstOrDefaultAsync
        (
            Expression<Func<TRoot, bool>> predicate,
            bool useEagerLoading = false,
            CancellationToken cancellationToken = default
        )
    {
        return await FindAll(predicate).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Counts the number of entities in the database matching a predicate
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <returns>The number of entities found</returns>
    protected virtual int Count(Expression<Func<TRoot, bool>> predicate)
    {
        return _readSet.Count(predicate);
    }

    /// <summary>
    /// Asynchronously counts the number of entities in the database matching a predicate
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of entities found</returns>
    protected virtual async Task<int> CountAsync(Expression<Func<TRoot, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _readSet.AsNoTracking().CountAsync(predicate, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Determines if an entity exist
    /// </summary>
    /// <param name="key">The lookup key</param>
    /// <returns>True, if the entity was found; otherwise false</returns>
    protected virtual bool Exists(string key)
    {
        return ExistsAsync(key).WaitAndUnwrapException();
    }

    /// <summary>
    /// Asynchronously determines if an entity exist
    /// </summary>
    /// <param name="predicate">The lookup key</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True, if the entity was found; otherwise false</returns>
    protected virtual async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(x => x.Key == key, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Determines if an entity (matching a predicate) exist
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <returns>True, if the entity was found; otherwise false</returns>
    protected virtual bool Exists(Expression<Func<TRoot, bool>> predicate)
    {
        return ExistsAsync(predicate).WaitAndUnwrapException();
    }

    /// <summary>
    /// Asynchronously determines if an entity (matching a predicate) exist
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True, if the entity was found; otherwise false</returns>
    protected virtual async Task<bool> ExistsAsync(Expression<Func<TRoot, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var count = await _readSet.AsNoTracking()
            .CountAsync(predicate, cancellationToken)
            .ConfigureAwait(false);

        return count > 0;
    }

    /// <summary>
    /// Updates an entity and notifies the context tracker
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <returns>The result of the operation</returns>
    protected virtual Result UpdateEntity(TRoot entity)
    {
        return UpdateEntityAsync(entity).WaitAndUnwrapException();
    }

    /// <summary>
    /// Asynchronously updates an entity and notifies the context tracker
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The result of the operation</returns>
    protected virtual async Task<Result> UpdateEntityAsync(TRoot entity, CancellationToken cancellationToken = default)
    {
        var key = entity.Key;
        var countTask = CountAsync(x => x.Key == key, cancellationToken);

        var duplicateKeyCount = await countTask.ConfigureAwait(false);

        if (duplicateKeyCount == 0)
        {
            var entry = WriteContext.Entry(entity);

            // Ensure the entity has been attached to the object state manager
            if (entry.State == EntityState.Detached)
            {
                _writeSet.Attach(entity);
            }

            if (entry.State != EntityState.Added)
            {
                entry.State = EntityState.Modified;
            }

            return Result.Success();
        }
        else
        {
            return Result.Failure($"The key '{key}' has already been used by another entity.");
        }
    }

    /// <summary>
    /// Removes a single entity from the collection in the database context
    /// </summary>
    /// <param name="key">The lookup key of the entity to delete</param>
    /// <returns>The result of the operation</returns>
    protected virtual Result RemoveEntity(string key)
    {
        return GetEntity(key, false)
            .ToResult("The entity could not be removed because it was not found.")
            .Tap(entity => RemoveEntity(entity!));
    }

    /// <summary>
    /// Asynchronously removes a single entity from the collection in the database context
    /// </summary>
    /// <param name="key">The lookup key of the entity to delete</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The result of the operation</returns>
    protected virtual async Task<Result> RemoveEntityAsync(string key, CancellationToken cancellationToken = default)
    {
        return await GetEntityAsync(key, false, cancellationToken)
            .ToResult("The entity could not be removed because it was not found.")
            .Tap(entity => RemoveEntity(entity!));
    }

    /// <summary>
    /// Removes a single entity from the collection in the database context
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    /// <returns>The result of the operation</returns>
    protected virtual void RemoveEntity(TRoot entity)
    {
        entity.Destroy();

        var dbEntry = WriteContext.Entry(entity);

        // Ensure the entity has been attached to the object state manager
        if (dbEntry.State == EntityState.Detached)
        {
            _writeSet.Attach(entity);
        }

        _writeSet.Remove(entity);
    }

    /// <summary>
    /// Applies eager loading logic to the query specified
    /// </summary>
    /// <param name="query">The query to apply eager loading to</param>
    /// <returns>The query, with eager loading applied</returns>
    protected virtual IQueryable<TRoot> ApplyEagerLoading(IQueryable<TRoot> query)
    {
        if (NavigationProperties != null)
        {
            foreach (var property in NavigationProperties)
            {
                query.Include(property.Name);
            }
        }

        return query;
    }
}

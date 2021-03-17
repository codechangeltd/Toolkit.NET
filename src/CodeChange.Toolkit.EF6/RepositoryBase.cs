﻿namespace CodeChange.Toolkit.EF6
{
    using CodeChange.Toolkit.Domain.Aggregate;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents the base class for all Entity Framework 6 repositories
    /// </summary>
    /// <typeparam name="TRoot">The aggregate root entity type</typeparam>
    public abstract class RepositoryBase<TRoot> : IAggregateRepository<TRoot>
        where TRoot : class, IAggregateRoot
    {
        private List<PropertyInfo> _navigationProperties;

        public RepositoryBase(DbContext context)
        {
            Validate.IsNotNull(context);

            this.ReadContext = context;
            this.WriteContext = context;
        }

        public RepositoryBase(DbContext readContext, DbContext writeContext)
        {
            Validate.IsNotNull(readContext);
            Validate.IsNotNull(writeContext);

            this.ReadContext = readContext;
            this.WriteContext = writeContext;
        }

        /// <summary>
        /// Gets a the read-only database context
        /// </summary>
        public DbContext ReadContext { get; }

        /// <summary>
        /// Gets a the write-only database context
        /// </summary>
        public DbContext WriteContext { get; }

        /// <summary>
        /// Gets a list of navigation properties found for the aggregate type
        /// </summary>
        protected List<PropertyInfo> NavigationProperties
        {
            get
            {
                if (_navigationProperties == null)
                {
                    _navigationProperties = GetNavigationProperties();
                }

                return _navigationProperties;
            }
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
        protected virtual void AddEntity(TRoot entity)
        {
            Validate.IsNotNull(entity);

            var key = entity.GetKeyValue();
            var hasBeenUsed = HasKeyBeenUsed(key);

            if (false == hasBeenUsed)
            {
                entity.DateCreated = DateTime.UtcNow;
                entity.DateModified = DateTime.UtcNow;

                this.WriteContext.Set<TRoot>().Add(entity);
            }
            else
            {
                throw new InvalidOperationException
                (
                    $"An item with the key '{key}' has already been added."
                );
            }
        }

        /// <summary>
        /// Adds or updates an entity in the repository
        /// </summary>
        /// <param name="entity">The entity to add or update</param>
        protected virtual void AddOrUpdateEntity(TRoot entity)
        {
            var context = this.WriteContext;
            var set = context.Set<TRoot>();

            var attached = set.Local.Any
            (
                _ => _.LookupKey.Equals(entity.LookupKey, StringComparison.OrdinalIgnoreCase)
            );

            if (attached)
            {
                var entry = context.Entry(entity);

                if (entry.State != EntityState.Added)
                {
                    entry.State = EntityState.Modified;
                }
            }
            else
            {
                var usedCount = set.AsNoTracking().Count
                (
                    _ => _.LookupKey.Equals(entity.LookupKey, StringComparison.OrdinalIgnoreCase)
                );

                if (usedCount == 0)
                {
                    AddEntity(entity);
                }
                else
                {
                    UpdateEntity(entity);
                }
            }
        }

        /// <summary>
        /// Determines if the key specified has already been used by another entity of the same type
        /// </summary>
        /// <param name="key">The key value to check</param>
        /// <returns>True, if the key has already been used; otherwise false</returns>
        protected virtual bool HasKeyBeenUsed(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return false;
            }

            var set = this.WriteContext.Set<TRoot>();

            var attached = set.Local.Any
            (
                _ => _.LookupKey.Equals(key, StringComparison.OrdinalIgnoreCase)
            );

            if (attached)
            {
                return true;
            }

            var usedCount = set.AsNoTracking().Count
            (
                _ => _.LookupKey.Equals(key, StringComparison.OrdinalIgnoreCase)
            );

            return usedCount > 0;
        }

        /// <summary>
        /// Gets a single entity from the database context using the key value
        /// </summary>
        /// <param name="key">The entities key value</param>
        /// <param name="useEagerLoading">If true, eager loading is applied to the entity</param>
        /// <returns>The matching entity</returns>
        protected virtual TRoot GetEntityByLookupKey(string key, bool useEagerLoading = false)
        {
            Validate.IsNotEmpty(key);

            var entity = GetAll(useEagerLoading).FirstOrDefault
            (
                _ => _.LookupKey.Equals(key, StringComparison.OrdinalIgnoreCase)
            );

            if (entity == default(TRoot))
            {
                // FALLBACK: if we can't find the entity in the database 
                // then try to find it in the change trackers added entries.
                var tracker = this.WriteContext.ChangeTracker;

                var addedEntities = tracker.Entries<TRoot>()
                    .Where(_ => _.State == EntityState.Added)
                    .Select(_ => _.Entity);

                entity = addedEntities.FirstOrDefault
                (
                    _ => _.LookupKey.Equals(key, StringComparison.OrdinalIgnoreCase)
                );

                if (entity == default(TRoot))
                {
                    var typeName = typeof(TRoot).Name;

                    throw new EntityNotFoundException
                    (
                        key,
                        $"Key does not match any {typeName} entities in the repository."
                    );
                }
            }

            return entity;
        }

        /// <summary>
        /// Gets a single entity from the database context using the ID value
        /// </summary>
        /// <param name="id">The entities ID value</param>
        /// <param name="useEagerLoading">If true, eager loading is applied to the entity</param>
        /// <returns>The matching entity</returns>
        protected virtual TRoot GetEntityById(long id, bool useEagerLoading = false)
        {
            var entity = GetAll(useEagerLoading).FirstOrDefault(_ => _.ID == id);

            if (entity == default(TRoot))
            {
                throw new EntityNotFoundException
                (
                    id.ToString(),
                    $"The ID supplied doesn't match any items in the repository."
                );
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
            var set = this.ReadContext.Set<TRoot>();
            var query = (IQueryable<TRoot>)set;

            if (useEagerLoading)
            {
                query = ApplyEagerLoading(query);
            }

            return query;
        }

        /// <summary>
        /// Updates a single entity and notifies the database context tracking manager
        /// </summary>
        /// <param name="entity">The entity to update</param>
        protected virtual void UpdateEntity(TRoot entity)
        {
            Validate.IsNotNull(entity);

            var key = entity.GetKeyValue();
            var lookupEntity = GetEntityByLookupKey(key);
            var context = this.WriteContext;

            if (lookupEntity != null && lookupEntity.ID != entity.ID)
            {
                throw new InvalidOperationException
                (
                    $"The key '{key}' has already been used by another entity."
                );
            }

            entity.DateModified = DateTime.UtcNow;

            var entry = context.Entry(entity);

            // Ensure the entity has been attached to the object state manager
            if (entry.State == EntityState.Detached)
            {
                context.Set<TRoot>().Attach(entity);
            }

            if (entry.State != EntityState.Added)
            {
                entry.State = EntityState.Modified;
            }
        }

        /// <summary>
        /// Deletes a single entity from the collection in the database context
        /// </summary>
        /// <param name="id">The ID of the entity to delete</param>
        protected virtual void RemoveEntity(long id)
        {
            var entity = GetEntityById(id, false);

            RemoveEntity(entity);
        }

        /// <summary>
        /// Deletes a single entity from the collection in the database context
        /// </summary>
        /// <param name="key">The lookup key of the entity to delete</param>
        protected virtual void RemoveEntity(string key)
        {
            var entity = GetEntityByLookupKey(key, false);

            RemoveEntity(entity);
        }

        /// <summary>
        /// Deletes a single entity from the collection in the database context
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        protected virtual void RemoveEntity(TRoot entity)
        {
            Validate.IsNotNull(entity);

            entity.Destroy();

            var context = this.WriteContext;
            var entry = context.Entry(entity);

            // Ensure the entity has been attached to the object state manager
            if (entry.State == EntityState.Detached)
            {
                context.Set<TRoot>().Attach(entity);
            }

            context.Set<TRoot>().Remove(entity);
        }

        /// <summary>
        /// Applies eager loading logic to the query specified
        /// </summary>
        /// <param name="query">The query to apply eager loading to</param>
        /// <returns>The query, with eager loading applied</returns>
        protected virtual IQueryable<TRoot> ApplyEagerLoading(IQueryable<TRoot> query)
        {
            var properties = this.NavigationProperties;

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    query.Include(property.Name);
                }
            }

            return query;
        }

        /// <summary>
        /// Uses reflection to get a list of navigation properties for the entity specified
        /// </summary>
        /// <returns>A list of matching navigation property</returns>
        protected List<PropertyInfo> GetNavigationProperties()
        {
            var properties = new List<PropertyInfo>();
            var entityType = typeof(TRoot);

            try
            {
                // Get the meta data associated with the entity
                var context = this.WriteContext;
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var objectSet = objectContext.CreateObjectSet<TRoot>();
                var entitySetElementType = objectSet.EntitySet.ElementType;

                foreach (var navigationProperty in entitySetElementType.NavigationProperties)
                {
                    var name = navigationProperty.Name;
                    var property = entityType.GetProperty(name);

                    properties.Add(property);
                }
            }
            catch (ArgumentException)
            {
                // This is to handle the following exception:
                // There are no EntitySets defined for the specified entity type '[Type]'.
                // If '[Type]' is a derived type, use the base type instead.

                // TODO: look into the above error to find a resolution for it
            }

            return properties;
        }
    }
}

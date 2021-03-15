namespace CodeChange.Toolkit.Domain.Events
{
    using CodeChange.Toolkit.Domain.Aggregate;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a domain event log aggregate root
    /// </summary>
    public class DomainEventLog : IAggregateRoot
    {
        /// <summary>
        /// Default protected parameter less constructor for the ORM to initialise the entity
        /// </summary>
        protected DomainEventLog()
        {
            this.UnpublishedEvents = new List<IDomainEvent>();
        }

        /// <summary>
        /// Constructs the domain event log
        /// </summary>
        /// <param name="event">The domain event</param>
        protected DomainEventLog(IDomainEvent @event)
        {
            this.LookupKey = new EntityKeyGenerator().GenerateKey();
            this.UnpublishedEvents = new List<IDomainEvent>();
            this.Details = new Collection<DomainEventLogDetail>();
            this.DateCreated = DateTime.UtcNow;
            this.DateModified = DateTime.UtcNow;

            PopulateLog(@event);
        }

        /// <summary>
        /// Constructs the domain event log
        /// </summary>
        /// <param name="aggregateKey">The aggregate key</param>
        /// <param name="aggregateType">The aggregate type</param>
        /// <param name="event">The domain event</param>
        protected DomainEventLog(string aggregateKey, Type aggregateType, IDomainEvent @event)
            : this(@event)
        {
            Validate.IsNotEmpty(aggregateKey);
            Validate.IsNotNull(aggregateType);
            
            this.AggregateKey = aggregateKey;
            this.AggregateTypeName = aggregateType.Name;
        }

        /// <summary>
        /// Creates a new domain event log
        /// </summary>
        /// <param name="event">The domain event</param>
        /// <returns>The event log created</returns>
        public static DomainEventLog CreateLog(IDomainEvent @event)
        {
            return new DomainEventLog(@event);
        }

        /// <summary>
        /// Creates a new domain event log
        /// </summary>
        /// <param name="aggregateKey">The aggregate key</param>
        /// <param name="aggregateType">The aggregate type</param>
        /// <param name="event">The domain event</param>
        /// <returns>The event log created</returns>
        public static DomainEventLog CreateLog(string aggregateKey, Type aggregateType, IDomainEvent @event)
        {
            return new DomainEventLog(aggregateKey, aggregateType, @event);
        }

        /// <summary>
        /// A database auto generated ID value, used internally for persistence
        /// </summary>
        public long ID { get; protected set; }

        /// <summary>
        /// A lookup key value for the entity, this must be unique for each entity of the same type
        /// </summary>
        public string LookupKey { get; protected set; }

        /// <summary>
        /// Gets the aggregate entities unique key value
        /// </summary>
        /// <returns>The key value</returns>
        public virtual string GetKeyValue() => this.LookupKey;

        /// <summary>
        /// Gets a list of unpublished domain events
        /// </summary>
        public IList<IDomainEvent> UnpublishedEvents { get; protected set; }

        /// <summary>
        /// Forces the aggregate root to destroy itself (similar to Dispose)
        /// </summary>
        /// <remarks>
        /// This allows the object to clean up any related data or perform any tasks that
        /// need to be completed before the object is destroyed (and deleted).
        /// </remarks>
        public void Destroy() { }
        
        /// <summary>
        /// Gets or sets the date the aggregate was created
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date the aggregate was last modified
        /// </summary>
        public DateTime DateModified { get; set; }

        /// <summary>
        /// Populates the domain event log
        /// </summary>
        /// <param name="event">The domain event</param>
        protected void PopulateLog(IDomainEvent @event)
        {
            Validate.IsNotNull(@event);
            
            this.EventTypeName = @event.GetType().Name;
            this.EventDescription = @event.ToString();

            PopulateDetails(@event);
        }

        /// <summary>
        /// Populates the domain event logs details
        /// </summary>
        /// <param name="event">The domain event</param>
        protected void PopulateDetails(IDomainEvent @event)
        {
            Validate.IsNotNull(@event);

            var properties = @event.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(_ => _.CanRead)
                .ToArray();

            foreach (var property in properties)
            {
                var detail = new DomainEventLogDetail(this, @event, property);

                this.Details.Add(detail);
            }
        }

        /// <summary>
        /// Gets the aggregate roots key
        /// </summary>
        public string AggregateKey { get; protected set; }

        /// <summary>
        /// Gets the name of the aggregate root type
        /// </summary>
        public string AggregateTypeName { get; protected set; }

        /// <summary>
        /// Gets the name of the domain events type
        /// </summary>
        public string EventTypeName { get; protected set; }

        /// <summary>
        /// Gets a description of the domain event
        /// </summary>
        public string EventDescription { get; protected set; }

        /// <summary>
        /// Gets a collection of domain event log details
        /// </summary>
        public virtual ICollection<DomainEventLogDetail> Details { get; protected set; }
    }
}

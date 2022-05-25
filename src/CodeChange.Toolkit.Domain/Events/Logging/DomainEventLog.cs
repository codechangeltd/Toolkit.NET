namespace CodeChange.Toolkit.Domain.Events.Logging
{
    using CodeChange.Toolkit.Domain.Events;

    /// <summary>
    /// Represents a domain event log aggregate root
    /// </summary>
    public class DomainEventLog
    {
        public DomainEventLog(string aggregateKey, Type aggregateType, IDomainEvent @event)
        {
            Validate.IsNotEmpty(aggregateKey);
            Validate.IsNotNull(aggregateType);
            Validate.IsNotNull(@event);

            AggregateKey = aggregateKey;
            AggregateTypeName = aggregateType.Name;
            EventTypeName = @event.GetType().Name;
            EventDescription = @event.ToString()!;
            Details = Array.Empty<DomainEventLogDetail>();

            PopulateDetails(@event!);
        }

        /// <summary>
        /// Populates the domain event logs details
        /// </summary>
        /// <param name="event">The domain event</param>
        protected void PopulateDetails(IDomainEvent @event)
        {
            var properties = @event.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanRead)
                .ToArray();

            var details = new List<DomainEventLogDetail>();

            foreach (var property in properties)
            {
                details.Add(new DomainEventLogDetail(@event, property));
            }

            Details = Details.ToArray();
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
        public IEnumerable<DomainEventLogDetail> Details { get; protected set; }
    }
}

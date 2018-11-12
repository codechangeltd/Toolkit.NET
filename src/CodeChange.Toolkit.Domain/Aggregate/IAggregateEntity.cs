namespace CodeChange.Toolkit.Domain.Aggregate
{
    /// <summary>
    /// Defines a contract for all domain aggregate entity implementations
    /// </summary>
    public interface IAggregateEntity
    {
        /// <summary>
        /// A database auto generated ID value, used internally for persistence
        /// </summary>
        long ID { get; }

        /// <summary>
        /// A lookup key value for the entity, this must be unique for each entity of the same type
        /// </summary>
        string LookupKey { get; }

        /// <summary>
        /// Gets the aggregate entities unique key value
        /// </summary>
        /// <returns>The key value</returns>
        string GetKeyValue();
    }
}

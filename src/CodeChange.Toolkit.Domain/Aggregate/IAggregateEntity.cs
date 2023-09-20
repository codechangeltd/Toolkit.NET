namespace CodeChange.Toolkit.Domain.Aggregate
{
    using System;

    /// <summary>
    /// Defines a contract for all domain aggregate entity implementations
    /// </summary>
    public interface IAggregateEntity
    {
        /// <summary>
        /// A key value for the entity, this must be unique for each entity of the same type
        /// </summary>
        string Key { get; }

        /// <summary>
        /// The date and time the aggregate was created
        /// </summary>
        DateTime DateCreated { get; }

        /// <summary>
        /// The date and time the aggregate was last modified
        /// </summary>
        DateTime DateModified { get; }
    }
}

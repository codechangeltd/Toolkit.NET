namespace CodeChange.Toolkit.Domain.Aggregate
{
    using System;
    
    /// <summary>
    /// Represents an exception for when an entity was not found
    /// </summary>
    public class EntityNotFoundException : Exception
    {
        /// <summary>
        /// Constructs the exception with the entity key and message
        /// </summary>
        /// <param name="entityKey">The entity key</param>
        /// <param name="message">The error message</param>
        public EntityNotFoundException
            (
                string entityKey
            )
            : base
            (
                $"No entity was found matching the key '{entityKey}'."
            )
        {
            this.EntityKey = entityKey;
        }

        /// <summary>
        /// Constructs the exception with the entity key and message
        /// </summary>
        /// <param name="entityKey">The entity key</param>
        /// <param name="message">The error message</param>
        public EntityNotFoundException
            (
                string entityKey,
                string message
            )
            : base(message)
        {
            Validate.IsNotEmpty(entityKey);

            this.EntityKey = entityKey;
        }

        /// <summary>
        /// Gets the key of the entity that was not found
        /// </summary>
        public string EntityKey { get; private set; }
    }
}

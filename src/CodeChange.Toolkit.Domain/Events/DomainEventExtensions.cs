namespace CodeChange.Toolkit.Domain.Events
{
    using System;
    
    /// <summary>
    /// Represents various extension methods for domain events
    /// </summary>
    public static class DomainEventExtensions
    {
        /// <summary>
        /// Determines if the event is pre-transaction
        /// </summary>
        /// <param name="event">The domain event</param>
        /// <returns>True, if the event is pre-transaction; otherwise false</returns>
        public static bool IsPreTransaction
            (
                this IDomainEvent @event
            )
        {
            Validate.IsNotNull(@event);

            var hasPreAttribute = Attribute.IsDefined
            (
                @event.GetType(),
                typeof(PreTransactionEventAttribute)
            );

            return hasPreAttribute;
        }

        /// <summary>
        /// Determines if the event is pre-transaction
        /// </summary>
        /// <param name="event">The domain event</param>
        /// <returns>True, if the event is pre-transaction; otherwise false</returns>
        public static bool IsPostTransaction
            (
                this IDomainEvent @event
            )
        {
            Validate.IsNotNull(@event);

            var hasPreAttribute = Attribute.IsDefined
            (
                @event.GetType(),
                typeof(PreTransactionEventAttribute)
            );

            if (false == hasPreAttribute)
            {
                return true;
            }
            else
            {
                var hasPostAttribute = Attribute.IsDefined
                (
                    @event.GetType(),
                    typeof(PostTransactionEventAttribute)
                );

                if (hasPostAttribute)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}

namespace CodeChange.Toolkit.Domain.Events
{
    using System;

    /// <summary>
    /// Represents various extension methods for domain event handlers
    /// </summary>
    public static class DomainEventHandlerExtensions
    {
        /// <summary>
        /// Determines if the event handler is pre-transaction
        /// </summary>
        /// <typeparam name="T">The domain event type</typeparam>
        /// <param name="handler">The domain event</param>
        /// <returns>True, if the event is pre-transaction; otherwise false</returns>
        public static bool IsPreTransaction<T>
            (
                this IDomainEventHandler<T> handler
            )

            where T : IDomainEvent
        {
            Validate.IsNotNull(handler);

            var hasPreAttribute = Attribute.IsDefined
            (
                handler.GetType(),
                typeof(PreTransactionHandlerAttribute)
            );

            return hasPreAttribute;
        }

        /// <summary>
        /// Determines if the event handler is pre-transaction
        /// </summary>
        /// <typeparam name="T">The domain event type</typeparam>
        /// <param name="handler">The domain event</param>
        /// <returns>True, if the event is pre-transaction; otherwise false</returns>
        public static bool IsPostTransaction<T>
            (
                this IDomainEventHandler<T> handler
            )

            where T : IDomainEvent
        {
            Validate.IsNotNull(handler);

            var hasPreAttribute = Attribute.IsDefined
            (
                handler.GetType(),
                typeof(PreTransactionHandlerAttribute)
            );

            if (false == hasPreAttribute)
            {
                return true;
            }
            else
            {
                var hasPostAttribute = Attribute.IsDefined
                (
                    handler.GetType(),
                    typeof(PostTransactionHandlerAttribute)
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

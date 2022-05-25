namespace CodeChange.Toolkit.Domain.Events;

using System;

public static class DomainEventHandlerExtensions
{
    /// <summary>
    /// Determines if the event handler is pre-transaction
    /// </summary>
    /// <typeparam name="T">The domain event type</typeparam>
    /// <param name="handler">The domain event</param>
    /// <returns>True, if the event is pre-transaction; otherwise false</returns>
    public static bool IsPreTransaction<T>(this IDomainEventHandler<T> handler)
        where T : IDomainEvent
    {
        Validate.IsNotNull(handler);

        return Attribute.IsDefined(handler.GetType(), typeof(PreTransactionHandlerAttribute));
    }

    /// <summary>
    /// Determines if the event handler is pre-transaction
    /// </summary>
    /// <typeparam name="T">The domain event type</typeparam>
    /// <param name="handler">The domain event</param>
    /// <returns>True, if the event is pre-transaction; otherwise false</returns>
    public static bool IsPostTransaction<T>(this IDomainEventHandler<T> handler)
        where T : IDomainEvent
    {
        Validate.IsNotNull(handler);

        var handlerType = handler.GetType();
        var hasPreAttribute = Attribute.IsDefined(handlerType, typeof(PreTransactionHandlerAttribute));

        if (false == hasPreAttribute)
        {
            return true;
        }
        else
        {
            return Attribute.IsDefined(handlerType, typeof(PostTransactionHandlerAttribute));
        }
    }
}

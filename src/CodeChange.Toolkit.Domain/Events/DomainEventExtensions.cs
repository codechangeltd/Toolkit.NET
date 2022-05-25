namespace CodeChange.Toolkit.Domain.Events;

using System;

public static class DomainEventExtensions
{
    /// <summary>
    /// Determines if the event is pre-transaction
    /// </summary>
    /// <param name="event">The domain event</param>
    /// <returns>True, if the event is pre-transaction; otherwise false</returns>
    public static bool IsPreTransaction(this IDomainEvent @event)
    {
        Validate.IsNotNull(@event);

        return Attribute.IsDefined(@event.GetType(), typeof(PreTransactionEventAttribute));
    }

    /// <summary>
    /// Determines if the event is pre-transaction
    /// </summary>
    /// <param name="event">The domain event</param>
    /// <returns>True, if the event is pre-transaction; otherwise false</returns>
    public static bool IsPostTransaction(this IDomainEvent @event)
    {
        Validate.IsNotNull(@event);

        var eventType = @event.GetType();
        var hasPreAttribute = Attribute.IsDefined(eventType, typeof(PreTransactionEventAttribute));

        if (false == hasPreAttribute)
        {
            return true;
        }
        else
        {
            var hasPostAttribute = Attribute.IsDefined(eventType, typeof(PostTransactionEventAttribute));

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

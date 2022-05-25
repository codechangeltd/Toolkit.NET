namespace CodeChange.Toolkit.Domain.Events;

using CodeChange.Toolkit.Domain.Aggregate;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a factory class for generating new event queues
/// </summary>
public static class EventQueueFactory
{
    /// <summary>
    /// Creates an event queue for all unpublished pre-transaction domain events
    /// </summary>
    /// <param name="aggregates">The aggregates to extract the events from</param>
    /// <returns>The event queue created</returns>
    public static IEventQueue CreatePreTransactionEventQueue(params IAggregateRoot[] aggregates)
    {
        return GenerateEventQueue(true, aggregates);
    }

    /// <summary>
    /// Creates an event queue for all unpublished post-transaction domain events
    /// </summary>
    /// <param name="aggregates">The aggregates to extract the events from</param>
    /// <returns>The event queue created</returns>
    public static IEventQueue CreatePostTransactionEventQueue(params IAggregateRoot[] aggregates)
    {
        return GenerateEventQueue(false, aggregates);
    }

    /// <summary>
    /// Generates a queue of unpublished domain events
    /// </summary>
    /// <param name="preTransaction">True for pre-transaction; false post-transaction events</param>
    /// <param name="aggregates">The aggregates to extract the events from</param>
    /// <returns>A collection of domain events</returns>
    private static IEventQueue GenerateEventQueue(bool preTransaction, params IAggregateRoot[] aggregates)
    {
        var queue = new EventQueue();

        foreach (var aggregate in aggregates)
        {
            var aggregateKey = aggregate.Key;
            var aggregateType = aggregate.GetType();
            var nextEvents = default(IList<IDomainEvent>);

            if (preTransaction)
            {
                nextEvents = aggregate.GetPreTransactionEvents();
            }
            else
            {
                nextEvents = aggregate.GetPostTransactionEvents();
            }

            if (nextEvents != null && nextEvents.Any())
            {
                foreach (var @event in nextEvents)
                {
                    queue.Add(aggregateKey, aggregateType, @event);
                }
            }
        }

        return queue;
    }
}

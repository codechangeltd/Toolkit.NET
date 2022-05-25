namespace CodeChange.Toolkit.Domain.Events;

using System.Collections.Generic;
using System.Linq;

public static class EventQueueExtensions
{
    /// <summary>
    /// Removes a collection of items from an event queue
    /// </summary>
    /// <param name="queue">The event queue</param>
    /// <param name="itemsToRemove">The items to remove the event queue</param>
    /// <returns>The new event queue, less the items to remove</returns>
    public static IEventQueue Remove(this IEventQueue queue, IEnumerable<EventQueueItem> itemsToRemove)
    {
        var newQueue = new EventQueue();
        var queueItems = queue.ToList();

        foreach (var item in queueItems)
        {
            var matchFound = itemsToRemove.Any(x => x.GetHashCode() == item.GetHashCode());

            if (false == matchFound)
            {
                newQueue.Add(item.AggregateKey, item.AggregateType, item.Event);
            }
        }

        return newQueue;
    }
}

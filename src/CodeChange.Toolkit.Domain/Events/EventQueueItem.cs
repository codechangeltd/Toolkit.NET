namespace CodeChange.Toolkit.Domain.Events;

using System;

/// <summary>
/// Represents a domain event queue item
/// </summary>
/// <param name="AggregateKey">The key of the aggregate root where the event came from</param>
/// <param name="AggregateType">The type of the aggregate root where the event came from</param>
/// <param name="Event">The domain event that is being queued</param>
public record class EventQueueItem(string AggregateKey, Type AggregateType, IDomainEvent Event);

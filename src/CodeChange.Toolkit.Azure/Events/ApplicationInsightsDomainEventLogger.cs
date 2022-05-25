namespace CodeChange.Toolkit.Azure.Events;

using CodeChange.Toolkit.Domain.Events;
using CodeChange.Toolkit.Domain.Events.Logging;
using CSharpFunctionalExtensions;
using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Represents an Azure ApplicationInsights implementation of a domain event logger
/// </summary>
public sealed class ApplicationInsightsDomainEventLogger : IDomainEventLogger
{
    private readonly TelemetryClient _telemetry;

    public ApplicationInsightsDomainEventLogger(TelemetryClient telemetry)
    {
        Validate.IsNotNull(telemetry);

        _telemetry = telemetry;
    }

    public Result LogEvent(string aggregateKey, Type aggregateType, IDomainEvent @event)
    {
        Validate.IsNotEmpty(aggregateKey);
        Validate.IsNotNull(aggregateType);

        var properties = CompileEventProperties(aggregateKey, aggregateType, @event);

        properties.Add("AggregateKey", aggregateKey);
        properties.Add("AggregateType", aggregateType.Name);

        _telemetry.TrackEvent(@event.ToString(), properties);

        return Result.Success();
    }

    public Task<Result> LogEventAsync(string aggregateKey, Type aggregateType, IDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(LogEvent(aggregateKey, aggregateType, @event));
    }

    /// <summary>
    /// Compiles the domain event properties onto a dictionary
    /// </summary>
    /// <param name="aggregateKey">The aggregate key</param>
    /// <param name="aggregateType">The aggregate type</param>
    /// <param name="event">The domain event</param>
    /// <returns>The event details as a dictionary</returns>
    private Dictionary<string, string?> CompileEventProperties(string aggregateKey, Type aggregateType, IDomainEvent @event)
    {
        Validate.IsNotNull(@event);

        var properties = new Dictionary<string, string?>()
        {
            { "EventTypeName", @event.GetType().Name },
            { "EventDescription", @event?.ToString() }
        };

        var eventLog = new DomainEventLog(aggregateKey, aggregateType, @event!);

        foreach (var detail in eventLog.Details)
        {
            AppendDetail(ref properties, detail);
        }

        return properties;
    }

    /// <summary>
    /// Appends the event log detail to a details dictionary
    /// </summary>
    /// <param name="properties">The event properties</param>
    /// <param name="detail">The event log detail to append</param>
    private void AppendDetail(ref Dictionary<string, string?> properties, DomainEventLogDetail detail)
    {
        var path = String.Empty;
        var parentDetail = detail.Parent;

        while (parentDetail != null)
        {
            path = $"{parentDetail.PropertyName}.{path}";
            parentDetail = parentDetail.Parent;
        }

        var key = $"{path}{detail.PropertyName}";
        var value = detail.PropertyStringValue;

        var usedCount = properties.Count
        (
            pair => pair.Key == key || (pair.Key.StartsWith(key) && pair.Key.EndsWith("]"))
        );

        if (usedCount > 0)
        {
            if (usedCount == 1)
            {
                var tempValue = properties[key];

                properties.Remove(key);
                properties.Add($"{key}[0]", tempValue);
            }

            properties.Add($"{key}[{usedCount}]", value);
        }
        else
        {
            properties.Add(key, value);
        }

        foreach (var nestedDetail in detail.NestedDetails)
        {
            AppendDetail(ref properties, nestedDetail);
        }
    }
}

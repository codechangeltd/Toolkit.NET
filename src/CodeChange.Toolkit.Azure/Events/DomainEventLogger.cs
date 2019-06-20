namespace CodeChange.Toolkit.Azure.Events
{
    using CodeChange.Toolkit.Domain.Events;
    using Microsoft.ApplicationInsights;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An Azure ApplicationInsights implementation of a domain event logger
    /// </summary>
    public sealed class DomainEventLogger : IDomainEventLogger
    {
        private readonly TelemetryClient _telemetry;

        public DomainEventLogger()
        {
            _telemetry = new TelemetryClient();
        }

        /// <summary>
        /// Logs the domain event specified
        /// </summary>
        /// <param name="event">The domain event</param>
        public void LogEvent
            (
                IDomainEvent @event
            )
        {
            var properties = CompileEventProperties(@event);

            _telemetry.TrackEvent
            (
                @event.ToString(),
                properties
            );
        }

        /// <summary>
        /// Logs the domain event specified
        /// </summary>
        /// <param name="aggregateKey">The aggregate key</param>
        /// <param name="aggregateType">The aggregate type</param>
        /// <param name="event">The domain event</param>
        public void LogEvent
            (
                string aggregateKey,
                Type aggregateType,
                IDomainEvent @event
            )
        {
            Validate.IsNotEmpty(aggregateKey);
            Validate.IsNotNull(aggregateType);

            var properties = CompileEventProperties(@event);

            properties.Add("AggregateKey", aggregateKey);
            properties.Add("AggregateType", aggregateType.Name);

            _telemetry.TrackEvent
            (
                @event.ToString(),
                properties
            );
        }

        /// <summary>
        /// Compiles the domain event properties onto a dictionary
        /// </summary>
        /// <param name="event">The domain event</param>
        /// <returns>The event details as a dictionary</returns>
        private Dictionary<string, string> CompileEventProperties
            (
                IDomainEvent @event
            )
        {
            Validate.IsNotNull(@event);

            var properties = new Dictionary<string, string>()
            {
                { "EventTypeName", @event.GetType().Name },
                { "EventDescription", @event.ToString() }
            };

            var eventLog = DomainEventLog.CreateLog
            (
                @event
            );

            foreach (var detail in eventLog.Details)
            {
                AppendDetail
                (
                    ref properties,
                    detail
                );
            }

            return properties;
        }

        /// <summary>
        /// Appends the event log detail to a details dictionary
        /// </summary>
        /// <param name="properties">The event properties</param>
        /// <param name="detail">The event log detail to append</param>
        private void AppendDetail
            (
                ref Dictionary<string, string> properties,
                DomainEventLogDetail detail
            )
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
                pair => pair.Key == key
                    || pair.Key.StartsWith(key) && pair.Key.EndsWith("]")
            );

            if (usedCount > 0)
            {
                if (usedCount == 1)
                {
                    var tempValue = properties[key];

                    properties.Remove(key);
                    properties.Add($"{key}[0]", tempValue);
                }

                properties.Add
                (
                    $"{key}[{usedCount}]",
                    value
                );
            }
            else
            {
                properties.Add(key, value);
            }

            foreach (var nestedDetail in detail.NestedDetails)
            {
                AppendDetail
                (
                    ref properties,
                    nestedDetail
                );
            }
        }
    }
}

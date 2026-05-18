using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace PlatformService.Logging;

// 1. The class
public class ActivityEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory factory)
    {
        var activity = Activity.Current;
        if (activity is null)
            return;

        logEvent.AddPropertyIfAbsent(
            factory.CreateProperty("TraceId", activity.TraceId.ToString())
        );
        logEvent.AddPropertyIfAbsent(factory.CreateProperty("SpanId", activity.SpanId.ToString()));
        logEvent.AddPropertyIfAbsent(
            factory.CreateProperty("ActivityDisplayName", activity.DisplayName.ToString())
        );
        logEvent.AddPropertyIfAbsent(
            factory.CreateProperty("ActivitySourceDisplayName", activity.Source.Name.ToString())
        );
    }
}

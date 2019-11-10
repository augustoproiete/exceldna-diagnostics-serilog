using System;
using System.Diagnostics;
using Serilog.Events;

namespace ExcelDna.Diagnostics.Serilog
{
    internal static class LogEventLevelConvert
    {
        public static LogEventLevel ToLogEventLevel(TraceEventType eventType)
        {
            switch (eventType)
            {
                case TraceEventType.Critical:
                    return LogEventLevel.Fatal;

                case TraceEventType.Error:
                    return LogEventLevel.Error;

                case TraceEventType.Warning:
                    return LogEventLevel.Warning;

                case TraceEventType.Information:
                    return LogEventLevel.Information;

                case TraceEventType.Start:
                case TraceEventType.Stop:
                case TraceEventType.Suspend:
                case TraceEventType.Resume:
                case TraceEventType.Transfer:
                    return LogEventLevel.Debug;

                case TraceEventType.Verbose:
                    return LogEventLevel.Verbose;

                default:
                    throw new ArgumentOutOfRangeException(nameof(eventType), eventType, $"Unknown {nameof(TraceEventType)}: `{eventType}`");
            }
        }
    }
}

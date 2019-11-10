using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Serilog;
using Serilog.Events;

namespace ExcelDna.Diagnostics.Serilog
{
    // Adapted from SerilogTraceListener by SerilogTraceListener Contributors
    // https://github.com/serilog-trace-listener/SerilogTraceListener

    /// <summary>
    /// TraceListener implementation that directs all output to Serilog.
    /// </summary>
    public partial class SerilogTraceListener : TraceListener
    {
        private ILogger _logger;
        private readonly InMemoryBufferSink _inMemoryBufferSink;

        private const string ActivityIdProperty = "ActivityId";
        private const string CategoryProperty = "Category";
        private const string EventIdProperty = "TraceEventId";
        private const string FailDetailMessageProperty = "FailDetails";
        private const string RelatedActivityIdProperty = "RelatedActivityId";
        private const string SourceProperty = "TraceSource";
        private const string TraceDataProperty = "TraceData";
        private const string TraceEventTypeProperty = "TraceEventType";
        private const LogEventLevel DefaultLogLevel = LogEventLevel.Debug;
        private const LogEventLevel FailLevel = LogEventLevel.Fatal;
        private const string NoMessageTraceEventMessageTemplate = "{TraceSource:l} {TraceEventType}: {TraceEventId}";
        private const string TraceDataMessageTemplate = "{TraceData}";

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogTraceListener" /> class
        /// </summary>
        public SerilogTraceListener()
            : this(name: string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogTraceListener" /> class using the specified name as the listener
        /// </summary>
        /// <param name="name">The name of the <see cref="SerilogTraceListener" /></param>
        public SerilogTraceListener(string name)
            : base(name)
        {
            _inMemoryBufferSink = new InMemoryBufferSink();

            _logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Sink(_inMemoryBufferSink)
                .CreateLogger();
        }

        /// <summary>
        /// Forwards messages written to the <see cref="SerilogTraceListener" /> to the <see cref="ILogger" /> specified
        /// Because Excel-DNA starts writing to the TraceListener before <code>WriteToSerilog</code> is called, all messages are buffered until <code>WriteToSerilog</code> is called
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" /> that trace messages should be written to</param>
        public void WriteToSerilog(ILogger logger)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            // ForContext shouldn't be needed here as the LogEvent could include the SourceContext
            // but this is needed for MinimumLevel overrides to work due to how Serilog was designed:
            // https://github.com/serilog/serilog/issues/1382
            var contextLogger = logger
                .ForContext(Constants.SourceContextPropertyName, Constants.ExcelDnaSourceContext);

            if (!(_inMemoryBufferSink is null))
            {
                using (_inMemoryBufferSink)
                {
                    _inMemoryBufferSink.WriteTo(contextLogger);
                }
            }

            _logger = contextLogger;
        }

        /// <inheritdoc />
        public override bool IsThreadSafe => true;

        /// <inheritdoc />
        public override void Fail(string message)
        {
            var properties = CreateFailProperties();
            Write(FailLevel, null, message, properties);
        }

        /// <inheritdoc />
        public override void Fail(string message, string detailMessage)
        {
            var properties = CreateFailProperties();
            SafeAddProperty(properties, FailDetailMessageProperty, detailMessage);
            Write(FailLevel, null, message, properties);
        }

        /// <inheritdoc />
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id,
            object data)
        {
            if (!ShouldTrace(eventCache, source, eventType, id, "", null, data, null))
            {
                return;
            }

            var properties = CreateTraceProperties(source, eventType, id);
            WriteData(eventType, properties, data);
        }

        /// <inheritdoc />
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id,
            params object[] data)
        {
            if (!ShouldTrace(eventCache, source, eventType, id, "", null, null, data))
            {
                return;
            }

            var properties = CreateTraceProperties(source, eventType, id);
            WriteData(eventType, properties, data);
        }

        /// <inheritdoc />
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            if (!ShouldTrace(eventCache, source, eventType, id, "", null, null, null))
            {
                return;
            }

            var properties = CreateTraceProperties(source, eventType, id);
            Write(eventType, null, NoMessageTraceEventMessageTemplate, properties);
        }

        /// <inheritdoc />
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id,
            string message)
        {
            if (!ShouldTrace(eventCache, source, eventType, id, message, null, null, null))
            {
                return;
            }

            var properties = CreateTraceProperties(source, eventType, id);
            Write(eventType, null, message, properties);
        }

        /// <inheritdoc />
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id,
            string format, params object[] args)
        {
            if (!ShouldTrace(eventCache, source, eventType, id, format, args, null, null))
            {
                return;
            }

            var properties = CreateTraceProperties(source, eventType, id);
            AddFormatArgs(properties, args, out var exception);
            Write(eventType, exception, format, properties);
        }

        /// <inheritdoc />
        public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message,
            Guid relatedActivityId)
        {
            const TraceEventType eventType = TraceEventType.Transfer;
            var properties = CreateTraceProperties(source, eventType, id);
            SafeAddProperty(properties, RelatedActivityIdProperty, relatedActivityId);
            Write(eventType, null, message, properties);
        }

        /// <inheritdoc />
        public override void Write(object data)
        {
            var properties = CreateProperties();
            SafeAddProperty(properties, TraceDataProperty, data);
            Write(DefaultLogLevel, null, TraceDataMessageTemplate, properties);
        }

        /// <inheritdoc />
        public override void Write(string message)
        {
            var properties = CreateProperties();
            Write(DefaultLogLevel, null, message, properties);
        }

        /// <inheritdoc />
        public override void Write(object data, string category)
        {
            var properties = CreateProperties();
            SafeAddProperty(properties, TraceDataProperty, data);
            SafeAddProperty(properties, CategoryProperty, category);
            Write(DefaultLogLevel, null, TraceDataMessageTemplate, properties);
        }

        /// <inheritdoc />
        public override void Write(string message, string category)
        {
            var properties = CreateProperties();
            SafeAddProperty(properties, CategoryProperty, category);
            Write(DefaultLogLevel, null, message, properties);
        }

        /// <inheritdoc />
        public override void WriteLine(string message)
        {
            Write(message);
        }

        /// <inheritdoc />
        public override void WriteLine(object data)
        {
            Write(data);
        }

        /// <inheritdoc />
        public override void WriteLine(string message, string category)
        {
            Write(message, category);
        }

        /// <inheritdoc />
        public override void WriteLine(object data, string category)
        {
            Write(data, category);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _inMemoryBufferSink?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void AddFormatArgs(IList<LogEventProperty> properties, IReadOnlyList<object> args, out Exception exception)
        {
            exception = null;

            if (args is null)
            {
                return;
            }

            for (var argIndex = 0; argIndex < args.Count; argIndex++)
            {
                SafeAddProperty(properties, argIndex.ToString(CultureInfo.InvariantCulture), args[argIndex]);

                // If there is any argument of type Exception (last wins), then use it
                if (args[argIndex] is Exception)
                {
                    exception = (Exception)args[argIndex];
                }
            }
        }

        private IList<LogEventProperty> CreateFailProperties()
        {
            var properties = CreateProperties();
            SafeAddProperty(properties, TraceEventTypeProperty, "Fail");
            return properties;
        }

        private IList<LogEventProperty> CreateProperties()
        {
            var properties = new List<LogEventProperty>();
            SafeAddProperty(properties, ActivityIdProperty, Trace.CorrelationManager.ActivityId);
            return properties;
        }

        private IList<LogEventProperty> CreateTraceProperties(string source, TraceEventType eventType, int id)
        {
            var properties = CreateProperties();
            SafeAddProperty(properties, SourceProperty, source);
            SafeAddProperty(properties, TraceEventTypeProperty, eventType);
            SafeAddProperty(properties, EventIdProperty, id);
            return properties;
        }

        private void SafeAddProperty(ICollection<LogEventProperty> properties, string name, object value)
        {
            var localLogger = _logger;
            if (localLogger.BindProperty(name, value, false, out var property))
            {
                properties.Add(property);
            }
        }

        private void Write(TraceEventType eventType, Exception exception, string messageTemplate, IEnumerable<LogEventProperty> properties)
        {
            var level = LogEventLevelConvert.ToLogEventLevel(eventType);
            Write(level, exception, messageTemplate, properties);
        }

        private void Write(LogEventLevel level, Exception exception, string messageTemplate, IEnumerable<LogEventProperty> properties)
        {
            // If user has passed null, then still log (as an empty message)
            if (messageTemplate == null)
            {
                messageTemplate = string.Empty;
            }

            var localLogger = _logger;

            // boundProperties will be empty and can be ignored
            if (localLogger.BindMessageTemplate(messageTemplate, null, out var parsedTemplate, out _))
            {
                var logEvent = new LogEvent(DateTimeOffset.Now, level, exception, parsedTemplate, properties);
                localLogger.Write(logEvent);
            }
        }

        private bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage,
            object[] args, object data1, object[] data)
        {
            return Filter?.ShouldTrace(cache, source, eventType, id, formatOrMessage, args, data1, data) ?? true;
        }

        private void WriteData(TraceEventType eventType, IList<LogEventProperty> properties, object data)
        {
            var level = LogEventLevelConvert.ToLogEventLevel(eventType);
            SafeAddProperty(properties, TraceDataProperty, data);
            Write(level, null, TraceDataMessageTemplate, properties);
        }
    }
}

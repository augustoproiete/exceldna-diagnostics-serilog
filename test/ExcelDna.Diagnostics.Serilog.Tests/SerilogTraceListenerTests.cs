using System;
using System.Diagnostics;
using System.Linq;
using ExcelDna.Diagnostics.Serilog.Tests.Support;
using ExcelDna.Integration;
using Serilog;
using Serilog.Events;
using Xunit;

namespace ExcelDna.Diagnostics.Serilog.Tests
{
    public class SerilogTraceListenerTests : IDisposable
    {
        private const TraceEventType WarningEventType = TraceEventType.Warning;
        private readonly string _category = Some.String("category");
        private readonly int _id = Some.Int();
        private readonly string _message = Some.String("message");
        private readonly string _source = Some.String("source");
        private readonly TraceEventCache _traceEventCache = new TraceEventCache();

        private readonly SerilogTraceListener _traceListener;
        private LogEvent _loggedEvent;

        public SerilogTraceListenerTests()
        {
            var delegatingSink = new DelegatingSink(evt =>
            {
                _loggedEvent = evt;
            });

            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Sink(delegatingSink)
                .CreateLogger();

            var traceSource = new TraceSource(SerilogTraceListener.Constants.ExcelDnaTraceSourceName,
                SourceLevels.All);

            traceSource.Listeners.Clear();

            var traceListener = new SerilogTraceListener(nameof(SerilogTraceListenerTests));
            traceSource.Listeners.Add(traceListener);

            _traceListener = traceListener;
            _loggedEvent = null;

            traceSource.WriteToSerilog(logger);
        }

        public void Dispose()
        {
            _traceListener.Dispose();
        }

        [Fact]
        public void Captures_Write()
        {
            _traceListener.Write(_message);

            LogEventAssert.HasMessage(_message, _loggedEvent);
            LogEventAssert.HasLevel(LogEventLevel.Debug, _loggedEvent);

            LogEventAssert.HasPropertyValue(SerilogTraceListener.Constants.ExcelDnaSourceContext,
                SerilogTraceListener.Constants.SourceContextPropertyName, _loggedEvent);
        }

        [Fact]
        public void Captures_Write_with_category()
        {
            _traceListener.Write(_message, _category);

            LogEventAssert.HasMessage(_message, _loggedEvent);
            LogEventAssert.HasLevel(LogEventLevel.Debug, _loggedEvent);
            LogEventAssert.HasPropertyValue(_category, "Category", _loggedEvent);
        }

        [Fact]
        public void Captures_Write_of_object()
        {
            var writtenObject = Tuple.Create(Some.String());
            _traceListener.Write(writtenObject);

            LogEventAssert.HasMessage($@"""{writtenObject}""", _loggedEvent);
            LogEventAssert.HasLevel(LogEventLevel.Debug, _loggedEvent);
        }

        [Fact]
        public void Write_of_object_has_TraceData_property()
        {
            var writtenObject = new[]
            {
                Some.Int(),
                Some.Int(),
            };

            _traceListener.Write(writtenObject);

            LogEventAssert.HasMessage($"[{writtenObject[0]}, {writtenObject[1]}]", _loggedEvent);
            LogEventAssert.HasLevel(LogEventLevel.Debug, _loggedEvent);

            var sequence = ((SequenceValue)_loggedEvent.Properties["TraceData"]).Elements.Select(pv => pv.LiteralValue());
            Assert.Equal(writtenObject.Cast<object>(), sequence);
        }

        [Fact]
        public void Captures_Write_of_object_with_category()
        {
            var writtenObject = Tuple.Create(Some.String());
            _traceListener.Write(writtenObject, _category);

            LogEventAssert.HasMessage($@"""{writtenObject}""", _loggedEvent);
            LogEventAssert.HasLevel(LogEventLevel.Debug, _loggedEvent);
            LogEventAssert.HasPropertyValue(_category, "Category", _loggedEvent);
        }

        [Fact]
        public void Captures_WriteLine()
        {
            _traceListener.WriteLine(_message);

            LogEventAssert.HasMessage(_message, _loggedEvent);
            LogEventAssert.HasLevel(LogEventLevel.Debug, _loggedEvent);
        }

        [Fact]
        public void Captures_WriteLine_with_category()
        {
            _traceListener.WriteLine(_message, _category);

            LogEventAssert.HasMessage(_message, _loggedEvent);
            LogEventAssert.HasLevel(LogEventLevel.Debug, _loggedEvent);
            LogEventAssert.HasPropertyValue(_category, "Category", _loggedEvent);
        }

        [Fact]
        public void Captures_WriteLine_of_object()
        {
            var writtenObject = Tuple.Create(Some.String());
            _traceListener.WriteLine(writtenObject);

            LogEventAssert.HasMessage($@"""{writtenObject}""", _loggedEvent);
            LogEventAssert.HasLevel(LogEventLevel.Debug, _loggedEvent);
        }

        [Fact]
        public void Captures_WriteLine_of_object_with_category()
        {
            var writtenObject = Tuple.Create(Some.String());
            _traceListener.WriteLine(writtenObject, _category);

            LogEventAssert.HasMessage($@"""{writtenObject}""", _loggedEvent);
            LogEventAssert.HasLevel(LogEventLevel.Debug, _loggedEvent);
            LogEventAssert.HasPropertyValue(_category, "Category", _loggedEvent);
        }

        [Fact]
        public void Captures_Fail()
        {
            _traceListener.Fail(_message);

            LogEventAssert.HasMessage(_message, _loggedEvent);
            LogEventAssert.HasLevel(LogEventLevel.Fatal, _loggedEvent);
        }

        [Fact]
        public void Captures_Fail_with_detailed_description()
        {
            var detailMessage = Some.String();

            _traceListener.Fail(_message, detailMessage);

            LogEventAssert.HasMessage(_message, _loggedEvent);
            LogEventAssert.HasLevel(LogEventLevel.Fatal, _loggedEvent);
            LogEventAssert.HasPropertyValue(detailMessage, "FailDetails", _loggedEvent);
        }

        [Fact]
        public void Captures_TraceEvent()
        {
            _traceListener.TraceEvent(_traceEventCache, _source, WarningEventType, _id);

            LogEventAssert.HasMessage($"{_source} {WarningEventType}: {_id}", _loggedEvent);
            LogEventAssert.HasLevel(LogEventLevel.Warning, _loggedEvent);

            LogEventAssert.HasPropertyValue(_id, "TraceEventId", _loggedEvent);
            LogEventAssert.HasPropertyValue(_source, "TraceSource", _loggedEvent);
            LogEventAssert.HasPropertyValue(WarningEventType, "TraceEventType", _loggedEvent);
        }

        [Fact]
        public void Captures_TraceEvent_with_message()
        {
            _traceListener.TraceEvent(_traceEventCache, _source, WarningEventType, _id, _message);

            LogEventAssert.HasMessage(_message, _loggedEvent);
            LogEventAssert.HasLevel(LogEventLevel.Warning, _loggedEvent);
            LogEventAssert.HasPropertyValue(_id, "TraceEventId", _loggedEvent);
            LogEventAssert.HasPropertyValue(_source, "TraceSource", _loggedEvent);
            LogEventAssert.HasPropertyValue(WarningEventType, "TraceEventType", _loggedEvent);
        }

        [Fact]
        public void Captures_TraceEvent_with_format_message()
        {
            const string format = "{0}-{1}-{2}";
            var args = new object[]
            {
                Some.Int(),
                Some.Int(),
                Some.Int(),
            };

            _traceListener.TraceEvent(_traceEventCache, _source, WarningEventType, _id, format, args);

            LogEventAssert.HasMessage(string.Format(format, args[0], args[1], args[2]), _loggedEvent);
            LogEventAssert.HasLevel(LogEventLevel.Warning, _loggedEvent);

            LogEventAssert.HasPropertyValue(_id, "TraceEventId", _loggedEvent);
            LogEventAssert.HasPropertyValue(_source, "TraceSource", _loggedEvent);
            LogEventAssert.HasPropertyValue(WarningEventType, "TraceEventType", _loggedEvent);
            LogEventAssert.HasPropertyValue(args[0], "0", _loggedEvent);
            LogEventAssert.HasPropertyValue(args[1], "1", _loggedEvent);
            LogEventAssert.HasPropertyValue(args[2], "2", _loggedEvent);
        }

        [Fact]
        public void Captures_TraceTransfer()
        {
            var relatedActivityId = Some.Guid();

            _traceListener.TraceTransfer(_traceEventCache, _source, _id, _message, relatedActivityId);

            LogEventAssert.HasMessage(_message, _loggedEvent);
            LogEventAssert.HasLevel(LogEventLevel.Debug, _loggedEvent);

            LogEventAssert.HasPropertyValue(_id, "TraceEventId", _loggedEvent);
            LogEventAssert.HasPropertyValue(_source, "TraceSource", _loggedEvent);
            LogEventAssert.HasPropertyValue(relatedActivityId, "RelatedActivityId", _loggedEvent);
            LogEventAssert.HasPropertyValue(TraceEventType.Transfer, "TraceEventType", _loggedEvent);
        }

        [Fact]
        public void Captures_TraceData()
        {
            var data = new
            {
                Info = Some.String(),
            };

            _traceListener.TraceData(_traceEventCache, _source, WarningEventType, _id, data);

            LogEventAssert.HasMessage($@"""{data}""", _loggedEvent);
            LogEventAssert.HasLevel(LogEventLevel.Warning, _loggedEvent);

            LogEventAssert.HasPropertyValue(_id, "TraceEventId", _loggedEvent);
            LogEventAssert.HasPropertyValue(_source, "TraceSource", _loggedEvent);
            LogEventAssert.HasPropertyValue(data.ToString(), "TraceData", _loggedEvent);
            LogEventAssert.HasPropertyValue(WarningEventType, "TraceEventType", _loggedEvent);
        }

        [Fact]
        public void Captures_TraceData_with_array_of_int()
        {
            var data = new[]
            {
                Some.Int(),
                Some.Int(),
            };

            _traceListener.TraceData(_traceEventCache, _source, WarningEventType, _id, data);

            LogEventAssert.HasMessage($"[{data[0]}, {data[1]}]", _loggedEvent);

            LogEventAssert.HasLevel(LogEventLevel.Warning, _loggedEvent);

            LogEventAssert.HasPropertyValue(_id, "TraceEventId", _loggedEvent);
            LogEventAssert.HasPropertyValue(_source, "TraceSource", _loggedEvent);

            var sequence = ((SequenceValue)_loggedEvent.Properties["TraceData"]).Elements.Select(pv => pv.LiteralValue());

            Assert.Equal(data.Cast<object>(), sequence);
        }

        [Fact]
        public void Captures_TraceData_with_multiple_data_args()
        {
            var data1 = new
            {
                Info = Some.String(),
            };

            var data2 = new
            {
                Info = Some.String(),
            };

            var data3 = new
            {
                Info = Some.Int(),
            };

            _traceListener.TraceData(_traceEventCache, _source, WarningEventType, _id, data1, data2, data3);

            // The square-brackets ('[' ,']') are because of Serilog behavior and are not how the stock TraceListener would behave
            LogEventAssert.HasMessage(
                $@"[""{string.Join(@""", """, data1, data2, data3)}""]", _loggedEvent);

            LogEventAssert.HasLevel(LogEventLevel.Warning, _loggedEvent);

            LogEventAssert.HasPropertyValue(_id, "TraceEventId", _loggedEvent);
            LogEventAssert.HasPropertyValue(_source, "TraceSource", _loggedEvent);
            LogEventAssert.HasPropertyValueSequenceValue(new object[]
            {
                data1,
                data2,
                data3,
            }, "TraceData", _loggedEvent);

            LogEventAssert.HasPropertyValue(WarningEventType, "TraceEventType", _loggedEvent);
        }

        [Fact]
        public void Can_log_from_TraceSource_Information()
        {
            const string traceSourceName = nameof(Can_log_from_TraceSource_Information);
            const string logMessage = "A simple message";

            var traceSource = new TraceSource(traceSourceName, SourceLevels.All);
            traceSource.Listeners.Clear();
            traceSource.Listeners.Add(_traceListener);

            traceSource.TraceInformation(logMessage);

            LogEventAssert.HasLevel(LogEventLevel.Information, _loggedEvent);
            LogEventAssert.HasMessage(logMessage, _loggedEvent);
            LogEventAssert.HasPropertyValue(traceSourceName, "TraceSource", _loggedEvent);
        }

        [Fact]
        public void Correctly_orders_format_args()
        {
            const string format = "{1}-{0}-{1}";
            var args = new object[]
            {
                Some.Int(),
                Some.Int(),
                Some.Int(),
            };

            _traceListener.TraceEvent(_traceEventCache, _source, WarningEventType, _id, format, args);

            LogEventAssert.HasMessage($"{args[1]}-{args[0]}-{args[1]}", _loggedEvent);
            LogEventAssert.HasPropertyValue(args[2], "2", _loggedEvent);
        }

        [Fact]
        public void Continues_logging_after_Close_is_called()
        {
            _traceListener.Close();

            _traceListener.Write(_message);

            LogEventAssert.HasMessage(_message, _loggedEvent);
        }

        [Fact]
        public void Handles_empty_format_string()
        {
            var args = new object[]
            {
                Some.Int(),
                Some.Int(),
                Some.Int(),
            };

            _traceListener.TraceEvent(_traceEventCache, _source, WarningEventType, _id, format: string.Empty, args);

            LogEventAssert.HasMessage(string.Empty, _loggedEvent);
            LogEventAssert.HasPropertyValue(args[0], "0", _loggedEvent);
            LogEventAssert.HasPropertyValue(args[1], "1", _loggedEvent);
            LogEventAssert.HasPropertyValue(args[2], "2", _loggedEvent);
        }

        [Fact]
        public void Handles_null_format_string()
        {
            var args = new object[]
            {
                Some.Int(),
                Some.Int(),
                Some.Int(),
            };

            // ReSharper disable once AssignNullToNotNullAttribute
            _traceListener.TraceEvent(_traceEventCache, _source, WarningEventType, _id, format: null, args);

            LogEventAssert.HasMessage(string.Empty, _loggedEvent);
            LogEventAssert.HasPropertyValue(args[0], "0", _loggedEvent);
            LogEventAssert.HasPropertyValue(args[1], "1", _loggedEvent);
            LogEventAssert.HasPropertyValue(args[2], "2", _loggedEvent);
        }

        [Fact]
        public void Honors_MinimumLevel()
        {
            LogEvent logEvent = null;

            var delegatingSink = new DelegatingSink(evt =>
            {
                logEvent = evt;
            });

            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Sink(delegatingSink)
                .CreateLogger();

            var traceSource = new TraceSource(nameof(Honors_MinimumLevel), SourceLevels.All);
            traceSource.Listeners.Clear();
            traceSource.Listeners.Add(new SerilogTraceListener(nameof(SerilogTraceListenerTests)));

            traceSource.WriteToSerilog(logger);

            const string verboseMessage = "This is a Verbose message";
            const string warningMessage = "This is an Warning message";

            traceSource.TraceEvent(TraceEventType.Verbose, id: 1, verboseMessage);
            Assert.Null(logEvent);

            traceSource.TraceEvent(TraceEventType.Warning, id: 2, warningMessage);
            LogEventAssert.HasMessage(warningMessage, logEvent);
        }

        [Fact]
        public void Honors_MinimumLevel_Override()
        {
            LogEvent logEvent = null;

            var delegatingSink = new DelegatingSink(evt =>
            {
                logEvent = evt;
            });

            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override(SerilogTraceListener.Constants.ExcelDnaSourceContext,
                    LogEventLevel.Warning)
                .WriteTo.Sink(delegatingSink)
                .CreateLogger();

            var traceSource = new TraceSource(nameof(Honors_MinimumLevel), SourceLevels.All);
            traceSource.Listeners.Clear();
            traceSource.Listeners.Add(new SerilogTraceListener(nameof(SerilogTraceListenerTests)));

            traceSource.WriteToSerilog(logger);

            const string verboseMessage = "This is a Verbose message";
            const string warningMessage = "This is an Warning message";

            traceSource.TraceEvent(TraceEventType.Verbose, id: 1, verboseMessage);
            Assert.Null(logEvent);

            traceSource.TraceEvent(TraceEventType.Warning, id: 2, warningMessage);
            LogEventAssert.HasMessage(warningMessage, logEvent);
        }

        [Fact]
        public void Honors_Trace_Filter()
        {
            _traceListener.Filter = new DummyFilter(id => id % 2 == 0);

            _traceListener.TraceData(new TraceEventCache(), _source, TraceEventType.Information, 3, _message);
            Assert.Null(_loggedEvent);

            _traceListener.TraceData(new TraceEventCache(), _source, TraceEventType.Information, 4, _message);
            Assert.NotNull(_loggedEvent);
        }

        [Fact]
        public void Constructor_sets_TraceListener_name_to_empty_string()
        {
            using (var traceListener = new SerilogTraceListener())
            {
                Assert.Equal(string.Empty, traceListener.Name);
            }
        }

        [Fact]
        public void Constructor_with_name_sets_name_of_TraceListener_to_name_specified()
        {
            const string name = "ExcelDnaToSerilog";

            using (var traceListener = new SerilogTraceListener(name))
            {
                Assert.Equal(name, traceListener.Name);
            }
        }

        private class DummyFilter : TraceFilter
        {
            readonly Predicate<int> _predicate;

            public DummyFilter(Predicate<int> predicate)
            {
                _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            }

            public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data)
            {
                return _predicate(id);
            }
        }
    }
}

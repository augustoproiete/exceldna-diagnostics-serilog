using System;
using System.Linq;
using Serilog.Events;
using Xunit;

namespace ExcelDna.Diagnostics.Serilog.Tests.Support
{
    public static class LogEventAssert
    {
        public static void HasLevel(LogEventLevel logLevel, LogEvent logEvent)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            Assert.Equal(logLevel, logEvent.Level);
        }

        public static void HasMessage(string message, LogEvent logEvent)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            Assert.Equal(message, logEvent.RenderMessage());
        }

        public static void HasProperty(string propertyName, LogEvent logEvent)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            Assert.Contains(propertyName, logEvent.Properties);
        }

        public static void HasPropertyValue(object propertyValue, string propertyName, LogEvent logEvent)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            HasProperty(propertyName, logEvent);

            var value = logEvent.Properties[propertyName];
            Assert.Equal(propertyValue, value.LiteralValue());
        }

        public static void HasPropertyValueSequenceValue(object[] propertyValue, string propertyName, LogEvent logEvent)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            HasProperty(propertyName, logEvent);

            var value = logEvent.Properties[propertyName];
            var sequence = ((SequenceValue) value).Elements.Select(pv => pv.LiteralValue());

            Assert.Equal(propertyValue.Select(_ => _.ToString()), sequence);
        }
    }
}

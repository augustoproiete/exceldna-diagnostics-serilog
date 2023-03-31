#region Copyright 2019-2023 C. Augusto Proiete & Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

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

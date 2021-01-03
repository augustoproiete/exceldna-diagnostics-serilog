#region Copyright 2019-2021 C. Augusto Proiete & Contributors
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
using System.Diagnostics;
using System.Linq;
using Serilog;

namespace ExcelDna.Diagnostics.Serilog
{
    /// <summary>
    /// Utility class to bind a <see cref="SerilogTraceListener" /> to an <see cref="ILogger" />
    /// </summary>
    public static class ExcelDnaTraceSource
    {
        /// <summary>
        /// Forwards messages written to the <see cref="SerilogTraceListener" /> to default Serilog Logger i.e. <code>Log.Logger</code>
        /// Because Excel-DNA starts writing to the TraceListener before <code>WriteToSerilog</code> is called, all messages are buffered until <code>WriteToSerilog</code> is called
        /// </summary>
        public static void WriteToSerilog()
        {
            WriteToSerilog(Log.Logger);
        }

        /// <summary>
        /// Forwards messages written to the <see cref="SerilogTraceListener" /> to the <see cref="ILogger" /> specified
        /// Because Excel-DNA starts writing to the TraceListener before <code>WriteToSerilog</code> is called, all messages are buffered until <code>WriteToSerilog</code> is called
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" /> that trace messages should be written to</param>
        public static void WriteToSerilog(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            var traceSource = new TraceSource(SerilogTraceListener.Constants.ExcelDnaTraceSourceName, SourceLevels.Warning);

            WriteToSerilog(logger, traceSource);
        }

        internal static void WriteToSerilog(ILogger logger, TraceSource traceSource)
        {
            var listenersBound = 0;

            foreach (var traceListener in traceSource.Listeners.OfType<SerilogTraceListener>())
            {
                traceListener.WriteToSerilog(logger);
                ++listenersBound;
            }

            if (listenersBound == 0)
            {
                logger.Warning("Unable to find any listener of type {ListenerTypeName}",
                    typeof(SerilogTraceListener).FullName);
            }
        }
    }
}

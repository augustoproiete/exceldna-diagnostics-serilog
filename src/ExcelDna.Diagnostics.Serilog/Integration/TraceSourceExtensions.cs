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

using System.Diagnostics;
using Serilog;
using ExcelDna.Diagnostics.Serilog;

namespace ExcelDna.Integration
{
    /// <summary>
    /// Adds the <code>WriteToSerilog()</code> extension method to <see cref="TraceSource"/>
    /// </summary>
    public static class TraceSourceExtensions
    {
        /// <summary>
        /// Forwards messages written to the <see cref="SerilogTraceListener" /> to default Serilog Logger i.e. <code>Log.Logger</code>
        /// Because Excel-DNA starts writing to the TraceListener before <code>WriteToSerilog</code> is called, all messages are buffered until <code>WriteToSerilog</code> is called
        /// </summary>
        /// <param name="traceSource">The <see cref="TraceSource" /> that has a <see cref="SerilogTraceListener" /></param>
        public static void WriteToSerilog(this TraceSource traceSource)
        {
            ExcelDnaTraceSource.WriteToSerilog(Log.Logger, traceSource);
        }

        /// <summary>
        /// Forwards messages written to the <see cref="SerilogTraceListener" /> to the <see cref="ILogger" /> specified
        /// Because Excel-DNA starts writing to the TraceListener before <code>WriteToSerilog</code> is called, all messages are buffered until <code>WriteToSerilog</code> is called
        /// </summary>
        /// <param name="traceSource">The <see cref="TraceSource" /> that has a <see cref="SerilogTraceListener" /></param>
        /// <param name="logger">The <see cref="ILogger" /> that messages should be written to</param>
        public static void WriteToSerilog(this TraceSource traceSource, ILogger logger)
        {
            ExcelDnaTraceSource.WriteToSerilog(logger, traceSource);
        }
    }
}

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

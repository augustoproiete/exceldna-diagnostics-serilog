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

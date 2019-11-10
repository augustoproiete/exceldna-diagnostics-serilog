namespace ExcelDna.Diagnostics.Serilog
{
    public partial class SerilogTraceListener
    {
        /// <summary>
        /// Constants used in the logging pipeline and associated types
        /// </summary>
        public class Constants
        {
            /// <summary>
            /// The name of the property included in the emitted log events
            /// </summary>
            public const string SourceContextPropertyName = global::Serilog.Core.Constants.SourceContextPropertyName;

            /// <summary>
            /// The value of the property <code>SourceContext</code> included in the emitted log events
            /// </summary>
            public const string ExcelDnaSourceContext = "ExcelDna.Integration";

            /// <summary>
            /// The name of the TraceSource used by Excel-DNA 
            /// </summary>
            public const string ExcelDnaTraceSourceName = "ExcelDna.Integration";
        }
    }
}

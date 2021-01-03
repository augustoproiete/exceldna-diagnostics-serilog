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

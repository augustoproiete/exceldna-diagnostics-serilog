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
using ExcelDna.Integration;
using ExcelDna.Logging;
using ExcelDna.Diagnostics.Serilog;
using ExcelDna.Integration.Extensibility;
using Serilog;

namespace SampleAddIn
{
    public class AddIn : ExcelComAddIn, IExcelAddIn
    {
        public void AutoOpen()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File($"{ExcelDnaUtil.XllPath}.log")
                .WriteTo.ExcelDnaLogDisplay()
                .CreateLogger();

            // Forward any messages written by Excel-DNA to Serilog
            ExcelDnaTraceSource.WriteToSerilog();

            Log.Information("Hello from {AddInName}! :)", DnaLibrary.CurrentLibrary.Name);
            LogDisplay.Show();

            ExcelComAddInHelper.LoadComAddIn(this);
        }

        public void AutoClose()
        {
            // Do nothing
        }

        public override void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            Log.Information("Goodbye from {AddInName}! :)", DnaLibrary.CurrentLibrary.Name);
            Log.CloseAndFlush();

            base.OnDisconnection(disconnectMode, ref custom);
        }
    }
}

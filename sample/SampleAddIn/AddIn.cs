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

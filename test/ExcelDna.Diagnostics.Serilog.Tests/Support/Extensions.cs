using Serilog.Events;

namespace ExcelDna.Diagnostics.Serilog.Tests.Support
{
    public static class Extensions
    {
        public static object LiteralValue(this LogEventPropertyValue logEventPropertyValue)
        {
            return ((ScalarValue)logEventPropertyValue).Value;
        }
    }
}

using System;
using System.Threading;

namespace ExcelDna.Diagnostics.Serilog.Tests.Support
{
    public static class Some
    {
        private static int _counter;

        public static int Int()
        {
            return Interlocked.Increment(ref _counter);
        }

        public static string String(string tag = null)
        {
            return (tag ?? "") + "__" + Int();
        }

        public static Guid Guid()
        {
            return System.Guid.NewGuid();
        }
    }
}

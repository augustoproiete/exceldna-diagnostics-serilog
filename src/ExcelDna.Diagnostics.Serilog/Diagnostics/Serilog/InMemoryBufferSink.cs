using System;
using System.Collections.Generic;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace ExcelDna.Diagnostics.Serilog
{
    internal class InMemoryBufferSink : ILogEventSink, IDisposable
    {
        private readonly object _syncRoot = new object();
        private bool _disposed;

        private List<LogEvent> _logEvents = new List<LogEvent>();

        public void Emit(LogEvent logEvent)
        {
            EnsureNotDisposed();

            lock (_syncRoot)
            {
                _logEvents.Add(logEvent);
            }
        }

        public void WriteTo(ILogger logger)
        {
            lock (_syncRoot)
            {
                foreach (var logEvent in _logEvents)
                {
                    logger.Write(logEvent);
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _logEvents.Clear();
            _logEvents = null;

            _disposed = true;
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(InMemoryBufferSink));
            }
        }
    }
}

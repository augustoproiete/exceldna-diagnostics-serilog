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

﻿#region Copyright 2019-2023 C. Augusto Proiete & Contributors
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

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Erp.Framework
{
    public enum TraceLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

    /// <summary>
    /// Logování ERP. V produkci zapisuje do tabulky SYS_TRACE (přes ITraceSink z Erp.Data),
    /// v testech do paměti. Nepoužívat Console ani System.Diagnostics.Trace.
    /// Formát zprávy: "[modul] text", modul = název třídy nebo oblasti (Mzdy, DPH, KH...).
    /// </summary>
    public static class Trace
    {
        private static readonly object _lock = new object();
        private static readonly List<string> _memory = new List<string>();

        public static TraceLevel MinLevel = TraceLevel.Info;

        /// <summary>Volitelný zápis navíc (produkce: SYS_TRACE). Null = jen paměť.</summary>
        public static Action<TraceLevel, string> Sink;

        public static void Log(string module, string message)
        {
            Log(TraceLevel.Info, module, message);
        }

        public static void Log(TraceLevel level, string module, string message)
        {
            if (level < MinLevel)
                return;
            string line = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)
                          + " " + level.ToString().ToUpperInvariant() + " [" + module + "] " + message;
            lock (_lock)
            {
                _memory.Add(line);
                if (_memory.Count > 5000)
                    _memory.RemoveRange(0, 1000);
            }
            var sink = Sink;
            if (sink != null)
                sink(level, line);
        }

        public static IList<string> Snapshot()
        {
            lock (_lock)
            {
                return _memory.ToArray();
            }
        }

        public static void Clear()
        {
            lock (_lock)
            {
                _memory.Clear();
            }
        }
    }
}

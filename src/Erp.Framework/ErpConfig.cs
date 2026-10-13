using System;
using System.Collections.Generic;
using System.Globalization;

namespace Erp.Framework
{
    /// <summary>
    /// Konfigurace aplikace (v produkci tabulka SYS_NASTAVENI, načtená při startu).
    /// Klíče: "Oblast.Nazev", např. "Mzdy.RokParametru". Hodnoty jsou řetězce.
    /// </summary>
    public static class ErpConfig
    {
        private static readonly Dictionary<string, string> _values =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static void Set(string key, string value)
        {
            lock (_values)
            {
                _values[key] = value;
            }
        }

        public static string Get(string key, string defaultValue)
        {
            lock (_values)
            {
                string v;
                return _values.TryGetValue(key, out v) ? v : defaultValue;
            }
        }

        public static bool GetBool(string key, bool defaultValue)
        {
            string v = Get(key, null);
            if (v == null)
                return defaultValue;
            return v == "1" || v.Equals("true", StringComparison.OrdinalIgnoreCase)
                   || v.Equals("ano", StringComparison.OrdinalIgnoreCase);
        }

        public static int GetInt(string key, int defaultValue)
        {
            string v = Get(key, null);
            int result;
            return v != null && int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out result)
                ? result
                : defaultValue;
        }

        public static void Clear()
        {
            lock (_values)
            {
                _values.Clear();
            }
        }
    }
}

using System;

namespace Erp.Framework
{
    /// <summary>
    /// Zaokrouhlování podle českých předpisů. V ERP se NEPOUŽÍVÁ Math.Round bez MidpointRounding
    /// (výchozí je bankovní zaokrouhlení, 0,5 → 0). Vždy přes tyto metody.
    /// </summary>
    public static class CzkRounding
    {
        /// <summary>Na celé koruny nahoru (pojistné, záloha na daň).</summary>
        public static decimal NahoruNaKoruny(decimal castka)
        {
            return Math.Ceiling(castka);
        }

        /// <summary>Na celé koruny dolů (srážková daň).</summary>
        public static decimal DoluNaKoruny(decimal castka)
        {
            return Math.Floor(castka);
        }

        /// <summary>Na celé stokoruny nahoru (základ zálohy na daň nad 100 Kč).</summary>
        public static decimal NahoruNaStovky(decimal castka)
        {
            return Math.Ceiling(castka / 100m) * 100m;
        }

        /// <summary>Matematicky na haléře (DPH na dokladu).</summary>
        public static decimal NaHalere(decimal castka)
        {
            return Math.Round(castka, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>Matematicky na celé koruny (zaokrouhlení hotovostní platby).</summary>
        public static decimal NaKoruny(decimal castka)
        {
            return Math.Round(castka, 0, MidpointRounding.AwayFromZero);
        }
    }
}

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Erp.Framework;

namespace Erp.Vat
{
    /// <summary>Jeden řádek rekapitulace DPH: součet za jednu sazbu.</summary>
    public class RekapitulaceRadek
    {
        public decimal Sazba { get; set; }
        public Money Zaklad { get; set; }
        public Money Dan { get; set; }

        public Money Celkem
        {
            get { return Zaklad + Dan; }
        }

        public override string ToString()
        {
            return Sazba.ToString(CultureInfo.InvariantCulture) + " %: " + Zaklad + " + " + Dan;
        }
    }

    /// <summary>
    /// Rekapitulace DPH dokladu. Pravidlo firmy: DPH za sazbu se počítá ze SOUČTU základů řádků
    /// s touto sazbou a zaokrouhluje se matematicky na haléře. NE sečtením DPH jednotlivých řádků.
    /// </summary>
    public static class DphRekapitulace
    {
        public static ErpResult<List<RekapitulaceRadek>> Spocitej(Doklad doklad)
        {
            if (doklad == null)
                return ErpResult<List<RekapitulaceRadek>>.Fail("DPH-001", "Doklad není zadán.");
            if (doklad.Radky.Count == 0)
                return ErpResult<List<RekapitulaceRadek>>.Fail("DPH-002", "Doklad " + doklad.Cislo + " nemá řádky.");

            var sazby = ServiceRegistry.Get<IDphSazby>();
            foreach (var r in doklad.Radky)
            {
                if (!sazby.JePlatna(r.Sazba, doklad.Duzp))
                    return ErpResult<List<RekapitulaceRadek>>.Fail("DPH-003",
                        "Neplatná sazba " + r.Sazba.ToString(CultureInfo.InvariantCulture) + " % na dokladu " + doklad.Cislo + ".");
            }

            var vysledek = new List<RekapitulaceRadek>();
            foreach (var skupina in doklad.Radky.GroupBy(r => r.Sazba).OrderByDescending(g => g.Key))
            {
                decimal zaklad = 0m;
                foreach (var r in skupina)
                    zaklad += r.Zaklad;
                var radek = new RekapitulaceRadek();
                radek.Sazba = skupina.Key;
                radek.Zaklad = Money.Czk(zaklad);
                radek.Dan = Money.Czk(CzkRounding.NaHalere(zaklad * skupina.Key / 100m));
                vysledek.Add(radek);
            }
            return ErpResult<List<RekapitulaceRadek>>.Ok(vysledek);
        }

        /// <summary>Celková částka dokladu včetně DPH. Při chybě rekapitulace vrací jen základ.</summary>
        public static Money CelkemSDph(Doklad doklad)
        {
            var r = Spocitej(doklad);
            if (!r.Success)
                return Money.Czk(doklad == null ? 0m : doklad.CelkemBezDph);
            Money celkem = Money.Zero;
            foreach (var radek in r.Value)
                celkem = celkem + radek.Celkem;
            return celkem;
        }
    }
}

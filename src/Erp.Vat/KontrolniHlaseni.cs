using System.Collections.Generic;
using Erp.Framework;

namespace Erp.Vat
{
    public enum KhOddil
    {
        A4,
        A5,
        B2,
        B3,
        Nezahrnovat
    }

    /// <summary>Doklad vykázaný jednotlivě (A.4, B.2).</summary>
    public class KhRadek
    {
        public KhOddil Oddil { get; set; }
        public string CisloDokladu { get; set; }
        public string DicPartnera { get; set; }
        public List<RekapitulaceRadek> Rekapitulace { get; set; }
    }

    /// <summary>Souhrnný řádek (A.5, B.3): součty základů a daně podle sazeb.</summary>
    public class KhSouhrn
    {
        public KhOddil Oddil { get; set; }
        public decimal Zaklad21 { get; set; }
        public decimal Dan21 { get; set; }
        public decimal Zaklad12 { get; set; }
        public decimal Dan12 { get; set; }
        public int PocetDokladu { get; set; }
    }

    public class KhPodklady
    {
        public KhPodklady()
        {
            Jednotlive = new List<KhRadek>();
            A5 = new KhSouhrn { Oddil = KhOddil.A5 };
            B3 = new KhSouhrn { Oddil = KhOddil.B3 };
        }

        public int Obdobi { get; set; }
        public List<KhRadek> Jednotlive { get; private set; }
        public KhSouhrn A5 { get; private set; }
        public KhSouhrn B3 { get; private set; }
    }

    /// <summary>
    /// Rozdělení dokladů do oddílů kontrolního hlášení DPH.
    /// FV plátci nad limit → A.4, ostatní FV → A.5. FP od plátce nad limit → B.2, ostatní → B.3.
    /// FP bez DIČ dodavatele se do KH nedává (bez nároku na odpočet).
    /// Limit: ErpConfig "DPH.KH.Limit", výchozí 10 000 Kč.
    /// </summary>
    public static class KontrolniHlaseniRozdelovac
    {
        public static ErpResult<KhPodklady> Rozdel(IEnumerable<Doklad> doklady, int obdobi)
        {
            var vysledek = new KhPodklady();
            vysledek.Obdobi = obdobi;
            decimal limit = ErpConfig.GetInt("DPH.KH.Limit", 10000);

            foreach (var d in doklady)
            {
                if (d.Obdobi != obdobi)
                    continue;
                var rekap = DphRekapitulace.Spocitej(d);
                if (!rekap.Success)
                    return ErpResult<KhPodklady>.Fail(rekap.ErrorCode, rekap.Message);

                KhOddil oddil = UrciOddil(d, limit);
                Trace.Log(TraceLevel.Debug, "KH", d.Cislo + " → " + oddil);
                if (oddil == KhOddil.Nezahrnovat)
                    continue;
                if (oddil == KhOddil.A4 || oddil == KhOddil.B2)
                {
                    var radek = new KhRadek();
                    radek.Oddil = oddil;
                    radek.CisloDokladu = d.Cislo;
                    radek.DicPartnera = d.DicPartnera;
                    radek.Rekapitulace = rekap.Value;
                    vysledek.Jednotlive.Add(radek);
                }
                else
                {
                    Pricti(oddil == KhOddil.A5 ? vysledek.A5 : vysledek.B3, rekap.Value);
                }
            }
            return ErpResult<KhPodklady>.Ok(vysledek);
        }

        public static KhOddil UrciOddil(Doklad d, decimal limit)
        {
            decimal castka = d.CelkemBezDph;
            if (d.Druh == DruhDokladu.FV)
                return d.MaPartneraPlatce && castka > limit ? KhOddil.A4 : KhOddil.A5;
            if (!d.MaPartneraPlatce)
                return KhOddil.Nezahrnovat;
            return castka > limit ? KhOddil.B2 : KhOddil.B3;
        }

        private static void Pricti(KhSouhrn souhrn, List<RekapitulaceRadek> rekap)
        {
            souhrn.PocetDokladu++;
            foreach (var r in rekap)
            {
                if (r.Sazba == DphSazby2024.Zakladni)
                {
                    souhrn.Zaklad21 += r.Zaklad.Amount;
                    souhrn.Dan21 += r.Dan.Amount;
                }
                else if (r.Sazba == DphSazby2024.Snizena)
                {
                    souhrn.Zaklad12 += r.Zaklad.Amount;
                    souhrn.Dan12 += r.Dan.Amount;
                }
            }
        }
    }
}

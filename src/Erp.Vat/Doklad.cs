using System;
using System.Collections.Generic;
using Erp.Framework;

namespace Erp.Vat
{
    /// <summary>Druh daňového dokladu. FV = faktura vydaná (výstup), FP = faktura přijatá (vstup).</summary>
    public enum DruhDokladu
    {
        FV = 0,
        FP = 1
    }

    [DbTable("DPH_DOKLAD")]
    public class Doklad : BusinessObject
    {
        public Doklad()
        {
            Radky = new List<DokladRadek>();
        }

        [DbField("DOK_ID", IsKey = true)]
        public override int Id { get; set; }

        [DbField("DOK_CISLO")]
        public string Cislo { get; set; }

        [DbField("DOK_DRUH")]
        public DruhDokladu Druh { get; set; }

        /// <summary>DIČ odběratele (u FV) nebo dodavatele (u FP). Prázdné = neplátce nebo spotřebitel.</summary>
        [DbField("DOK_DIC_PARTNERA")]
        public string DicPartnera { get; set; }

        /// <summary>Datum uskutečnění zdanitelného plnění.</summary>
        [DbField("DOK_DUZP")]
        public DateTime Duzp { get; set; }

        /// <summary>Období DPH ve tvaru RRRRMM.</summary>
        [DbField("DOK_OBDOBI")]
        public int Obdobi { get; set; }

        /// <summary>Řádky dokladu. Načítá je KhRepository zvlášť (tabulka DPH_DOKLAD_RADEK).</summary>
        public List<DokladRadek> Radky { get; private set; }

        public decimal CelkemBezDph
        {
            get
            {
                decimal s = 0m;
                foreach (var r in Radky)
                    s += r.Zaklad;
                return s;
            }
        }

        public bool MaPartneraPlatce
        {
            get { return !string.IsNullOrWhiteSpace(DicPartnera); }
        }
    }

    [DbTable("DPH_DOKLAD_RADEK")]
    public class DokladRadek : BusinessObject
    {
        [DbField("RAD_ID", IsKey = true)]
        public override int Id { get; set; }

        [DbField("RAD_DOK_ID")]
        public int DokladId { get; set; }

        [DbField("RAD_POPIS")]
        public string Popis { get; set; }

        /// <summary>Základ daně řádku v Kč (bez DPH), na haléře.</summary>
        [DbField("RAD_ZAKLAD")]
        public decimal Zaklad { get; set; }

        /// <summary>Sazba DPH v procentech: 21, 12 nebo 0 (osvobozeno).</summary>
        [DbField("RAD_SAZBA")]
        public decimal Sazba { get; set; }

        public static DokladRadek Novy(string popis, decimal zaklad, decimal sazba)
        {
            var r = new DokladRadek();
            r.Popis = popis;
            r.Zaklad = zaklad;
            r.Sazba = sazba;
            return r;
        }
    }
}

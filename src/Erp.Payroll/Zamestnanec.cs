using Erp.Framework;

namespace Erp.Payroll
{
    [DbTable("MZD_ZAMESTNANEC")]
    public class Zamestnanec : BusinessObject
    {
        private string _prijmeni;
        private int _pocetDeti;

        [DbField("ZAM_ID", IsKey = true)]
        public override int Id { get; set; }

        [DbField("ZAM_OSOBNI_CISLO")]
        public string OsobniCislo { get; set; }

        [DbField("ZAM_JMENO")]
        public string Jmeno { get; set; }

        [DbField("ZAM_PRIJMENI")]
        public string Prijmeni
        {
            get { return _prijmeni; }
            set { _prijmeni = value; MarkDirty(); }
        }

        [DbField("ZAM_TYP_UVAZKU")]
        public TypUvazku TypUvazku { get; set; }

        /// <summary>Podepsané prohlášení poplatníka (§ 38k ZDP) u tohoto zaměstnavatele.</summary>
        [DbField("ZAM_PROHLASENI")]
        public bool Prohlaseni { get; set; }

        /// <summary>Počet vyživovaných dětí, na které zaměstnanec uplatňuje daňové zvýhodnění.</summary>
        [DbField("ZAM_POCET_DETI")]
        public int PocetDeti
        {
            get { return _pocetDeti; }
            set { _pocetDeti = value; MarkDirty(); }
        }

        /// <summary>Výjimka z minimálního vyměřovacího základu zdravotního pojištění (např. státní pojištěnec).</summary>
        [DbField("ZAM_VYJIMKA_MIN_ZDR")]
        public bool VyjimkaMinZdr { get; set; }

        public string CeleJmeno
        {
            get { return (Jmeno + " " + Prijmeni).Trim(); }
        }

        public override ErpResult Validate()
        {
            if (string.IsNullOrEmpty(OsobniCislo))
                return ErpResult.Fail("MZD-010", "Chybí osobní číslo.");
            if (PocetDeti < 0 || PocetDeti > 20)
                return ErpResult.Fail("MZD-011", "Neplatný počet dětí: " + PocetDeti);
            return ErpResult.Ok();
        }
    }
}

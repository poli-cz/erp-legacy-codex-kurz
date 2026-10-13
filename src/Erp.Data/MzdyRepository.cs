using System.Collections.Generic;
using Erp.Framework;
using Erp.Payroll;

namespace Erp.Data
{
    /// <summary>Načítání zaměstnanců a ukládání výpočtů mezd.</summary>
    public class MzdyRepository
    {
        private const string SqlZamestnanec =
            "SELECT ZAM_ID, ZAM_OSOBNI_CISLO, ZAM_JMENO, ZAM_PRIJMENI, ZAM_TYP_UVAZKU, ZAM_PROHLASENI, " +
            "ZAM_POCET_DETI, ZAM_VYJIMKA_MIN_ZDR FROM MZD_ZAMESTNANEC WHERE ZAM_ID = @id";

        private const string SqlAktivni =
            "SELECT ZAM_ID, ZAM_OSOBNI_CISLO, ZAM_JMENO, ZAM_PRIJMENI, ZAM_TYP_UVAZKU, ZAM_PROHLASENI, " +
            "ZAM_POCET_DETI, ZAM_VYJIMKA_MIN_ZDR FROM MZD_ZAMESTNANEC WHERE ZAM_AKTIVNI = 1 ORDER BY ZAM_OSOBNI_CISLO";

        private readonly IDbGateway _db;

        public MzdyRepository()
            : this(ServiceRegistry.Get<IDbGateway>())
        {
        }

        public MzdyRepository(IDbGateway db)
        {
            _db = db;
        }

        public Zamestnanec NactiZamestnance(int id)
        {
            var rows = _db.Query(SqlZamestnanec, new Dictionary<string, object> { { "id", id } });
            if (rows.Count == 0)
                return null;
            var z = new Zamestnanec();
            z.LoadFromRow(rows[0]);
            return z;
        }

        public List<Zamestnanec> NactiAktivni()
        {
            var result = new List<Zamestnanec>();
            foreach (var row in _db.Query(SqlAktivni, null))
            {
                var z = new Zamestnanec();
                z.LoadFromRow(row);
                result.Add(z);
            }
            return result;
        }

        /// <summary>
        /// Hrubá mzda za období ze mzdových složek (MZD_SLOZKA). Mzdové složky se v tomto výřezu neřeší,
        /// suma je hotová.
        /// </summary>
        public decimal NactiHrubou(int zamestnanecId, int obdobi)
        {
            object v = _db.Scalar(
                "SELECT SUM(SLO_CASTKA) FROM MZD_SLOZKA WHERE SLO_ZAM_ID = @zam AND SLO_OBDOBI = @obdobi",
                new Dictionary<string, object> { { "zam", zamestnanecId }, { "obdobi", obdobi } });
            return v == null ? 0m : System.Convert.ToDecimal(v, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Uloží výpočet přes proceduru usp_Mzdy_Vypocet_Uloz. Procedura smaže předchozí výpočet
        /// téhož zaměstnance a období a dopočítá sloupce, které C# neposílá (viz db/procedury).
        /// </summary>
        public ErpResult UlozVypocet(MzdovyListek listek)
        {
            var p = new Dictionary<string, object>();
            p["ZamId"] = listek.ZamestnanecId;
            p["Obdobi"] = listek.Obdobi;
            p["Hruba"] = listek.HrubaMzda;
            p["SocZam"] = listek.SocialniZamestnanec;
            p["SocFirma"] = listek.SocialniZamestnavatel;
            p["ZakladDane"] = listek.ZakladDane;
            p["Zaloha"] = listek.ZalohaDane;
            p["Srazkova"] = listek.SrazkovaDan;
            p["Bonus"] = listek.DanovyBonus;
            p["Cista"] = listek.CistaMzda;
            var rows = _db.ExecuteProcedure("usp_Mzdy_Vypocet_Uloz", p);
            if (rows.Count == 0 || rows[0]["MZD_ID"] == null)
                return ErpResult.Fail("DAT-001", "Procedura nevrátila ID výpočtu.");
            listek.Id = System.Convert.ToInt32(rows[0]["MZD_ID"], System.Globalization.CultureInfo.InvariantCulture);
            listek.MarkClean();
            return ErpResult.Ok();
        }

        /// <summary>Smaže výpočty období (přepočet celé uzávěrky).</summary>
        public int SmazVypocty(int obdobi)
        {
            // obdobi je int, takže skládání je "bezpečné" (původní kód z 2011)
            return _db.Execute("DELETE FROM MZD_VYPOCET WHERE MZD_OBDOBI = " + obdobi, null);
        }
    }
}

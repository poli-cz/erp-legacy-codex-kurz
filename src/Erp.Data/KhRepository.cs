using System.Collections.Generic;
using System.Globalization;
using Erp.Framework;
using Erp.Vat;

namespace Erp.Data
{
    /// <summary>Doklady pro kontrolní hlášení.</summary>
    public class KhRepository
    {
        private readonly IDbGateway _db;

        public KhRepository()
            : this(ServiceRegistry.Get<IDbGateway>())
        {
        }

        public KhRepository(IDbGateway db)
        {
            _db = db;
        }

        public List<Doklad> NactiDoklady(int obdobi)
        {
            var doklady = new Dictionary<int, Doklad>();
            var result = new List<Doklad>();
            var p = new Dictionary<string, object> { { "obdobi", obdobi } };

            foreach (var row in _db.Query(
                         "SELECT DOK_ID, DOK_CISLO, DOK_DRUH, DOK_DIC_PARTNERA, DOK_DUZP, DOK_OBDOBI " +
                         "FROM DPH_DOKLAD WHERE DOK_OBDOBI = @obdobi AND DOK_STORNO = 0", p))
            {
                var d = new Doklad();
                d.LoadFromRow(row);
                doklady[d.Id] = d;
                result.Add(d);
            }

            foreach (var row in _db.Query(
                         "SELECT r.RAD_ID, r.RAD_DOK_ID, r.RAD_POPIS, r.RAD_ZAKLAD, r.RAD_SAZBA " +
                         "FROM DPH_DOKLAD_RADEK r JOIN DPH_DOKLAD d ON d.DOK_ID = r.RAD_DOK_ID " +
                         "WHERE d.DOK_OBDOBI = @obdobi AND d.DOK_STORNO = 0", p))
            {
                var r = new DokladRadek();
                r.LoadFromRow(row);
                Doklad d;
                if (doklady.TryGetValue(r.DokladId, out d))
                    d.Radky.Add(r);
            }
            return result;
        }

        /// <summary>
        /// Podklady KH spočítané v databázi (procedura usp_KH_Podklady). Používá je tisková sestava KH,
        /// obrazovka KH používá KontrolniHlaseniRozdelovac v C#.
        /// </summary>
        public Dictionary<string, KhOddil> NactiOddilyZDb(int obdobi)
        {
            var result = new Dictionary<string, KhOddil>();
            foreach (var row in _db.ExecuteProcedure("usp_KH_Podklady",
                         new Dictionary<string, object> { { "Obdobi", obdobi } }))
            {
                string cislo = System.Convert.ToString(row["DOK_CISLO"], CultureInfo.InvariantCulture);
                string oddil = System.Convert.ToString(row["ODDIL"], CultureInfo.InvariantCulture);
                result[cislo] = (KhOddil)System.Enum.Parse(typeof(KhOddil), oddil);
            }
            return result;
        }
    }
}

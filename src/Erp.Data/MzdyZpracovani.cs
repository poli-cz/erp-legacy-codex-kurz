using System.Collections.Generic;
using Erp.Framework;
using Erp.Payroll;

namespace Erp.Data
{
    /// <summary>
    /// Hromadné zpracování mezd za období (uzávěrka): pro každého aktivního zaměstnance načte hrubou,
    /// spočítá výpočet a uloží ho. Chyba u jednoho zaměstnance nezastaví ostatní.
    /// </summary>
    public class MzdyZpracovani
    {
        public ErpResult<List<MzdovyListek>> ZpracujObdobi(int obdobi)
        {
            var repo = ServiceRegistry.Get<MzdyRepository>();
            var kalkulace = new MzdaKalkulace();
            var hotove = new List<MzdovyListek>();
            int chyby = 0;

            foreach (var zam in repo.NactiAktivni())
            {
                decimal hruba = repo.NactiHrubou(zam.Id, obdobi);
                var r = kalkulace.Vypocitej(zam, hruba, obdobi);
                if (!r.Success)
                {
                    chyby++;
                    Trace.Log(TraceLevel.Error, "Uzávěrka", zam.OsobniCislo + ": " + r);
                    continue;
                }
                var ulozeni = repo.UlozVypocet(r.Value);
                if (!ulozeni.Success)
                {
                    chyby++;
                    Trace.Log(TraceLevel.Error, "Uzávěrka", zam.OsobniCislo + ": " + ulozeni);
                    continue;
                }
                hotove.Add(r.Value);
            }

            Trace.Log("Uzávěrka", "Období " + obdobi + ": zpracováno " + hotove.Count + ", chyb " + chyby + ".");
            var vysledek = ErpResult<List<MzdovyListek>>.Ok(hotove);
            if (chyby > 0)
                vysledek.Warnings.Add("Počet zaměstnanců s chybou: " + chyby);
            return vysledek;
        }
    }
}

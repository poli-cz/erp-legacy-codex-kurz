using System;

namespace Erp.Payroll
{
    /// <summary>
    /// Parametry mezd pro rok 2026. Zdroje a ověření: docs/PARAMETRY-2026.md.
    /// Při změně roku: nová třída ParametryMezdRRRR, registrace v PayrollModule.
    /// </summary>
    public class ParametryMezd2026 : IParametryMezd
    {
        public int Rok
        {
            get { return 2026; }
        }

        // nařízení vlády o průměrné mzdě pro účely pojistného a daní
        public decimal PrumernaMzda
        {
            get { return 48967m; }
        }

        public decimal MinimalniMzda
        {
            get { return 22400m; }
        }

        public decimal SlevaNaPoplatnika
        {
            get { return 2570m; }
        }

        public decimal ZvyhodneniNaDite(int poradi)
        {
            if (poradi < 1)
                throw new ArgumentOutOfRangeException("poradi");
            if (poradi == 1)
                return 1267m;
            if (poradi == 2)
                return 1860m;
            return 2320m;
        }

        public decimal RozhodnyPrijemDpp
        {
            get { return 12000m; }
        }

        public decimal RozhodnyPrijemMalyRozsah
        {
            get { return 4500m; }
        }
    }
}

namespace Erp.Payroll
{
    /// <summary>Druh pracovního vztahu. V DB sloupec ZAM_TYP_UVAZKU jako text (HPP, DPP, DPC).</summary>
    public enum TypUvazku
    {
        /// <summary>Pracovní poměr (hlavní i vedlejší).</summary>
        HPP = 0,

        /// <summary>Dohoda o provedení práce.</summary>
        DPP = 1,

        /// <summary>Dohoda o pracovní činnosti.</summary>
        DPC = 2
    }
}

// ============================================================
//  StareKonstanty.cs
//  Konstanty pøevzaté ze staré verze mezd (Delphi, MZDY.INC).
//  Soubor je uložen ve Windows-1250 a s CRLF – NEPØEKÓDOVÁVAT!
//  Pøi pøevodu do UTF-8 v roce 2019 se rozbily tiskové sestavy
//  (výplatní pásky šly na jehlièkovou tiskárnu s èeštinou CP1250).
//  Autor: J. Novák, 2009. Úpravy každý leden podle novel.
// ============================================================

namespace Erp.Payroll
{
    /// <summary>Historické konstanty mezd. Nové parametry patøí do IParametryMezd.</summary>
    public static class StareKonstanty
    {
        // Záloha na daò: 15 % do násobku prùmìrné mzdy, z èásti základu nad ním 23 %.
        // Hranice = NASOBEK_PM_SAZBA_23 × prùmìrná mzda (z IParametryMezd).
        public const decimal NASOBEK_PM_SAZBA_23 = 4m;

        // Polovina minimální mzdy – podmínka nároku na daòový bonus.
        // Aktualizovat k 1. 1. !!!
        public const decimal MIN_MZDA_POLOVINA = 10400m;

        // Mìsíèní daòový bonus se vyplácí, jen když dosáhne aspoò této èástky.
        public const decimal BONUS_MIN_MESIC = 50m;

        // Základ zálohy do této èástky se zaokrouhluje na koruny, nad ní na stokoruny.
        public const decimal HRANICE_ZAOKR_ZAKLADU = 100m;

        // Texty na výplatní pásce.
        public const string TEXT_HRUBA = "Hrubá mzda";
        public const string TEXT_SOCIALNI = "Sociální pojištìní";
        public const string TEXT_ZDRAVOTNI = "Zdravotní pojištìní";
        public const string TEXT_ZAKLAD = "Základ danì";
        public const string TEXT_SLEVA_POPL = "Sleva na poplatníka";
        public const string TEXT_SLEVA_DETI = "Sleva na dìti";
        public const string TEXT_ZALOHA = "Záloha na daò";
        public const string TEXT_SRAZKOVA = "Srážková daò";
        public const string TEXT_BONUS = "Daòový bonus";
        public const string TEXT_CISTA = "Èistá mzda";

        // 2021: superhrubá mzda zrušena. Nechat kvùli importu starých období (pøepoèty pøed 2021).
        // public const decimal SUPERHRUBA_KOEF = 1.338m;
    }
}

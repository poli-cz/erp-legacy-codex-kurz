using Erp.Framework;

namespace Erp.Payroll
{
    /// <summary>Výsledek výpočtu mzdy za jedno období (měsíc). Tabulka MZD_VYPOCET.</summary>
    [DbTable("MZD_VYPOCET")]
    public class MzdovyListek : BusinessObject
    {
        [DbField("MZD_ID", IsKey = true)]
        public override int Id { get; set; }

        [DbField("MZD_ZAM_ID")]
        public int ZamestnanecId { get; set; }

        /// <summary>Období ve tvaru RRRRMM, např. 202603.</summary>
        [DbField("MZD_OBDOBI")]
        public int Obdobi { get; set; }

        [DbField("MZD_TYP_UVAZKU")]
        public TypUvazku TypUvazku { get; set; }

        [DbField("MZD_HRUBA")]
        public decimal HrubaMzda { get; set; }

        [DbField("MZD_SOC_ZAM")]
        public decimal SocialniZamestnanec { get; set; }

        [DbField("MZD_ZDR_ZAM")]
        public decimal ZdravotniZamestnanec { get; set; }

        [DbField("MZD_SOC_FIRMA")]
        public decimal SocialniZamestnavatel { get; set; }

        /// <summary>Počítá procedura usp_Mzdy_Vypocet_Uloz, C# hodnotu jen zobrazuje.</summary>
        [DbField("MZD_ZDR_FIRMA", ReadOnly = true)]
        public decimal ZdravotniZamestnavatel { get; set; }

        [DbField("MZD_ZAKLAD_DANE")]
        public decimal ZakladDane { get; set; }

        [DbField("MZD_DAN_PRED_SLEVAMI")]
        public decimal DanPredSlevami { get; set; }

        [DbField("MZD_SLEVA_POPL")]
        public decimal SlevaPoplatnik { get; set; }

        [DbField("MZD_SLEVA_DETI")]
        public decimal SlevaNaDeti { get; set; }

        [DbField("MZD_BONUS")]
        public decimal DanovyBonus { get; set; }

        [DbField("MZD_ZALOHA")]
        public decimal ZalohaDane { get; set; }

        [DbField("MZD_SRAZKOVA")]
        public decimal SrazkovaDan { get; set; }

        [DbField("MZD_CISTA")]
        public decimal CistaMzda { get; set; }

        /// <summary>Zdravotní pojištění zaměstnance a zaměstnavatele celkem (13,5 %). Jen pro sestavy.</summary>
        public decimal ZdravotniCelkem
        {
            get { return ZdravotniZamestnanec + ZdravotniZamestnavatel; }
        }
    }
}

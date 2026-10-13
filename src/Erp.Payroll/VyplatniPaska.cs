using System.Globalization;
using System.Text;

namespace Erp.Payroll
{
    /// <summary>
    /// Textová výplatní páska (tisk na pásku, export do PDF dělá jiná vrstva).
    /// Šířka 40 znaků, částky zarovnané vpravo, formát cs-CZ. Texty řádků jsou v StareKonstanty.
    /// </summary>
    public static class VyplatniPaska
    {
        private const int Sirka = 40;
        private static readonly CultureInfo Cs = CultureInfo.GetCultureInfo("cs-CZ");

        public static string Formatuj(Zamestnanec zam, MzdovyListek l)
        {
            var sb = new StringBuilder();
            sb.Append("VÝPLATNÍ PÁSKA ").Append(l.Obdobi % 100).Append('/').Append(l.Obdobi / 100).Append('\n');
            sb.Append(zam.OsobniCislo).Append(' ').Append(zam.CeleJmeno).Append(" (").Append(zam.TypUvazku).Append(")\n");
            sb.Append(new string('-', Sirka)).Append('\n');
            Radek(sb, StareKonstanty.TEXT_HRUBA, l.HrubaMzda);
            Radek(sb, StareKonstanty.TEXT_SOCIALNI, -l.SocialniZamestnanec);
            Radek(sb, StareKonstanty.TEXT_ZDRAVOTNI, -l.ZdravotniZamestnanec);
            if (l.SrazkovaDan > 0)
            {
                Radek(sb, StareKonstanty.TEXT_SRAZKOVA, -l.SrazkovaDan);
            }
            else
            {
                Radek(sb, StareKonstanty.TEXT_ZAKLAD, l.ZakladDane);
                if (l.SlevaPoplatnik > 0)
                    Radek(sb, StareKonstanty.TEXT_SLEVA_POPL, l.SlevaPoplatnik);
                if (l.SlevaNaDeti > 0)
                    Radek(sb, StareKonstanty.TEXT_SLEVA_DETI, l.SlevaNaDeti);
                Radek(sb, StareKonstanty.TEXT_ZALOHA, -l.ZalohaDane);
                if (l.DanovyBonus > 0)
                    Radek(sb, StareKonstanty.TEXT_BONUS, l.DanovyBonus);
            }
            sb.Append(new string('=', Sirka)).Append('\n');
            Radek(sb, StareKonstanty.TEXT_CISTA, l.CistaMzda);
            return sb.ToString();
        }

        private static void Radek(StringBuilder sb, string text, decimal castka)
        {
            string c = castka.ToString("N2", Cs).Replace(' ', ' ').Replace(' ', ' ');
            int mezera = Sirka - text.Length - c.Length;
            if (mezera < 1)
                mezera = 1;
            sb.Append(text).Append(' ', mezera).Append(c).Append('\n');
        }
    }
}

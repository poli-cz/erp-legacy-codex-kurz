using System;

namespace Erp.Vat
{
    /// <summary>Platné sazby DPH k datu plnění.</summary>
    public interface IDphSazby
    {
        bool JePlatna(decimal sazba, DateTime duzp);
    }

    /// <summary>
    /// Sazby od 1. 1. 2024: základní 21 %, snížená 12 % (dřív 15 % a 10 %). 0 = osvobozené plnění.
    /// </summary>
    public class DphSazby2024 : IDphSazby
    {
        public const decimal Zakladni = 21m;
        public const decimal Snizena = 12m;
        public const decimal Osvobozeno = 0m;

        public bool JePlatna(decimal sazba, DateTime duzp)
        {
            if (duzp < new DateTime(2024, 1, 1))
                return sazba == 21m || sazba == 15m || sazba == 10m || sazba == 0m;
            return sazba == Zakladni || sazba == Snizena || sazba == Osvobozeno;
        }
    }
}

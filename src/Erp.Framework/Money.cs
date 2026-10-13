using System;
using System.Globalization;

namespace Erp.Framework
{
    /// <summary>
    /// Peněžní částka s měnou. Novější moduly (DPH) používají Money, starší (mzdy) holý decimal.
    /// Sčítat lze jen stejnou měnu. Zaokrouhlování přes CzkRounding.
    /// </summary>
    public struct Money : IEquatable<Money>, IComparable<Money>
    {
        public const string CZK = "CZK";

        private static readonly CultureInfo _cs = CultureInfo.GetCultureInfo("cs-CZ");

        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency ?? CZK;
        }

        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        public static Money Czk(decimal amount)
        {
            return new Money(amount, CZK);
        }

        public static Money Zero
        {
            get { return Czk(0m); }
        }

        public Money RoundHalere()
        {
            return new Money(CzkRounding.NaHalere(Amount), Currency);
        }

        public static Money operator +(Money a, Money b)
        {
            CheckCurrency(a, b);
            return new Money(a.Amount + b.Amount, a.Currency);
        }

        public static Money operator -(Money a, Money b)
        {
            CheckCurrency(a, b);
            return new Money(a.Amount - b.Amount, a.Currency);
        }

        public static Money operator *(Money a, decimal factor)
        {
            return new Money(a.Amount * factor, a.Currency);
        }

        public static bool operator >(Money a, Money b)
        {
            CheckCurrency(a, b);
            return a.Amount > b.Amount;
        }

        public static bool operator <(Money a, Money b)
        {
            CheckCurrency(a, b);
            return a.Amount < b.Amount;
        }

        public static bool operator ==(Money a, Money b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Money a, Money b)
        {
            return !a.Equals(b);
        }

        public bool Equals(Money other)
        {
            return Amount == other.Amount && string.Equals(Currency ?? CZK, other.Currency ?? CZK);
        }

        public override bool Equals(object obj)
        {
            return obj is Money && Equals((Money)obj);
        }

        public override int GetHashCode()
        {
            return Amount.GetHashCode() ^ (Currency ?? CZK).GetHashCode();
        }

        public int CompareTo(Money other)
        {
            CheckCurrency(this, other);
            return Amount.CompareTo(other.Amount);
        }

        /// <summary>Formát pro tiskové sestavy: "1 234,50 CZK".</summary>
        public override string ToString()
        {
            return Amount.ToString("N2", _cs) + " " + (Currency ?? CZK);
        }

        private static void CheckCurrency(Money a, Money b)
        {
            if ((a.Currency ?? CZK) != (b.Currency ?? CZK))
                throw new InvalidOperationException("Nelze kombinovat měny " + a.Currency + " a " + b.Currency + ".");
        }
    }
}

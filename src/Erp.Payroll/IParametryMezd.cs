namespace Erp.Payroll
{
    /// <summary>
    /// Roční parametry mezd. Implementace pro rok 2026 je ParametryMezd2026.
    /// Pozor: ne všechno je tady. Část konstant je historicky v StareKonstanty.cs
    /// a část přímo v MzdaKalkulace (sazby pojistného a daně).
    /// </summary>
    public interface IParametryMezd
    {
        int Rok { get; }

        decimal PrumernaMzda { get; }

        decimal MinimalniMzda { get; }

        decimal SlevaNaPoplatnika { get; }

        /// <summary>Měsíční daňové zvýhodnění na dítě podle pořadí (1, 2, 3 a další).</summary>
        decimal ZvyhodneniNaDite(int poradi);

        /// <summary>Rozhodný příjem: od této částky měsíčně je DPP účastna pojištění.</summary>
        decimal RozhodnyPrijemDpp { get; }

        /// <summary>Rozhodný příjem zaměstnání malého rozsahu (DPČ, krátký úvazek).</summary>
        decimal RozhodnyPrijemMalyRozsah { get; }
    }
}

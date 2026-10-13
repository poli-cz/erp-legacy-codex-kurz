using Erp.Payroll;

namespace Erp.Tests;

/// <summary>
/// Hlídá, že kompilátor čte StareKonstanty.cs správně (Windows-1250, viz docs/KODOVANI.md).
/// Když někdo soubor překóduje, rozbijí se texty na výplatní pásce a tento test spadne.
/// </summary>
public class KodovaniTests
{
    [Fact]
    public void Texty_ze_starych_konstant_maji_spravnou_diakritiku()
    {
        Assert.Equal("Čistá mzda", StareKonstanty.TEXT_CISTA);
        Assert.Equal("Daňový bonus", StareKonstanty.TEXT_BONUS);
        Assert.Equal("Srážková daň", StareKonstanty.TEXT_SRAZKOVA);
    }
}

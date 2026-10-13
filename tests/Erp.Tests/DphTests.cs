using Erp.Tests.Infrastruktura;
using Erp.Vat;

namespace Erp.Tests;

public class DphTests : ErpTestBase
{
    private static Doklad Doklad(DruhDokladu druh, string dic, params DokladRadek[] radky)
    {
        var d = new Doklad { Cislo = "FV2026-0001", Druh = druh, DicPartnera = dic, Duzp = new DateTime(2026, 3, 15), Obdobi = 202603 };
        d.Radky.AddRange(radky);
        return d;
    }

    [Fact]
    public void Dph_se_pocita_ze_souctu_zakladu_za_sazbu()
    {
        var d = Doklad(DruhDokladu.FV, "CZ12345678",
            DokladRadek.Novy("A", 10.01m, 21m), DokladRadek.Novy("B", 10.01m, 21m), DokladRadek.Novy("C", 10.01m, 21m));

        var r = DphRekapitulace.Spocitej(d);

        Assert.True(r.Success);
        Assert.Single(r.Value);
        Assert.Equal(6.31m, r.Value[0].Dan.Amount);
    }

    [Fact]
    public void Velky_doklad_platci_jde_do_A4()
    {
        var d = Doklad(DruhDokladu.FV, "CZ12345678", DokladRadek.Novy("Služba", 20000m, 21m));

        var r = KontrolniHlaseniRozdelovac.Rozdel(new[] { d }, 202603);

        Assert.True(r.Success);
        Assert.Single(r.Value.Jednotlive);
        Assert.Equal(KhOddil.A4, r.Value.Jednotlive[0].Oddil);
    }
}

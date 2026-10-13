using Erp.Payroll;
using Erp.Tests.Infrastruktura;

namespace Erp.Tests;

public class MzdaKalkulaceTests : ErpTestBase
{
    private static Zamestnanec Zam(TypUvazku typ, bool prohlaseni, int deti) => new()
    {
        OsobniCislo = "T001",
        Jmeno = "Test",
        Prijmeni = "Testovací",
        TypUvazku = typ,
        Prohlaseni = prohlaseni,
        PocetDeti = deti
    };

    [Fact]
    public void Hpp_40000_bez_deti()
    {
        var r = new MzdaKalkulace().Vypocitej(Zam(TypUvazku.HPP, true, 0), 40000m, 202604);

        Assert.True(r.Success);
        Assert.Equal(2840m, r.Value.SocialniZamestnanec);
        Assert.Equal(1800m, r.Value.ZdravotniZamestnanec);
        Assert.Equal(3430m, r.Value.ZalohaDane);
        Assert.Equal(31930m, r.Value.CistaMzda);
    }

    [Fact]
    public void Hpp_35000_dve_deti_bonus()
    {
        var r = new MzdaKalkulace().Vypocitej(Zam(TypUvazku.HPP, true, 2), 35000m, 202603);

        Assert.True(r.Success);
        Assert.Equal(0m, r.Value.ZalohaDane);
        Assert.Equal(447m, r.Value.DanovyBonus);
        Assert.Equal(31387m, r.Value.CistaMzda);
    }

    [Fact]
    public void Dpp_bez_prohlaseni_srazkova_dan()
    {
        var r = new MzdaKalkulace().Vypocitej(Zam(TypUvazku.DPP, false, 0), 5000m, 202601);

        Assert.True(r.Success);
        Assert.Equal(750m, r.Value.SrazkovaDan);
        Assert.Equal(4250m, r.Value.CistaMzda);
    }

    [Fact]
    public void Obdobi_jineho_roku_je_chyba()
    {
        var r = new MzdaKalkulace().Vypocitej(Zam(TypUvazku.HPP, true, 0), 40000m, 202512);

        Assert.False(r.Success);
        Assert.Equal("MZD-004", r.ErrorCode);
    }
}

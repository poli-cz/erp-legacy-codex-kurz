using Erp.Payroll;
using Erp.Tests.Infrastruktura;

namespace Erp.Tests;

public class VyplatniPaskaTests : ErpTestBase
{
    [Fact]
    public void Paska_hpp_dve_deti()
    {
        var zam = new Zamestnanec
        {
            OsobniCislo = "1042", Jmeno = "Jana", Prijmeni = "Vzorová",
            TypUvazku = TypUvazku.HPP, Prohlaseni = true, PocetDeti = 2
        };
        var r = new MzdaKalkulace().Vypocitej(zam, 35000m, 202603);

        Approvals.Verify(VyplatniPaska.Formatuj(zam, r.Value));
    }
}

using Erp.Data;
using Erp.Payroll;
using Erp.Tests.Infrastruktura;

namespace Erp.Tests;

public class MzdyRepositoryTests : ErpTestBase
{
    [Fact]
    public void Ulozeni_vola_proceduru_a_nastavi_id()
    {
        var db = new InMemoryDbGateway()
            .Vrat("usp_Mzdy_Vypocet_Uloz", new Dictionary<string, object> { ["MZD_ID"] = 77 });
        var listek = new MzdovyListek { ZamestnanecId = 5, Obdobi = 202603, HrubaMzda = 35000m, CistaMzda = 29527m };

        var r = new MzdyRepository(db).UlozVypocet(listek);

        Assert.True(r.Success);
        Assert.Equal(77, listek.Id);
        var volani = Assert.Single(db.Historie);
        Assert.True(volani.Procedura);
        Assert.Equal(35000m, volani.Parametry["Hruba"]);
    }
}

using Erp.Framework;
using Erp.Payroll;
using Erp.Vat;

namespace Erp.Tests.Infrastruktura;

/// <summary>
/// Základ testů: zaregistruje moduly do statického ServiceRegistry.
/// Pozor: ServiceRegistry je sdílený celým procesem, proto testy běží v jedné kolekci (bez paralelizace).
/// </summary>
[Collection("ServiceRegistry")]
public abstract class ErpTestBase
{
    protected ErpTestBase()
    {
        ServiceRegistry.Clear();
        ErpConfig.Clear();
        PayrollModule.Register();
        VatModule.Register();
    }
}

[CollectionDefinition("ServiceRegistry", DisableParallelization = true)]
public class ServiceRegistryCollection
{
}

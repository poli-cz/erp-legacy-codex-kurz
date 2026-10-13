using Erp.Framework;

namespace Erp.Data
{
    /// <summary>Registrace datové vrstvy. Volá se po PayrollModule a VatModule.</summary>
    public static class DataModule
    {
        public static void Register(IDbGateway gateway)
        {
            ServiceRegistry.Register<IDbGateway>(gateway);
            ServiceRegistry.Register<MzdyRepository>(() => new MzdyRepository());
            ServiceRegistry.Register<KhRepository>(() => new KhRepository());
        }
    }
}

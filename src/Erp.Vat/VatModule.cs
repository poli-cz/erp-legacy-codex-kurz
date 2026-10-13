using Erp.Framework;

namespace Erp.Vat
{
    /// <summary>Registrace služeb modulu DPH do ServiceRegistry. Volat jednou při startu.</summary>
    public static class VatModule
    {
        public static void Register()
        {
            ServiceRegistry.Register<IDphSazby>(new DphSazby2024());
            Trace.Log("DPH", "Modul DPH registrován.");
        }
    }
}

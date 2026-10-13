using Erp.Framework;

namespace Erp.Payroll
{
    /// <summary>Registrace služeb modulu Mzdy do ServiceRegistry. Volat jednou při startu.</summary>
    public static class PayrollModule
    {
        public static void Register()
        {
            ServiceRegistry.Register<IParametryMezd>(new ParametryMezd2026());
            Trace.Log("Mzdy", "Modul Mzdy registrován, parametry 2026.");
        }
    }
}

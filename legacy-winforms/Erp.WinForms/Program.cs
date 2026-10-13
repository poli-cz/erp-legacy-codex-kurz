using System;
using System.Windows.Forms;
using Erp.Payroll;
using Erp.Vat;

namespace Erp.WinForms
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            // ErpApplication.Init(): registrace modulů do ServiceRegistry
            PayrollModule.Register();
            VatModule.Register();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HlavniForm());
        }
    }
}

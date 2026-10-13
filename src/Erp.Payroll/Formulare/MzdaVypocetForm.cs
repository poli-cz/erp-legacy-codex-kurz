using System;
using System.Globalization;
using Erp.Framework;
using Erp.Framework.Ui;

namespace Erp.Payroll.Formulare
{
    /// <summary>
    /// Formulář rychlého výpočtu mzdy (modelový výpočet bez uložení).
    /// Rozložení je v MzdaVypocetForm.Designer.cs (generuje návrhář, ručně neupravovat).
    /// </summary>
    public partial class MzdaVypocetForm : ErpForm
    {
        public MzdaVypocetForm()
        {
            InitializeComponent();
        }

        // pro testy a automatizaci (UI test robot)
        public ErpTextEdit OsobniCislo { get { return txtOsobniCislo; } }
        public ErpSpinEdit Hruba { get { return spnHruba; } }
        public ErpTextEdit Obdobi { get { return txtObdobi; } }
        public ErpComboBox TypUvazkuCombo { get { return cmbTypUvazku; } }
        public ErpCheckEdit Prohlaseni { get { return chkProhlaseni; } }
        public ErpSpinEdit Deti { get { return spnDeti; } }
        public ErpButton Spocitej { get { return btnSpocitej; } }
        public ErpTextEdit Cista { get { return txtCista; } }
        public ErpLabel Stav { get { return lblStav; } }

        private void btnSpocitej_Click(object sender, EventArgs e)
        {
            // logika je přímo ve formuláři (historicky), MzdaKalkulace bere z ServiceRegistry
            var zam = new Zamestnanec();
            zam.OsobniCislo = string.IsNullOrEmpty(txtOsobniCislo.Text) ? "MODEL" : txtOsobniCislo.Text;
            zam.TypUvazku = (TypUvazku)cmbTypUvazku.SelectedIndex;
            zam.Prohlaseni = chkProhlaseni.Checked;
            zam.PocetDeti = (int)spnDeti.Value;

            int obdobi;
            if (!int.TryParse(txtObdobi.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out obdobi))
                obdobi = DateTime.Today.Year * 100 + DateTime.Today.Month;

            var kalk = new MzdaKalkulace();
            var r = kalk.Vypocitej(zam, spnHruba.Value, obdobi);
            if (!r.Success)
            {
                txtCista.Text = "";
                lblStav.Text = r.ErrorCode + ": " + r.Message;
                return;
            }
            txtCista.Text = r.Value.CistaMzda.ToString("N2", CultureInfo.GetCultureInfo("cs-CZ"));
            lblStav.Text = r.Warnings.Count > 0 ? string.Join(" ", r.Warnings) : "OK";
        }
    }
}

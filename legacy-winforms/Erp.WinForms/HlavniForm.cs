using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Erp.Payroll;

namespace Erp.WinForms
{
    /// <summary>Okno modelového výpočtu mzdy. V produkci formulář z komerční knihovny ovládacích prvků s návrhářem.</summary>
    public class HlavniForm : Form
    {
        private readonly NumericUpDown _hruba = new NumericUpDown { Maximum = 9999999, DecimalPlaces = 2, Value = 40000, Left = 140, Top = 12, Width = 120 };
        private readonly ComboBox _typ = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = 140, Top = 42, Width = 120 };
        private readonly CheckBox _prohlaseni = new CheckBox { Text = "Podepsané prohlášení", Checked = true, Left = 140, Top = 72, Width = 200 };
        private readonly NumericUpDown _deti = new NumericUpDown { Maximum = 20, Left = 140, Top = 102, Width = 60 };
        private readonly Button _spocitej = new Button { Text = "Spočítej", Left = 140, Top = 135, Width = 120 };
        private readonly TextBox _vysledek = new TextBox { Multiline = true, ReadOnly = true, Left = 12, Top = 170, Width = 360, Height = 240, Font = new Font(FontFamily.GenericMonospace, 9f) };

        public HlavniForm()
        {
            Text = "Výpočet mzdy 2026";
            ClientSize = new Size(390, 425);
            _typ.Items.AddRange(new object[] { "HPP", "DPP", "DPC" });
            _typ.SelectedIndex = 0;
            Controls.Add(new Label { Text = "Hrubá mzda:", Left = 12, Top = 15, Width = 120 });
            Controls.Add(new Label { Text = "Pracovní vztah:", Left = 12, Top = 45, Width = 120 });
            Controls.Add(new Label { Text = "Počet dětí:", Left = 12, Top = 105, Width = 120 });
            Controls.AddRange(new Control[] { _hruba, _typ, _prohlaseni, _deti, _spocitej, _vysledek });
            _spocitej.Click += Spocitej_Click;
        }

        private void Spocitej_Click(object sender, EventArgs e)
        {
            var zam = new Zamestnanec
            {
                OsobniCislo = "MODEL",
                Prijmeni = "Modelový výpočet",
                TypUvazku = (TypUvazku)_typ.SelectedIndex,
                Prohlaseni = _prohlaseni.Checked,
                PocetDeti = (int)_deti.Value
            };
            int obdobi = DateTime.Today.Year * 100 + DateTime.Today.Month;
            var r = new MzdaKalkulace().Vypocitej(zam, _hruba.Value, obdobi);
            _vysledek.Text = r.Success
                ? VyplatniPaska.Formatuj(zam, r.Value).Replace("\n", Environment.NewLine)
                : r.ErrorCode + ": " + r.Message;
        }
    }
}

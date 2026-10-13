using System;

namespace Erp.Framework.Ui
{
    // Náhrady ovládacích prvků (v produkci DevExpress TextEdit, SpinEdit, CheckEdit, SimpleButton).
    // Výukový výřez nesmí záviset na WinForms ani DevExpress, proto jen holá data.
    // Generovaný kód (*.Designer.cs) je používá stejně, jako by šlo o skutečné prvky.

    public class ErpControl
    {
        public string Name { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int TabIndex { get; set; }
        public bool Enabled { get; set; } = true;
        public bool Visible { get; set; } = true;
    }

    public class ErpLabel : ErpControl
    {
        public string Text { get; set; }
    }

    public class ErpTextEdit : ErpControl
    {
        public string Text { get; set; }
        public bool ReadOnly { get; set; }
        public string EditMask { get; set; }
    }

    public class ErpSpinEdit : ErpControl
    {
        public decimal Value { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
    }

    public class ErpCheckEdit : ErpControl
    {
        public string Text { get; set; }
        public bool Checked { get; set; }
    }

    public class ErpComboBox : ErpControl
    {
        public string[] Items { get; set; }
        public int SelectedIndex { get; set; }
    }

    public class ErpButton : ErpControl
    {
        public string Text { get; set; }
        public event EventHandler Click;

        public void PerformClick()
        {
            var handler = Click;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }

    /// <summary>Základ formuláře. V produkci DevExpress XtraForm.</summary>
    public class ErpForm
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}

using System.Collections.Generic;

namespace Erp.Data
{
    /// <summary>
    /// Přístup k databázi. Jediné místo, kudy jde SQL. Parametry se předávají VŽDY přes slovník
    /// (název bez @), nikdy skládáním do řetězce. Řádek výsledku = slovník sloupec → hodnota.
    /// V testech se používá in-memory fake, skutečná DB není potřeba.
    /// </summary>
    public interface IDbGateway
    {
        IList<IDictionary<string, object>> Query(string sql, IDictionary<string, object> parameters);

        object Scalar(string sql, IDictionary<string, object> parameters);

        int Execute(string sql, IDictionary<string, object> parameters);

        /// <summary>Spustí uloženou proceduru. Vrací řádky prvního result setu (může být prázdný).</summary>
        IList<IDictionary<string, object>> ExecuteProcedure(string procedureName, IDictionary<string, object> parameters);
    }
}

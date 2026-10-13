using Erp.Data;

namespace Erp.Tests.Infrastruktura;

/// <summary>
/// Fake databáze pro testy. Zaznamenává volání a vrací předem nastavené řádky.
/// Odpověď se vybírá podle podřetězce SQL nebo názvu procedury.
/// </summary>
public class InMemoryDbGateway : IDbGateway
{
    public record Volani(string Sql, bool Procedura, IDictionary<string, object> Parametry);

    private readonly List<(string Klic, List<IDictionary<string, object>> Radky)> _odpovedi = new();

    private readonly List<Volani> _historie = new();

    /// <summary>Všechna volání v pořadí, jak přišla.</summary>
    public IReadOnlyList<Volani> Historie => _historie;

    /// <summary>Nastaví řádky, které se vrátí pro SQL (nebo proceduru) obsahující daný podřetězec.</summary>
    public InMemoryDbGateway Vrat(string sqlObsahuje, params IDictionary<string, object>[] radky)
    {
        _odpovedi.Add((sqlObsahuje, radky.ToList()));
        return this;
    }

    public IList<IDictionary<string, object>> Query(string sql, IDictionary<string, object> parameters)
    {
        Zaznamenej(sql, false, parameters);
        return Najdi(sql);
    }

    public object Scalar(string sql, IDictionary<string, object> parameters)
    {
        Zaznamenej(sql, false, parameters);
        var r = Najdi(sql);
        return r.Count == 0 ? null! : r[0].Values.First();
    }

    public int Execute(string sql, IDictionary<string, object> parameters)
    {
        Zaznamenej(sql, false, parameters);
        return 1;
    }

    public IList<IDictionary<string, object>> ExecuteProcedure(string procedureName, IDictionary<string, object> parameters)
    {
        Zaznamenej(procedureName, true, parameters);
        return Najdi(procedureName);
    }

    private void Zaznamenej(string sql, bool proc, IDictionary<string, object>? p) =>
        _historie.Add(new Volani(sql, proc, p == null ? new Dictionary<string, object>() : new Dictionary<string, object>(p)));

    private IList<IDictionary<string, object>> Najdi(string sql)
    {
        foreach (var (klic, radky) in _odpovedi)
            if (sql.Contains(klic, StringComparison.OrdinalIgnoreCase))
                return radky;
        return new List<IDictionary<string, object>>();
    }
}

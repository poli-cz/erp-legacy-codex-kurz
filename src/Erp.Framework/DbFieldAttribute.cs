using System;

namespace Erp.Framework
{
    /// <summary>
    /// Mapování vlastnosti business objektu na sloupec tabulky.
    /// Konvence: název sloupce velkými písmeny s prefixem tabulky (ZAM_, MZD_, DOK_, RAD_).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class DbFieldAttribute : Attribute
    {
        public DbFieldAttribute(string columnName)
        {
            ColumnName = columnName;
        }

        public string ColumnName { get; private set; }

        /// <summary>Primární klíč. Při INSERTu se nezapisuje (IDENTITY).</summary>
        public bool IsKey { get; set; }

        /// <summary>Hodnotu počítá databáze (procedura, trigger). C# ji jen čte.</summary>
        public bool ReadOnly { get; set; }
    }

    /// <summary>Tabulka, do které se business objekt ukládá.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DbTableAttribute : Attribute
    {
        public DbTableAttribute(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; private set; }
    }
}

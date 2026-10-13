using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Erp.Framework
{
    /// <summary>
    /// Základ všech business objektů (BO). BO nese data jednoho řádku tabulky,
    /// mapování na sloupce je přes [DbField]. Ukládání řeší repozitáře v Erp.Data,
    /// BO samo do databáze nesahá.
    /// </summary>
    public abstract class BusinessObject
    {
        private static readonly Dictionary<Type, List<KeyValuePair<PropertyInfo, DbFieldAttribute>>> _mapCache =
            new Dictionary<Type, List<KeyValuePair<PropertyInfo, DbFieldAttribute>>>();

        private bool _isDirty;

        /// <summary>Primární klíč. Potomek ho přepíše s vlastním [DbField("XXX_ID", IsKey = true)].</summary>
        public virtual int Id { get; set; }

        public bool IsNew
        {
            get { return Id <= 0; }
        }

        public bool IsDirty
        {
            get { return _isDirty || IsNew; }
        }

        protected void MarkDirty()
        {
            _isDirty = true;
        }

        public void MarkClean()
        {
            _isDirty = false;
        }

        public string TableName
        {
            get
            {
                var attr = (DbTableAttribute)Attribute.GetCustomAttribute(GetType(), typeof(DbTableAttribute));
                if (attr == null)
                    throw new InvalidOperationException("BO " + GetType().Name + " nemá atribut [DbTable].");
                return attr.TableName;
            }
        }

        /// <summary>Hodnoty pro INSERT/UPDATE: sloupec → hodnota. Bez klíče a bez ReadOnly sloupců.</summary>
        public IDictionary<string, object> ToDbValues()
        {
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in GetMap(GetType()))
            {
                if (pair.Value.IsKey || pair.Value.ReadOnly)
                    continue;
                object value = pair.Key.GetValue(this, null);
                if (value is bool)
                    value = ((bool)value) ? 1 : 0; // v DB je BIT, staré procedury čekají 0/1
                if (value is Enum)
                    value = value.ToString();
                result[pair.Value.ColumnName] = value ?? DBNull.Value;
            }
            return result;
        }

        /// <summary>Naplní BO z řádku (sloupec → hodnota). Neznámé sloupce ignoruje.</summary>
        public void LoadFromRow(IDictionary<string, object> row)
        {
            foreach (var pair in GetMap(GetType()))
            {
                object raw;
                if (!row.TryGetValue(pair.Value.ColumnName, out raw))
                    continue;
                if (raw == null || raw is DBNull)
                {
                    pair.Key.SetValue(this, null, null);
                    continue;
                }
                Type target = Nullable.GetUnderlyingType(pair.Key.PropertyType) ?? pair.Key.PropertyType;
                object converted;
                if (target.IsEnum)
                    converted = Enum.Parse(target, Convert.ToString(raw, CultureInfo.InvariantCulture), true);
                else if (target == typeof(bool))
                    converted = Convert.ToInt32(raw, CultureInfo.InvariantCulture) != 0;
                else
                    converted = Convert.ChangeType(raw, target, CultureInfo.InvariantCulture);
                pair.Key.SetValue(this, converted, null);
            }
            _isDirty = false;
        }

        public virtual ErpResult Validate()
        {
            return ErpResult.Ok();
        }

        internal static List<KeyValuePair<PropertyInfo, DbFieldAttribute>> GetMap(Type type)
        {
            lock (_mapCache)
            {
                List<KeyValuePair<PropertyInfo, DbFieldAttribute>> map;
                if (_mapCache.TryGetValue(type, out map))
                    return map;
                map = new List<KeyValuePair<PropertyInfo, DbFieldAttribute>>();
                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var attr = (DbFieldAttribute)Attribute.GetCustomAttribute(prop, typeof(DbFieldAttribute), true);
                    if (attr != null)
                        map.Add(new KeyValuePair<PropertyInfo, DbFieldAttribute>(prop, attr));
                }
                _mapCache[type] = map;
                return map;
            }
        }
    }
}

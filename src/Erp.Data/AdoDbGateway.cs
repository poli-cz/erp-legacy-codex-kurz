using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Erp.Framework;

namespace Erp.Data
{
    /// <summary>
    /// ADO.NET implementace IDbGateway. Připojení dodává továrna (v produkci SqlConnection
    /// z Microsoft.Data.SqlClient, connection string z ErpConfig "Db.ConnectionString").
    /// Tento výukový výřez na SQL Server nezávisí, proto jen DbConnection.
    /// </summary>
    public class AdoDbGateway : IDbGateway
    {
        private readonly Func<DbConnection> _connectionFactory;

        public int CommandTimeoutSeconds = 120; // uzávěrka mezd trvá dlouho

        public AdoDbGateway(Func<DbConnection> connectionFactory)
        {
            if (connectionFactory == null)
                throw new ArgumentNullException("connectionFactory");
            _connectionFactory = connectionFactory;
        }

        public IList<IDictionary<string, object>> Query(string sql, IDictionary<string, object> parameters)
        {
            return Run(sql, CommandType.Text, parameters, true);
        }

        public object Scalar(string sql, IDictionary<string, object> parameters)
        {
            using (var conn = _connectionFactory())
            using (var cmd = Prepare(conn, sql, CommandType.Text, parameters))
            {
                conn.Open();
                object v = cmd.ExecuteScalar();
                return v is DBNull ? null : v;
            }
        }

        public int Execute(string sql, IDictionary<string, object> parameters)
        {
            using (var conn = _connectionFactory())
            using (var cmd = Prepare(conn, sql, CommandType.Text, parameters))
            {
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public IList<IDictionary<string, object>> ExecuteProcedure(string procedureName, IDictionary<string, object> parameters)
        {
            return Run(procedureName, CommandType.StoredProcedure, parameters, true);
        }

        private IList<IDictionary<string, object>> Run(string text, CommandType type, IDictionary<string, object> parameters, bool read)
        {
            var rows = new List<IDictionary<string, object>>();
            var start = DateTime.Now;
            using (var conn = _connectionFactory())
            using (var cmd = Prepare(conn, text, type, parameters))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (read && reader.Read())
                    {
                        var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                        for (int i = 0; i < reader.FieldCount; i++)
                            row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        rows.Add(row);
                    }
                }
            }
            var ms = (DateTime.Now - start).TotalMilliseconds;
            if (ms > 2000)
                Trace.Log(TraceLevel.Warning, "Db", "Pomalý dotaz (" + (int)ms + " ms): " + text);
            return rows;
        }

        private DbCommand Prepare(DbConnection conn, string text, CommandType type, IDictionary<string, object> parameters)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = text;
            cmd.CommandType = type;
            cmd.CommandTimeout = CommandTimeoutSeconds;
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var par = cmd.CreateParameter();
                    par.ParameterName = "@" + p.Key;
                    par.Value = p.Value ?? DBNull.Value;
                    cmd.Parameters.Add(par);
                }
            }
            return cmd;
        }
    }
}

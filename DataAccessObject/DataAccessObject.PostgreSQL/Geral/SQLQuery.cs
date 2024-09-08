using DataAccessObject.Query;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataAccessObject.PostgreSQL.Geral
{
    public class SQLQuery : ISQLQuery
    {

        NpgsqlConnection connection;

        public SQLQuery(string ConnectionString)
        {
            connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
        }

        public void Dispose()
        {
            connection?.Close();
            connection?.Dispose();
        }

        public DataTable Query(string sql, object[] parameters)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
            {
                using (NpgsqlDataReader dr = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();

                    // Adiciona as colunas ao DataTable
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        dt.Columns.Add(dr.GetName(i), dr.GetFieldType(i));
                    }

                    // Se houver um registro, adiciona a primeira linha ao DataTable
                    if (dr.Read())
                    {
                        DataRow row = dt.NewRow();
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            row[i] = dr[i];
                        }
                        dt.Rows.Add(row);
                    }

                    return dt;
                }
            }
        }
    }
}

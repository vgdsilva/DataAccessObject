using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace DataAccessObject.PostgreSQL.Geral
{
    public class SQLQuery 
    {

        NpgsqlConnection connection;

        /// <summary>
        /// Instrução SQL
        /// </summary>
        public string SQL { get; set; }

        /// <summary>
        /// Resultado da consulta SQL quando o SQLQuery for uma instrução SELECT
        /// </summary>
        public DataTable dtDados { get; set; }

        public int RecordCount { get { return IsEmpty ? 0 : dtDados.Rows.Count; } }
        public bool IsEmpty { get { return !(dtDados != null && dtDados.Rows.Count > 0); } }

        public Dictionary<string, object> ParamByName { get; set; }
        private int RecordIndex { get; set; }
        public long TempoDeExecucao { get; private set; }

        public SQLQuery(string ConnectionString)
        {
            connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
        }

        public void Dispose()
        {
            if (this == null)
                return;

            if (dtDados != null)
                dtDados.Dispose();

            if (ParamByName != null)
            {
                ParamByName.Clear();
                ParamByName = null;
            }

            SQL = null;

            connection?.Close();
            connection?.Dispose();
        }

        /// <summary>
        /// Executa instrução INSERT, UPDATE ou DELETE
        /// </summary>
        /// <returns>Quantidade de registros afetados</returns>
        public int ExecutaSQL()
        {
            int viRetorno = 0;

            try
            {

                using (NpgsqlCommand objCommand = new NpgsqlCommand(SQL, connection))
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    viRetorno = objCommand.ExecuteNonQuery();
                    watch.Stop();
                }
            }
            catch (Exception)
            {

            }

            return viRetorno;
        }

        /// <summary>
        /// Preenche o DataTable com o conjunto de dados oriundos da instrução SELECT
        /// </summary>
        public DataTable Open()
        {
            try
            {
                int tentativas = 0;
                int maxTentativas = 10;

                dtDados = new DataTable();

                using (NpgsqlCommand objCommand = new NpgsqlCommand(SQL, connection))
                {
                    using (NpgsqlDataAdapter objDataAdapter = new NpgsqlDataAdapter(objCommand))
                    {
                        objDataAdapter.Fill(dtDados);
                    }
                }

                return dtDados;
            }
            catch (Exception)
            {

                throw;
            }
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

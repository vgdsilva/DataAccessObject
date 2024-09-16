using DataAccessObject.PostgreSQL.Geral;
using DataAccessObject.Query;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataAccessObject.PostgreSQL
{
    public class Database : AbstractDatabase
    {
        public string ConnectionString { get; set; } = string.Empty;

        public Database(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }



        public DataRowCollection GetItems(string sql, object[] parans = null)
        {
            using (ISQLQuery oSQL = new SQLQuery(ConnectionString))
            {
                return oSQL.Query(sql, parans).Rows;
            }
        }

        public DataRow GetItem(string SQL, object[] objects = null)
        {
            using (ISQLQuery oSQL = new SQLQuery(ConnectionString))
            {
                return oSQL.Query(SQL, objects).Rows[0];
            }
        }

        public override DataRow GetItem<T>(object id) where T : class
        {
            Type EntityType = typeof(T);


            using (SQLQuery oSQL = new SQLQuery(ConnectionString))
            {




            }
        }

        public override T GetItems<T>()
        {
            throw new NotImplementedException();
        }
    }
}

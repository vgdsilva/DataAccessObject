using DataAccessObject.Builders;
using DataAccessObject.PostgreSQL.Geral;
using System;
using System.Data;

namespace DataAccessObject.PostgreSQL
{
    public class Database 
    {
        public string ConnectionString { get; set; } = string.Empty;

        public Database(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }

        public DataRow GetItem
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessObject.SQLite
{
    public class Database : IDatabase
    {
        public string ConnectionString { get; set; } = string.Empty;

        public string SQL { get; set; } = string.Empty;


        public void AddParameter(string parameterName, object parameterValue)
        {
            
        }

        public void Dispose()
        {
            
        }

        public void Execute()
        {
            
        }

        public IEnumerable<T> Query<T>()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessObject
{
    public interface IDatabase : IDisposable
    {
        string ConnectionString { get; set; }

        string SQL { get; set; }

        void Execute();

        IEnumerable<T> Query<T>();

        void AddParameter(string parameterName, object parameterValue);


    }
}

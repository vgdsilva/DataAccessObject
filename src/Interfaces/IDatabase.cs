using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.SQLite.Interfaces;
public interface IDatabase : IDisposable
{
    string SQL { get; set; }
    void Execute();
    void AddParameter(string name, object value);
    Task<List<T>> QueryAsync<T>();
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataAccessObject.Query
{
    public interface ISQLQuery : IDisposable
    {
        DataTable Query(string sql, object[] parameters);
    }
}

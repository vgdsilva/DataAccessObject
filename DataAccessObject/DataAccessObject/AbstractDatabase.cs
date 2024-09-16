using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataAccessObject
{
    public abstract class AbstractDatabase
    {

        public abstract DataRow GetItem<T>(object id);

        public abstract T GetItems<T>();
    }
}

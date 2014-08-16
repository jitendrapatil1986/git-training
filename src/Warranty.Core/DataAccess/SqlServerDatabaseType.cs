using System;
using System.Data;
using NPoco.DatabaseTypes;

namespace Warranty.Core.DataAccess
{
    public class SqlServerDatabaseType : SqlServer2012DatabaseType
    {
        public SqlServerDatabaseType()
        {
            AddTypeMap(typeof(DateTime), DbType.DateTime2);
            AddTypeMap(typeof(DateTime?), DbType.DateTime2);
            AddTypeMap(typeof(string), DbType.AnsiString);
        }
    }
}
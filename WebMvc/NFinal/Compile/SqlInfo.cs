using System;
using System.Collections.Generic;
using System.Web;

namespace NFinal.Compile
{
    public class SqlInfo
    {
        public List<Column> Columns = new List<Column>();
        public string ColumnsSql = "";
        public List<Table> Tables = new List<Table>();
        public string TablesSql = "";
        public List<NFinal.Compile.SqlVarParameter> sqlVarParameters = new List<SqlVarParameter>();

        public string sql;
        public char[] sqls;
        public NFinal.DB.DBType dbType = DB.DBType.Unknown;

        public SqlInfo(string sql, NFinal.DB.DBType dbType)
        {
            this.sql = sql;
            sqls = sql.ToCharArray();
            this.dbType = dbType;
        }
    }
}
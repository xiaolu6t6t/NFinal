using System;
using System.Collections.Generic;
using System.Web;

namespace NFinal.Compile
{
    public class SelectStatementInfo
    {
        public string sql;
        public string selectClause;
        public string fromClause;
        public string whereClause;
        public string otherClause;
        public string sqlWithOutSubSelect;
        public List<SelectStatementInfo> selects;
        public SelectStatementInfo(string sql)
        {
            this.sql = sql;
            this.sqlWithOutSubSelect = sql;
        }
    }
}
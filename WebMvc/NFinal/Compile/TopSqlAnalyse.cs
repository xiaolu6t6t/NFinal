using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace NFinal.Compile
{
    public class TopSqlAnalyse
    {
        public string topSql;
        public string sql;
        public NFinal.DB.DBType dbType;

        public TopSqlAnalyse(string sql, NFinal.DB.DBType dbType)
        {
            this.sql = sql;
            this.dbType = dbType;
        }

        public void Parse()
        {
            //如果数据库是sqlserver
            if (dbType == DB.DBType.SqlServer)
            {
                string selectFromParttern = @"(select\s+)";
                Regex selectFromReg = new Regex(selectFromParttern, RegexOptions.IgnoreCase);
                Match mat = selectFromReg.Match(sql);
                if (mat.Success)
                {
                    topSql = sql.Insert(mat.Index + mat.Length, " top {0} ");
                }
            }
            //如果数据库是mysql
            else if (dbType == DB.DBType.MySql)
            {
                topSql = sql + " limit {0}";
            }
            //如果数据库是sqlite
            else if (dbType == DB.DBType.Sqlite)
            {
                topSql = sql + " Limit {0}";
            }
        }
    }
}
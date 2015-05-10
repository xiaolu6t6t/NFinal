using System;///
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NFinal.Compile
{
    //解析出sql语句中所有的表名与列名
    public class SqlParser
    {
        public NFinal.DB.DBType dbType;
        public SqlParser(NFinal.DB.DBType dbType)
        {
            this.dbType = dbType;
        }

        public List<SqlInfo> Parse(string sql)
        {
            string temp=sql.Trim().ToLower();
           
            if( temp.StartsWith("select"))
            {
                SelectStatement select = new SelectStatement(sql, dbType);
                return select.Parse();
            }
            else if (temp.StartsWith("insert"))
            {
                InsertStatement insert = new InsertStatement(sql, dbType);
                insert.Parse();
                List<SqlInfo> sqlInfos = new List<SqlInfo>();
                sqlInfos.Add(insert.sqlInfo);
                return sqlInfos;
            }
            else if (temp.StartsWith("update"))
            {
                UpdateStatement update = new UpdateStatement(sql, dbType);
                update.Parse();
                List<SqlInfo> sqlInfos = new List<SqlInfo>();
                sqlInfos.Add(update.sqlInfo);
                return sqlInfos;
            }
            else if (temp.StartsWith("delete"))
            {
                DeleteStatement delete = new DeleteStatement(sql, dbType);
                delete.Parse();
                List<SqlInfo> sqlInfos = new List<SqlInfo>();
                sqlInfos.Add(delete.sqlInfo);
                return sqlInfos;
            }
            else
            {
                return new List<SqlInfo>();
            }
        }
    }
}
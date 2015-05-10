using System;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

namespace NFinal.Compile
{
    //选取随机行
    public class RandomSqlAnalyse
    {
        public string sql;
        public string randomSql;
        public NFinal.DB.DBType dbType;
        public RandomSqlAnalyse(string sql,NFinal.DB.DBType dbType)
        {
            this.sql = sql;
            this.dbType = dbType;
        }
        public void Parse()
        {
            string pattern = string.Empty;
            if (dbType == DB.DBType.MySql)
            {
                randomSql = sql + " order by rand() limit {0}";

            }
            else if (dbType == DB.DBType.SqlServer)
            {
                pattern = @"\s*(select)\s+";
                Regex selectReg=new Regex(pattern);
                Match selectMat=selectReg.Match(sql);
                if(selectMat.Success)
                {
                    randomSql = sql.Insert(selectMat.Index+selectMat.Length," top {0} ");
                    randomSql += " order by random()";
                }
            }
            else if(dbType == DB.DBType.Sqlite)
            {
                randomSql = sql + " order by random() limit {0}";
            }
        }
    }
}
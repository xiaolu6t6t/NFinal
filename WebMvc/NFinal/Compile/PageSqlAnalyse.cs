using System;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

namespace NFinal.Compile
{
    public class PageSqlAnalyse
    {
        public string pageSql;
        public string countSql;
        public string sql;
        public NFinal.DB.DBType dbType;
        public PageSqlAnalyse(string sql, NFinal.DB.DBType dbType)
        {
            this.sql = sql;
            this.dbType = dbType;
        }

        public void Parse()
        {
            string temp=string.Empty;
            string selectFromParttern = @"select\s+([\s\S]+)\s+from\s+";
            Regex selectFromReg = new Regex(selectFromParttern, RegexOptions.IgnoreCase);
            Match mat = selectFromReg.Match(sql);
            if (mat.Success)
            {
                countSql = sql.Remove(mat.Groups[1].Index, mat.Groups[1].Length);
                countSql = countSql.Insert(mat.Groups[1].Index,"count(*)");
            }
            //如果数据库是sqlserver
            if (dbType == DB.DBType.SqlServer)
            {
                string selectParttern = @"\s*(select)\s+";
                Regex selectReg = new Regex(selectParttern, RegexOptions.IgnoreCase);
                Match selectMat = selectReg.Match(sql);

                string whereParttern = @"\s+(where|from)\s+";
                Regex whereReg = new Regex(whereParttern,RegexOptions.IgnoreCase|RegexOptions.RightToLeft);
                Match whereMat = whereReg.Match(sql);
                //如果是select语句必定有select和from语句
                if (selectMat.Success && whereMat.Success)
                {
                    //如果是select from where 的形式
                    if (whereMat.Groups[1].Value.ToLower() == "where")
                    {
                        pageSql = sql.Insert(selectMat.Index + selectMat.Length,
                            " top {0} ");
                        temp = "select top {1} id " + sql.Substring(whereMat.Groups[1].Index);
                        pageSql +=string.Format(" and id not in({0})",temp);
                    }
                    //如果是select from 的形式
                    else
                    {
                        pageSql = sql.Insert(selectMat.Index + selectMat.Length,
                            " top {0} ");
                        temp = "select top {1} id " + sql.Substring(whereMat.Groups[1].Index);
                        pageSql += string.Format(" where id not in({0})", temp);
                    }
                }
            }
            //如果数据库是mysql
            else if (dbType == DB.DBType.MySql)
            {
                pageSql =sql+ " limit {0},{1}";
            }
            //如果数据库是sqlite
            else if (dbType == DB.DBType.Sqlite)
            {
                pageSql =sql+ " Limit {0} Offset {1}";
            }
        }
        public string GetSql(string pageSql,string countSql,int pageIndex,int pageSize,int totalSize,NFinal.DB.DBType dbType)
        {
            int pageCount = (totalSize % pageSize==0)?totalSize/pageSize:totalSize/pageSize+1;
            if (pageIndex > pageCount)
            {
                pageIndex = pageCount;
            }
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (dbType == NFinal.DB.DBType.SqlServer)
            {
                pageSql = string.Format(pageSql, pageIndex * pageSize, (pageIndex - 1) * pageSize);
            }
            else if (dbType == NFinal.DB.DBType.MySql)
            {
                pageSql = string.Format(pageSql, (pageIndex - 1) * pageSize, pageSize);
            }
            else if(dbType ==NFinal.DB.DBType.Sqlite)
            {
                pageSql = string.Format(pageSql, pageSize, (pageIndex - 1) * pageSize);
            }
            return pageSql;
        }
        
    }
}
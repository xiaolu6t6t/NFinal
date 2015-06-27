using System;///
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NFinal.Compile
{
    //解析出sql语句中所有的表名与列名
    public class SqlParser
    {
        public NFinal.DB.Coding.DataUtility dataUtility;
        public SqlParser(NFinal.DB.Coding.DataUtility dataUtility)
        {
            this.dataUtility = dataUtility;
        }

        public List<SqlInfo> Parse(string sql)
        {
            string temp=sql.Trim().ToLower();
            List<SqlInfo> sqlInfos = null;
            if( temp.StartsWith("select"))
            {
                SelectStatement select = new SelectStatement(sql,dataUtility);
                sqlInfos = new List<SqlInfo>();
            }
            else if (temp.StartsWith("insert"))
            {
                InsertStatement insert = new InsertStatement(sql, dataUtility.dbType);
                insert.Parse();
                sqlInfos = new List<SqlInfo>();
                sqlInfos.Add(insert.sqlInfo);
            }
            else if (temp.StartsWith("update"))
            {
                UpdateStatement update = new UpdateStatement(sql, dataUtility.dbType);
                update.Parse();
                sqlInfos = new List<SqlInfo>();
                sqlInfos.Add(update.sqlInfo);
            }
            else if (temp.StartsWith("delete"))
            {
                DeleteStatement delete = new DeleteStatement(sql, dataUtility.dbType);
                delete.Parse();
                sqlInfos = new List<SqlInfo>();
                sqlInfos.Add(delete.sqlInfo);
            }
            else
            {
                sqlInfos=new List<SqlInfo>();
            }
            //如果是Oracle数据库,则要把所有的表名转为大写
            if (dataUtility.dbType == DB.DBType.Oracle)
            {
                if (sqlInfos.Count > 0)
                {
                    for (int i = 0; i < sqlInfos.Count; i++)
                    {
                        if (sqlInfos[i].Tables.Count > 0)
                        {
                            //则要把所有的表名转为小写
                            for (int j = 0; j < sqlInfos[i].Tables.Count; j++)
                            {
                                sqlInfos[i].Tables[j].name=sqlInfos[i].Tables[j].name.ToLower();
                                sqlInfos[i].Tables[j].fullName = sqlInfos[i].Tables[j].fullName.ToLower();
                                sqlInfos[i].Tables[j].asName = sqlInfos[i].Tables[j].asName.ToLower();
                            }
                            //把所有列名转为小写
                            for (int j = 0; j < sqlInfos[i].Columns.Count; j++)
                            {
                                sqlInfos[i].Columns[j].name = sqlInfos[i].Columns[j].name.ToLower();
                                sqlInfos[i].Columns[j].fullName = sqlInfos[i].Columns[j].fullName.ToLower();
                                sqlInfos[i].Columns[j].asName = sqlInfos[i].Columns[j].asName.ToLower();
                            }
                        }
                    }
                }
            }

            return sqlInfos;
        }
    }
}
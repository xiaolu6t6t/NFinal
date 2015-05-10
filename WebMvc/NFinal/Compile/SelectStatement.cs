using System;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

namespace NFinal.Compile
{
    public class SelectStatement:SqlStatement
    {
        public string selectClause;
        public string fromClause;
        public string whereClause;
        public SelectStatement select;


        //语句分三部分,select+columns+fromTables
        //select(?:\s+distinct|\s+top\s+[0-9]+|\s+top\s+[0-9]+\s+percent)?
        //((?:\s+[^,\s]+)(?:\s+as\s+(?:[^,\s]+))?(?:\s*,\s*(?:[^,\s]+)(?:\s+as\s+(?:[^,\s]+))?)*)
        //(?:\s+from((?:\s+[^,\s]+)(?:\s+as\s+(?:[^,\s]+))?(?:\s*,\s*(?:[^,\s]+)(?:\s+as\s+(?:[^,\s]+))?)*))?
        static string selectStatementReg = @"select(?:\s+distinct|\s+top\s+\S+|\s+top\s+\S+\s+percent)?((?:\s+[^,\s]+)(?:\s+as\s+(?:[^,\s]+))?(?:\s*,\s*(?:[^,\s]+)(?:\s+as\s+(?:[^,\s]+))?)*)(?:\s+from((?:\s+[^,\s]+)(?:\s+as\s+(?:[^,\s]+))?(?:\s*,\s*(?:[^,\s]+)(?:\s+as\s+(?:[^,\s]+))?)*))?";
        public SelectStatement(string sql, NFinal.DB.DBType dbType)
            : base(sql, dbType)
        {
        }

        /// <summary>
        /// 解析select语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<SqlInfo> Parse()
        {
            List<SqlInfo> selects = new List<SqlInfo>();
            Regex reg = new Regex(selectStatementReg, RegexOptions.IgnoreCase);
            MatchCollection mc = reg.Matches(sqlInfo.sql);
            if (mc.Count > 0)
            {
                SqlInfo selectStatement = new SqlInfo(sqlInfo.sql, sqlInfo.dbType);
                foreach (Match m in mc)
                {
                    selectStatement.sql = sqlInfo.sql;
                    selectStatement.ColumnsSql = m.Groups[1].Value;
                    selectStatement.Columns = ParseColumn(selectStatement.ColumnsSql);
                    selectStatement.TablesSql = m.Groups[2].Value;
                    selectStatement.Tables = ParseTable(selectStatement.TablesSql);
                    selectStatement.sqlVarParameters = ParseVarName(sqlInfo.sql);
                    selects.Add(selectStatement);
                }
            }
            return selects;
        }       
       
        /// <summary>
        /// 从SQL语句中获取表信息
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>表信息</returns>
        public List<Table> ParseTable(string sql)
        {
            List<Table> tables = new List<Table>();
            string[] tableSqls = sql.Split(',');
            Regex tableStatementReg = new Regex(tableStatement, RegexOptions.IgnoreCase);
            Match tableStatementM = null;

            for (int i = 0; i < tableSqls.Length; i++)
            {
                Table tab = new Table();

                tableStatementM = tableStatementReg.Match(tableSqls[i]);
                if (tableStatementM.Success)
                {
                    //前面变量相关的字符串
                    tab.sql = tableStatementM.Groups[1].Value;
                    //asName相关的字符串
                    tab.asName = tableStatementM.Groups[2].Value;
                    tab = GetTable(tab.sql);
                }
                tables.Add(tab);
            }
            return tables;
        }
    }
}
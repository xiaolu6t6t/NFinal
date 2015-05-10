using System;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

namespace NFinal.Compile
{
    public class InsertStatement:SqlStatement
    {
        //INSERT INTO 表名称 VALUES (值1, 值2,....)
        public string sqlReg = @"insert\s+into\s+([^,\(\)\s]+)(?:\s*\(\s*((?:[^,\s\(\)]+)(?:\s*,\s*[^,\s\(\)]+)*)\s*\))?\s+values\s*\(\s*((?:[^,\s\(\)]+)(?:\s*,\s*[^,\s\(\)]+)*)\s*\)";
        public string varNameReg = @"@([^,\s]+)";
        public InsertStatement(string sql, NFinal.DB.DBType dbType)
            : base(sql, dbType)
        { 
            
        }
        public void Parse()
        {
            Regex reg=new Regex(sqlReg,RegexOptions.IgnoreCase);
            Match mat = reg.Match(sqlInfo.sql);
            Table table=null;
            if (mat.Success)
            {
                table = GetTable(mat.Groups[1].Value);
                this.sqlInfo.Tables.Add(table);
                this.sqlInfo.ColumnsSql = mat.Groups[2].Value;
                this.sqlInfo.Columns= ParseColumn(mat.Groups[2].Value);
                string[] varNames = mat.Groups[3].Value.Split(',');
                if (sqlInfo.Columns.Count == varNames.Length)
                {
                    SqlVarParameter sqlVarParameter = null;
                    Regex varReg = new Regex(varNameReg);
                    Match varMat = null;
                    for (int i = 0; i < sqlInfo.Columns.Count; i++)
                    {
                        varMat= varReg.Match(varNames[i]);
                        if (varMat.Success)
                        {
                            sqlVarParameter = new SqlVarParameter();
                            sqlVarParameter.sql=varNames[i];
                            sqlVarParameter.name= varMat.Groups[1].Value;
                            sqlVarParameter.columnName = sqlInfo.Columns[i].name;
                            sqlInfo.sqlVarParameters.Add(sqlVarParameter);
                        }
                    }
                }
            }
        }
    }
}
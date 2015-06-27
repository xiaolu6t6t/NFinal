using System;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

namespace NFinal.Compile
{
    public class DeleteStatement:SqlStatement
    {
        public string deleteReg = @"delete\s+from\s+(\S+)";
        public DeleteStatement(string sql, NFinal.DB.DBType dbType)
            : base(sql, dbType)
        { 
            
        }
        public void Parse()
        {
            Regex reg = new Regex(deleteReg, RegexOptions.IgnoreCase);
            Match mat = reg.Match(this.sqlInfo.sql);
            if (mat.Success)
            {
                Table tab = GetTable(mat.Groups[1].Value);
                this.sqlInfo.Tables.Add(tab);
                this.sqlInfo.sqlVarParameters = ParseVarName(sqlInfo.sql);
            }
        }
    }
}
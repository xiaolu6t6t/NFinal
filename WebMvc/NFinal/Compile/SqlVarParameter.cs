using System;
using System.Collections.Generic;
using System.Web;

namespace NFinal.Compile
{
    /// <summary>
    /// 从SQL语句中分析的参数信息
    /// </summary>
    public class SqlVarParameter
    {
        public string sql;
        public string name;
        public string tableName;
        public string fullName;
        public string csharpType;
        public string csharpName;
        public string columnName;
        public NFinal.DB.Coding.CsTypeLink link = null;
        public bool hasSqlError;
        public string sqlError;
        public NFinal.DB.Coding.Field field;
    }
}
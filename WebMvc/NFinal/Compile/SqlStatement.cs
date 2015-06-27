using System;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

namespace NFinal.Compile
{
    /// <summary>
    /// sql语句信息
    /// </summary>
    public class SqlStatement
    {
        public SqlInfo sqlInfo;

        public SqlStatement(string sql, NFinal.DB.DBType dbType)
        {
            sqlInfo = new SqlInfo(sql, dbType);
        }
        /// <summary>
        /// 获得@转?的SQL语句,也就是标准的SQL,因为MySQL是非标准的SQL语句,所以必须要转
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string GetSql(string sql)
        {
            sql = sql.Trim('\"');
            return sql;
        }
        public string FormatSql(string sql)
        {
            if (sqlInfo.dbType == DB.DBType.MySql)
            {
                return sql.Replace('@', '?');
            }
            else if (sqlInfo.dbType == DB.DBType.Oracle)
            {
                return sql.Replace('@', ':');
            }
            return sql;
        }
        public string GetIdInSql(string sql)
        {
            string sqlTmp = sql;
            string parttern = @"\s+in\s+@([^@,\s]+)";
            Regex reg = new Regex(parttern);
            MatchCollection mac = reg.Matches(sql);
            if (mac.Count > 0)
            {
                for (int i = 0; i < mac.Count; i++)
                {
                    string name = mac[i].Groups[1].Value;
                    sqlTmp = sqlTmp.Remove(mac[i].Groups[1].Index - 1, mac[i].Groups[1].Length + 1);
                    sqlTmp = sqlTmp.Insert(mac[i].Groups[1].Index - 1, "(\"+String.Join(\",\"," + name + ")+\")");
                }
            }
            return sqlTmp;
        }
        public Table GetTable(string sql)
        {
            Regex fullNameReg = new Regex(tableFullName, RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
            Match fullNameM = null;
            Table tab = null;
            fullNameM = fullNameReg.Match(sql);
            //查找变量
            if (fullNameM.Success)
            {
                tab = new Table();
                tab.sql = sql;
                tab.dataBaseName = fullNameM.Groups[1].Value;
                tab.dboName = fullNameM.Groups[2].Value;
                tab.name = fullNameM.Groups[3].Value;
            }
            return tab;
        }
        public Column GetColumn(string sql)
        {
            Regex fullNameReg = new Regex(columnFullName, RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
            Match fullNameM = null;
            Column col = null;
            fullNameM = fullNameReg.Match(sql);
            if (fullNameM.Success)
            {
                col = new Column();
                col.sql = sql;
                col.dataBaseName = fullNameM.Groups[1].Value;
                col.tableName = fullNameM.Groups[2].Value;
                col.name = fullNameM.Groups[3].Value;
            }
            return col;
        }
        /// <summary>
        /// 从SQL语句中获取列信息
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>列信息</returns>
        public List<Column> ParseColumn(string sql)
        {
            List<Column> columns = new List<Column>();
            string[] columnSqls = sql.Split(',');
            Regex columnStatementReg = new Regex(columnStatement, RegexOptions.IgnoreCase);
            Match columnStatementM = null;
            Regex functionReg = new Regex(function, RegexOptions.IgnoreCase);
            Match functionM = null;

            for (int i = 0; i < columnSqls.Length; i++)
            {
                Column col = new Column();

                columnStatementM = columnStatementReg.Match(columnSqls[i]);
                if (columnStatementM.Success)
                {
                    //前面变量相关的字符串
                    col.sql = columnStatementM.Groups[1].Value;
                    //asName相关的字符串
                    col.asName = columnStatementM.Groups[2].Value;
                    functionM = functionReg.Match(col.sql);
                    //查找function,如果找到函数则查找返回类型
                    if (functionM.Success)
                    {
                        col.functionName = functionM.Groups[1].Value;
                        for (int j = 0; j < AggregateFunctions.Length; j++)
                        {
                            if (AggregateFunctions[j].name == col.functionName.ToUpper())
                            {
                                col.returnType = AggregateFunctions[j].returnType;
                                break;
                            }
                        }
                        //把函数的第一个参数取出来,分析列名
                        col = GetColumn(functionM.Groups[2].Value);
                    }
                    //如果没的找到函数,则直接分析列名
                    else
                    {
                        col = GetColumn(col.sql);
                    }
                }
                columns.Add(col);
            }
            return columns;
        }
        /// <summary>
        /// 输出SQL分析出的列信息
        /// </summary>
        /// <param name="col">列信息</param>
        public void WriteColumn(Column col)
        {
            Console.WriteLine("column:");
            Console.WriteLine("db:" + col.dataBaseName);
            Console.WriteLine("tab:" + col.tableName);
            Console.WriteLine("name:" + col.name);
            Console.WriteLine("as:" + col.asName);
            Console.WriteLine("fun nction:" + col.functionName);
        }
        /// <summary>
        /// 输出从SQL分析出的表信息
        /// </summary>
        /// <param name="tab">表信息</param>
        public void WriteTable(Table tab)
        {
            Console.WriteLine("table:");
            Console.WriteLine("db:" + tab.dataBaseName);
            Console.WriteLine("dbo:" + tab.dboName);
            Console.WriteLine("name:" + tab.name);
            Console.WriteLine("as:" + tab.asName);
        }

        /// <summary>
        /// SQL中的聚合函数
        /// </summary>
        public static Function[] AggregateFunctions = new Function[] {
            new Function("AVG","int",1,1),
            new Function("COUNT","int",1,1),
            new Function("FIRST",null,1,1),
            new Function("LAST",null,1,1),
            new Function("MAX","int",1,1),
            new Function("MIN","int",1,1),
            new Function("SUM","int",1,1)
            };
        /// <summary>
        /// SQL中的函数类
        /// </summary>
        public class Function
        {
            public Function(string name, string returnType, int parametersCount, int parameterPosition)
            {
                this.name = name;
                this.returnType = returnType;
                this.parametersCount = parametersCount;
                this.parameterPosition = parameterPosition;
            }
            public string name = "AVG";
            public string returnType = "int";
            public int parametersCount = 1;
            public int parameterPosition = 1;
        }
        /// <summary>
        /// SQL中的其它函数
        /// </summary>
        public static string[] ScalarFuncitons = { "UCASE", "LCASE", "MID", "LEN", "ROUND", "NOW", "FORMAT" };
        public static string varName = @"[^@]@([_a-zA-z][_a-zA-Z0-9])*";
        public static string columnStatement = @"([^,\s]+)(?:\s+as\s+([^,\s]+))?";
        public static string tableStatement = @"([^,\s]+)(?:\s+as\s+([^,\s]+))?";
        public static string columnFullName = @"(?:([_a-zA-Z][_a-zA-Z0-9]*)\s*.\s*)?(?:([_a-zA-Z][_a-zA-Z0-9]*)\s*.\s*)?([_a-zA-Z*][_a-zA-Z0-9]*)";
        public static string tableFullName = @"(?:([_a-zA-Z][_a-zA-Z0-9]*)\s*.\s*)?(?:([_a-zA-Z][_a-zA-Z0-9]*)\s*.\s*)?([_a-zA-Z][_a-zA-Z0-9]*)";
        public static string function = @"([_a-zA-Z][_a-zA-Z0-9]*)\s*\(\s*([^\(\)\s,]+)((?:\s*,\s*[^\(\)\s,]+)*)\s*\)";
        /// <summary>
        /// 解析SQL语句中的参数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        public List<SqlVarParameter> ParseVarName(string sql)
        {
            List<SqlVarParameter> parameters = new List<SqlVarParameter>();
            SqlVarParameter par = null;
            Regex varNameReg = new Regex(varName);
            string varNameString = string.Empty;
            Match mat = null;

            // (top|limit) @num
            string topOrLimitField = @"\s+(?:top|limit)\s+@([_a-zA-Z][_a-zA-Z0-9]*)";
            Regex topOrLimitFieldReg = new Regex(topOrLimitField, RegexOptions.IgnoreCase);
            MatchCollection mac = topOrLimitFieldReg.Matches(sql);
            if (mac.Count > 0)
            {
                for (int i = 0; i < mac.Count; i++)
                {
                    mat = mac[i];
                    if (mat.Success)
                    {
                        par = new SqlVarParameter();
                        par.name = mat.Groups[1].Value;
                        par.csharpName = mat.Groups[1].Value;
                        par.csharpType = "int";
                    }
                }
            }
            //参数 与 列名 比较
            string parameterCompareField =
                @"((?:([_a-zA-Z0-9]+)\s*\.\s*)?([_a-zA-Z0-9]+))\s*(?:>|<|=|>\s*=|<\s*=|<\s*>)\s*@([_a-zA-z][_a-zA-Z0-9]*)";
            Regex parameterCompareFieldReg = new Regex(parameterCompareField);
            mac = parameterCompareFieldReg.Matches(sql);

            if (mac.Count > 0)
            {
                for (int i = 0; i < mac.Count; i++)
                {
                    mat = mac[i];
                    if (mat.Success)
                    {
                        par = new SqlVarParameter();
                        par.sql = mat.Value;
                        par.fullName = mat.Groups[1].Value;
                        par.tableName = mat.Groups[2].Value;
                        par.columnName = mat.Groups[3].Value;
                        par.name = mat.Groups[4].Value;
                        parameters.Add(par);
                    }
                }
            }
            //列名 与 参数 比较
            string fieldCompareParameter =
                @"@([_a-zA-z][_a-zA-Z0-9]*)\s*(?:>|<|=|>\s*=|<\s*=|<\s*>)\s*((?:([_a-zA-Z0-9]+)\s*\.\s*)?([_a-zA-Z0-9]+))";
            Regex fieldCompareParameterReg = new Regex(fieldCompareParameter);
            mac = fieldCompareParameterReg.Matches(sql);
            if (mac.Count > 0)
            {
                for (int i = 0; i < mac.Count; i++)
                {
                    mat = mac[i];
                    if (mat.Success)
                    {
                        par = new SqlVarParameter();
                        par.sql = mat.Value;
                        par.name = mat.Groups[1].Value;
                        par.fullName = mat.Groups[2].Value;
                        par.tableName = mat.Groups[3].Value;
                        par.columnName = mat.Groups[4].Value;
                        parameters.Add(par);
                    }
                }
            }
            Match varNameMat;
            //列名 like 参数1
            string fieldLikeParameter =
                @"((?:([_a-zA-Z0-9]+)\s*\.\s*)?([_a-zA-Z0-9]+))\s+like\s+@([_a-zA-z][_a-zA-Z0-9]*)";
            Regex fieldLikeParameterReg = new Regex(fieldLikeParameter, RegexOptions.IgnoreCase);
            mac = fieldLikeParameterReg.Matches(sql);
            if (mac.Count > 0)
            {
                for (int i = 0; i < mac.Count; i++)
                {
                    mat = mac[i];
                    if (mat.Success)
                    {
                        par = new SqlVarParameter();
                        par.sql = mat.Value;
                        par.fullName = mat.Groups[1].Value;
                        par.tableName = mat.Groups[2].Value;
                        par.columnName = mat.Groups[3].Value;
                        par.name = mat.Groups[4].Value;
                        parameters.Add(par);
                    }
                }
            }
            //列名 between 参数1 and 参数2
            string fieldBetweenParameter =
                @"((?:([_a-zA-Z0-9]+)\s*\.\s*)?([_a-zA-Z0-9]+))\s+between\s+(\S+)\s+and\s+(\S+)";
            Regex fieldBetweenParameterReg = new Regex(fieldBetweenParameter, RegexOptions.IgnoreCase);
            mac = fieldBetweenParameterReg.Matches(sql);

            mat = null;
            if (mac.Count > 0)
            {
                for (int i = 0; i < mac.Count; i++)
                {
                    mat = mac[i];
                    if (mat.Success)
                    {
                        varNameString = mat.Groups[4].Value;
                        varNameMat = varNameReg.Match(varNameString);
                        if (varNameMat.Success)
                        {
                            par = new SqlVarParameter();
                            par.fullName = mat.Groups[1].Value;
                            par.tableName = mat.Groups[2].Value;
                            par.columnName = mat.Groups[3].Value;
                            par.sql = mat.Value;
                            par.name = varNameMat.Groups[1].Value;
                            parameters.Add(par);
                        }
                        varNameString = mat.Groups[5].Value;
                        varNameMat = varNameReg.Match(varNameString);
                        if (varNameMat.Success)
                        {
                            par = new SqlVarParameter();
                            par.fullName = mat.Groups[1].Value;
                            par.tableName = mat.Groups[2].Value;
                            par.columnName = mat.Groups[3].Value;
                            par.sql = mat.Value;
                            par.name = varNameMat.Groups[1].Value;
                            parameters.Add(par);
                        }
                    }
                }
            }
            return parameters;
        }
    }
}
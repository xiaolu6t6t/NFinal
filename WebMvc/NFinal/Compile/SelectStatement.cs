using System;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

namespace NFinal.Compile
{

    public class SelectStatement : SqlStatement
    {
        public NFinal.DB.Coding.DataUtility dataUtility;
        //语句分三部分,select+columns+fromTables
        //select(?:\s+distinct|\s+top\s+[0-9]+|\s+top\s+[0-9]+\s+percent)?
        //((?:\s+[^,\s]+)(?:\s+as\s+(?:[^,\s]+))?(?:\s*,\s*(?:[^,\s]+)(?:\s+as\s+(?:[^,\s]+))?)*)
        //(?:\s+from((?:\s+[^,\s]+)(?:\s+as\s+(?:[^,\s]+))?(?:\s*,\s*(?:[^,\s]+)(?:\s+as\s+(?:[^,\s]+))?)*))?
        //static string selectStatementReg = @"select(?:\s+distinct|\s+top\s+\S+|\s+top\s+\S+\s+percent)?((?:\s+[^,\s]+)(?:\s+as\s+(?:[^,\s]+))?(?:\s*,\s*(?:[^,\s]+)(?:\s+as\s+(?:[^,\s]+))?)*)(?:\s+from((?:\s+[^,\s]+)(?:\s+as\s+(?:[^,\s]+))?(?:\s*,\s*(?:[^,\s]+)(?:\s+as\s+(?:[^,\s]+))?)*))?";
        public SelectStatement(string sql, NFinal.DB.Coding.DataUtility dataUtility)
            : base(sql, dataUtility.dbType)
        {
            this.dataUtility = dataUtility;
        }
        /// <summary>
        /// 解析出SQL中的select,from,where子语句
        /// </summary>
        public void ParseSQL(ref SelectStatementInfo selectStatement)
        {
            //string sql="select count(*),id,name from (select * from users) right on a where id in(select id from users)";
            Regex selectReg = new Regex(@"\s*select\s+", RegexOptions.IgnoreCase);
            Match selectMat = selectReg.Match(selectStatement.sql);
            int LeftBracket = 0;
            int RightBracket = 0;
            char[] sqlString = selectStatement.sql.ToCharArray();
            bool[] InBracket = new bool[sqlString.Length];
            for (int i = 0; i < sqlString.Length; i++)
            {
                if (sqlString[i] == '(')
                {
                    LeftBracket++;
                }
                else if (sqlString[i] == ')')
                {
                    RightBracket++;
                }
                if (LeftBracket == RightBracket)
                {
                    InBracket[i] = false;
                }
                else
                {
                    InBracket[i] = true;
                }
            }
            Regex fromReg = new Regex(@"\s+from\s+", RegexOptions.IgnoreCase);
            MatchCollection fromMac = fromReg.Matches(selectStatement.sql);
            Match fromMat = null;
            for (int i = 0; i < fromMac.Count; i++)
            {
                if (!InBracket[fromMac[i].Index])
                {
                    fromMat = fromMac[i];
                    break;
                }
            }
            Regex whereReg = new Regex(@"\s+where\s+", RegexOptions.IgnoreCase);
            MatchCollection whereMac = whereReg.Matches(selectStatement.sql);
            Match whereMat = null;
            for (int i = 0; i < whereMac.Count; i++)
            {
                if (!InBracket[whereMac[i].Index])
                {
                    whereMat = whereMac[i];
                    break;
                }
            }
            Regex otherReg = new Regex(@"\s+(group\s+by|having|order\s+by|with|limit)\s+");
            selectStatement.selectClause = selectStatement.sql.Substring(selectMat.Index + selectMat.Length, fromMat.Index - selectMat.Index - selectMat.Length);
            //如果where不存在
            if (whereMat == null)
            {
                selectStatement.fromClause = selectStatement.sql.Substring(fromMat.Index + fromMat.Length);
                selectStatement.whereClause = null;

                Match otherMat = otherReg.Match(selectStatement.fromClause);
                if (otherMat.Success)
                {
                    selectStatement.fromClause = selectStatement.fromClause.Substring(0, otherMat.Index);
                }
                selectStatement.selects = new List<SelectStatementInfo>();
            }
            //如果where存在
            else
            {
                selectStatement.fromClause = selectStatement.sql.Substring(fromMat.Index + fromMat.Length, whereMat.Index - fromMat.Index - fromMat.Length);
                selectStatement.whereClause = selectStatement.sql.Substring(whereMat.Index + whereMat.Length);
                Match otherMat = otherReg.Match(selectStatement.whereClause);
                if (otherMat.Success)
                {
                    selectStatement.whereClause = selectStatement.whereClause.Substring(0, otherMat.Index);
                }
                selectStatement.selects = new List<SelectStatementInfo>();
                ParseSubSelect(ref selectStatement);
            }
        }
        public void ParseSubSelect(ref SelectStatementInfo MainSelecteStatement)
        {
            string subSelectPattern = @"\(\s*select\s+((?<open>\()|(?<-open>\))|[\s\S])*(?(open)(?!))\)";
            Regex subSelectReg = new Regex(subSelectPattern, RegexOptions.IgnoreCase);
            MatchCollection mac = subSelectReg.Matches(MainSelecteStatement.sql);
            string subSelectSql = null;
            int relative_position = 0;
            MainSelecteStatement.sqlWithOutSubSelect = MainSelecteStatement.sql;
            for (int i = 0; i < mac.Count; i++)
            {
                //去掉查到的子查询
                MainSelecteStatement.sqlWithOutSubSelect = MainSelecteStatement.sqlWithOutSubSelect.Remove(mac[i].Index + relative_position, mac[i].Length);
                relative_position -= mac[i].Length;
                //去掉两边的括号
                subSelectSql = mac[i].Value.Substring(1, mac[i].Value.Length - 2);
                SelectStatementInfo selectStatement = new SelectStatementInfo(subSelectSql);
                ParseSQL(ref selectStatement);
                MainSelecteStatement.selects.Add(selectStatement);
            }
        }

        public List<SqlVarParameter> GetParameters(DB.Coding.DataUtility dataUtility, SelectStatementInfo selectStatement, ref List<SqlVarParameter> allSqlVarParameters)
        {
            List<SqlVarParameter> sqlVarParameters = ParseVarName(selectStatement.sqlWithOutSubSelect);
            if (sqlVarParameters.Count > 0)
            {
                string sqlParametersSelect = "select ";
                for (int i = 0; i < sqlVarParameters.Count; i++)
                {
                    if (i != 0)
                    {
                        sqlParametersSelect += ",";
                    }
                    sqlParametersSelect += sqlVarParameters[i].fullName;
                }
                sqlParametersSelect += " from " + selectStatement.fromClause + " where 1>2 ";


                Regex fromReg = new Regex(@"\s+from\s+", RegexOptions.IgnoreCase);

                System.Data.Common.DbCommand cmd = dataUtility.GetDbCommand(FormatSql(sqlParametersSelect), dataUtility.con);
                System.Data.DataTable dt = new System.Data.DataTable();
                System.Data.Common.DbDataReader reader = cmd.ExecuteReader();
                dt = reader.GetSchemaTable();
                reader.Read();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SqlVarParameter parameter = sqlVarParameters[i];

                    DB.Coding.Field field = new DB.Coding.Field();
                    field.name = dt.Rows[i]["BaseColumnName"].ToString();
                    field.structFieldName = dt.Rows[i]["ColumnName"].ToString();
                    field.allowNull = Convert.ToBoolean(dt.Rows[i]["AllowDBNull"]);
                    field.csTypeLink = dataUtility.csTypeDic[dt.Rows[i]["DataType"].ToString().Split('.')[1]];
                    if (dataUtility.dbType == DB.DBType.Sqlite)
                    {
                        field.sqlType = dt.Rows[i]["DataTypeName"].ToString();
                    }
                    else
                    {
                        field.sqlType = reader.GetDataTypeName(i);
                    }
                    field.isId = Convert.ToBoolean(dt.Rows[i]["IsKey"]);
                    field.length = Convert.ToInt32(dt.Rows[i]["ColumnSize"]);
                    field.localType = dt.Rows[i]["DataType"].ToString();
                    field.isId = Convert.ToBoolean(dt.Rows[i]["IsKey"]);
                    field.position = Convert.ToInt32(dt.Rows[i]["ColumnOrdinal"]);
                    field.dbType = dataUtility.GetDbType(Convert.ToInt32(dt.Rows[i]["ProviderType"]));
                    //field.hasDefault = dt.Rows[i]["DefaultValue"] != null;
                    //field.defautlValue = field.hasDefault ? dt.Rows[i]["DefaultValue"].ToString() : null;
                    if (!string.IsNullOrEmpty(field.sqlType))
                    {
                        if (dataUtility.typeDic.ContainsKey(field.sqlType.ToLower()))
                        {
                            field.simpleType = dataUtility.typeDic[field.sqlType.ToLower()].simpleType;
                        }
                        else
                        {
                            field.simpleType = "String";
                        }
                    }
                    else
                    {
                        field.simpleType = "";
                    }
                    parameter.field = field;
                    allSqlVarParameters.Add(parameter);
                }
                reader.Close();
            }
            //分析子查询里的参数
            for (int i = 0; i < selectStatement.selects.Count; i++)
            {
                GetParameters(dataUtility, selectStatement.selects[i], ref allSqlVarParameters);
            }
            return sqlVarParameters;
        }
        public List<DB.Coding.Field> GetFields(DB.Coding.DataUtility dataUtility, SelectStatementInfo selectStatement)
        {
            List<DB.Coding.Field> fields = new List<DB.Coding.Field>();
            string selectFiledsSql = string.Format("select {0} from {1} where 1>2", selectStatement.selectClause, selectStatement.fromClause);
            System.Data.Common.DbCommand cmd = dataUtility.GetDbCommand(FormatSql(selectFiledsSql), dataUtility.con);
            System.Data.DataTable dt = new System.Data.DataTable();
            System.Data.Common.DbDataReader reader = cmd.ExecuteReader();
            dt = reader.GetSchemaTable();
            reader.Read();
            Regex nameReg = new Regex(@"^[_0-9a-zA-Z]+$");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DB.Coding.Field field = new DB.Coding.Field();
                field.name = dt.Rows[i]["BaseColumnName"].ToString();
                field.structFieldName = dt.Rows[i]["ColumnName"].ToString();
                //查看名称是否合法,不合法则重命名.
                if (!nameReg.IsMatch(field.name))
                {
                    field.name = "_column" + i.ToString();
                }
                if (!nameReg.IsMatch(field.structFieldName))
                {
                    field.structFieldName = "_column" + i.ToString();
                }
                field.allowNull = Convert.ToBoolean(dt.Rows[i]["AllowDBNull"]);
                field.csTypeLink = dataUtility.csTypeDic[dt.Rows[i]["DataType"].ToString().Split('.')[1]];
                if (dataUtility.dbType == DB.DBType.Sqlite)
                {
                    field.sqlType = dt.Rows[i]["DataTypeName"].ToString();
                }
                else
                {
                    field.sqlType = reader.GetDataTypeName(i);
                }
                field.isId = Convert.ToBoolean(dt.Rows[i]["IsKey"]);
                field.length = Convert.ToInt32(dt.Rows[i]["ColumnSize"]);
                field.localType = dt.Rows[i]["DataType"].ToString();
                field.isId = Convert.ToBoolean(dt.Rows[i]["IsKey"]);
                field.position = Convert.ToInt32(dt.Rows[i]["ColumnOrdinal"]);
                field.dbType = dataUtility.GetDbType(Convert.ToInt32(dt.Rows[i]["ProviderType"]));
                //field.hasDefault = dt.Rows[i]["DefaultValue"] != null;
                //field.defautlValue = field.hasDefault ? dt.Rows[i]["DefaultValue"].ToString() : null;
                //如果没有相对应的数据库类型
                if (!string.IsNullOrEmpty(field.sqlType))
                {
                    if (dataUtility.typeDic.ContainsKey(field.sqlType.ToLower()))
                    {
                        field.simpleType = dataUtility.typeDic[field.sqlType.ToLower()].simpleType;
                    }
                    else
                    {
                        switch (field.localType)
                        {
                            case "System.Int16": field.simpleType = "Number"; break;
                            case "System.Int32": field.simpleType = "Number"; break;
                            case "System.Int64": field.simpleType = "Number"; break;
                            case "System.UInt16": field.simpleType = "Number"; break;
                            case "System.UInt32": field.simpleType = "Number"; break;
                            case "System.UInt64": field.simpleType = "Number"; break;
                            case "System.Byte": field.simpleType = "Number"; break;
                            case "System.SByte": field.simpleType = "Number"; break;
                            case "System.Single": field.simpleType = "Number"; break;
                            case "System.Decimal": field.simpleType = "Number"; break;
                            case "System.Double": field.simpleType = "Number"; break;
                            case "System.TimeSpan": field.simpleType = "Time"; break;
                            case "System.DateTime:": field.simpleType = "Time"; break;
                            default: field.simpleType = field.simpleType = "String"; break;
                        }
                    }
                }
                else
                {
                    switch (field.localType)
                    {
                        case "System.Int16": field.simpleType = "Number"; break;
                        case "System.Int32": field.simpleType = "Number"; break;
                        case "System.Int64": field.simpleType = "Number"; break;
                        case "System.UInt16": field.simpleType = "Number"; break;
                        case "System.UInt32": field.simpleType = "Number"; break;
                        case "System.UInt64": field.simpleType = "Number"; break;
                        case "System.Byte": field.simpleType = "Number"; break;
                        case "System.SByte": field.simpleType = "Number"; break;
                        case "System.Single": field.simpleType = "Number"; break;
                        case "System.Decimal": field.simpleType = "Number"; break;
                        case "System.Double": field.simpleType = "Number"; break;
                        case "System.TimeSpan": field.simpleType = "Time"; break;
                        case "System.DateTime:": field.simpleType = "Time"; break;
                        default: field.simpleType = field.simpleType = "String"; break;
                    }
                }
                fields.Add(field);
            }
            reader.Close();
            return fields;
        }
        /// <summary>
        /// 复杂SQL解析,解析出所有的列
        /// </summary>
        /// <param name="functionData"></param>
        /// <returns></returns>
        public DbFunctionData GetFunctionData(DbFunctionData functionData)
        {

            dataUtility.con.Open();
            SelectStatementInfo selectStatement = new SelectStatementInfo(sqlInfo.sql);
            ParseSQL(ref selectStatement);
            functionData.fields = GetFields(dataUtility, selectStatement);
            functionData.sqlVarParameters = new List<SqlVarParameter>();
            GetParameters(dataUtility, selectStatement, ref functionData.sqlVarParameters);
            dataUtility.con.Close();
            //转换为可执行的SQL
            functionData.sql = FormatSql(functionData.sql);
            return functionData;
        }
        public List<Column> getColumns()
        {
            //System.Data.Common.DbDataAdapter dataAdapter= dataUtility.GetDataAdapter(FormatSql(sqlInfo.sql),dataUtility.con);
            dataUtility.con.Open();
            System.Data.Common.DbCommand cmd = new System.Data.SQLite.SQLiteCommand(FormatSql(sqlInfo.sql), (System.Data.SQLite.SQLiteConnection)dataUtility.con);
            System.Data.DataTable dt = new System.Data.DataTable();
            List<Column> columns = new List<Column>();
            System.Data.Common.DbDataReader reader = cmd.ExecuteReader();
            dt = reader.GetSchemaTable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Column column = new Column();
                column.name = dt.Rows[i]["BaseColumnName"].ToString();
                column.asName = dt.Rows[i]["ColumnName"].ToString();
                column.tableName = dt.Rows[i]["BaseTableName"].ToString();
                column.returnType = dt.Rows[i]["DataType"].ToString();
                columns.Add(column);
            }
            dataUtility.con.Close();
            return columns;
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
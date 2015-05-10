using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace NFinal.Compile
{
    /// <summary>
    /// sql语句与数据库相结合,分析出sql语句中所有的表信息,列信息.
    /// </summary>
    class SqlAnalyse
    {
        /// <summary>
        /// 从Web.config文件中的字符串信息获取所有的表信息,列信息.
        /// </summary>
        /// <param name="appRoot">网站根目录</param>
        public void GetDB(string appRoot)
        {
            //读取数据库信息
            Frame.CreateDB(appRoot);
            NFinal.DB.Coding.DataUtility dataUtility = null;
            if (Config.ConnectionStrings.Count > 0)
            {
                NFinal.DB.ConnectionString conStr;
                NFinal.DB.Coding.DB.DbStore.Clear();
                for (int i = 0; i < Config.ConnectionStrings.Count; i++)
                {
                    conStr = Config.ConnectionStrings[i];
                    if (conStr.type == NFinal.DB.DBType.MySql)
                    {
                        dataUtility = new NFinal.DB.Coding.MySQLDataUtility(conStr.value);
                        dataUtility.GetAllTables(dataUtility.con.Database);
                        NFinal.DB.Coding.DB.DbStore.Add(conStr.name, dataUtility);
                    }
                    else if (conStr.type == NFinal.DB.DBType.Sqlite)
                    {
                        dataUtility = new NFinal.DB.Coding.SQLiteDataUtility(conStr.value);
                        dataUtility.GetAllTables(dataUtility.con.Database);
                        NFinal.DB.Coding.DB.DbStore.Add(conStr.name, dataUtility);
                    }
                    else if (conStr.type == NFinal.DB.DBType.SqlServer)
                    {
                        dataUtility = new NFinal.DB.Coding.SQLDataUtility(conStr.value);
                        dataUtility.GetAllTables(dataUtility.con.Database);
                        NFinal.DB.Coding.DB.DbStore.Add(conStr.name, dataUtility);
                    }
                    else if (conStr.type == NFinal.DB.DBType.Oracle)
                    {
                        dataUtility = new NFinal.DB.Coding.OracleDataUtility(conStr.value);
                        dataUtility.GetAllTables(dataUtility.con.Database);
                        NFinal.DB.Coding.DB.DbStore.Add(conStr.name, dataUtility);
                    }
                }
            }
        }
        
        /// <summary>
        /// 把数据库信息填入从Csharp分析出的函数信息中
        /// </summary>
        /// <param name="DbStore">WebConfig中数据库相关的信息</param>
        /// <param name="functionDataList">从Csharp中分析出的函数信息</param>
        /// <returns>函数信息</returns>
        public List<DbFunctionData> FillFunctionDataList(Dictionary<string, NFinal.DB.Coding.DataUtility> DbStore, List<DbFunctionData> functionDataList)
        {
            if (functionDataList.Count > 0)
            {
                for (int i = 0; i < functionDataList.Count; i++)
                {
                    functionDataList[i] = FillFunctionData(DbStore,functionDataList[i]);
                }
            }
            return functionDataList;
        }

        /// <summary>
        /// 把数据库信息填入从SQL语句分析出的参数中
        /// </summary>
        /// <param name="tables">数据库表信息</param>
        /// <param name="parameters">从SQL语句中分析出的参数信息</param>
        /// <returns>完整的参数信息</returns>
        public List<SqlVarParameter> FillParameter(Dictionary<string,NFinal.DB.Coding.CsTypeLink> csTypeLinkDic,List<NFinal.DB.Coding.Table> tables, List<SqlVarParameter> parameters)
        {
            bool hasField = false;
            if (parameters.Count > 0)
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    //查看是否有列名,如果有则查找列名相关的列信息
                    if (!string.IsNullOrEmpty(parameters[i].columnName))
                    {
                        if (tables.Count > 0)
                        {
                            hasField = false;
                            //循环sql语句中所有表的所有列
                            for (int j = 0; j < tables.Count; j++)
                            {
                                if (tables[j].fields.Count > 0)
                                {
                                    for (int k = 0; k < tables[j].fields.Count; k++)
                                    {
                                        if (parameters[i].columnName == tables[j].fields[k].name)
                                        {
                                            tables[j].fields[k].structFieldName = parameters[i].columnName;
                                            parameters[i].field = tables[j].fields[k];
                                            parameters[i].link= csTypeLinkDic[parameters[i].field.localType];
                                            hasField = true;
                                        }
                                    }
                                }
                            }
                            if (!hasField)
                            {
                                parameters[i].hasSqlError = true;
                                parameters[i].sqlError = "变量" + parameters[i].name + "找不到列属性";
                            }
                        }
                    }
                    //如果没有列信息
                    else
                    { 
                    
                    }
                }
            }
            return parameters;
        }
        /// <summary>
        /// 把数据库信息填入从Csharp分析出的函数信息中
        /// </summary>
        /// <param name="DbStore">数据库表信息</param>
        /// <param name="functionData">从Csharp中分析出的函数信息</param>
        /// <returns>完整的函数信息</returns>
        public DbFunctionData FillFunctionData(Dictionary<string, NFinal.DB.Coding.DataUtility> DbStore, DbFunctionData functionData)
        {
            //查找出sql语句中用的是哪个数据库
            NFinal.DB.Coding.DataUtility dataUtility = NFinal.DB.Coding.DB.DbStore[functionData.connectionName];
            SqlParser sqlParser = new SqlParser(dataUtility.dbType);
            SqlStatement sqlSatement = new SqlStatement("", dataUtility.dbType);
            string sql =sqlSatement.GetSql(functionData.parameters[0]);
            functionData.sql = sqlSatement.FormatSql(sql);
            List<SqlInfo> sqlInfos = sqlParser.Parse(sql);
            SqlInfo sqlInfo = null;
            functionData.hasSqlError = false;
            bool hasTable = false;
            bool hasField = false;
            if (sqlInfos.Count > 0)
            {
                sqlInfo = sqlInfos[0];
                
                //分析表名,查看此表名是否存在
                if (sqlInfo.Tables.Count > 0)
                {
                    functionData.tables = new List<DB.Coding.Table>();
                    DB.Coding.Table table = new DB.Coding.Table();
                    //循环sql语句中的表名
                    for (int i = 0; i < sqlInfo.Tables.Count; i++)
                    {
                        hasTable = false;
                        //循环数据库中的表名
                        for (int j = 0; j < dataUtility.tables.Count; j++)
                        {
                            //如果表名存在,则添加表
                            if (sqlInfo.Tables[i].name == dataUtility.tables[j].name)
                            {
                                functionData.tables.Add(dataUtility.tables[j]);
                                hasTable = true;
                                break;
                            }
                        }
                        //如果表名不存在,则证明sql语句有错误.
                        if (!hasTable)
                        {
                            functionData.hasSqlError = true;
                            functionData.sqlError = "表" + sqlInfo.Tables[i].name + "不存在!";
                            return functionData;
                        }
                    }
                }
            }
            //分析sql中的参数信息
            functionData.sqlVarParameters = FillParameter(dataUtility.csTypeDic, functionData.tables, sqlInfo.sqlVarParameters);
            //分析列信息
            if (sqlInfos.Count > 0)
            {
                sqlInfo = sqlInfos[0];

                if (sqlInfo.Columns.Count > 0)
                {
                    functionData.fields = new List<DB.Coding.Field>();
                    DB.Coding.Field field=new DB.Coding.Field();
                    DB.Coding.Table table = new DB.Coding.Table();

                    for (int i = 0; i < sqlInfo.Columns.Count; i++)
                    {
                        //如果列中有表名
                        if (sqlInfo.Columns[i].tableName != string.Empty)
                        {
                            hasTable = false;
                            //查看表名是否存在于sql语句的表中,如果不存在,则视为出错
                            for (int j = 0; j < functionData.tables.Count; j++)
                            {
                                if (functionData.tables[j].name == sqlInfo.Columns[i].tableName)
                                {
                                    table = functionData.tables[j];
                                    hasTable = true;
                                    break;
                                }
                            }
                            if (!hasTable)
                            {
                                functionData.hasSqlError = true;
                                functionData.sqlError = "表" + sqlInfo.Columns[i].tableName + "不存在";
                                return functionData;
                            }
                            else
                            {
                                //查看是否使用了table.*
                                if (sqlInfo.Columns[i].name == "*")
                                {
                                    //functionData.fields.AddRange(table.fields);
                                    for (int k = 0; k < table.fields.Count; k++)
                                    {
                                        table.fields[k].structFieldName = table.fields[k].name;
                                        functionData.fields.Add(table.fields[k]);
                                    }
                                }
                                //如果使用了table.field
                                else
                                {
                                    hasField = false;
                                    //查看该field是否存在
                                    for (int k = 0; k < table.fields.Count; k++)
                                    {
                                        if (sqlInfo.Columns[i].name == table.fields[k].name)
                                        {
                                            //查看是否有AsName
                                            if (string.IsNullOrEmpty(sqlInfo.Columns[i].asName))
                                            {
                                                table.fields[k].structFieldName = sqlInfo.Columns[i].name;
                                            }
                                            else
                                            {
                                                table.fields[k].structFieldName = sqlInfo.Columns[i].asName;
                                            }
                                            functionData.fields.Add(table.fields[k]);
                                            hasField = true;
                                        }
                                    }
                                    if (!hasField)
                                    {
                                        functionData.hasSqlError = true;
                                        functionData.sqlError = "在表" + table.name + "中,列" + sqlInfo.Columns[i].name + "不存在";
                                        return functionData;
                                    }
                                }
                            }
                        }
                        //如果列中没有表名
                        else
                        {
                            hasField = false;
                            if (sqlInfo.Columns[i].name == "*")
                            {
                                for (int j = 0; j < functionData.tables.Count; j++)
                                {
                                    for (int k = 0; k < functionData.tables[j].fields.Count; k++)
                                    {
                                        functionData.tables[j].fields[k].structFieldName = functionData.tables[j].fields[k].name;
                                        functionData.fields.Add(functionData.tables[j].fields[k]);
                                    }
                                }
                            }
                            else
                            {
                                //循环sql语句中所有表的所有列
                                for (int j = 0; j < functionData.tables.Count; j++)
                                {
                                    for (int k = 0; k < functionData.tables[j].fields.Count; k++)
                                    {
                                        if (sqlInfo.Columns[i].name == functionData.tables[j].fields[k].name)
                                        {
                                            //查看是否有AsName
                                            if (string.IsNullOrEmpty(sqlInfo.Columns[i].asName))
                                            {
                                                functionData.tables[j].fields[k].structFieldName = sqlInfo.Columns[i].name;
                                            }
                                            else
                                            {
                                                functionData.tables[j].fields[k].structFieldName = sqlInfo.Columns[i].asName;
                                            }
                                            functionData.fields.Add(functionData.tables[j].fields[k]);
                                            hasField = true;
                                        }
                                    }
                                }
                                if (!hasField)
                                {
                                    functionData.hasSqlError = true;
                                    functionData.sqlError = "列" + sqlInfo.Columns[i].name + "不存在";
                                    return functionData;
                                }
                            }
                        }
                    }
                }
            }
            return functionData;
        }
    }
}

using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace NFinal.DB.Coding
{
    public class OracleDataUtility:DataUtility
    {
        public OracleDataUtility(string conStr)
            : base(conStr,NFinal.DB.DBType.MySql)
        {
            //name(数据库名称)
            sql_getAllDataBase = @"SELECT name AS ""name"" FROM v$database";
            //name(表名称)
            sql_getAllTables = @"SELECT TABLE_NAME AS ""name"" FROM USER_TABLES";
            //name(字段名称),position(字段位置),default_value(默认值),is_nullable(是否允许为空),data_type(数据类型),max_length(长度),oct_length(长度按字节)
            sql_getAllColumns = @"SELECT COLUMN_NAME AS ""name"",COLUMN_ID AS ""position"",DATA_DEFAULT AS ""default_value"",NULLABLE AS ""is_nullable"",DATA_TYPE as ""data_type""
                ,CHAR_LENGTH as ""max_length"",CHAR_COL_DECL_LENGTH as ""oct_length""
                  FROM USER_TAB_COLUMNS WHERE TABLE_NAME='{1}'";
            //name(主键名称),position(主键的位置)
            sql_getAllIds = @"SELECT  col.column_name as ""name"",col.position as ""position"" 
                FROM user_constraints con,user_cons_columns col where con.constraint_name=col.constraint_name 
                and con.constraint_type='P' and col.table_name='{1}'";
            DbTypeEnumName = "OracleDbType";
            UsingNameSpace = "\r\nusing Oracle.ManagedDataAccess.Client;";
            FieldClassName = "OracleField";
            ParameterClassName = "OracleParameter";
            DbHelperClassName = "DbHelperOracle";
            con = new OracleConnection(conStr);
        }
        /// <summary>
        /// 数据库适配器类
        /// </summary>
        /// <param name="cmd">数据库命令</param>
        /// <param name="con">数据库连接</param>
        /// <returns>数据库适配类</returns>
        public override System.Data.Common.DbDataAdapter GetDataAdapter(string cmd, System.Data.Common.DbConnection con)
        {
            return new OracleDataAdapter(cmd, (OracleConnection)con);
        }
        /// <summary>
        /// 设置字段信息
        /// </summary>
        /// <param name="field">字段实体类</param>
        /// <param name="dr">表中一行</param>
        public override void SetField(ref Field field, DataRow dr)
        {
            //name(字段名称),position(字段位置),default_value(默认值),is_nullable(是否允许为空),data_type(数据类型),max_length(长度),oct_length(长度按字节)
            field.name = dr["name"].ToString();
            field.nameCs = GetNameCs(field.name);//csharp中的名称
            field.nameJs = GetNameJs(field.name);//js中的名称
            field.position = Convert.ToInt32(dr["position"]);
            field.hasDefault = dr["default_value"] == DBNull.Value ? false : true;
            field.defautlValue = dr["default_value"].ToString();
            field.allowNull = dr["is_nullable"].ToString() == "0" ? false : true;
            field.sqlType = dr["data_type"].ToString();
            int temp = dr["max_length"] == DBNull.Value ? 0 : Convert.ToInt32(dr["max_length"]);
            if (temp < 0 || temp > int.MaxValue)
            {
                field.length = 0;
            }
            else
            {
                field.length = temp;
            }
            temp = dr["oct_length"] == DBNull.Value ? 0 : Convert.ToInt32(dr["oct_length"]);
            if (temp < 0 || temp > int.MaxValue)
            {
                field.octLength = 0;
            }
            else
            {
                field.octLength = temp;
            }
        }
    }
}
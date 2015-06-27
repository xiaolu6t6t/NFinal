using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace NFinal.DB.Coding
{
    /// <summary>
    /// MySql数据库信息类
    /// </summary>
    public class MySQLDataUtility : DataUtility
    {
        public MySQLDataUtility(string conStr)
            : base(conStr,NFinal.DB.DBType.MySql)
        {
            //name(数据库名称)
            sql_getAllDataBase = "SELECT SCHEMA_NAME AS name FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME!='information_schema' AND SCHEMA_NAME!='mysql'";
            //name(表名称)
            sql_getAllTables = "SELECT TABLE_NAME AS name FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='{0}'";
            //name(字段名称),position(字段位置),default_value(默认值),is_nullable(是否允许为空),data_type(数据类型),max_length(长度),oct_length(长度按字节)
            sql_getAllColumns = @"SELECT COLUMN_NAME AS 'name',ORDINAL_POSITION AS 'position',COLUMN_DEFAULT AS 'default_value',IS_NULLABLE AS 'is_nullable',DATA_TYPE as 'data_type'
                ,CHARACTER_MAXIMUM_LENGTH as 'max_length',CHARACTER_OCTET_LENGTH as 'oct_length'
                  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='{0}' AND TABLE_NAME='{1}'";
            //name(主键名称),position(主键的位置)
            sql_getAllIds = "SELECT COLUMN_NAME AS 'name',ORDINAL_POSITION AS 'position' FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE  WHERE TABLE_SCHEMA='{0}' AND TABLE_NAME='{1}'";
            DbTypeEnumName = "MySqlDbType";
            UsingNameSpace = "\r\nusing MySql.Data.MySqlClient;";
            FieldClassName = "MySqlField";
            ParameterClassName = "MySqlParameter";
            DbHelperClassName = "DbHelperMySQL";
            con = new MySqlConnection(conStr);
        }
        public override string GetDbType(int dbType)
        {
            return ((MySql.Data.MySqlClient.MySqlDbType)dbType).ToString();
        }
        public override DbCommand GetDbCommand(string cmd, DbConnection con)
        {
            return new MySqlCommand(cmd, (MySqlConnection)con);
        }
        /// <summary>
        /// 数据库适配器类
        /// </summary>
        /// <param name="cmd">数据库命令</param>
        /// <param name="con">数据库连接</param>
        /// <returns>数据库适配类</returns>
        public override DbDataAdapter GetDataAdapter(string cmd, DbConnection con)
        {
            return new MySqlDataAdapter(cmd, (MySqlConnection)con);
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
                field.octLength =temp;
            }
        }
    }
}

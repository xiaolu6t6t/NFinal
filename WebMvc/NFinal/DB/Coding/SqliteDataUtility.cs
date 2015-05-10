using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Data.SqlClient;

namespace NFinal.DB.Coding
{
    /// <summary>
    /// Sqlite数据库信息类
    /// </summary>
    public class SQLiteDataUtility : DataUtility
    {
        public SQLiteDataUtility(string conStr)
            : base(conStr,NFinal.DB.DBType.Sqlite)
        {
            //name(数据库名称)
            sql_getAllDataBase = "PRAGMA database_list";
            //name(表名称)
            sql_getAllTables = "select name from {0}.sqlite_master where type='table' and name!= 'sqlite_sequence'";
            //name(字段名称),position(字段位置),default_value(默认值),is_nullable(是否允许为空),data_type(数据类型),max_length(长度),oct_length(长度按字节)
            sql_getAllColumns = "PRAGMA {0}.table_info('{1}')";
            //name(主键名称),position(主键的位置)
            sql_getAllIds = "PRAGMA {0}.table_info('{1}')";
            DbTypeEnumName = "DbType";
            UsingNameSpace = "using System.Data.SQLite;\r\nusing System.Data.SqlClient;";
            FieldClassName = "SQLiteField";
            ParameterClassName = "SQLiteParameter";
            DbHelperClassName = "DbHelperSQLite";
            con = new SQLiteConnection(conStr); System.Data.SQLite.SQLiteParameter par = new SQLiteParameter();
        }
        /// <summary>
        /// 数据库适配器类
        /// </summary>
        /// <param name="cmd">数据库命令</param>
        /// <param name="con">数据库连接</param>
        /// <returns>数据库适配类</returns>
        public override DbDataAdapter GetDataAdapter(string cmd, DbConnection con)
        {
            return new SQLiteDataAdapter(cmd,(SQLiteConnection)con);
        }
        /// <summary>
        /// 设置字段信息
        /// </summary>
        /// <param name="field">字段实体类</param>
        /// <param name="dr">表中一行</param>
        public override void SetField(ref Field field, DataRow dr)
        {
            field.name = dr["name"].ToString();
            //name(字段名称),position(字段位置),default_value(默认值),is_nullable(是否允许为空),data_type(数据类型),max_length(长度),oct_length(长度按字节)
            field.nameCs = GetNameCs(field.name);//csharp中的名称
            field.nameJs = GetNameJs(field.name);//js中的名称
            field.position = Convert.ToInt32(dr["cid"]);
            field.hasDefault = dr["dflt_value"] == DBNull.Value ? false : true;
            field.defautlValue = dr["dflt_value"].ToString();
            field.allowNull = dr["notnull"].ToString() == "0" ? true : false;
            field.sqlType = dr["type"].ToString().ToLower();
            field.length = 0;
            field.octLength = 0;
        }
        /// <summary>
        /// 获取表主键
        /// </summary>
        /// <param name="dataBase">数据库名</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        public override string GetId(string dataBase, string table)
        {
            DbDataAdapter adp = GetDataAdapter(string.Format(sql_getAllIds, dataBase, table), con);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            string id = null;
            adp.Dispose();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["pk"].ToString() == "1")
                    {
                        id = dt.Rows[i]["name"].ToString();
                    }
                }
            }
            dt.Dispose();
            return id;
        }
    }
}

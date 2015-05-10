using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace NFinal.DB.Coding
{
    /// <summary>
    /// 数据库信息类
    /// </summary>
    public class DataUtility
    {
        public string connectionString;
        public string conStr;
        public string DbTypeEnumName = "";
        public string UsingNameSpace = "";
        public string FieldClassName = "";
        public string ParameterClassName = "";
        public string DbHelperClassName = "";
        public DbConnection con = null;
        public System.Collections.Generic.List<Table> tables = new System.Collections.Generic.List<Table>();
        public Dictionary<string, CsTypeLink> csTypeDic = null;
        public string[] dataBases;
        public string[] keywordsCs;
        public string[] keywordsJs;
        public string sql_getAllDataBase = "SELECT name FROM Master..SysDatabases order by Name";
        public string sql_getAllTables = "SELECT * FROM {0}..SysObjects Where XType='U' ORDER BY Name";
        public string sql_getAllColumns = "SELECT * FROM SysColumns WHERE id=Object_Id('{0}')";
        public string sql_getAllIds = "SELECT name FROM SysColumns WHERE id=Object_Id('{0}') and colid=(select  top 1 keyno from sysindexkeys where id=Object_Id('{0}'))";
        public NFinal.DB.DBType dbType = DBType.Unknown;
        /// <summary>
        /// 数据库信息类
        /// </summary>
        /// <param name="conStr">连接字符串</param>
        /// <param name="typeLinkFileName">数据库所对应的相关类型</param>
        /// <param name="csTypeLinkFileName">csharp所对应的相关类型</param>
        public DataUtility(string conStr, NFinal.DB.DBType dbType)
        {
            this.conStr = conStr;
            this.dbType = dbType;
            CsTypeTable table = new CsTypeTable(this.dbType);
            this.csTypeDic = table.GetCsTypeTableDic();
            //.net关键字列表
            keywordsCs = new string[] { 
                "abstract", "event", "new","struct",
                "as","explicit","null","switch",
                "base","extern","object","this",
                "bool","false","operator","throw",
                "break","finally","out","true",
                "byte","fixed","override","try",
                "case","float","params","typeof",
                "catch","for","private","uint",
                "char","foreach","protected","ulong",
                "checked","goto","public","unchecked",
                "class","if","readonly","unsafe",
                "const","implicit","ref","ushort",
                "continue","in","return","using",
                "decimal","int","sbyte","virtual",
                "default","interface","sealed","volatile",
                "delegate","internal","short","void",
                "do","is","sizeof","while",
                "double","lock","stackalloc",
                "else","long","static",
                "enum","namespace","string"
            };
            //javascript关键字列表
            keywordsJs = new string[] { 
                "break", "case", "catch", "continue", "debugger",
                "default", "delete", "do", "else", "false", "finally", 
                "for", "function", "if", "in", "instanceof", "new", 
                "null", "return", "switch", "this", "throw", "true", 
                "try", "typeof", "var", "void", "while", "with", 
                "abstract", "boolean", "byte", "char", "class", 
                "const", "double", "enum", "export", "extends", 
                "final", "float", "goto", "implements", "import", 
                "int", "interface", "long", "native", "package", 
                "private", "protected", "public", "short", "static", 
                "super", "synchronized", "throws", "transient", "volatile", 
                "arguments", "let", "yield"
            };
        }
        /// <summary>
        /// 修正JS字段名称
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>修正的名称</returns>
        public string GetNameJs(string name)
        {
            foreach (string key in keywordsJs)
            {
                if (name == key)
                {
                    return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToUpper(name);
                }
            }
            return name;
        }
        /// <summary>
        /// 修正CS字段名称
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>修正的名称</returns>
        public string GetNameCs(string name)
        {
            foreach (string key in keywordsCs)
            {
                if (name == key)
                {
                    return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToUpper(name);
                }
            }
            return name;
        }
        /// <summary>
        /// 获取所有的数据库名称
        /// </summary>
        /// <returns>数据库名数组</returns>
        public virtual string[] GetAllDataBase()
        {
            DbDataAdapter adp = GetDataAdapter(sql_getAllDataBase,con);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            dataBases = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dataBases[i] = dt.Rows[i]["name"].ToString();
            }
            dt.Dispose();
            adp.Dispose();
            return dataBases;
        }
        /// <summary>
        /// 数据库适配器类
        /// </summary>
        /// <param name="cmd">数据库命令</param>
        /// <param name="con">数据库连接</param>
        /// <returns>数据库适配类</returns>
        public virtual DbDataAdapter GetDataAdapter(string cmd, DbConnection con)
        {
            return new SqlDataAdapter(cmd, (SqlConnection)con);
        }
        /// <summary>
        /// 获取所有的表名
        /// </summary>
        /// <param name="dataBase">数据库名</param>
        /// <returns>所有的表名</returns>
        public virtual System.Collections.Generic.List<string> GetAllTableNames(string dataBase)
        {
            DbDataAdapter adp = GetDataAdapter(string.Format(sql_getAllTables, dataBase), con);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            adp.Dispose();
            System.Collections.Generic.List<string> table_names = new System.Collections.Generic.List<string>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    table_names.Add(dt.Rows[i]["name"].ToString());
                }
            }
            dt.Dispose();
            return table_names;
        }
        /// <summary>
        /// 获取所有的字段信息
        /// </summary>
        /// <param name="dataBase">数据库名称</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        public virtual DataTable GetAllColumns(string dataBase, string table)
        {
            DbDataAdapter adp = GetDataAdapter(string.Format(sql_getAllColumns, dataBase, table), con);
            DataTable dtCols = new DataTable();
            adp.Fill(dtCols);
            adp.Dispose();
            return dtCols;
        }
        /// <summary>
        /// 设置字段信息
        /// </summary>
        /// <param name="field">字段实体类</param>
        /// <param name="dr">表中一行</param>
        public virtual void SetField(ref Field field, DataRow dr)
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
            field.length =  dr["max_length"] == DBNull.Value ? 0 : Convert.ToInt32(dr["max_length"]);
            field.octLength = dr["oct_length"] == DBNull.Value ? 0 : Convert.ToInt32(dr["oct_length"]);
        }
        /// <summary>
        /// 获取表主键
        /// </summary>
        /// <param name="dataBase">数据库名</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        public virtual string GetId(string dataBase, string table)
        {
            DbDataAdapter adp = GetDataAdapter(string.Format(sql_getAllIds, dataBase, table), con);
            DataTable dtIdName = new DataTable();
            adp.Fill(dtIdName);
            string id = dtIdName.Rows.Count == 0 ? null : dtIdName.Rows[0][0].ToString();
            adp.Dispose();
            dtIdName.Dispose();
            return id;
        }
        /// <summary>
        /// 获取所有的表
        /// </summary>
        /// <param name="dataBase">数据库名</param>
        public void GetAllTables(string dataBase)
        {
            con.Open();
            TypeTable typeTable = new TypeTable(this.dbType);
            Dictionary<string, TypeLink> links = typeTable.GetTypeLinks();
            Table table;
            //获取所有的表
            System.Collections.Generic.List<string> dt_tables = GetAllTableNames(dataBase);
            DataTable dtCols = new DataTable();
            DataTable dtIdName = new DataTable();
            Field field;
            string idName = null;
            //循环查找所有的Table
            for (int i = 0; i < dt_tables.Count; i++)
            {
                table = new Table(dt_tables[i]);
                //相关参数
                table.DbTypeEnumName = this.DbTypeEnumName;
                table.UsingNameSpace = this.UsingNameSpace;
                table.FieldClassName = this.FieldClassName;
                table.ParameterClassName = this.ParameterClassName;
                table.DbHelperClassName = this.DbHelperClassName;
                //获取表名
                table.nameCs = GetNameCs(table.name);
                table.nameJs = GetNameJs(table.name);
                //获取表中所有的字段

                dtCols = GetAllColumns(dataBase, dt_tables[i]);

                //获取表中的主键值
                idName = GetId(dataBase, dt_tables[i]);
                table.hasId = idName != null;

                //添加列信息
                table.fields = new System.Collections.Generic.List<Field>();
                for (int j = 0; j < dtCols.Rows.Count; j++)
                {
                    field = new Field();
                    SetField(ref field, dtCols.Rows[j]);
                    field.nameCs = GetNameCs(field.name);
                    field.nameJs = GetNameJs(field.name);
                    if (idName == null)
                    {
                        field.isId = false;
                    }
                    else
                    {
                        field.isId = field.name == idName.ToString();
                    }
                    

                    //数据库中的类型
                    field.sqlType = links[field.sqlType].sqlType;
                    //csharp 基本类型
                    field.localType = links[field.sqlType].localType;
                    //csharp System.Data 中的类型
                    field.dbType = links[field.sqlType].dbType;
                    //数据转换时所标识的类型
                    field.simpleType = links[field.sqlType].simpleType;
                    if (field.isId)
                    {
                        table.id = field;
                    }
                    //列类型所对应的数据转换成csharp成员时所对应的类型及转换函数
                    field.csTypeLink=csTypeDic[field.localType];
                    table.fields.Add(field);
                }
                dtCols.Dispose();
                tables.Add(table);
            }
            con.Close();
        }
        /// <summary>
        /// 保存数据库信息
        /// </summary>
        public void Save()
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(List<Table>));
            System.IO.StreamWriter sw = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/base.xml", false, Encoding.UTF8);
            ser.Serialize(sw, tables);
            sw.Close();
            sw.Dispose();
        }
    }
}

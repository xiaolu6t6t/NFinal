using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Text;

namespace NFinal.DB.Coding
{
    /// <summary>
    /// 读取配置文件中csharp对应的数据库类型,数据库参数枚举类型
    /// </summary>
    public class CsTypeTable
    {
        string fileName = "/NFinal/mysql.cs.txt";
        public CsTypeTable(DBType dbType)
        {
            switch (dbType)
            {
                case DBType.MySql: fileName = "/NFinal/SqlTemplate/mysql.cs.txt"; break;
                case DBType.Sqlite: fileName = "/NFinal/SqlTemplate/sqlite.cs.txt"; break;
                case DBType.SqlServer: fileName = "/NFinal/SqlTemplate/sqlserver.cs.txt"; break;
                default: fileName = "/NFinal/SqlTemplate/sqlserver.cs.txt"; break;
            }
        }
        public Dictionary<string, CsTypeLink> GetCsTypeTableDic()
        {
            Dictionary<string, CsTypeLink> csTypeTableDic = new Dictionary<string, CsTypeLink>();
            System.IO.StreamReader sr = new System.IO.StreamReader(Frame.MapPath(fileName), Encoding.UTF8);
            string[] lines = null;
            string[] sqlDbGetPar = null;
            CsTypeLink table = null;
            while (!sr.EndOfStream)
            {
                lines = sr.ReadLine().Split('\t');
                table = new CsTypeLink();
                table.csharpType = lines[0];
                sqlDbGetPar = lines[1].Split(':');
                table.sqlDbGetMethod = sqlDbGetPar[0];
                if (sqlDbGetPar.Length > 1)
                {
                    table.sqlDbGetType = sqlDbGetPar[1];
                }
                else
                {
                    table.sqlDbGetType = "";
                }
                table.sqlType = lines[2];
                table.sqlDbType = lines[3];
                csTypeTableDic.Add(table.csharpType, table);
            }
            sr.Close();
            return csTypeTableDic;
        }
    }
}
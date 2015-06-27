using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NFinal.DB.Coding
{
    /// <summary>
    /// 字段类型表
    /// </summary>
    public class TypeTable
    {
        public string fileName = "";
        public TypeTable(NFinal.DB.DBType dbType)
        {
            switch (dbType)
            {
                case DBType.MySql: fileName = "/NFinal/SqlTemplate/mysql.txt";break;
                case DBType.Sqlite: fileName = "/NFinal/SqlTemplate/sqlite.txt"; break;
                case DBType.SqlServer: fileName = "/NFinal/SqlTemplate/sqlserver.txt"; break;
                case DBType.Oracle: fileName = "/NFinal/SqlTemplate/oracle.txt"; break;
                default: fileName = "/NFinal/SqlTemplate/sqlserver.txt"; break;
            }

        }
        public Dictionary<string, TypeLink> GetTypeLinks()
        {
            Dictionary<string, TypeLink> links = new Dictionary<string, TypeLink>();
            System.IO.StreamReader sr = new System.IO.StreamReader(Frame.MapPath(fileName), Encoding.UTF8);
            TypeLink link = new TypeLink();
            string[] row;
            while (!sr.EndOfStream)
            {
                row = sr.ReadLine().Split('\t');
                link = new TypeLink();
                //数据库中的类型
                link.sqlType = row[0].ToLower();
                //csharp 基本类型
                link.localType = row[1];
                //csharp System.Data 中的类型
                link.dbType = row[2];
                //数据转换时所标识的类型
                link.simpleType = row[3];
                links.Add(link.sqlType, link);
            }
            sr.Close();
            return links;
        }
    }
}

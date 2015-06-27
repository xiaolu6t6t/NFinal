using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using NFinal.Compile;

namespace NFinal
{
    public class Frame
    {
        public static string appRoot;
        public static string AssemblyTitle;
       
        public static List<NFinal.DB.ConnectionString> ConnectionStrings = new List<NFinal.DB.ConnectionString>();

        public Frame(string appRoot)
        {
            Frame.appRoot = appRoot;
            string[] fileNames = Directory.GetFiles(appRoot, "*.csproj");
            if (fileNames.Length > 0)
            {
                AssemblyTitle = Path.GetFileNameWithoutExtension(fileNames[0]);
            }
            else
            {
                string temp;
                temp = appRoot.Trim('\\');
                AssemblyTitle = temp.Substring(temp.LastIndexOf('\\') + 1);
            }
        }

        /// <summary>
        /// 把基于网站根目录的绝对路径改为相对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MapPath(string url)
        {
            return appRoot + url.Trim('/').Replace('/', '\\');
        }

        public void GetDB()
        {
            //获取WebConfig中的连接字符串信息
            string configFileName = appRoot + "Web.config";
            if (File.Exists(configFileName))
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(configFileName);
                System.Xml.XmlNode root = doc.DocumentElement.SelectSingleNode("/configuration");
                System.Xml.XmlNodeList nodeList = root.SelectNodes("connectionStrings/add[@connectionString]");
                Frame.ConnectionStrings.Clear();
                if (nodeList.Count > 0)
                {
                    System.Xml.XmlElement ele = null;
                    foreach (System.Xml.XmlNode node in nodeList)
                    {
                        ele = node as System.Xml.XmlElement;
                        var connectionString = new NFinal.DB.ConnectionString();
                        connectionString.name = ele.Attributes["name"].Value;
                        connectionString.value = ele.Attributes["connectionString"].Value;
                        connectionString.provider = ele.Attributes["providerName"].Value;
              
                        if (connectionString.provider.ToLower().IndexOf("mysql") > -1)
                        {
                            connectionString.type = NFinal.DB.DBType.MySql;
                        }
                        else if (connectionString.provider.ToLower().IndexOf("sqlclient") > -1)
                        {
                            connectionString.type = NFinal.DB.DBType.SqlServer;
                        }
                        else if (connectionString.provider.ToLower().IndexOf("sqlite") > -1)
                        {
                            connectionString.type = NFinal.DB.DBType.Sqlite;
                        }
                        else if (connectionString.provider.ToLower().IndexOf("oracle") > -1)
                        {
                            connectionString.type = NFinal.DB.DBType.Oracle;
                        }
                        else
                        {
                            connectionString.type = NFinal.DB.DBType.Unknown;
                        }
                        Frame.ConnectionStrings.Add(connectionString);
                    }
                }
            }
            //读取数据库信息
            NFinal.DB.Coding.DataUtility dataUtility = null;
            if (Frame.ConnectionStrings.Count > 0)
            {
                NFinal.DB.ConnectionString conStr;
                NFinal.DB.Coding.DB.DbStore.Clear();
                for (int i = 0; i < Frame.ConnectionStrings.Count; i++)
                {
                    conStr = Frame.ConnectionStrings[i];
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
#if NET2
#else
                        dataUtility = new NFinal.DB.Coding.OracleDataUtility(conStr.value);
                        dataUtility.GetAllTables(dataUtility.con.Database);
                        NFinal.DB.Coding.DB.DbStore.Add(conStr.name, dataUtility);
#endif
                    }
                }
            }
        }

        /// <summary>
        /// 创建主路由
        /// </summary>
        /// <param name="apps"></param>
        public void CreateMain(string[] apps)
        {
            VTemplate.Engine.TemplateDocument doc = null;
            doc=new VTemplate.Engine.TemplateDocument(MapPath("NFinal/Template/Main.tpl"), System.Text.Encoding.UTF8);
            doc.SetValue("project", NFinal.Frame.AssemblyTitle);
            doc.SetValue("apps", apps);
            doc.SetValue("ControllerSuffix", "Controller");
            doc.RenderTo(MapPath("NFinal/Main.cs"), System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 获取所有的应用模块
        /// </summary>
        /// <returns></returns>
        public string[] GetApps()
        {
            string webConfigFileName= MapPath("/Web.config");
            XmlDocument doc = new XmlDocument();
            doc.Load(webConfigFileName);
            XmlNode appsNode = doc.DocumentElement.SelectSingleNode("appSettings/add[@key='Apps']");
            string[] Apps = null;

            if (appsNode != null && appsNode.Attributes.Count > 0 && appsNode.Attributes["value"] != null)
            {
                Apps = appsNode.Attributes["value"].Value.Split(',');
            }
            else
            {
                Apps =new string[] {"App"};
            }
            return Apps;
        }
        
    }

}
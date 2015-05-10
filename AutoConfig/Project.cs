using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace AutoConfig
{
    public class Project
    {
        public class Assm
        {
            //是否是framework内部引用
            public bool IsFrameWork = false;
            //引入路径
            public Assm(string Include)
            {
                this.Include = Include;
                IsFrameWork = true;
            }
            //初始化函数
            public Assm(string Include, bool SpecificVersion, string HintPath)
            {
                this.Include = Include;
                this.SpecificVersion = SpecificVersion;
                this.HintPath = HintPath;
                IsFrameWork = false;
            }
            //获取相关的xml
            public XmlNode GetXMLNode(XmlDocument doc, string nameSpace)
            {
                XmlNode nodeReference = doc.CreateElement("Reference", nameSpace);
                XmlAttribute attrInclude = doc.CreateAttribute("Include");
                attrInclude.Value = this.Include;
                nodeReference.Attributes.Append(attrInclude);
                if (!IsFrameWork)
                {
                    XmlNode nodeSpecificVersion = doc.CreateElement("SpecificVersion", nameSpace);
                    nodeSpecificVersion.InnerText = this.SpecificVersion.ToString();
                    XmlNode nodeHintPath = doc.CreateElement("HintPath", nameSpace);
                    nodeHintPath.InnerText = this.HintPath;
                    nodeReference.AppendChild(nodeSpecificVersion);
                    nodeReference.AppendChild(nodeHintPath);
                }
                return nodeReference;
            }
            public string Include = string.Empty;
            public bool SpecificVersion = false;
            public string HintPath = string.Empty;
        }
        //将相对根目录的路径转为绝对路径
        public static string MapPath(string url)
        {
            return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName + url.Replace('/', '\\');
        }
        public void AddLibrary(string projectFile)
        {
            Assm AssmSQLite = new Assm("System.Data.SQLite, Version=1.0.96.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139, processorArchitecture=MSIL",
                    false, "NFinal/Resource/System.Data.SQLite.dll");
            Assm AssmMySQL = new Assm("MySql.Data, Version=5.1.5.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d, processorArchitecture=MSIL",
                false, "NFinal/Resource/MySql.Data.dll");

            List<Assm> AssmList = new List<Assm>();
            AssmList.Add(AssmSQLite);
            AssmList.Add(AssmMySQL);
            AssmList.Add(new Assm("System.DirectoryServices"));
            AssmList.Add(new Assm("System.Configuration"));
            AssmList.Add(new Assm("System.Data"));
            AssmList.Add(new Assm("System.Web"));
            AssmList.Add(new Assm("Microsoft.CSharp"));

            //复制文件到执行目录下
            string root = Path.GetDirectoryName(projectFile);
            string dllFileName = null;
            string resourceName = null;
            for (int i = 0; i < AssmList.Count; i++)
            {
                if (!AssmList[i].IsFrameWork)
                {
                    dllFileName = root + "\\bin\\" + Path.GetFileName(AssmList[i].HintPath); 
                    resourceName = root +"\\"+ AssmList[i].HintPath.Replace('/', '\\');
                    if (!File.Exists(dllFileName))
                    {
                        File.Copy(resourceName, dllFileName);
                    }
                }
            }
            //修改工程文件,加入dll引入
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(projectFile);
            string nameSpace = doc.DocumentElement.Attributes["xmlns"].Value;
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.NameTable);
            namespaceManager.AddNamespace("x", nameSpace);
            XmlNode GroupReference = doc.DocumentElement.SelectSingleNode("x:ItemGroup/x:Reference[1]", namespaceManager).ParentNode;
            bool hasChanged = false;
            string xpath = null;
            for (int i = 0; i < AssmList.Count; i++)
            {
                xpath = string.Format("x:Reference[@Include='{0}']", AssmList[i].Include);
                XmlNode node = GroupReference.SelectSingleNode(xpath, namespaceManager);
                if (node == null)
                {
                    hasChanged = true;
                    GroupReference.AppendChild(AssmList[i].GetXMLNode(doc, nameSpace));
                }
            }
            //如果文件有更改则保存
            if (hasChanged)
            {
                doc.Save(projectFile);
            }
        }
        public void AddFolders(System.Xml.XmlDocument doc,XmlNamespaceManager namespaceManager,string[] apps)
        {
            string app;
            string root;
            for (int i = 0; i < apps.Length; i++)
            {
                app = apps[i];
                System.IO.Directory.GetDirectories(MapPath("/"+app+""));
            }
        }
        public void AddFiles()
        { 
            
        }
    }
}

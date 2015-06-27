using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace AutoConfig
{
    public class Project
    {
        //dl引用实体类
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
        private string projectFile=null;
        private XmlNamespaceManager namespaceManager = null;
        private XmlDocument doc = null;
        private string root;
        private bool hasChanged=false;
        private string nameSpace = null;
        public Project(string projectFile)
        {
            this.projectFile = projectFile;
            this.root= new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName + "\\";
            //修改工程文件,加入dll引入
            doc = new System.Xml.XmlDocument();
            doc.Load(projectFile);
            nameSpace = doc.DocumentElement.Attributes["xmlns"].Value;
            namespaceManager = new XmlNamespaceManager(doc.NameTable);
            namespaceManager.AddNamespace("x", nameSpace);
        }
        public void Save()
        {
            doc.Save(projectFile);
        }
        //将相对根目录的路径转为绝对路径
        public string MapPath(string url)
        {
            return root + url.Trim('/').Replace('/', '\\');
        }
        public string MPath(string url)
        {
            return  url.Replace(root,"")+"\\";
        }
        //添加DLL引用
        public void AddLibrary()
        {
            Assm AssmSQLite = new Assm("System.Data.SQLite, Version=1.0.96.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139, processorArchitecture=MSIL",
                    false, "NFinal/Resource/System.Data.SQLite.dll");
            Assm AssmMySQL = new Assm("MySql.Data, Version=5.1.5.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d, processorArchitecture=MSIL",
                false, "NFinal/Resource/MySql.Data.dll");
            Assm AssmOracle = new Assm("Oracle.ManagedDataAccess, Version=4.121.1.0, Culture=neutral, PublicKeyToken=89b483f429c47342, processorArchitecture=MSIL",
                false,"NFinal/Resource/Oracle.ManagedDataAccess.dll");
            List<Assm> AssmList = new List<Assm>();
            AssmList.Add(AssmSQLite);
            AssmList.Add(AssmMySQL);
            AssmList.Add(AssmOracle);
            AssmList.Add(new Assm("Microsoft.CSharp"));
            AssmList.Add(new Assm("System"));
            AssmList.Add(new Assm("System.Configuration"));
            AssmList.Add(new Assm("System.Data"));
            AssmList.Add(new Assm("System.Web"));
            AssmList.Add(new Assm("System.XML"));

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
            XmlNode GroupReference = doc.DocumentElement.SelectSingleNode("x:ItemGroup/x:Reference[1]", namespaceManager).ParentNode;
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
            //引入32位64位通用dll文件
            if (!File.Exists(root + "\\bin\\x64\\SQLite.Interop.dll"))
            {
                File.Copy(root + "\\NFinal\\Resource\\x64\\SQLite.Interop.dll",GetFileName(root + "\\bin\\x64\\SQLite.Interop.dll"), true);
            }
            if (!File.Exists(root + "\\bin\\x86\\SQLite.Interop.dll"))
            {
                File.Copy(root + "\\NFinal\\Resource\\x86\\SQLite.Interop.dll",GetFileName(root + "\\bin\\x86\\SQLite.Interop.dll"), true);
            }
        }
        public string GetFileName(string fileName)
        {
            string dir = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return fileName;
        }
        public bool hasAdded(string fileName)
        {
            return true;
        }
        public  void AddFile(string fileName, string Extension)
        {
            string ext = Path.GetExtension(fileName);
            if (Extension != "")
            {
                if (fileName.EndsWith(".cs"))
                {  
                    XmlNode GroupReference = doc.DocumentElement.SelectSingleNode("x:ItemGroup[2]", namespaceManager);
                    //截取Include值
                    string filestr = (fileName.Replace(Path.GetDirectoryName(projectFile), "")).Substring(1, (fileName.Replace(Path.GetDirectoryName(projectFile), "")).Length - 1);
                    string fileInclude = string.Format("x:ItemGroup/x:Compile[@Include='{0}']", filestr);
                    XmlNode node = doc.DocumentElement.SelectSingleNode(fileInclude, namespaceManager);
                    if (node == null)
                    {
                        XmlElement Content = doc.CreateElement("Compile", nameSpace);
                        
                        XmlElement SubType = doc.CreateElement("SubType", nameSpace);
                        if (fileName.EndsWith(".ashx.cs"))
                        {
                            XmlElement DependentUpon = doc.CreateElement("DependentUpon", nameSpace);
                            DependentUpon.InnerText = Extension.Replace(".cs", "");
                           
                            Content.AppendChild(DependentUpon);
                        }
                        else if (fileName.EndsWith(".aspx.cs"))
                        {
                            XmlElement DependentUpon = doc.CreateElement("DependentUpon", nameSpace);
                            DependentUpon.InnerText = Extension.Replace(".cs", "");
                            SubType.InnerText = "ASPXCodeBehind";
                            Content.AppendChild(DependentUpon);
                            Content.AppendChild(SubType);
                        }
                        else if (fileName.EndsWith(".designer.cs"))
                        {
                            XmlElement DependentUpon = doc.CreateElement("DependentUpon", nameSpace);
                            DependentUpon.InnerText = Extension.Replace(".designer.cs", "");
                            Content.AppendChild(DependentUpon);
                        }
                        else if (fileName.EndsWith(".ascx.cs"))
                        {
                            XmlElement DependentUpon = doc.CreateElement("DependentUpon", nameSpace);
                            DependentUpon.InnerText = Extension.Replace(".cs", "");
                            SubType.InnerText = "ASPXCodeBehind";
                            Content.AppendChild(DependentUpon);
                            Content.AppendChild(SubType);
                        }
                        else
                        {
                            SubType.InnerText = "Code";
                            Content.AppendChild(SubType);
                        }
                        XmlAttribute attrInclude = doc.CreateAttribute("Include");
                        attrInclude.InnerText = filestr;
                        Content.Attributes.Append(attrInclude);
                        
                        GroupReference.AppendChild(Content);
                    }
                }
                else if (fileName.EndsWith(".Debug.config")||fileName.EndsWith(".Release.config"))
                {   
                }
                else
                {
                    XmlNode GroupReference = doc.DocumentElement.SelectSingleNode("x:ItemGroup[2]", namespaceManager);
                    //截取Include值
                    string filestr = (fileName.Replace(Path.GetDirectoryName(projectFile), "")).Substring(1, (fileName.Replace(Path.GetDirectoryName(projectFile), "")).Length - 1);
                    string fileInclude = string.Format("x:ItemGroup/x:Content[@Include='{0}']", filestr);
                    XmlNode node = doc.DocumentElement.SelectSingleNode(fileInclude, namespaceManager);
                    if (node == null)
                    {
                        XmlElement Content = doc.CreateElement("Content", nameSpace);
                        XmlAttribute attrInclude = doc.CreateAttribute("Include");
                        attrInclude.Value = filestr;
                        Content.Attributes.Append(attrInclude);
                        GroupReference.AppendChild(Content);
                    }
                }

            }
            else
            {
                XmlNode GroupReference = doc.DocumentElement.SelectSingleNode("x:ItemGroup[2]", namespaceManager);
                //截取Include值
                string filestr = (fileName.Replace(Path.GetDirectoryName(projectFile), "")).Substring(1, (fileName.Replace(Path.GetDirectoryName(projectFile), "")).Length - 1);
                string fileInclude = string.Format("x:ItemGroup/x:Folder[@Include='{0}']", filestr);
                XmlNode node = doc.DocumentElement.SelectSingleNode(fileInclude, namespaceManager);
                if (node == null)
                {
                    XmlElement Content = doc.CreateElement("Folder", nameSpace);
                    XmlAttribute attrInclude = doc.CreateAttribute("Include");
                    attrInclude.Value = filestr;
                    Content.Attributes.Append(attrInclude);
                    GroupReference.AppendChild(Content);
                }
            }
        }
        /// <summary>
        /// 添加文件夹
        /// </summary>
        /// <param name="folder"></param>
        public  void AddFiles(string folder)
        {
            string path = MapPath(folder);
            string[] filenames = Directory.GetFiles(path);//查找指定目录下文件名
            foreach (string files in filenames)
            {
                string ext = Path.GetExtension(files);
                if (ext == ".cs")
                {
                    ext = files.Replace(path, "");
                }
                AddFile(files,ext.Trim('\\'));
            }
            string[] SubDirectorys = Directory.GetDirectories(path);//查询指定路径下的子目录
            foreach (string SubDirectory in SubDirectorys)
            {
                string[] SunDirectorys = Directory.GetDirectories(SubDirectory);//子目录下是否存在子子目录
                string[] Subfilenames = Directory.GetFiles(SubDirectory);//子目录下是否存在文件
                if (Subfilenames.Length != 0 || SunDirectorys.Length != 0)
                {
                    AddFiles(MPath(SubDirectory));
                }
                else
                {
                    AddFile(SubDirectory, "");
                }
            }
        }
        /// <summary>
        /// 添加单文件
        /// </summary>
        /// <param name="path"></param>
        public void AddSingleFile(string path)
        {
            path =  path.Trim('/');
            if (File.Exists(MapPath("/"+path)))//判断是否存在
            {
                XmlNode GroupReference = doc.DocumentElement.SelectSingleNode("x:ItemGroup[2]", namespaceManager);
                string fileInclude = string.Format("x:ItemGroup/x:Content[@Include='{0}']",path);
                XmlNode node = doc.DocumentElement.SelectSingleNode(fileInclude, namespaceManager);
                if (node == null)//判断是否包含项目中
                {
                    XmlElement Content = doc.CreateElement("Content", nameSpace);
                    XmlAttribute attrInclude = doc.CreateAttribute("Include");
                    attrInclude.Value = path;
                    Content.Attributes.Append(attrInclude);
                    GroupReference.AppendChild(Content);
                }
            }
        }
        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="folder"></param>
        public void AddModule(string folder)
        {
            AddFiles(folder);
            AddSingleFile("Index.html");
            AddSingleFile("IDE.html");
        }
    }
}

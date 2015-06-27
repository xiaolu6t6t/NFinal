using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.DirectoryServices;

namespace AutoConfig
{
    class Config
    {
        public static string appRoot;
        public static string AssemblyTitle;
        public static bool autoGeneration = false;
        public enum IISVersion { 
            IIS6,IIS7,IIS8,Unknown
        }
        public Config(string appRoot)
        {
            Config.appRoot = appRoot;
            string[] fileNames= Directory.GetFiles(appRoot, "*.csproj");
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
        /// 设置config
        /// </summary>
        /// <returns></returns>
        public static IISVersion GetIISVersion()
        {
            DirectoryEntry getEntity = new DirectoryEntry("IIS://localhost/W3SVC/INFO");
            try
            {
                double Version = Convert.ToDouble(getEntity.Properties["MajorIISVersionNumber"].Value);
                Console.WriteLine("IIS:"+Version.ToString());
                if (Version <7)
                {
                    return IISVersion.IIS6;
                }
                if (Version >= 7 && Version <8)
                {
                    return IISVersion.IIS7;
                }
                else if (Version >= 8)
                {
                    return IISVersion.IIS8;
                }
                else
                {
                    return IISVersion.Unknown;
                }
            }
            catch
            {
                return IISVersion.Unknown;
            }
        }
        public void ModifyGlobal(string[] apps)
        {
            string registRouterPattern = @"RouteConfig\s*.\s*RegisterRoutes\s*\(";
            string globalFileName = MapPath("/Global.asax.cs");
            if(File.Exists(globalFileName))
            {
                StreamReader globalReader = new StreamReader(globalFileName);
                string globalString=globalReader.ReadToEnd();
                globalReader.Close();
                Regex globalReg=new Regex(registRouterPattern);
                Match globalMat= globalReg.Match(globalString);
                if(globalMat.Success)
                {
                    string ignoreRoute=string.Empty;
                    for(int i=0;i<apps.Length;i++)
                    {
                        if(globalString.IndexOf(string.Format("\"{0}/{{*pathInfo}}\"",apps[i]))<0)
                        {
                            ignoreRoute+=string.Format("RouteTable.Routes.Add(new Route(\"{0}/{{*pathInfo}}\",new StopRoutingHandler()));\r\n\t\t\t",apps[i]);
                        }
                    }
                    globalString= globalString.Insert(globalMat.Index,ignoreRoute);
                    StreamWriter globalWriter = new StreamWriter(globalFileName);
                    globalWriter.Write(globalString);
                    globalWriter.Close();
                }
            }
        }
        public void InitMain(string[] apps)
        { 
            string fileName=MapPath("/NFinal/Template/Main.tpl");
            if (File.Exists(fileName))
            {
                VTemplate.Engine.TemplateDocument doc = new VTemplate.Engine.TemplateDocument(fileName,System.Text.Encoding.UTF8);
                doc.SetValue("apps",apps);
                doc.SetValue("project",AssemblyTitle);
                doc.RenderTo(MapPath("/NFinal/Main.cs"));
            }
        }
        public void InitRouter(string app)
        {
            string fileName = MapPath("/NFinal/Template/App/Router_Init.tpl");
            if (File.Exists(fileName))
            {
                VTemplate.Engine.TemplateDocument doc = new VTemplate.Engine.TemplateDocument(fileName,System.Text.Encoding.UTF8);
                doc.SetValue("namespace",AssemblyTitle+"."+app);
                doc.SetValue("app",app);
                doc.RenderTo(MapPath("/" + app + "/Router.cs"));
            }
        }

        public void InitExtension(string app)
        {
            string fileName = MapPath("/NFinal/Template/App/Extension.tpl");
            if (File.Exists(fileName))
            {
                VTemplate.Engine.TemplateDocument doc = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                doc.SetValue("namespace",AssemblyTitle+"."+app);
                doc.SetValue("app",app);
                doc.RenderTo(MapPath("/"+app+"/Extension.cs"));
            }
        }
        public string getfileName(string fileName)
        {
            string dir = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return fileName;
        }
        public void InitConfig(string app)
        {
            string fileName = MapPath("/NFinal/Template/App/Config.tpl");
            if (File.Exists(fileName))
            {
                VTemplate.Engine.TemplateDocument doc = new VTemplate.Engine.TemplateDocument(fileName,System.Text.Encoding.UTF8);
                doc.SetValue("namespace", AssemblyTitle + "." + app);
                doc.SetValue("app", app);
                doc.RenderTo(getfileName(MapPath("/" + app + "/Config.cs")));
            }
        }
        public void InitAppData()
        {
            if (!Directory.Exists(MapPath("/App_Data")))
            {
                Directory.CreateDirectory(MapPath("/App_Data"));
            }
            string dir = MapPath("/NFinal/Resource/App_Data");
            if (Directory.Exists(dir))
            {
                CopyDirectory(dir, MapPath("/App_Data"));
            }
        }
        public void InitIndexHTML()
        {
            
            string fileName = MapPath("/NFinal/Template/IDE.html");
            if (!File.Exists(MapPath("/IDE.html")))
            {
                File.Copy(fileName, MapPath("/IDE.html"));
            }
            fileName = MapPath("/NFinal/Template/Index.html");
            if (!File.Exists(MapPath("/Index.html")))
            {
                File.Copy(fileName, MapPath("/NFinal/Template/Index.html"));
            }
        }
        public void InitCommon(string app)
        {
            VTemplate.Engine.TemplateDocument docCommon = null;
            string fileName = MapPath("/NFinal/Template/App/Common/Data/CookieInfo.tpl");
            if (File.Exists(fileName))
            {
                docCommon = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                docCommon.SetValue("project", AssemblyTitle);
                docCommon.SetValue("app", app);
                docCommon.RenderTo(getfileName(MapPath("/" + app + "/Common/Data/CookieInfo.cs")));
            }
            fileName = MapPath("/NFinal/Template/App/Common/Data/CookieManager_Init.tpl");
            if (File.Exists(fileName))
            {
                docCommon = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                docCommon.SetValue("project", AssemblyTitle);
                docCommon.SetValue("app", app);
                docCommon.RenderTo(getfileName(MapPath("/" + app + "/Common/Data/CookieManager.cs")));
            }
            fileName = MapPath("/NFinal/Template/App/Common/Controller.tpl");
            if (File.Exists(fileName))
            {
                docCommon = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                docCommon.SetValue("project", AssemblyTitle);
                docCommon.SetValue("app", app);
                docCommon.RenderTo(getfileName(MapPath("/" + app + "/Common/Controller.cs")));
            }
        }
        public void InitWebCompiler(string app)
        {
            string WebCompiler = "";
            VTemplate.Engine.TemplateDocument docWeb = null;
            WebCompiler = MapPath("/NFinal/Template/App/WebCompiler.aspx.tpl");
            if (File.Exists(WebCompiler))
            {
                docWeb = new VTemplate.Engine.TemplateDocument(WebCompiler, System.Text.Encoding.UTF8);
                docWeb.SetValue("project", AssemblyTitle);
                docWeb.SetValue("app", app);
                docWeb.RenderTo(getfileName(MapPath("/" + app + "/WebCompiler.aspx")));
            }
            WebCompiler = MapPath("/NFinal/Template/App/WebCompiler.aspx.cs.tpl");
            if (File.Exists(WebCompiler))
            {
                docWeb = new VTemplate.Engine.TemplateDocument(WebCompiler, System.Text.Encoding.UTF8);
                docWeb.SetValue("project", AssemblyTitle);
                docWeb.SetValue("app",app);
                docWeb.RenderTo(getfileName(MapPath("/" + app + "/WebCompiler.aspx.cs")));
            }
            WebCompiler = MapPath("/NFinal/Template/App/WebCompiler.aspx.designer.cs.tpl");
            if (File.Exists(WebCompiler))
            {
                docWeb = new VTemplate.Engine.TemplateDocument(WebCompiler, System.Text.Encoding.UTF8);
                docWeb.SetValue("project", AssemblyTitle);
                docWeb.SetValue("app", app);
                docWeb.RenderTo(getfileName(MapPath("/" + app + "/WebCompiler.aspx.designer.cs")));
            }
        }
        public void CreateAppFolders(string app,string[] folders)
        {
            for (int i = 0; i < folders.Length; i++)
            {
                if (!Directory.Exists(MapPath("/"+app+folders[i])))
                {
                    Directory.CreateDirectory(MapPath("/" + app + folders[i]));
                }
            }
        }
        public void InitFolder(string app)
        {
            string[] folders=new string[]{"/Common",
            "/Content","/Controllers","/Models","/Models/BLL","/Models/DAL","/Views","/Web"};
            CreateAppFolders(app, folders);
        }
        public void InitModels(string app)
        {
            string fileName = MapPath("/NFinal/Template/App/Models/Common.cs.tpl");
            if (File.Exists(fileName))
            {
                VTemplate.Engine.TemplateDocument doc = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                doc.SetValue("project",AssemblyTitle);
                doc.SetValue("app",app);
                doc.RenderTo(getfileName(MapPath("/"+app+"/Models/Common.cs")));
            }
            fileName = MapPath("/NFinal/Template/App/Models/ConnectionStrings_Init.cs.tpl");
            if (File.Exists(fileName))
            {
                VTemplate.Engine.TemplateDocument doc = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                doc.SetValue("project", AssemblyTitle);
                doc.SetValue("app", app);
                doc.RenderTo(getfileName(MapPath("/" + app + "/Models/ConnectionStrings.cs")));
            }
        }
        public void InitViews(string app)
        {
            string appWebConfigPath=null;
            //配置Views目录的安全配置
            appWebConfigPath=MapPath("/NFinal/Template/App/Views/Web.config.tpl");
            if (!File.Exists(MapPath("/" + app + "/Views/Web.config")))
            {
                File.Copy(appWebConfigPath, MapPath("/" + app + "/Views/Web.config"));
            }
            string fileName =MapPath( "/NFinal/Template/App/Views/Default/Common/Public/Error.aspx.tpl");
            VTemplate.Engine.TemplateDocument doc = null;
            if (File.Exists(fileName))
            {
                doc = new VTemplate.Engine.TemplateDocument(fileName,System.Text.Encoding.UTF8);
                doc.SetValue("proejct",AssemblyTitle);
                doc.SetValue("app",app);
                doc.RenderTo(getfileName(MapPath("/"+app+"/Views/Default/Common/Public/Error.aspx")));
            }

            fileName =MapPath( "/NFinal/Template/App/Views/Default/Common/Public/Footer.ascx.tpl");
            if (File.Exists(fileName))
            {
                doc = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                doc.SetValue("proejct", AssemblyTitle);
                doc.SetValue("app", app);
                doc.RenderTo(getfileName(MapPath("/" + app + "/Views/Default/Common/Public/Footer.ascx")));
            }

            fileName =MapPath( "/NFinal/Template/App/Views/Default/Common/Public/Header.ascx.tpl");
            if (File.Exists(fileName))
            {
                doc = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                doc.SetValue("proejct", AssemblyTitle);
                doc.SetValue("app", app);
                doc.RenderTo(getfileName(MapPath("/" + app + "/Views/Default/Common/Public/Header.ascx")));
            }

            fileName = MapPath("/NFinal/Template/App/Views/Default/Common/Public/Success.aspx.tpl");
            if (File.Exists(fileName))
            {
                doc = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                doc.SetValue("proejct", AssemblyTitle);
                doc.SetValue("app", app);
                doc.RenderTo(getfileName(MapPath("/" + app + "/Views/Default/Common/Public/Success.aspx")));
            }

            fileName = MapPath("/NFinal/Template/App/Views/Default/IndexController/Index.aspx.tpl");
            if (File.Exists(fileName))
            {
                doc = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                doc.SetValue("proejct", AssemblyTitle);
                doc.SetValue("app", app);
                doc.RenderTo(getfileName(MapPath("/" + app + "/Views/Default/IndexController/Index.aspx")));
            }
        }
        public void InitWeb(string app)
        {
            VTemplate.Engine.TemplateDocument doc = null;
            string fileName = null;
            
            fileName=MapPath("/NFinal/Template/App/Web/Default/Common/Public/Error.cs.tpl");
            if (File.Exists(fileName))
            {
                doc = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                doc.SetValue("project",AssemblyTitle);
                doc.SetValue("app",app);
                doc.RenderTo(getfileName(MapPath("/" + app + "/Web/Default/Common/Public/Error.cs")));
            }
            fileName = MapPath("/NFinal/Template/App/Web/Default/Common/Public/Error.html");
            if (File.Exists(fileName))
            {
                doc = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                doc.SetValue("project", AssemblyTitle);
                doc.SetValue("app", app);
                doc.RenderTo(getfileName(MapPath("/" + app + "/Web/Default/Common/Public/Error.html")));
            }
            fileName = MapPath("/NFinal/Template/App/Web/Default/Common/Public/Success.cs.tpl");
            if (File.Exists(fileName))
            {
                doc = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                doc.SetValue("project", AssemblyTitle);
                doc.SetValue("app", app);
                doc.RenderTo(getfileName(MapPath("/" + app + "/Web/Default/Common/Public/Success.cs")));
            }
            fileName = MapPath("/NFinal/Template/App/Web/Default/Common/Public/Success.html");
            if (File.Exists(fileName))
            {
                doc = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                doc.SetValue("project", AssemblyTitle);
                doc.SetValue("app", app);
                doc.RenderTo(getfileName(MapPath("/" + app + "/Web/Default/Common/Public/Success.html")));
            }
            fileName = MapPath("/NFinal/Template/App/Web/Default/IndexController/Index.cs.tpl");
            if (File.Exists(fileName))
            {
                doc = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                doc.SetValue("project", AssemblyTitle);
                doc.SetValue("app", app);
                doc.RenderTo(getfileName(MapPath("/" + app + "/Web/Default/IndexController/Index.cs")));
            }
            fileName = MapPath("/NFinal/Template/App/Web/Default/IndexController/Index.html");
            if (File.Exists(fileName))
            {
                doc = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                doc.SetValue("project", AssemblyTitle);
                doc.SetValue("app", app);
                doc.RenderTo(getfileName(MapPath("/" + app + "/Web/Default/IndexController/Index.html")));
            }
        }
        void CopyDirectory(string SourcePath,string DestinationPath)
        {
　　        //创建所有目录
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
            }
　　        //复制所有文件
           foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
           {
               if (!File.Exists(newPath.Replace(SourcePath, DestinationPath)))
               {
                   File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath));
               }
           }
        }
        public void InitContent(string app)
        {
            CopyDirectory(MapPath("/NFinal/Content"),MapPath("/"+app+"/Content"));
        }
        public void InitControllers(string app)
        {
            string fileName = MapPath("/NFinal/Template/App/Controllers/Common/Public.cs.tpl");
            if (File.Exists(fileName))
            {
                VTemplate.Engine.TemplateDocument doc = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                doc.SetValue("project",AssemblyTitle);
                doc.SetValue("app",app);
                doc.RenderTo(getfileName(MapPath("/"+app+"/Controllers/Common/Public.cs")));
            }
            fileName = MapPath("/NFinal/Template/App/Controllers/IndexController.cs.tpl");
            if (File.Exists(fileName))
            {
                VTemplate.Engine.TemplateDocument doc = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                doc.SetValue("project", AssemblyTitle);
                doc.SetValue("app", app);
                doc.RenderTo(getfileName(MapPath("/" + app + "/Controllers/IndexController.cs")));
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
        public static void SetAutoCreateConfig(bool autoGen)
        {
            string projectFileName = MapPath("/" + AssemblyTitle + ".csproj");
            bool changed = false;

            if (File.Exists(projectFileName))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(projectFileName);
                XmlNamespaceManager xmanager = new XmlNamespaceManager(doc.NameTable);
                string nameSpace = doc.DocumentElement.Attributes["xmlns"].Value;
                xmanager.AddNamespace("x", nameSpace);

                XmlNode nodePostBuildEvent = doc.SelectSingleNode("/x:Project/x:PropertyGroup/x:PostBuildEvent", xmanager);
                if (autoGen)
                {
                    if (nodePostBuildEvent == null)
                    {
                        changed = true;
                        nodePostBuildEvent = doc.CreateElement("PostBuildEvent", nameSpace);
                        nodePostBuildEvent.InnerText = "$(ProjectDir)NFinal\\WebCompiler.exe";
                        XmlNode nodePropertyGroup = doc.CreateElement("PropertyGroup", nameSpace);
                        nodePropertyGroup.AppendChild(nodePostBuildEvent);
                        doc.DocumentElement.AppendChild(nodePropertyGroup);
                    }
                    else
                    {
                        if (nodePostBuildEvent.InnerText == string.Empty)
                        {
                            changed = true;
                            nodePostBuildEvent.InnerText = "$(ProjectDir)NFinal\\WebCompiler.exe";
                        }
                    }
                }
                else
                {
                    if (nodePostBuildEvent != null)
                    {
                        changed = true;
                        nodePostBuildEvent.ParentNode.RemoveChild(nodePostBuildEvent);
                    }
                }
                if (changed)
                {
                    doc.Save(projectFileName);
                }
            }
        }

        public static XmlNode CreateXMLPath(XmlDocument doc, string root)
        {
            string[] pathes = root.Trim('/').Split('/');
            string oldXpath = "", newXpath = "";
            if (pathes.Length > 1)
            {
                newXpath = pathes[0];
                XmlNode newNode = null, oldNode = null;
                for (int i = 1; i < pathes.Length; i++)
                {
                    oldXpath = newXpath;
                    newXpath += "/" + pathes[i];
                    newNode = doc.SelectSingleNode(newXpath);

                    oldNode = doc.SelectSingleNode(oldXpath);
                    if (newNode == null)
                    {
                        newNode = doc.CreateElement(pathes[i]);
                        oldNode.AppendChild(newNode);
                    }
                }
                return newNode;
            }
            return doc.DocumentElement;
        }
        public static void SetWebApp(string App)
        { 
            
        }


        //自动配置WebConfig
        public static string[] SetWebConfig(IISVersion version, string appRoot,string AppName)
        {
            Config.appRoot = appRoot;
            Config.AssemblyTitle = new DirectoryInfo(Config.appRoot).Name;
            //文档是否变动
            bool changed = false;
            
            //配置.net信息
            string webConfig = MapPath("/Web.config");
            if (!File.Exists(webConfig))
            {
                changed = true;
                File.Copy(MapPath("/NFinal/Template/Web.config.tpl"), MapPath("/Web.config"));
                File.Copy(MapPath("/NFinal/Template/Web.config.Debug.tpl"), MapPath("/Web.Debug.config"));
                File.Copy(MapPath("/NFinal/Template/Web.config.Release.tpl"), MapPath("/Web.Release.config"));
            }
            //根据环境配置webconfig
            System.Xml.XmlDocument doc = new XmlDocument();
            doc.Load(webConfig);
            XmlNode root = doc.DocumentElement;
            if (root == null)
            {
                return null;
            }
            //首选注释掉ConnectionString连接,以免连接字符串不正确而导致生成失败
            XmlNode connsNode = doc.DocumentElement.SelectSingleNode("connectionStrings/add[@name='Common']");
            XmlNode connsNodeRoot = doc.DocumentElement.SelectSingleNode("connectionStrings");
            //如果通用数据库没有添加,则注释掉原有的连接,建立通用连接
            if (connsNode == null)
            {
                changed = true;
                if (connsNodeRoot == null)
                {
                    connsNodeRoot= doc.CreateElement("connectionStrings");
                    doc.DocumentElement.AppendChild(connsNodeRoot);
                }
                connsNodeRoot.InnerXml = "<!--" + connsNodeRoot.InnerXml + "-->"
                +"<add name=\"Common\" connectionString=\"Data Source=|DataDirectory|\\Common.db;Pooling=true;FailIfMissing=false\" providerName=\"System.Data.SQLite\"/>";
            }
            //读取Apps中的值
            if(string.IsNullOrEmpty(AppName))
            {
                AppName="App";
            }
            XmlNode appsNode = doc.DocumentElement.SelectSingleNode("appSettings/add[@key='Apps']");
            string[] Apps = null;
            List<string> AppsList = new List<string>();
            if (appsNode != null && appsNode.Attributes.Count > 0 && appsNode.Attributes["value"] != null)
            {
                if (appsNode.Attributes["value"].Value.Trim() != string.Empty)
                {
                    Apps = appsNode.Attributes["value"].Value.Split(',');
                    for (int i = 0; i < Apps.Length; i++)
                    {
                        if (Directory.Exists(MapPath("/" + Apps[i])))
                        {
                            AppsList.Add(Apps[i]);
                        }
                    }
                    bool hasAppName = false;
                    for (int i = 0; i < Apps.Length; i++)
                    {
                        if (AppName == Apps[i])
                        {
                            hasAppName = true;
                        }
                    }
                    if (!hasAppName)
                    {
                        changed = true;
                        AppsList.Add(AppName);
                        Apps = AppsList.ToArray();
                        appsNode.Attributes["value"].Value = String.Join(",", Apps);
                    }
                }
                else
                {
                    changed = true;
                    Apps =new string[]{ AppName};
                    appsNode.Attributes["value"].Value = AppName;
                    AppsList.Add(AppName);
                    Apps = AppsList.ToArray();
                    appsNode.Attributes["value"].Value = String.Join(",", Apps);
                }
            }
            else
            {
                changed = true;
                XmlNode appSettingNode = CreateXMLPath(doc, "configuration/appSettings");
                appsNode = doc.CreateElement("add");
                XmlAttribute attrKey = doc.CreateAttribute("key");
                attrKey.Value = "Apps";
                XmlAttribute attrValue = doc.CreateAttribute("value");
                attrValue.Value = AppName;
                appsNode.Attributes.RemoveAll();
                appsNode.Attributes.Append(attrKey);
                appsNode.Attributes.Append(attrValue);
                appSettingNode.AppendChild(appsNode);
                Apps = new string[] { AppName };
                AppsList.Add(AppName);
                Apps = AppsList.ToArray();
                appsNode.Attributes["value"].Value = String.Join(",", Apps);
            }
            //读取是否自动生成
            XmlNode autoGenerationNode = doc.DocumentElement.SelectSingleNode("appSettings/add[@key='AutoGeneration']");
            bool autoGeneration = false;
            if (autoGenerationNode != null)
            {
                autoGeneration = autoGenerationNode.Attributes["value"].Value.ToLower() == "true";
            }
            else
            {
                changed = true;
                XmlNode appSettingNode = CreateXMLPath(doc, "configuration/appSettings");
                appsNode = doc.CreateElement("add");
                XmlAttribute attrKey = doc.CreateAttribute("key");
                attrKey.Value = "AutoGeneration";
                XmlAttribute attrValue = doc.CreateAttribute("value");
                attrValue.Value = "false";
                appsNode.Attributes.RemoveAll();
                appsNode.Attributes.Append(attrKey);
                appsNode.Attributes.Append(attrValue);
                appSettingNode.AppendChild(appsNode);
                autoGeneration = false;
            }
            Config.autoGeneration = autoGeneration;
            SetAutoCreateConfig(autoGeneration);
            //创建URL重写的主页文件
            if (!File.Exists(MapPath("/Index.html")))
            {
                VTemplate.Engine.TemplateDocument docIndex = new VTemplate.Engine.TemplateDocument(MapPath("/NFinal/Template/Index.html"), System.Text.Encoding.UTF8);
                docIndex.SetValue("App", Apps[0]);
                docIndex.RenderTo(MapPath("/Index.html"));
            }
            //删除UrlRewriter
            string urlRewriterFileName = MapPath("/bin/URLRewriter.dll");
            if (File.Exists(urlRewriterFileName))
            {
                File.Delete(urlRewriterFileName);
            }
            //添加UrlRewriter
            XmlNode nodeConfigSection = doc.DocumentElement.SelectSingleNode("configSections/section[@name='RewriterConfig']");
            if (nodeConfigSection == null)
            {
                changed = true;

                XmlNode nodeSection = doc.CreateElement("section");
                XmlAttribute attrName = doc.CreateAttribute("name");
                attrName.Value = "RewriterConfig";
                XmlAttribute attrType = doc.CreateAttribute("type");
                attrType.Value = "NFinal.URLRewriter.Config.RewriterConfigSerializerSectionHandler";
                nodeSection.Attributes.Append(attrName);
                nodeSection.Attributes.Append(attrType);

                XmlNode nodeConfigSections = doc.DocumentElement.SelectSingleNode("configSections");

                if (nodeConfigSections == null)
                {
                    nodeConfigSections = doc.CreateElement("configSections");
                    doc.DocumentElement.InsertBefore(nodeConfigSections, doc.DocumentElement.FirstChild);
                }
                nodeConfigSections.AppendChild(nodeSection);
            }

            XmlNode nodeRewriteSection = doc.DocumentElement.SelectSingleNode("configSections/section[@name='RewriterConfig']");
            //修正属性
            if (nodeRewriteSection.Attributes["type"].Value.IndexOf(",") > -1)
            {
                changed = true;
                nodeRewriteSection.Attributes["type"].Value = "NFinal.URLRewriter.Config.RewriterConfigSerializerSectionHandler";
            }

            XmlNode nodeRewriterConfig = doc.DocumentElement.SelectSingleNode("RewriterConfig");
            if (nodeRewriterConfig == null)
            {
                changed = true;
                XmlNode nodeRules = CreateXMLPath(doc, "configuration/RewriterConfig/Rules");
                XmlNode nodeRewriterRule = doc.CreateElement("RewriterRule");
                XmlNode nodeLookFor = doc.CreateElement("LookFor");
                nodeLookFor.InnerText = "~/Index.html";
                XmlNode nodeSendTo = doc.CreateElement("SendTo");
                nodeSendTo.InnerText = string.Format("~/{0}/IndexController/Index.htm",AppName);
                nodeRewriterRule.AppendChild(nodeLookFor);
                nodeRewriterRule.AppendChild(nodeSendTo);
                nodeRules.AppendChild(nodeRewriterRule);
            }
            else
            {
                XmlNodeList nodeRewriterRules = doc.DocumentElement.SelectNodes("RewriterConfig/Rules/RewriterRule");
                if (nodeRewriterRules != null)
                {
                    for (int i = 0; i < nodeRewriterRules.Count; i++)
                    {
                        XmlNode nodeLookFor= nodeRewriterRules[i].SelectSingleNode("LookFor");
                        if (nodeLookFor != null && nodeLookFor.InnerText == "~/Index.html")
                        {
                            XmlNode nodeSendTo = nodeRewriterRules[i].SelectSingleNode("SendTo");
                            if (nodeSendTo != null)
                            {
                                changed = true;
                                nodeSendTo.InnerText = string.Format("~/{0}/IndexController/Index.htm", AppName);
                            }
                        }
                    }
                }
            }
            //如果是IIS7,IIS8新版本,要添加system.webserver配置节
            if (version == IISVersion.IIS7 || version == IISVersion.IIS8)
            {
                //取消验证
                XmlNode newValidationNode = doc.DocumentElement.SelectSingleNode("system.webServer/validation");
                if (newValidationNode == null)
                {
                    newValidationNode = CreateXMLPath(doc, "configuration/system.webServer/validation");
                }
                newValidationNode = doc.DocumentElement.SelectSingleNode("system.webServer/validation");
                if (newValidationNode.Attributes["validateIntegratedModeConfiguration"] != null)
                {
                    newValidationNode.Attributes["validateIntegratedModeConfiguration"].Value = "false";
                }
                else
                {
                    XmlAttribute validateIntegratedModeConfigurationAttr= doc.CreateAttribute("validateIntegratedModeConfiguration");
                    validateIntegratedModeConfigurationAttr.Value = "false";
                    newValidationNode.Attributes.Append(validateIntegratedModeConfigurationAttr);
                }                
                //添加NFinal节点属性
                XmlNode newFactoryNode = doc.DocumentElement.SelectSingleNode("system.webServer/handlers/add[@type='NFinal.Handler.HandlerFactory']");
                //不存在则添加
                if (newFactoryNode == null)
                {
                    changed = true;
                    XmlNode handlersNode = CreateXMLPath(doc, "configuration/system.webServer/handlers");
                    XmlNode NFinalHandlerNode = doc.CreateElement("add");
                    XmlAttribute attrName = doc.CreateAttribute("name");
                    attrName.Value = "NFinalHandlerFactory";
                    XmlAttribute attrVerb = doc.CreateAttribute("verb");
                    attrVerb.Value = "*";
                    XmlAttribute attrPath = doc.CreateAttribute("path");
                    attrPath.Value = "*.htm";
                    XmlAttribute attrType = doc.CreateAttribute("type");
                    attrType.Value = "NFinal.Handler.HandlerFactory";
                    XmlAttribute attrPreCondition = doc.CreateAttribute("preCondition");
                    attrPreCondition.Value = "integratedMode";
                    NFinalHandlerNode.Attributes.RemoveAll();
                    NFinalHandlerNode.Attributes.Append(attrName);
                    NFinalHandlerNode.Attributes.Append(attrVerb);
                    NFinalHandlerNode.Attributes.Append(attrPath);
                    NFinalHandlerNode.Attributes.Append(attrType);
                    NFinalHandlerNode.Attributes.Append(attrPreCondition);
                    handlersNode.AppendChild(NFinalHandlerNode);
                }
                //添加Rewriter节点属性
                XmlNode newRewriterNode = doc.DocumentElement.SelectSingleNode("system.webServer/modules/add[@name='ModuleRewriter']");
                if (newRewriterNode == null)
                {
                    changed = true;
                    XmlNode modulesNode = CreateXMLPath(doc, "configuration/system.webServer/modules");
                    XmlNode urlRewriterNode = doc.CreateElement("add");
                    XmlAttribute attrName = doc.CreateAttribute("name");
                    attrName.Value = "ModuleRewriter";
                    XmlAttribute attrType = doc.CreateAttribute("type");
                    attrType.Value = "NFinal.URLRewriter.ModuleRewriter";
                    urlRewriterNode.Attributes.Append(attrName);
                    urlRewriterNode.Attributes.Append(attrType);
                    modulesNode.AppendChild(urlRewriterNode);
                }
                XmlNode nodeModules = doc.DocumentElement.SelectSingleNode("system.webServer/modules/add[@name='ModuleRewriter']");
                if (nodeModules.Attributes["type"] != null)
                {
                    if (nodeModules.Attributes["type"].Value.IndexOf(",") > -1)
                    {
                        changed = true;
                        nodeModules.Attributes["type"].Value = "NFinal.URLRewriter.ModuleRewriter";
                    }
                }
            }
            //IIS8下要删除老的配置节点
            if(version==IISVersion.IIS8)
            {
                XmlNode FactoryNode = doc.DocumentElement.SelectSingleNode("system.web/httpHandlers");
                if (FactoryNode != null)
                {
                    FactoryNode.ParentNode.RemoveChild(FactoryNode);
                }
                XmlNode RewriterNode = doc.DocumentElement.SelectSingleNode("system.web/httpModules");
                if (RewriterNode != null)
                {
                    RewriterNode.ParentNode.RemoveChild(RewriterNode);
                }
            }
            //IS6,IIS7下的system.web配置
            if (version == IISVersion.IIS6 || version == IISVersion.IIS7)
            {
                //添加NFinal节点属性
                XmlNode FactoryNode = doc.DocumentElement.SelectSingleNode("system.web/httpHandlers/add[@type='NFinal.Handler.HandlerFactory']");
                //不存在则添加
                if (FactoryNode == null)
                {
                    changed = true;
                    XmlNode handlersNode = CreateXMLPath(doc, "configuration/system.web/httpHandlers");
                    XmlNode NFinalHandlerNode = doc.CreateElement("add");
                    XmlAttribute attrVerb = doc.CreateAttribute("verb");
                    attrVerb.Value = "*";
                    XmlAttribute attrPath = doc.CreateAttribute("path");
                    attrPath.Value = "*.htm";
                    XmlAttribute attrType = doc.CreateAttribute("type");
                    attrType.Value = "NFinal.Handler.HandlerFactory";
                    NFinalHandlerNode.Attributes.RemoveAll();
                    NFinalHandlerNode.Attributes.Append(attrVerb);
                    NFinalHandlerNode.Attributes.Append(attrPath);
                    NFinalHandlerNode.Attributes.Append(attrType);
                    handlersNode.AppendChild(NFinalHandlerNode);
                }

                //添加Rewriter节点属性
                XmlNode RewriterNode = doc.DocumentElement.SelectSingleNode("system.web/httpModules/add[@type='NFinal.URLRewriter.ModuleRewriter']");
                if (RewriterNode == null)
                {
                    changed = true;
                    XmlNode modulesNode = CreateXMLPath(doc, "configuration/system.web/httpModules");
                    XmlNode urlRewriterNode = doc.CreateElement("add");
                    XmlAttribute attrName = doc.CreateAttribute("name");
                    attrName.Value = "ModuleRewriter";
                    XmlAttribute attrType = doc.CreateAttribute("type");
                    attrType.Value = "NFinal.URLRewriter.ModuleRewriter";
                    urlRewriterNode.Attributes.Append(attrName);
                    urlRewriterNode.Attributes.Append(attrType);
                    modulesNode.AppendChild(urlRewriterNode);
                }
                //防止使用Rewriter的项目已经使用了别的URL重写模块
                XmlNode rewriterModuleNode = doc.DocumentElement.SelectSingleNode("system.web/httpModules/add[@name='ModuleRewriter']");
                if (rewriterModuleNode.Attributes["type"] != null)
                {
                    if (rewriterModuleNode.Attributes["type"].Value.IndexOf(",") > -1)
                    {
                        changed = true;
                        rewriterModuleNode.Attributes["type"].Value = "NFinal.URLRewriter.ModuleRewriter";
                    }
                }
            }
            //IIS6下要删除新的system.webServer的配置
            if (version == IISVersion.IIS6)
            {
                XmlNode newWebServerNode = doc.DocumentElement.SelectSingleNode("system.webServer");
                if (newWebServerNode != null)
                {
                    newWebServerNode.ParentNode.RemoveChild(newWebServerNode);
                }
            }
            //如果文档变动就保存
            if (changed)
            {
                doc.Save(webConfig);
            }
            return Apps;
        }
    }
}

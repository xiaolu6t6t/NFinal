using System;
using System.Collections.Generic;
using System.Text;
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
        public void CheckConnectionString()
        { 
           
        }
        /// <summary>
        /// 设置config
        /// </summary>
        /// <returns></returns>
        public static bool IsNewIIS()
        {
            DirectoryEntry getEntity = new DirectoryEntry("IIS://localhost/W3SVC/INFO");
            try
            {
                double Version = Convert.ToDouble(getEntity.Properties["MajorIISVersionNumber"].Value);
                if (Version >= 7)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return true;
            }
        }
        public void InitMain()
        { 
            string fileName=MapPath("/NFinal/Template/Main_Init.tpl");
            if (File.Exists(fileName))
            {
                File.Copy(fileName, MapPath("/NFinal/Main.cs"),true);
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
        //自动配置WebConfig
        public static void SetWebConfig(bool isNewIIS, string appRoot)
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
                return;
            }
            //首选注释掉ConnectionString连接,以免连接字符串不正确而导致生成失败
            XmlNode connsNode = doc.DocumentElement.SelectSingleNode("connectionStrings/add[@name]");
            XmlNode connsNodeRoot = doc.DocumentElement.SelectSingleNode("connectionStrings");
            if (connsNode != null)
            {
                changed = true;
                connsNodeRoot.InnerXml = "<!--" + connsNodeRoot.InnerXml + "-->";
            }
            //读取Apps中的值
            XmlNode appsNode = doc.DocumentElement.SelectSingleNode("appSettings/add[@key='Apps']");
            string[] Apps = null;

            if (appsNode != null && appsNode.Attributes.Count > 0 && appsNode.Attributes["value"] != null)
            {
                Apps = appsNode.Attributes["value"].Value.Split(',');
            }
            else
            {
                changed = true;
                XmlNode appSettingNode = CreateXMLPath(doc, "configuration/appSettings");
                appsNode = doc.CreateElement("add");
                XmlAttribute attrKey = doc.CreateAttribute("key");
                attrKey.Value = "Apps";
                XmlAttribute attrValue = doc.CreateAttribute("value");
                attrValue.Value = "App";
                appsNode.Attributes.RemoveAll();
                appsNode.Attributes.Append(attrKey);
                appsNode.Attributes.Append(attrValue);
                appSettingNode.AppendChild(appsNode);
                Apps = new string[] { "App" };
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
                nodeSendTo.InnerText = string.Format("~/{0}/IndexController/Index.htm", Apps[0]);
                nodeRewriterRule.AppendChild(nodeLookFor);
                nodeRewriterRule.AppendChild(nodeSendTo);
                nodeRules.AppendChild(nodeRewriterRule);
            }
            //如果是IIS7以上版本
            if (isNewIIS)
            {
                //如果是旧版本,则注释掉旧版本的节点
                XmlNode oldConfigSection = doc.DocumentElement.SelectSingleNode("system.web");
                if (oldConfigSection != null)
                {
                    changed = true;
                    root.RemoveChild(oldConfigSection);
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
            //如果是IIS7以下版本
            else
            {
                XmlNode newConfigSection = doc.DocumentElement.SelectSingleNode("system.webServer");

                if (newConfigSection != null)
                {
                    changed = true;
                    root.RemoveChild(newConfigSection);
                }
                //添加NFinal节点属性
                XmlNode newFactoryNode = doc.DocumentElement.SelectSingleNode("system.web/httpHandlers/add[@type='NFinal.Handler.HandlerFactory']");
                //不存在则添加
                if (newFactoryNode == null)
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
                XmlNode newRewriterNode = doc.DocumentElement.SelectSingleNode("system.web/httpModules/add[@type='NFinal.URLRewriter.ModuleRewriter']");
                if (newRewriterNode == null)
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
                XmlNode nodeModules = doc.DocumentElement.SelectSingleNode("system.web/httpModules/add[@name='ModuleRewriter']");
                if (nodeModules.Attributes["type"] != null)
                {
                    if (nodeModules.Attributes["type"].Value.IndexOf(",") > -1)
                    {
                        changed = true;
                        nodeModules.Attributes["type"].Value = "NFinal.URLRewriter.ModuleRewriter";
                    }
                }
            }
            //如果文档变动就保存
            if (changed)
            {
                doc.Save(webConfig);
            }
        }
    }
}

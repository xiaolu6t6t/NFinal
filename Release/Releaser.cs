using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Release
{
    class Releaser
    {
        public static string appRoot;
        public static string AssemblyTitle;
        public static bool autoGeneration = false;
        public Releaser(string appRoot)
        {
            Releaser.appRoot = appRoot;
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
        /// 把基于网站根目录的绝对路径改为相对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MapPath(string url)
        {
            return appRoot + url.Trim('/').Replace('/', '\\');
        }
        public static string[] GetWebConfig()
        {
            string webConfigFileName = MapPath("/Web.config");
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
                Apps = new string[] { "App" };
            }
            return Apps;
        }
        public void Copy(string[] webFileNames,string appRoot, string folder)
        {
            if (webFileNames.Length > 0)
            {
                string[] webShortNames = new string[webFileNames.Length];
                string[] destFileNames = new string[webShortNames.Length];
                string tmpFolder;
                for (int j = 0; j < webFileNames.Length; j++)
                {
                    webShortNames[j] = webFileNames[j].Substring(appRoot.Length - 1);
                    destFileNames[j] = appRoot + "Release" + webShortNames[j];
                    tmpFolder = Path.GetDirectoryName(destFileNames[j]);
                    if (!Directory.Exists(tmpFolder))
                    {
                        Directory.CreateDirectory(tmpFolder);
                    }
                    File.Copy(webFileNames[j], destFileNames[j], true);
                }
            }
        }
        public void Main()
        {
            string folder = "Release";
            string[] Apps = GetWebConfig();
            //复制项目
            for (int i = 0; i < Apps.Length; i++)
            {
                string appDir=appRoot+Apps[i]+"\\Web\\";
                string[] webFileNames = Directory.GetFiles(appDir, "*.cs", SearchOption.AllDirectories);
                Copy(webFileNames, appRoot, folder);
                string[] classFileNames = Directory.GetFiles(appRoot + Apps[i] + "\\Common\\","*.cs", SearchOption.AllDirectories);
                Copy(classFileNames, appRoot, folder);
                string[] conentFileNames = Directory.GetFiles(appRoot + Apps[i] + "\\Content\\", "*.*", SearchOption.AllDirectories);
                Copy(conentFileNames, appRoot, folder);
                string[] appFiles = new string[] { 
                    appRoot+Apps[i]+"\\ConnectionString.cs",
                    appRoot+Apps[i]+"\\Router.cs"
                };
                Copy(appFiles,appRoot,folder);
                //string[] dbFileNames = Directory.GetFiles(appRoot + Apps[i] + "\\DB\\", "*.cs", SearchOption.AllDirectories);
                //Copy(dbFileNames, appRoot, folder);
            }
            //复制NFinal
            string[] nfDbFileNames = new string[]{
                //appRoot + "NFinal\\DB\\Connection.cs",
                //appRoot+"NFinal\\DB\\DBBase.cs",
                //appRoot+"NFinal\\DB\\DBObject.cs",
                appRoot+"NFinal\\DB\\SqlObject.cs",
                //appRoot+"NFinal\\DB\\Transation.cs",
                appRoot+"NFinal\\Handler\\Handler.cs",
                appRoot+"NFinal\\Handler\\HandlerFactory .cs",
                appRoot+"NFinal\\Handler\\HttpAsyncHandler.cs",
                appRoot+"NFinal\\BaseAction.cs",
                appRoot+"NFinal\\Main.cs"
            };
            Copy(nfDbFileNames, appRoot, folder);
            //litJson
            string[] litJsonFileNames = Directory.GetFiles(appRoot + "\\NFinal\\LitJson\\", "*.cs", SearchOption.AllDirectories);
            Copy(litJsonFileNames, appRoot, folder);
            //urlRewriter
            string[] urlRewriterFileNames = Directory.GetFiles(appRoot + "\\NFinal\\UrlRewriter\\", "*.cs", SearchOption.AllDirectories);
            Copy(urlRewriterFileNames, appRoot, folder);
            //Vtemplate
            string[] vtemplateFileNames = Directory.GetFiles(appRoot + "\\NFinal\\VTemplate\\", "*.cs", SearchOption.AllDirectories);
            Copy(vtemplateFileNames, appRoot, folder);
        }
    }
}

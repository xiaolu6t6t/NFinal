using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Collections.Specialized;

namespace NFinal
{
    //配置文件
    public class Config
    {
        public string APP = "/";
        public string Controller = "Controllers/";
        public string Common = "Common/";
        public string Views = "Views/";
        public string Models = "Models/";
        public string BLL = "BLL/";
        public string DAL = "DAL/";
        public string Web = "Web/";
        public string Content = "Content/";
        public string ContentJS = "js/";
        public string ContentCss = "css/";
        public string ContentImages = "images/";
        public string defaultStyle = "Default/";
        public bool AutoGeneration = false;
        public bool IsDebug = false;
        private bool hasInit = false;
        public string[] TemplateParseString = new string[] { 
            "__APP__","/App",
            "__CONTENT__","/App/Content",
            "__JS__","/App/Content/js",
            "__CSS__","/App/Content/css",
            "__IMAGE__","/App/Content/images",
            "__VIEWS__","/App/Default"
        };
        public class ConnectionString
        {
        }
        //控制器的后缀名
        public string ControllerSuffix = "Controller";

        public Config()
        { }
        //配置初始化
        public void Init(string appName)
        {
            if (!hasInit)
            {
                hasInit = true;
                if (string.IsNullOrEmpty(appName))
                {
                    APP = "/";
                }
                else
                {
                    APP = "/" + appName.Trim(new char[] { '/' }) + "/";
                }
                Controller = APP + Controller;
                Common = APP + Common;
                Views = APP + Views;
                Models = APP + Models;
                BLL = Models + BLL;
                DAL = Models + DAL;
                Web = APP + Web;
                Content = APP + Content;
                ContentCss = Content + ContentCss;
                ContentJS = Content + ContentJS;
                ContentImages = Content + ContentImages;
            }
        }
        //获取命名空间
        public string GetFullName(string projectName,string name)
        {
            name = projectName + name;
            return name.Trim('/').Replace('/','.');
        }
        public string ChangeControllerName(string projectName, string controllerFullName)
        {
            string controllerName = Frame.AssemblyTitle + Controller;
            return projectName + controllerFullName.Substring(controllerName.Length,
                controllerFullName.Length - controllerName.Length);
        }
        public string ChangeBLLName(string projectName, string BLLFullName)
        {
            string BLLName = (Frame.AssemblyTitle + BLL).TrimEnd('/');
            return projectName + BLLFullName.Substring(BLLName.Length,BLLFullName.Length-BLLName.Length);
        }
        public static string MapPath(string url)
        {
            return Frame.MapPath(url);
        }
       
    }
}
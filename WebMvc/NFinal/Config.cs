using System;
using System.Collections.Generic;
using System.Web;
using System.Reflection;
using System.IO;
using System.Xml;

namespace NFinal
{
    //配置文件
    public class Config
    {
        public string APP = "/";
        public string Controller = "Controllers/";
        public string Common = "Common/";
        public string Views = "Views/";
        public string BLL = "BLL/";
        public string DAL = "DAL/";
        public string Web = "Web/";
        public string Content = "Content/";
        public string ContentJS = "js/";
        public string ContentCss = "css/";
        public string ContentImages = "images/";
        public string defaultStyle = "Default/";
        public string DB = "DB/";
 
        public bool IsDebug = false;
        //控制器的后缀名
        public string ControllerSuffix = "Controller";
        public static List<NFinal.DB.ConnectionString> ConnectionStrings = new List<NFinal.DB.ConnectionString>();




        //配置初始化
        public Config(string appName)
        {
            
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
            BLL = APP + BLL;
            DAL = APP + DAL;
            Web = APP + Web;
            Content = APP + Content;
            ContentCss = Content + ContentCss;
            ContentJS = Content + ContentJS;
            ContentImages = Content + ContentImages;
            DB = APP + DB;
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
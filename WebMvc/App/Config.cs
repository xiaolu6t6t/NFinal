using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App
{
    public class Config
    {
        public static NFinal.Config set = new NFinal.Config();
        public Config()
        {
            set.Controller = "Controllers/";
            set.Common = "Common/";
            set.Views = "Views/";
            set.Models = "Models/";
            set.BLL = "BLL/";
            set.DAL = "DAL/";
            set.Web = "Web/";
            set.Content = "Content/";
            set.ContentJS = "js/";
            set.ContentCss = "css/";
            set.ContentImages = "images/";
            set.defaultStyle = "Default/";
            set.ControllerSuffix = "Controller";
            set.AutoGeneration = true;
            set.IsDebug = false;
            set.TemplateParseString = new string[] {
                "__APP__","/App",
                "__CONTENT__","/App/Content",
                "__JS__","/App/Content/js",
                "__CSS__","/App/Content/css",
                "__IMAGE__","/App/Content/images",
                "__VIEWS__","/App/Default"
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Web;

namespace {$namespace}
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
                "__APP__","/{$app}",
                "__CONTENT__","/{$app}/Content",
                "__JS__","/{$app}/Content/js",
                "__CSS__","/{$app}/Content/css",
                "__IMAGE__","/{$app}/Content/images",
                "__VIEWS__","/{$app}/Default"
            };
        }
    }
}
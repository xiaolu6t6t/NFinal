using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CompileConfig
{
    class FilePath
    {
        public string appRoot;
        public FilePath()
        {
            string refrerencePath= Microsoft.Build.Utilities.ToolLocationHelper.GetPathToDotNetFrameworkReferenceAssemblies(Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.Version40);
            this.appRoot =new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName + "\\";
        }
        public string MapPath(string url)
        {
            return appRoot + url.Trim('/').Replace('/','\\');
        }
    }
}

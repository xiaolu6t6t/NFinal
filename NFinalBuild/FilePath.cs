using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NFinalBuild
{
    class FilePath
    {
        public string appRoot;
        public FilePath()
        {
            this.appRoot =new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName + "\\";
        }
        public string MapPath(string url)
        {
            return appRoot + url.Trim('/').Replace('/','\\');
        }
    }
}

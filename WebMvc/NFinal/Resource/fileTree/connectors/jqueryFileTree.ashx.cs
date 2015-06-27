using System;
using System.Collections.Generic;
using System.Web;

namespace NFinal.Resource.fileTree.connectors
{
    /// <summary>
    /// jqueryFileTree 的摘要说明
    /// </summary>
    public class jqueryFileTree : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string dir;
            if (context.Request.Form["dir"] == null || context.Request.Form["dir"].Length <= 0)
            {
                dir = "/";
            }
            else
            {
                dir = context.Server.UrlDecode(context.Request.Form["dir"]);
            }
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(context.Server.MapPath(dir));
            context.Response.Write("<ul class=\"jqueryFileTree\" style=\"display: none;\">\n");
            foreach (System.IO.DirectoryInfo di_child in di.GetDirectories())
            {
                context.Response.Write("\t<li class=\"directory collapsed\"><a href=\"#\" rel=\"" + dir + di_child.Name + "/\">" + di_child.Name + "</a></li>\n");
            }
            foreach (System.IO.FileInfo fi in di.GetFiles())
            {
                string ext = "";
                if (fi.Extension.Length > 1)
                {
                    ext = fi.Extension.Substring(1).ToLower();
                }
                context.Response.Write("\t<li class=\"file ext_" + ext + "\"><a href=\"#\" rel=\"" + dir + fi.Name + "\">" + fi.Name + "</a></li>\n");
            }
            context.Response.Write("</ul>");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
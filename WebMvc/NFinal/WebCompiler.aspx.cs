using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NFinal
{
    public partial class WebCompiler : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write("生成开始<br/>");
            Response.Flush();
            Builder builder = new Builder(Server.MapPath("/"));
            builder.Create();
            Response.Write("生成结束<br/>");
            Response.End();
        }
    }
}
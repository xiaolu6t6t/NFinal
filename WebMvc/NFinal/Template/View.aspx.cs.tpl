using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace {$nameSpace}
{
    public partial class {$methodName} : System.Web.UI.Page
    {
		public HttpContext context = null;
        public System.Collections.Specialized.NameValueCollection get = null;
        public System.IO.TextWriter tw = null;
        public string action;
        public string controller;
        public string app;

		//变量声明
		<vt:foreach var="$declares" item="declare">
		public {$declare.typeName} {$declare.varName};
		</vt:foreach>

        protected void Page_Load(object sender, EventArgs e)
        {
			
        }
    }
}
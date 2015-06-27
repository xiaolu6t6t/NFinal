using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Web.Default.Common.Public
{
    public class HeaderAction  : Controller
	{
		public HeaderAction(System.IO.TextWriter tw):base(tw){}
		public HeaderAction(string fileName) : base(fileName) {}
        
        
        public void Header(string message)
        {
            
			Write("<header class=\"header\"> NFinal框架:这是头,来自模版");
			Write(message);
			Write("</header>");
        }
    }
}
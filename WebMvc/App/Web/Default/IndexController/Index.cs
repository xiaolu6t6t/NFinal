using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Web.Default.IndexController
{
    public class IndexAction  : Controller
	{
		public IndexAction(System.IO.TextWriter tw):base(tw){}
		public IndexAction(string fileName) : base(fileName) {}
        //
        public void Index()
        {
            string message = "我是页头,来自viewBag.";
            App.Common.UserControl.Footer footer = new App.Common.UserControl.Footer();
            footer.message = "我是页角说明,来自控件.";
            
			Write("<!doctype html><html><head><script src=\"/App/Content/js/jquery-1.11.2.min.js\"></script><link href=\"/App/Content/css/frame.css\" type=\"text/css\" rel=\"stylesheet\" /></head><body>");
			Write("<header class=\"header\"> NFinal框架:这是头,来自模版");
			Write(message);
			Write("</header><article> Hello,NFinal! </article>");
			footer.__render__ = (FooterViewBag)=>{ 
				Write("<footer> NFinal框架.这是尾");
				Write(FooterViewBag.message);
				Write("</footer>");
			};footer.__render__(footer);
			Write("</body></html>");
        }
    }
}
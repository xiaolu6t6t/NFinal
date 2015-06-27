using System;
using System.Collections.Generic;
using System.Web;

namespace {$project}.{$app}.Web.Default.IndexController
{
    public class IndexAction  : Controller
	{
		public IndexAction(System.IO.TextWriter tw):base(tw){}
		public IndexAction(string fileName) : base(fileName) {}
        public void Index()
        {
            
			Write("<!doctype html><html><head><script src=\"/App/Content/js/jquery-1.11.2.min.js\"></script><link href=\"/App/Content/css/frame.css\" type=\"text/css\" rel=\"stylesheet\" /><style type=\"text/css\"> .auto-style1 { width: 140px; } .auto-style2 { width: 197px; } </style></head><body>");
			Write("\r\n<header class=\"header\">\r\n    NFinal框架:这是头\r\n</header><article> Hello,NFinal! </article>");
			Write("\r\n<footer>\r\n     NFinal框架.这是尾\r\n</footer></body></html>");
        }
    }
}
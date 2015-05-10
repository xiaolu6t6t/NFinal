using System;
using System.Collections.Generic;
using System.Web;
using WebMvc.App.Common;

namespace WebMvc.App.Web.Default.IndexController
{
    public class GetAction  : NFinal.BaseAction
	{
		public GetAction(System.IO.TextWriter tw):base(tw){}
		public GetAction(string fileName) : base(fileName) {}
        
        public void Get()
        {
            
			Write("<!DOCTYPE html><html xmlns=\"http://www.w3.org/1999/xhtml\"><head runat=\"server\"><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/><title></title></head><body><form id=\"form1\" runat=\"server\"><div></div></form></body></html>");
        }
    }
}
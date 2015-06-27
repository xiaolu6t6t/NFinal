using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Web.Default.ViewController
{
    public class HTMLTagAction  : Controller
	{
		public HTMLTagAction(System.IO.TextWriter tw):base(tw){}
		public HTMLTagAction(string fileName) : base(fileName) {}
        
        
        public void HTMLTag()
        {
            string a = "123";
            int b = 1;
            float c = 1.5f;
            DateTime d = DateTime.Now;
            string[] e = new string[] { "a", "b", "c", "d" };
        }
    }
}
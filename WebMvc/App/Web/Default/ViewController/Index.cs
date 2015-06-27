using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Web.Default.ViewController
{
    public class IndexAction  : Controller
	{
		public IndexAction(System.IO.TextWriter tw):base(tw){}
		public IndexAction(string fileName) : base(fileName) {}
        public void Index()
        {
            string a="123";
            int b=1;
            float c=1.5f;
            DateTime d=DateTime.Now;
            string[] e=new string[]{"a","b","c","d"};
        }
        
        
    }
}
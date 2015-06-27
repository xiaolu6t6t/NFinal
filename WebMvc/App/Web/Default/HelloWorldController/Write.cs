using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Web.Default.HelloWorldController
{
    public class WriteAction  : Controller
	{
		public WriteAction(System.IO.TextWriter tw):base(tw){}
		public WriteAction(string fileName) : base(fileName) {}
        #region 输出
        //输出文本
        public void Write()
        {
            string words = "Hello World!";
            Write(words);
        }
        //输出HTML
        
        //输出Json
        
        #endregion
        #region 输入
        //URL
        
        
        //POST提交
        
        #endregion
    }
}
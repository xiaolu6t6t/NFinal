using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Web.Default.HelloWorldController
{
    public class DefaultAction  : Controller
	{
		public DefaultAction(System.IO.TextWriter tw):base(tw){}
		public DefaultAction(string fileName) : base(fileName) {}
        #region 输出
        //输出文本
        
        //输出HTML
        
        //输出Json
        
        #endregion
        #region 输入
        //URL
        
        public void Default(int id)
        {
            WriteLine("http://localhost/App/HelloWorldController/Default.htm");
            Write(id);
        }
        //POST提交
        
        #endregion
    }
}
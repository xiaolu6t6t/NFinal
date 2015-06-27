using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Web.Default.HelloWorldController
{
    public class WriteJsonAction  : Controller
	{
		public WriteJsonAction(System.IO.TextWriter tw):base(tw){}
		public WriteJsonAction(string fileName) : base(fileName) {}
        #region 输出
        //输出文本
        
        //输出HTML
        
        //输出Json
        public void WriteJson()
        {
            //代码,消息
            AjaxReturn(1, "Hello World!");
        }
        #endregion
        #region 输入
        //URL
        
        
        //POST提交
        
        #endregion
    }
}
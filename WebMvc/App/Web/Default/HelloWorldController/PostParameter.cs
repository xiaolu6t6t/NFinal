using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Web.Default.HelloWorldController
{
    public class PostParameterAction  : Controller
	{
		public PostParameterAction(System.IO.TextWriter tw):base(tw){}
		public PostParameterAction(string fileName) : base(fileName) {}
        #region 输出
        //输出文本
        
        //输出HTML
        
        //输出Json
        
        #endregion
        #region 输入
        //URL
        
        
        //POST提交
        public void PostParameter(string say)
        {
            Write(say);
        }
        #endregion
    }
}
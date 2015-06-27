using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Web.Default.HelloWorldController
{
    public class ParameterAction  : Controller
	{
		public ParameterAction(System.IO.TextWriter tw):base(tw){}
		public ParameterAction(string fileName) : base(fileName) {}
        #region 输出
        //输出文本
        
        //输出HTML
        
        //输出Json
        
        #endregion
        #region 输入
        //URL
        public void Parameter(string name)
        {
            WriteLine("http://localhost/App/HelloWorldController/Parameter/name/NFinal.htm");
            Write("您好:");
            Write(name);
        }
        
        //POST提交
        
        #endregion
    }
}
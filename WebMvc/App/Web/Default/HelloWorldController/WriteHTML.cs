using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Web.Default.HelloWorldController
{
    public class WriteHTMLAction  : Controller
	{
		public WriteHTMLAction(System.IO.TextWriter tw):base(tw){}
		public WriteHTMLAction(string fileName) : base(fileName) {}
        #region 输出
        //输出文本
        
        //输出HTML
        public void WriteHTML(string name,string[] list)
        {
            //HTML模板文件
            
			Write("<!DOCTYPE html><html xmlns=\"http://www.w3.org/1999/xhtml\"><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /><title></title></head><body><form id=\"form1\"><div> Hello World! </div></form></body></html>");
        }
        //输出Json
        
        #endregion
        #region 输入
        //URL
        
        
        //POST提交
        
        #endregion
    }
}
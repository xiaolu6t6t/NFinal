using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Controllers
{
    public class HelloWorldController : Controller
    {
        #region 输出
        //输出文本
        public void Write()
        {
            string words = "Hello World!";
            Write(words);
        }
        //输出HTML
        public void WriteHTML(string name,string[] list)
        {
            //HTML模板文件
            View("WriteHTML.aspx");
        }
        //输出Json
        public void WriteJson()
        {
            //代码,消息
            AjaxReturn(1, "Hello World!");
        }
        #endregion
        #region 输入
        //URL
        public void Parameter(string name)
        {
            WriteLine("http://localhost/App/HelloWorldController/Parameter/name/NFinal.htm");
            Write("您好:");
            Write(name);
        }
        public void Default(int id = 1)
        {
            WriteLine("http://localhost/App/HelloWorldController/Default.htm");
            Write(id);
        }
        //POST提交
        public void PostParameter(string say)
        {
            Write(say);
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Controllers.Common
{
    public class Public: Controller
    {
        public void Success(string message,string url,int second)
        {
            View("Public/Success.aspx");
        }
        public void Error(string message, string url, int second)
        {
            View("Public/Success.aspx");
        }
        public void Header(string message)
        {
            View("Public/Header.ascx");
        }
    }
}
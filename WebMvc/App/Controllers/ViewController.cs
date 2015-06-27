using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Controllers
{
    public class ViewController : Controller
    {
        public void Index()
        {
            string a="123";
            int b=1;
            float c=1.5f;
            DateTime d=DateTime.Now;
            string[] e=new string[]{"a","b","c","d"};
        }
        public void AutoComplete()
        {
            Models.Common.OpenConnection();
            var users = Models.Common.QueryAll("select id as uid from users");
            Models.Common.CloseConnection();
            string message = "Hello";
            int a = 1;
            Models.DAL.Users us = new Models.DAL.Users();
            //List<Models.DAL.Users.__GetUsers_users__> uss = us.GetUsers();
            //HTML模板文件
            View("AutoComplete.aspx");
        }
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
using System;
using System.Collections.Generic;
using System.Web;
using WebMvc.App.Common;

namespace WebMvc.App.Controllers
{
    public class IndexController: NFinal.BaseAction
    {
        public void Index(int[] a)
        {
            int id = 13;
            string title = "title";
            string site_url = "site_url";
            var b = DB.dtcmsdb3.Insert("insert into dt_link(title,site_url) values(@title,@site_url)");
            
        }
        public void Get()
        {
            View("");
        }
    }
}
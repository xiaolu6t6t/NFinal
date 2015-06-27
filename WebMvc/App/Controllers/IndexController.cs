using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Controllers
{
    public class IndexController: Controller
    {
        //
        public void Index()
        {
            string message = "我是页头,来自viewBag.";
            App.Common.UserControl.Footer footer = new App.Common.UserControl.Footer();
            footer.message = "我是页角说明,来自控件.";
            View("Index.aspx");
        }
    }
}
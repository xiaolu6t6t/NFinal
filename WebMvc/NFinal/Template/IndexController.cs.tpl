using System;
using System.Collections.Generic;
using System.Web;

namespace {$nameSpace}
{
    public class IndexController:NFinal.BaseAction
    {
        public void Index()
        {
			string text="Hello World!";
            View("IndexController/Index");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Web;

namespace {$project}.{$app}.Controllers
{
    public class IndexController: Controller
    {
        public void Index()
        {
            View("Index.aspx");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Web;
using System.Collections.Specialized;
using System.IO;

namespace {$project}.{$app}
{
    //Controller基类
    public class Controller : NFinal.BaseAction
    {
        public Controller() { }
        public Controller(string fileName)
            : base(fileName)
        {
        }
        public Controller(TextWriter tw)
            : base(tw)
        {
        }
        public Common.Data.CookieManager _cookies;
    }
}
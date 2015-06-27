using System;
using System.Collections.Generic;
using System.Web;

namespace {$project}.{$app}.Controllers.Common
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
    }
}
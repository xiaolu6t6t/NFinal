using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NFinal.Common.SMS.Open189
{
    public class Receive
    {
        public void Run(System.Web.HttpContext context)
        {
            string phone = context.Request.Form["phone"];
            string message = context.Request.Form["message"];
            string mo_time = context.Request.Form["mo_time"];
            string app_id = context.Request.Form["app_id"];
            string time_stamp = context.Request.Form["time_stamp"];
            string sign = context.Request.Form["sign"];
        }
    }
}
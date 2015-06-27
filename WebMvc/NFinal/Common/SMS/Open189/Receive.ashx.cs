using System;
using System.Collections.Generic;
using System.Web;

namespace NFinal.Common.SMS.Open189
{
    /// <summary>
    /// Handler1 的摘要说明
    /// </summary>
    public class Receive : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string phone = context.Request.Form["phone"];
            string message = context.Request.Form["message"];
            string mo_time = context.Request.Form["mo_time"];
            string app_id = context.Request.Form["app_id"];
            string time_stamp = context.Request.Form["time_stamp"];
            string sign = context.Request.Form["sign"];


            context.Response.ContentType = "applicatin/json";
            context.Response.Write("{\"res_code\":\"0\"}");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Models
{
    public class ConnectionStrings
    {
        public static string Common = @"Data Source=|DataDirectory|Common.db;Pooling=true;FailIfMissing=false";
    }
}
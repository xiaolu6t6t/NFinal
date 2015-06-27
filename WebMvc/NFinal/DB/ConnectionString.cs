using System;
using System.Collections.Generic;
using System.Web;

namespace NFinal.DB
{
    /// <summary>
    /// Web.Config中连字符串实体类
    /// </summary>
    public class ConnectionString
    {
        public string name;
        public string value;
        public string provider;
        public NFinal.DB.DBType type;
    }
}
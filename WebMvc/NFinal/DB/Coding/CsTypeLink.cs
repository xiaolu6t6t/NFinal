using System;
using System.Collections.Generic;
using System.Web;
using System.IO;

namespace NFinal.DB.Coding
{
    /// <summary>
    /// sharp所对应的数据库类型,数据库参数枚举类型
    /// </summary>
    public class CsTypeLink
    {
        public string csharpType;
        public string sqlDbGetType;
        public string sqlDbGetMethod;
        public string sqlType;
        public string sqlDbType;
    }
}
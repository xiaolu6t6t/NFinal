using System;
using System.Collections.Generic;
using System.Web;

namespace NFinal.DB
{
    /// <summary>
    /// 数据库类型枚举类
    /// </summary>
    public enum DBType
    {
        MySql,
        SqlServer,
        Sqlite,
        Oracle,
        Unknown
    }
}
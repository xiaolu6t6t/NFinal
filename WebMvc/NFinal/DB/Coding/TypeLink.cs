using System;
using System.Collections.Generic;
using System.Text;

namespace NFinal.DB.Coding
{
    /// <summary>
    /// 字段类型
    /// </summary>
    public struct TypeLink
    {
        //数据库中的类型
        public string sqlType;
        //csharp 基本类型
        public string localType;
        //csharp System.Data 中的类型
        public string dbType;
        //数据转换时所需的类型
        public string simpleType;
    }
}

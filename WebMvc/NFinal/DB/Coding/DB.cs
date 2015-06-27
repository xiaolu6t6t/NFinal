using System;
using System.Collections.Generic;
using System.Text;

namespace NFinal.DB.Coding
{
    /// <summary>
    /// 从Web.Config获取的数据库类,包括所有的类型信息等
    /// </summary>
    class DB
    {
        public static Dictionary<string, NFinal.DB.Coding.DataUtility> DbStore = new Dictionary<string, NFinal.DB.Coding.DataUtility>();
    }
}

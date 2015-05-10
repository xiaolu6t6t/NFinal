using System;
using System.Collections.Generic;
using System.Web;

namespace NFinal.DB
{
    /// <summary>
    /// SQL事务魔法类
    /// </summary>
    public class Transation:DBObject 
    {
        public bool Process()
        {
            return true;
        }
    }
}
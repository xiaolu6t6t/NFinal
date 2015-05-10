using System;
using System.Collections.Generic;
using System.Web;

namespace NFinal.DB
{
    public class DBObject
    {
        /// <summary>
        /// 执行SQL并返回行对象数组
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<dynamic> QueryAll(string sql)
        {
            return new List<dynamic>();
        }
        public List<dynamic> Page(string sql, int pageSize, int pageIndex, out int pageCount)
        {
            pageCount = 1;
            return new List<dynamic>();
        }
        /// <summary>
        /// 执行SQL并返回行对象
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public dynamic QueryRow(string sql)
        {
            dynamic a = 0;
            return a;
        }
       
        public object QueryObject(string sql)
        {
            return new SqlObject(null);
        }
        /// <summary>
        /// 执行SQL并返回ID
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Insert(string sql)
        {
            return 0;
        }
        public int Update(string sql)
        {
            return 0;
        }
        public int Delete(string sql)
        {
            return 0;
        }
        /// <summary>
        /// 执行SQL并返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExcuteNonQuery(string sql)
        {
            return 0;
        }
    }
}
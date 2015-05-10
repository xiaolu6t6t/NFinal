using System;
using System.Web;

namespace NFinal.DB
{
    /// <summary>
    /// 数据库魔法函数类,此类只为自动提示时使用,并不编译执行
    /// </summary>
    public class DBBase
    {
        /// <summary>
        /// 执行SQL并返回相应行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public static List<dynamic> QueryRandom(string sql,int top)
        {
            return new List<dynamic>();
        }
        /// <summary>
        /// 执行SQL并返回行对象数组
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<dynamic> QueryAll(string sql)
        {
            return new List<dynamic>();
        }
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="sql">选取所有记录的SQL语句</param>
        /// <param name="pageIndexVarName">传送页码所需的变量,默认为pageIndex</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <returns></returns>
        public static List<dynamic> Page(string sql,int pageSize)
        {
            return new List<dynamic>();
        }

        /// <summary>
        /// 执行SQL并返回行对象
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static dynamic QueryRow(string sql)
        {
            dynamic a = 0;
            return a;
        }
        public static SqlObject QueryObject(string sql)
        {
            return new SqlObject(null);
        }
        /// <summary>
        /// 执行SQL并返回ID
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int Insert(string sql)
        {
            return 0;
        }
        public static int Update(string sql)
        {
            return 0;
        }
        public static int Delete(string sql)
        {
            return 0;
        }
        /// <summary>
        /// 执行SQL并返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int ExcuteNonQuery(string sql)
        {
            return 0;
        }
    }
}
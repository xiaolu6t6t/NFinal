using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Web.Default.DataBaseController
{
    public class QueryObjectAction  : Controller
	{
		public QueryObjectAction(System.IO.TextWriter tw):base(tw){}
		public QueryObjectAction(string fileName) : base(fileName) {}
        #region 查询
        //查询首行首列
        public void QueryObject()
        {
            int id = 1;
            var __QueryObject_Common_con__ = new System.Data.SQLite.SQLiteConnection(Models.ConnectionStrings.Common);
			__QueryObject_Common_con__.Open();
            	#region	var count; 选取首行首列的值
			var __QueryObject_count_command__ = new System.Data.SQLite.SQLiteCommand("select count(*) from users where id=@id", __QueryObject_Common_con__);
			
			var __QueryObject_count_parameters__=new System.Data.SQLite.SQLiteParameter[1];
			
			__QueryObject_count_parameters__[1-1] = new System.Data.SQLite.SQLiteParameter("@id",System.Data.DbType.Int64,8);
			__QueryObject_count_parameters__[1-1].Value = id;
			
			__QueryObject_count_command__.Parameters.AddRange(__QueryObject_count_parameters__);
			
			var count = new NFinal.DB.SqlObject(__QueryObject_count_command__.ExecuteScalar()).ToInt();
	#endregion
			
            __QueryObject_Common_con__.Close();
            Write(count);
        }
        //查询所有行
        
        //查询随机行
        
        //查询前多少行
        
        //查询一行
        

        #endregion
        #region 增删改
        
        
        
        
        #endregion
        #region 父子表,多表关系
        
        #endregion
    }
}
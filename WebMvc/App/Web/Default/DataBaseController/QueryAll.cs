using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Web.Default.DataBaseController
{
    public class QueryAllAction  : Controller
	{
		public QueryAllAction(System.IO.TextWriter tw):base(tw){}
		public QueryAllAction(string fileName) : base(fileName) {}
        #region 查询
        //查询首行首列
        
        //查询所有行
        public class __QueryAll_users__:NFinal.DB.Struct
		{
			
			public System.Int64 uid;
			
			public System.Int64 _column1;
			
			#region 写Json字符串
			public override void WriteJson(System.IO.TextWriter tw)
			{
				tw.Write("{");
				
						tw.Write("\"uid\":");
						tw.Write(uid.ToString());
					
						tw.Write(",");
					
						tw.Write("\"_column1\":");
						tw.Write(_column1.ToString());
					
				tw.Write("}");
			}
			#endregion
		}
		public void QueryAll()
        {
            var __QueryAll_Common_con__ = new System.Data.SQLite.SQLiteConnection(Models.ConnectionStrings.Common);
			__QueryAll_Common_con__.Open();
            	#region	var users;选取所有记录
			var users = new NFinal.DB.NList<__QueryAll_users__>();
            
			var __QueryAll_users_command__ = new System.Data.SQLite.SQLiteCommand("select u.id as uid,count(*) from users as u where u.id<0", __QueryAll_Common_con__);
			
			var __QueryAll_users_reader__= __QueryAll_users_command__.ExecuteReader();
			if (__QueryAll_users_reader__.HasRows)
			{
				while (__QueryAll_users_reader__.Read())
				{
					var __QueryAll_users_temp__ = new __QueryAll_users__();
					
					if(!__QueryAll_users_reader__.IsDBNull(1-1)){__QueryAll_users_temp__.uid = __QueryAll_users_reader__.GetInt64(1-1);}
					
					if(!__QueryAll_users_reader__.IsDBNull(2-1)){__QueryAll_users_temp__._column1 = __QueryAll_users_reader__.GetInt64(2-1);}
					users.Add(__QueryAll_users_temp__);
				}
			}
			__QueryAll_users_reader__.Close();
	#endregion
			
            __QueryAll_Common_con__.Close();
            //foreach (var user in users)
            //{
            //    Write(user.id);
            //    Write(user.name);
            //    Write(user.pwd);
            //}
        }
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
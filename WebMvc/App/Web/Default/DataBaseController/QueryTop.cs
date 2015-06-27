using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Web.Default.DataBaseController
{
    public class QueryTopAction  : Controller
	{
		public QueryTopAction(System.IO.TextWriter tw):base(tw){}
		public QueryTopAction(string fileName) : base(fileName) {}
        #region 查询
        //查询首行首列
        
        //查询所有行
        
        //查询随机行
        
        //查询前多少行
        public class __QueryTop_users__:NFinal.DB.Struct
		{
			
			public System.Int64 id;
			
			public System.String name;
			
			public System.String pwd;
			
			#region 写Json字符串
			public override void WriteJson(System.IO.TextWriter tw)
			{
				tw.Write("{");
				
						tw.Write("\"id\":");
						tw.Write(id.ToString());
					
						tw.Write(",");
					
						tw.Write("\"name\":");//
						tw.Write("\"");
						WriteString(name==null?"null":name.ToString(),tw);
						tw.Write("\"");
					
						tw.Write(",");
					
						tw.Write("\"pwd\":");//
						tw.Write("\"");
						WriteString(pwd==null?"null":pwd.ToString(),tw);
						tw.Write("\"");
					
				tw.Write("}");
			}
			#endregion
		}
		public void QueryTop()
        {
            var __QueryTop_Common_con__ = new System.Data.SQLite.SQLiteConnection(Models.ConnectionStrings.Common);
			__QueryTop_Common_con__.Open();
            	#region	var users; 随机选取前N行
			var users = new NFinal.DB.NList<__QueryTop_users__>();
            
			var __QueryTop_users_command__ = new System.Data.SQLite.SQLiteCommand(string.Format("select * from users Limit {0}",2), __QueryTop_Common_con__);
			
			var __QueryTop_users_reader__= __QueryTop_users_command__.ExecuteReader();
			if (__QueryTop_users_reader__.HasRows)
			{
				while (__QueryTop_users_reader__.Read())
				{
					var __QueryTop_users_temp__ = new __QueryTop_users__();
					
					if(!__QueryTop_users_reader__.IsDBNull(1-1)){__QueryTop_users_temp__.id = __QueryTop_users_reader__.GetInt64(1-1);}
					
					if(!__QueryTop_users_reader__.IsDBNull(2-1)){__QueryTop_users_temp__.name = __QueryTop_users_reader__.GetString(2-1);}
					
					if(!__QueryTop_users_reader__.IsDBNull(3-1)){__QueryTop_users_temp__.pwd = __QueryTop_users_reader__.GetString(3-1);}
					users.Add(__QueryTop_users_temp__);
				}
			}
			__QueryTop_users_reader__.Close();
	#endregion
			
            __QueryTop_Common_con__.Close();
            //foreach (var user in users)
            //{
            //    Write(user.id);
            //    Write(user.name);
            //    Write(user.pwd);
            //}
        }
        //查询一行
        

        #endregion
        #region 增删改
        
        
        
        
        #endregion
        #region 父子表,多表关系
        
        #endregion
    }
}
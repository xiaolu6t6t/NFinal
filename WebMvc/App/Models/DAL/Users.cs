using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Models.DAL
{
    public class Users
    {
        public class __GetUsers_users__:NFinal.DB.Struct
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
		public NFinal.DB.NList<__GetUsers_users__> GetUsers()
        {
            var __GetUsers_Common_con__ = new System.Data.SQLite.SQLiteConnection(Models.ConnectionStrings.Common);
			__GetUsers_Common_con__.Open();
            	#region	var users;选取所有记录
			var users = new NFinal.DB.NList<__GetUsers_users__>();
            
			var __GetUsers_users_command__ = new System.Data.SQLite.SQLiteCommand("select * from users", __GetUsers_Common_con__);
			
			var __GetUsers_users_reader__= __GetUsers_users_command__.ExecuteReader();
			if (__GetUsers_users_reader__.HasRows)
			{
				while (__GetUsers_users_reader__.Read())
				{
					var __GetUsers_users_temp__ = new __GetUsers_users__();
					
					if(!__GetUsers_users_reader__.IsDBNull(1-1)){__GetUsers_users_temp__.id = __GetUsers_users_reader__.GetInt64(1-1);}
					
					if(!__GetUsers_users_reader__.IsDBNull(2-1)){__GetUsers_users_temp__.name = __GetUsers_users_reader__.GetString(2-1);}
					
					if(!__GetUsers_users_reader__.IsDBNull(3-1)){__GetUsers_users_temp__.pwd = __GetUsers_users_reader__.GetString(3-1);}
					users.Add(__GetUsers_users_temp__);
				}
			}
			__GetUsers_users_reader__.Close();
	#endregion
			
            __GetUsers_Common_con__.Close();
            return users;
        }
    }
}
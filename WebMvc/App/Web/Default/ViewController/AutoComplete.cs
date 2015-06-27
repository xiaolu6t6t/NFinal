using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Web.Default.ViewController
{
    public class AutoCompleteAction  : Controller
	{
		public AutoCompleteAction(System.IO.TextWriter tw):base(tw){}
		public AutoCompleteAction(string fileName) : base(fileName) {}
        
        public class __AutoComplete_users__:NFinal.DB.Struct
		{
			
			public System.Int64 uid;
			
			#region 写Json字符串
			public override void WriteJson(System.IO.TextWriter tw)
			{
				tw.Write("{");
				
						tw.Write("\"uid\":");
						tw.Write(uid.ToString());
					
				tw.Write("}");
			}
			#endregion
		}
		public void AutoComplete()
        {
            var __AutoComplete_Common_con__ = new System.Data.SQLite.SQLiteConnection(Models.ConnectionStrings.Common);
			__AutoComplete_Common_con__.Open();
            	#region	var users;选取所有记录
			var users = new NFinal.DB.NList<__AutoComplete_users__>();
            
			var __AutoComplete_users_command__ = new System.Data.SQLite.SQLiteCommand("select id as uid from users", __AutoComplete_Common_con__);
			
			var __AutoComplete_users_reader__= __AutoComplete_users_command__.ExecuteReader();
			if (__AutoComplete_users_reader__.HasRows)
			{
				while (__AutoComplete_users_reader__.Read())
				{
					var __AutoComplete_users_temp__ = new __AutoComplete_users__();
					
					if(!__AutoComplete_users_reader__.IsDBNull(1-1)){__AutoComplete_users_temp__.uid = __AutoComplete_users_reader__.GetInt64(1-1);}
					users.Add(__AutoComplete_users_temp__);
				}
			}
			__AutoComplete_users_reader__.Close();
	#endregion
			
            __AutoComplete_Common_con__.Close();
            string message = "Hello";
            int a = 1;
            Models.DAL.Users us = new Models.DAL.Users();
            //List<Models.DAL.Users.__GetUsers_users__> uss = us.GetUsers();
            //HTML模板文件
            
			Write("<!DOCTYPE html><html xmlns=\"http://www.w3.org/1999/xhtml\"><head runat=\"server\"><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/><title></title></head><body><form id=\"form1\" runat=\"server\" ><div></div></form>");
			if(users.Count>0 ){
			}else{
				Write("<asp:Label ID=\"Label1\" runat=\"server\" Text=\"Label\"></asp:Label>");
			}
			var user= users.GetEnumerator(); while(user.MoveNext()){
				Write("<div>");
				Write(user.Current.uid);
				Write("</div>");
			}
			Write("</body></html>");
        }
        
    }
}
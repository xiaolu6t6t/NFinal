using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Web.Default.DataBaseController
{
    public class QueryRowAction  : Controller
	{
		public QueryRowAction(System.IO.TextWriter tw):base(tw){}
		public QueryRowAction(string fileName) : base(fileName) {}
        #region 查询
        //查询首行首列
        
        //查询所有行
        
        //查询随机行
        
        //查询前多少行
        
        //查询一行
        public class __QueryRow_user__:NFinal.DB.Struct
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
		public void QueryRow()
        {
            var __QueryRow_Common_con__ = new System.Data.SQLite.SQLiteConnection(Models.ConnectionStrings.Common);
			__QueryRow_Common_con__.Open();
            	#region	var user; 选取一行
			var user = new __QueryRow_user__();
            
			var __QueryRow_user_command__ = new System.Data.SQLite.SQLiteCommand("select * from users where id=1", __QueryRow_Common_con__);
			
			var __QueryRow_user_reader__= __QueryRow_user_command__.ExecuteReader();
			if (__QueryRow_user_reader__.HasRows)
			{
				__QueryRow_user_reader__.Read();

				user = new __QueryRow_user__();
				
				if(!__QueryRow_user_reader__.IsDBNull(1-1)){user.id = __QueryRow_user_reader__.GetInt64(1-1);}
				
				if(!__QueryRow_user_reader__.IsDBNull(2-1)){user.name = __QueryRow_user_reader__.GetString(2-1);}
				
				if(!__QueryRow_user_reader__.IsDBNull(3-1)){user.pwd = __QueryRow_user_reader__.GetString(3-1);}
				
			}
			__QueryRow_user_reader__.Close();
	#endregion
			
            __QueryRow_Common_con__.Close();
            //Write(user.id);
            //Write(user.name);
            //Write(user.pwd);
        }

        #endregion
        #region 增删改
        
        
        
        
        #endregion
        #region 父子表,多表关系
        
        #endregion
    }
}
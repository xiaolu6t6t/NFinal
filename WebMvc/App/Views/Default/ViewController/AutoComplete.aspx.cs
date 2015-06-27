using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebMvc.App.Views.Default.ViewController
{
    public partial class AutoComplete : System.Web.UI.Page
    {
		//数据库模型
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
		
		//函数内变量
        public class AutoComplete_AutoComplete:Controller
        {
			//参数变量
			
			//数据库变量
			
					public NFinal.DB.NList<__AutoComplete_users__> users;
				
			//一般变量
			
				public string message;
			
				public int a;
			
				public Models.DAL.Users us;
			
				public List<Models.DAL.Users.__GetUsers_users__> uss;
			
			//DAL函数声明变量
        }
		//变量存储类,用于自动完成.
        public AutoComplete_AutoComplete ViewBag = new AutoComplete_AutoComplete();
    }
}
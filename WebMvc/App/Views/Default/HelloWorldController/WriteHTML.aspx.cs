using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebMvc.App.Views.Default.HelloWorldController
{
    public partial class WriteHTML : System.Web.UI.Page
    {
		//数据库模型
        
		//函数内变量
        public class WriteHTML_AutoComplete:Controller
        {
			//参数变量
			
					public string name;
				
					public string[] list;
				
			//数据库变量
			
			//一般变量
			
			//DAL函数声明变量
        }
		//变量存储类,用于自动完成.
        public WriteHTML_AutoComplete ViewBag = new WriteHTML_AutoComplete();
    }
}
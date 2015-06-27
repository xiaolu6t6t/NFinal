using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebMvc.App.Views.Default.Common.Public
{
    public partial class Header : System.Web.UI.Page
    {
		//数据库模型
        
		//函数内变量
        public class Header_AutoComplete:Controller
        {
			//参数变量
			
					public string message;
				
			//数据库变量
			
			//一般变量
			
			//DAL函数声明变量
        }
		//变量存储类,用于自动完成.
        public Header_AutoComplete ViewBag = new Header_AutoComplete();
    }
}
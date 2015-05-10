using System;
using System.Collections.Generic;
using System.Web;
using WebMvc.App.Common;

namespace WebMvc.App.Web.Default.IndexController
{
    public class IndexAction  : NFinal.BaseAction
	{
		public IndexAction(System.IO.TextWriter tw):base(tw){}
		public IndexAction(string fileName) : base(fileName) {}
        public void Index(int[] a)
        {
            int id = 13;
            string title = "title";
            string site_url = "site_url";
            	var __Index_b_con__ = new System.Data.SqlClient.SqlConnection(ConnectionString.dtcmsdb3);
			__Index_b_con__.Open();
			var __Index_b_command__ = new System.Data.SqlClient.SqlCommand("insert into dt_link(title,site_url) values(@title,@site_url);select @@IDENTITY;", __Index_b_con__);
			
			var __Index_b_parameters__=new System.Data.SqlClient.SqlParameter[2];
			
			__Index_b_parameters__[1-1] = new System.Data.SqlClient.SqlParameter("@title",System.Data.SqlDbType.NVarChar,255);
			__Index_b_parameters__[1-1].Value = title;
			
			__Index_b_parameters__[2-1] = new System.Data.SqlClient.SqlParameter("@site_url",System.Data.SqlDbType.NVarChar,255);
			__Index_b_parameters__[2-1].Value = site_url;
			
			__Index_b_command__.Parameters.AddRange(__Index_b_parameters__);
			
			var b = int.Parse(__Index_b_command__.ExecuteScalar().ToString());
			__Index_b_con__.Close();
			
            
        }
        
    }
}
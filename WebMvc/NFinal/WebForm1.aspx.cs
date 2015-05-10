using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SQLite;
using System.Data;

namespace NFinal
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private static string connectionString = @"Data Source=|DataDirectory|\Session.db";
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            NFinal.Session.Session se = new Session.Session(null, new TimeSpan(1, 0, 0));
            string id= se.SetSession(null,"namea","lucasa");
           string val= se.GetSession(id,"namea");
           Response.Write(val);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebMvc.App.Views.Default.Common.Public
{
    public partial class Footer : System.Web.UI.UserControl
    {
        public App.Common.UserControl.Footer FooterViewBag { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}
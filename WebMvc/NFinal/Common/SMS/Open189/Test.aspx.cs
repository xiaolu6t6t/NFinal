using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NFinal.Common.SMS.Open189
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //NFinal.Common.SMS.Open189.BaoMingTemplate template = new BaoMingTemplate("91004968");
            //template.SendSMS("15639279137", "Lucas", "410101");
            NFinal.Common.SMS.Open189.VerifyCodeTemplate template = new VerifyCodeTemplate("91004969");
            //template.SendSMS("15639279137");
            template.VerifyCode("15639279137", "1d9f");
        }
    }
}
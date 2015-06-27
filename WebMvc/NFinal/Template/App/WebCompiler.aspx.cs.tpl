using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace {$project}.{$app}
{
    public partial class WebCompiler : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write("生成开始<br/>");
            Response.Flush();

            string appRoot = Server.MapPath("/");
            //获取WebConfig中的配置信息
            NFinal.Frame frame = new NFinal.Frame(appRoot);
            string[] Apps = frame.GetApps();
            //获取所有的数据库信息
            frame.GetDB();
            //创建主路由
            frame.CreateMain(Apps);
            
            //创建并生成Web站点
            NFinal.Config config = {$project}.{$app}.Config.set;
            config.Init("{$app}");
            NFinal.Application application = new NFinal.Application(config);
            application.CreateApp();
            application.CreateModelsFile();
            application.CreateRouter();
            application.CreateDAL(true);
            application.CreateCompile(true);

            Response.Write("生成结束<br/>");
            Response.Flush();

            Response.End();
        }
    }
}
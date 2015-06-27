using System;
using System.Collections.Generic;
using System.Web;

namespace NFinal
{
    public class Builder
    {
        private string appRoot=string.Empty;
        public Builder(string appRoot)
        {
            this.appRoot = appRoot;
        }
        //生成类
        public void Create()
        {
            
            ////获取WebConfig中的配置信息
            //Frame.Set(appRoot);
            //string[] Apps = Frame.GetWebConfig();
            ////获取所有的数据库信息
            //Frame.GetDB();
            ////创建主路由
            //Frame.CreateMain(Apps);
            //Config configApp = null;
            //Frame frameApp = null;
            ////创建并生成Web站点
            //foreach (string app in Apps)
            //{
            //    configApp = new Config(app);
            //    frameApp = new Frame(configApp);
            //    frameApp.CreateApp();
            //    frameApp.SetDBFile();
            //    frameApp.CreateRouter();
            //    frameApp.createDAL();
            //    frameApp.createCompile();
            //}
        }
    }
}
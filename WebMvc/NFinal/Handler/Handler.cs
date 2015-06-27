using System;
using System.Collections.Generic;
using System.Web;
using System.Threading;
using System.IO;

namespace NFinal.Handler
{
    /// <summary>
    /// NFinal框架重写的HttpHandler类
    /// </summary>
    public class Handler:NFinal.Handler.HttpAsyncHandler
    {

        public override void BeginProcess(System.Web.HttpContext context)
        {
            try
            {
                Main main = new Main();
                main.Run(context,context.Request.RawUrl);
                
            }
            catch (Exception ex)
            {
                context.Response.Write("exception eccurs ex info : " + ex.Message);
            }
            finally
            {
                context.Response.End();
                EndProcess();////最后别忘了end  
            }


        }  
        // 摘要: 
        //     获取一个值，该值指示其他请求是否可以使用 System.Web.IHttpHandler 实例。
        //
        // 返回结果: 
        //     如果 System.Web.IHttpHandler 实例可再次使用，则为 true；否则为 false。
        //public bool IsReusable { get { return false; } }

        // 摘要: 
        //     通过实现 System.Web.IHttpHandler 接口的自定义 HttpHandler 启用 HTTP Web 请求的处理。
        //
        // 参数: 
        //   context:
        //     System.Web.HttpContext 对象，它提供对用于为 HTTP 请求提供服务的内部服务器对象（如 Request、Response、Session
        //     和 Server）的引用。

        /// <summary>
        /// 处理Http请求
        /// </summary>
        /// <param name="context">Http请求参数</param>
        //public void ProcessRequest(HttpContext context)
        //{
        //    Main main = new Main();
        //    main.Run(context);
        //    context.Response.End();
        //}
    }
}
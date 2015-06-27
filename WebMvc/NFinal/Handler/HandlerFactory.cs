using System;
using System.Collections.Generic;
using System.Web;

namespace NFinal.Handler
{
    /// <summary>
    /// NFinal框架重写的HttpHandlerFactory
    /// </summary>
    public class HandlerFactory:IHttpHandlerFactory
    {
        /// <summary>
        /// 返回一个用于处理HttpRequest的类
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestType"></param>
        /// <param name="url"></param>
        /// <param name="pathTranslated"></param>
        /// <returns></returns>
        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            return new Handler();
        }
        /// <summary>
        /// 释放占用的资源,实际无效果
        /// </summary>
        /// <param name="handler"></param>
        public void ReleaseHandler(IHttpHandler handler)
        { 
            
        }
    }
}
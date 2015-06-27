using System;
using System.Collections.Generic;
using System.Web;
using System.Threading;
using System.Globalization;
using System.Collections.Specialized;

namespace {$namespace}
{
    public class Router
    {
		private string GetString(string val, string de)
        {
            return string.IsNullOrEmpty(val) ? de : val;
        }
        private string UCFirst(string val)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            return cultureInfo.TextInfo.ToTitleCase(val);
        }
        public void Main(System.Web.HttpContext context)
        {
            string module = "index";
            string action = "index";
            string[] pathInfo = null;
            NameValueCollection get = new NameValueCollection();
            if (!string.IsNullOrEmpty(context.Request.PathInfo))
            {
                pathInfo = context.Request.PathInfo.Trim(new char[] { '/' }).Split('/');
                module=UCFirst(GetString(pathInfo[0], module));
                action=GetString(pathInfo[1], action);
                if (pathInfo.Length > 2)
                {
                    for (int i = 2; i < pathInfo.Length; i += 2)
                    {
                        get.Add(pathInfo[i], pathInfo[i + 1]);
                    }
                }
            }
            else
            {
                module =UCFirst(GetString(context.Request.QueryString["m"], module));
                action =GetString(context.Request.QueryString["a"], action);
            }
            
            //Run(context,module,action,get);
        }
        public void Run(System.Web.HttpContext context,string subdomain,string app,string folder,string controller,string action,NameValueCollection get)
        {
			switch (controller)
			{
				<vt:foreach from="$classDataList" item="classData" index="i">
				//relativeName="/Manage/IndexController"
				//RelativeDotName=".Manage.IndexController"
				case "{$classData.RelativeName}":
					{
                        
						switch (action)
						{
							<vt:foreach from="$classData.MethodDataList" item="method" index="j">
							<vt:if var="method.isPublic" value="True">
							case "{$method.name}": 
							{
								Web.{$defaultStyle}{$classData.RelativeDotName}.{$method.name}Action control= new Web.{$defaultStyle}{$classData.RelativeDotName}.{$method.name}Action(context.Response.Output);
								<vt:foreach from="$method.parameterDataList" item="parameterData">
									{$parameterData.getParamterCode}
								</vt:foreach>
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.{$method.name}({$method.parameterNames});
								control.After();
								break;
							}
							</vt:if>
							</vt:foreach>
							default: context.Response.Write("找不到类" + controller + "下的" + action + "方法");context.Response.End(); break;
						}
						break;
					}
				</vt:foreach>
				default: context.Response.Write("找不到类" + controller); context.Response.End(); break; 
			}
        }
        
    }
}
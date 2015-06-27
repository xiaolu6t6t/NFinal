using System;
using System.Collections.Generic;
using System.Web;
using System.Threading;
using System.Globalization;
using System.Collections.Specialized;

namespace WebMvc.App
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
				
				//relativeName="/Manage/IndexController"
				//RelativeDotName=".Manage.IndexController"
				case "/DataBaseController":
					{
                        
						switch (action)
						{
							
							case "QueryObject": 
							{
								Web.Default.DataBaseController.QueryObjectAction control= new Web.Default.DataBaseController.QueryObjectAction(context.Response.Output);
								
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.QueryObject();
								control.After();
								break;
							}
							
							case "QueryAll": 
							{
								Web.Default.DataBaseController.QueryAllAction control= new Web.Default.DataBaseController.QueryAllAction(context.Response.Output);
								
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.QueryAll();
								control.After();
								break;
							}
							
							case "QueryRandom": 
							{
								Web.Default.DataBaseController.QueryRandomAction control= new Web.Default.DataBaseController.QueryRandomAction(context.Response.Output);
								
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.QueryRandom();
								control.After();
								break;
							}
							
							case "QueryTop": 
							{
								Web.Default.DataBaseController.QueryTopAction control= new Web.Default.DataBaseController.QueryTopAction(context.Response.Output);
								
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.QueryTop();
								control.After();
								break;
							}
							
							case "QueryRow": 
							{
								Web.Default.DataBaseController.QueryRowAction control= new Web.Default.DataBaseController.QueryRowAction(context.Response.Output);
								
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.QueryRow();
								control.After();
								break;
							}
							
							case "Page": 
							{
								Web.Default.DataBaseController.PageAction control= new Web.Default.DataBaseController.PageAction(context.Response.Output);
								
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.Page();
								control.After();
								break;
							}
							
							case "Insert": 
							{
								Web.Default.DataBaseController.InsertAction control= new Web.Default.DataBaseController.InsertAction(context.Response.Output);
								
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.Insert();
								control.After();
								break;
							}
							
							case "Delete": 
							{
								Web.Default.DataBaseController.DeleteAction control= new Web.Default.DataBaseController.DeleteAction(context.Response.Output);
								
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.Delete();
								control.After();
								break;
							}
							
							case "Update": 
							{
								Web.Default.DataBaseController.UpdateAction control= new Web.Default.DataBaseController.UpdateAction(context.Response.Output);
								
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.Update();
								control.After();
								break;
							}
							
							case "Tree": 
							{
								Web.Default.DataBaseController.TreeAction control= new Web.Default.DataBaseController.TreeAction(context.Response.Output);
								
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.Tree();
								control.After();
								break;
							}
							
							default: context.Response.Write("找不到类" + controller + "下的" + action + "方法");context.Response.End(); break;
						}
						break;
					}
				
				//relativeName="/Manage/IndexController"
				//RelativeDotName=".Manage.IndexController"
				case "/HelloWorldController":
					{
                        
						switch (action)
						{
							
							case "Write": 
							{
								Web.Default.HelloWorldController.WriteAction control= new Web.Default.HelloWorldController.WriteAction(context.Response.Output);
								
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.Write();
								control.After();
								break;
							}
							
							case "WriteHTML": 
							{
								Web.Default.HelloWorldController.WriteHTMLAction control= new Web.Default.HelloWorldController.WriteHTMLAction(context.Response.Output);
								string name=get["name"]==null?null:get["name"];string[] list=get["list"]==null?null:get["list"].Split('^');
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.WriteHTML(name,list);
								control.After();
								break;
							}
							
							case "WriteJson": 
							{
								Web.Default.HelloWorldController.WriteJsonAction control= new Web.Default.HelloWorldController.WriteJsonAction(context.Response.Output);
								
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.WriteJson();
								control.After();
								break;
							}
							
							case "Parameter": 
							{
								Web.Default.HelloWorldController.ParameterAction control= new Web.Default.HelloWorldController.ParameterAction(context.Response.Output);
								string name=get["name"]==null?null:get["name"];
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.Parameter(name);
								control.After();
								break;
							}
							
							case "Default": 
							{
								Web.Default.HelloWorldController.DefaultAction control= new Web.Default.HelloWorldController.DefaultAction(context.Response.Output);
								int id=get["id"]==null?1:int.Parse(get["id"]);
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.Default(id);
								control.After();
								break;
							}
							
							case "PostParameter": 
							{
								Web.Default.HelloWorldController.PostParameterAction control= new Web.Default.HelloWorldController.PostParameterAction(context.Response.Output);
								string say=get["say"]==null?null:get["say"];
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.PostParameter(say);
								control.After();
								break;
							}
							
							default: context.Response.Write("找不到类" + controller + "下的" + action + "方法");context.Response.End(); break;
						}
						break;
					}
				
				//relativeName="/Manage/IndexController"
				//RelativeDotName=".Manage.IndexController"
				case "/IndexController":
					{
                        
						switch (action)
						{
							
							case "Index": 
							{
								Web.Default.IndexController.IndexAction control= new Web.Default.IndexController.IndexAction(context.Response.Output);
								
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.Index();
								control.After();
								break;
							}
							
							default: context.Response.Write("找不到类" + controller + "下的" + action + "方法");context.Response.End(); break;
						}
						break;
					}
				
				//relativeName="/Manage/IndexController"
				//RelativeDotName=".Manage.IndexController"
				case "/ViewController":
					{
                        
						switch (action)
						{
							
							case "Index": 
							{
								Web.Default.ViewController.IndexAction control= new Web.Default.ViewController.IndexAction(context.Response.Output);
								
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.Index();
								control.After();
								break;
							}
							
							case "AutoComplete": 
							{
								Web.Default.ViewController.AutoCompleteAction control= new Web.Default.ViewController.AutoCompleteAction(context.Response.Output);
								
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.AutoComplete();
								control.After();
								break;
							}
							
							case "HTMLTag": 
							{
								Web.Default.ViewController.HTMLTagAction control= new Web.Default.ViewController.HTMLTagAction(context.Response.Output);
								
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.HTMLTag();
								control.After();
								break;
							}
							
							default: context.Response.Write("找不到类" + controller + "下的" + action + "方法");context.Response.End(); break;
						}
						break;
					}
				
				//relativeName="/Manage/IndexController"
				//RelativeDotName=".Manage.IndexController"
				case "/Common/Public":
					{
                        
						switch (action)
						{
							
							case "Success": 
							{
								Web.Default.Common.Public.SuccessAction control= new Web.Default.Common.Public.SuccessAction(context.Response.Output);
								string message=get["message"]==null?null:get["message"];string url=get["url"]==null?null:get["url"];int second=get["second"]==null?0:int.Parse(get["second"]);
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.Success(message,url,second);
								control.After();
								break;
							}
							
							case "Error": 
							{
								Web.Default.Common.Public.ErrorAction control= new Web.Default.Common.Public.ErrorAction(context.Response.Output);
								string message=get["message"]==null?null:get["message"];string url=get["url"]==null?null:get["url"];int second=get["second"]==null?0:int.Parse(get["second"]);
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.Error(message,url,second);
								control.After();
								break;
							}
							
							case "Header": 
							{
								Web.Default.Common.Public.HeaderAction control= new Web.Default.Common.Public.HeaderAction(context.Response.Output);
								string message=get["message"]==null?null:get["message"];
								control._cookies = new Common.Data.CookieManager(context);
								control._context = context;
								control._subdomain = subdomain;
								control._url=context.Request.RawUrl;
								control._get = get;
								control._app=app;
								control._controller = controller;
                                control._action = action;
								control.Before();
								control.Header(message);
								control.After();
								break;
							}
							
							default: context.Response.Write("找不到类" + controller + "下的" + action + "方法");context.Response.End(); break;
						}
						break;
					}
				
				default: context.Response.Write("找不到类" + controller); context.Response.End(); break; 
			}
        }
        
    }
}
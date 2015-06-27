using System;
using System.Collections.Generic;
using System.Web;
using System.Collections.Specialized;
using System.IO;
#if NET2
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
    public sealed class ExtensionAttribute : Attribute { }
}
#endif
namespace NFinal
{
    public class MagicViewBag
    {
        public void Add(object obj)
        { 
        
        }
        public void Add(object[] obj)
        { 
        
        }
    }
    //魔法函数,不允许继承.
    public class MagicAction
    {
#if NET2
#else
        public MagicViewBag ViewBag;
#endif
        public virtual string postEventArgumentID { get { return null; } }
        public virtual string postEventSourceID { get { return null; } }
        /// <summary>
        /// 插入Csharp代码段
        /// </summary>
        /// <param name="textFilePath">含有csharp代码的文本文件路径</param>
        public virtual void InsertCodeSegment(string textFilePath) { }
        /// <summary>
        /// 插入Csharp类文件函数中的代码段
        /// </summary>
        /// <param name="classFilePath">csharp类文件路径</param>
        /// <param name="methodName">类文件中的函数名</param>
        public virtual void InsertCodeSegment(string classFilePath, string methodName) { }
        /// <summary>
        /// 渲染html模板
        /// </summary>
        /// <param name="tplPath">模板相对路径,一般相对于App/Views/Default目录</param>
        public virtual void View(string tplPath) { }
        /// <summary>
        /// 渲染html模板
        /// </summary>
        /// <param name="name">模板样式</param>
        /// <param name="tplPath">模板相对路径,一般相对于App/Views/Default目录</param>
        public virtual void View(string name, string tplPath) { }
        public virtual void Redirect(string url)
        { 
            
        }
    }
    //Controller基类
    public class BaseAction : MagicAction
    {
        public HttpContext _context = null;
        public NameValueCollection _get = null;
        public TextWriter _tw = null;
        public string _subdomain;//二级域名
        public string _action;
        public string _controller;
        public string _app;
        public string _url;
        public NFinal.Session.Session _session = null;
        private bool _requiresSessionState =false;

        /// <summary>
        /// 插入Csharp代码段
        /// </summary>
        /// <param name="textFilePath">含有csharp代码的文本文件路径</param>
        public sealed override void InsertCodeSegment(string textFilePath)
        {
        }
        /// <summary>
        /// 插入Csharp类文件函数中的代码段
        /// </summary>
        /// <param name="classFilePath">csharp类文件路径</param>
        /// <param name="methodName">类文件中的函数名</param>
        public sealed override void InsertCodeSegment(string classFilePath, string methodName)
        { 
        }

        public virtual void Before()
        {
            
        }
        public virtual void After()
        { 
            
        }

        #region 初始化函数
        public BaseAction() { }
        
        public string MapPath(string url)
        {
            return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName + "\\" + url.Trim('/').Replace('/', '\\');
        }
        public BaseAction(string fileName)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(MapPath(fileName), false, System.Text.Encoding.UTF8);
            this._tw = sw;
        }
        public void Close()
        {
            this._tw.Close();
        }
        public BaseAction(System.IO.TextWriter tw)
        {
            this._tw = tw;
        }
        public BaseAction(System.Web.HttpContext context)
        {
            this._tw = context.Response.Output;
            this._context = context;
            if (this is System.Web.SessionState.IRequiresSessionState)
            {
                this._session = new Session.Session(context, new TimeSpan(7, 0, 0, 0));
                this._requiresSessionState = true;
            }
        }
        //设置Session
        public void SetSession(string key,string val)
        {
            if(this._requiresSessionState)
            {
                this._session.SetSession(key,val);
            }
        }
        //获取Session
        public string GetSession(string key)
        {
            if (this._requiresSessionState)
            {
                return this._session.GetSession(key);
            }
            else
            {
                return null;
            }
        }
        #endregion
        #region 输出函数
        public void WriteLine(string val)
        {
            _tw.Write(val);
            _tw.Write("<br/>");
        }
        public void Write(object obj)
        {
            _tw.Write(obj.ToString());
        }
        public void Write(string val)
        {
            _tw.Write(val);
        }
        #endregion
        #region Cookie操作
        protected string GetCookie(string key)
        { 
            return  _context.Request.Cookies[key].Value;
        }
        protected void SetCookie(string key, string value)
        { 
            HttpCookie cookie=new HttpCookie(key,value);
            _context.Response.Cookies.Add(cookie);
        }
        protected void ClearCookie(string key)
        {
            _context.Response.Cookies.Remove(key);
        }
        #endregion
        #region Ajax返回数据
        public void AjaxReturn(string json)
        {
            AjaxReturn(json, 1, "");
        }
        public void AjaxReturn(string json, int code)
        {
            AjaxReturn(json, code, "");
        }
        public void AjaxReturn(string json, int code, string msg)
        {
            this._context.Response.ContentType = "application/json";
            this._context.Response.Write(string.Format("{{\"code\":{0},\"msg\":\"{1}\",\"result\":{2}}}", code, msg, json));
        }
        public void AjaxReturn(int code, string msg)
        {
            this._context.Response.ContentType = "application/json";
            this._context.Response.Write(string.Format("{{\"code\":{0},\"msg\":\"{1}\"}}", code, msg));
        }
        #endregion
        #region 获取URL
        /// <summary>
        /// 获取动态链接
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string U(string url)
        {
            int pos = url.IndexOf('/');
            if (pos == 0)
            {
                return _app + url;
            }
            else if (pos > 0)
            {
                return _app + "/"+url;
            }
            else
            {
                return _app+_controller+"/"+url;
            }
        }
        /// <summary>
        /// 获取静态页链接
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string A(string url)
        {
            int pos = url.IndexOf('/');
            if (pos == 0)
            {
                return _app +"/HTML"+ url;
            }
            else if (pos > 0)
            {
                return _app + "/HTML/" + url;
            }
            else
            {
                return _app +"/HTML"+ _controller + "/" + url;
            }
        }
        #endregion

        //魔法函数
        public sealed override void View(string tplPath)
        { 
            
        }
        public sealed override void View(string name, string tplPath)
        { 
            
        }
        public sealed override void Redirect(string url)
        {
            Main main = new Main();
            url = "/" + url.TrimStart('/');
            main.Run(_context, url);
        }
    }
}
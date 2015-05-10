using System;
using System.Collections.Generic;
using System.Web;
using System.Collections.Specialized;
using System.IO;

namespace NFinal
{
    //Controller基类
    public class BaseAction
    {
        public HttpContext _context = null;
        public NameValueCollection _get = null;
        public TextWriter _tw = null;
        public string _action;
        public string _controller;
        public string _app;
        public string _url;
        public NFinal.Session.Session _session = null;
        private bool _requiresSessionState =false;
        public static string ToJson(NFinal.DB.List<NFinal.DB.Struct> str)
        {
            System.IO.StringWriter tw = new System.IO.StringWriter();
            WriteJson(str, tw);
            string result = tw.ToString();
            tw.Close();
            return result;
        }
        public static void WriteJson(NFinal.DB.List<NFinal.DB.Struct> str, System.IO.TextWriter tw)
        {
            if (str == null)
            {
                tw.Write("null");
            }
            else
            {
                tw.Write("[");
                if (str.Count > 0)
                {
                    for (int i = 0; i < str.Count; i++)
                    {
                        if (i != 0)
                        {
                            tw.Write(",");
                        }
                        str[i].WriteJson(tw);
                    }
                }
                tw.Write("]");
            }
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
            return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName + "\\" + url.Trim('/').Replace('/', '\\');
        }
        public BaseAction(string fileName)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(MapPath(fileName), false, System.Text.Encoding.UTF8);
            this._tw = sw;
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
        protected void Write(string val)
        {
            _tw.Write(val);
        }
        protected void Write(int val)
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
        #region URL跳转
        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="url"></param>
        public static void Success(string msg, string url)
        {
            Success(msg, url, 3);
        }
        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="url"></param>
        /// <param name="second"></param>
        public static void Success(string msg, string url, int second)
        { 
        
        }
        /// <summary>
        /// 返回失败
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="url"></param>
        public static void Error(string msg, string url)
        {
            Success(msg, url, 3);
        }
        /// <summary>
        /// 返回失败
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="url"></param>
        /// <param name="second"></param>
        public static void Error(string msg, string url, int second)
        {

        }
        #endregion
        //魔法函数
        public static void View(string tplPath)
        { 
            
        }
        public static void View(string name, string tplPath)
        { 
            
        }
    }
}
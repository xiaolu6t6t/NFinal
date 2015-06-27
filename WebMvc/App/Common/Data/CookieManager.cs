using System;
using System.Collections.Generic;
using System.Web;

namespace WebMvc.App.Common.Data
{
    public class CookieManager
    {
        private HttpContext _context;
        private CookieInfo _cookieInfo;
        private bool _isStatic;
        public CookieManager()
        {
            this._context = null;
            this._isStatic = true;
            _cookieInfo = new CookieInfo();
        }
        public CookieManager(HttpContext context)
        {
            this._context = context;
            if (context == null)
            {
                this._isStatic = true;
                _cookieInfo = new CookieInfo();
            }
            else
            {
                this._isStatic = false;
            }
        }
        
        public string name
        {
            get
            {
                if (_isStatic)
                {
                    return _cookieInfo.name;
                }
                else
                {
                    if (_context.Request.Cookies["name"] == null)
                    {
                        return _cookieInfo.name;;
                    }
                    else
                    {
                        return _context.Request.Cookies["name"].Value;
                    }
                }
            }
            set
            {
                if (_isStatic)
                {
                    _cookieInfo.name = value;
                }
                else
                {
                    if (_context.Response.Cookies["name"] == null)
                    {
                        HttpCookie cookie = new HttpCookie("name", value.ToString());
                        _context.Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        _context.Response.Cookies["name"].Value = value.ToString();
                    }
                }
            }
        }
        
        public bool islogin
        {
            get
            {
                if (_isStatic)
                {
                    return _cookieInfo.islogin;
                }
                else
                {
                    if (_context.Request.Cookies["islogin"] == null)
                    {
                        return _cookieInfo.islogin;;
                    }
                    else
                    {
                        return Convert.ToBoolean(_context.Request.Cookies["islogin"].Value);
                    }
                }
            }
            set
            {
                if (_isStatic)
                {
                    _cookieInfo.islogin = value;
                }
                else
                {
                    if (_context.Response.Cookies["islogin"] == null)
                    {
                        HttpCookie cookie = new HttpCookie("islogin", value.ToString());
                        _context.Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        _context.Response.Cookies["islogin"].Value = value.ToString();
                    }
                }
            }
        }
        
        public int age
        {
            get
            {
                if (_isStatic)
                {
                    return _cookieInfo.age;
                }
                else
                {
                    if (_context.Request.Cookies["age"] == null)
                    {
                        return _cookieInfo.age;;
                    }
                    else
                    {
                        return Convert.ToInt32(_context.Request.Cookies["age"].Value);
                    }
                }
            }
            set
            {
                if (_isStatic)
                {
                    _cookieInfo.age = value;
                }
                else
                {
                    if (_context.Response.Cookies["age"] == null)
                    {
                        HttpCookie cookie = new HttpCookie("age", value.ToString());
                        _context.Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        _context.Response.Cookies["age"].Value = value.ToString();
                    }
                }
            }
        }
        
    }
}
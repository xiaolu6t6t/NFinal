using System;
using System.Collections.Generic;
using System.Web;

namespace {$project}.{$app}.Common.Data
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
        <vt:foreach from="$declarations" item="declaraton">
        public {$declaraton.typeName} {$declaraton.varName}
        {
            get
            {
                if (_isStatic)
                {
                    return _cookieInfo.{$declaraton.varName};
                }
                else
                {
                    if (_context.Request.Cookies["{$declaraton.varName}"] == null)
                    {
                        return _cookieInfo.{$declaraton.varName};;
                    }
                    else
                    {
                        return {$declaraton.covertMethod};
                    }
                }
            }
            set
            {
                if (_isStatic)
                {
                    _cookieInfo.{$declaraton.varName} = value;
                }
                else
                {
                    if (_context.Response.Cookies["{$declaraton.varName}"] == null)
                    {
                        HttpCookie cookie = new HttpCookie("{$declaraton.varName}", value.ToString());
                        _context.Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        _context.Response.Cookies["{$declaraton.varName}"].Value = value.ToString();
                    }
                }
            }
        }
        </vt:foreach>
    }
}
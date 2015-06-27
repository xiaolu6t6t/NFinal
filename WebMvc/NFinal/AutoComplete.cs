using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;

namespace NFinal
{
    public class AutoComplete
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMvc.App.Common.UserControl
{
    public class Footer
    {
        public delegate void __Render__(Footer footer);
        public __Render__ __render__;
        public string message;
    }
}
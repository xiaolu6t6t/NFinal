using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMvc.App.Common.UserControl
{
    public partial class Navigator
    {
        public delegate void __Render__(Navigator x);
        public __Render__ __render__ = null;
        public int pageIndex = 0;
        public int pageCount = 0;
        public int pageSize = 0;
    }
}
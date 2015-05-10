using System;
using System.Collections.Generic;
using System.Web;
using System.IO;

namespace NFinal.Tool
{
    public class Json
    {
        public int code;
        public string result;
        public string msg;

        public TextWriter tw;

        public Json(TextWriter tw)
        {
            if (tw == null)
            {
                tw = new StringWriter();
            }
            else
            {
                this.tw = tw;
            }
        }

        public void AjaxRetun(string result)
        { 
            
        }
    }
}
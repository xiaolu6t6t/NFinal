using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace WebMvc.App.Common
{
    public class User:NFinal.DB.Struct
    {
        public int id;
        public string name;
        public override void WriteJson(System.IO.TextWriter tw)
        {
            tw.Write("{");
            tw.Write("\"id\":");
            tw.Write(id);
            tw.Write("\"name\":");
            tw.Write("\"");
            WriteString(name,tw);
            tw.Write("\"");
            tw.Write(",");
            tw.Write("}");
        }
    }
}
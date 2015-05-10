using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace NFinal.DB
{
    public class Struct
    {
        public string ToJson()
        {
            System.IO.StringWriter tw = new System.IO.StringWriter();
            WriteJson(tw);
            string result = tw.ToString();
            tw.Close();
            return result;
        }
        public virtual void WriteJson(System.IO.TextWriter tw)
        {
        }
        public virtual void WriteJson(NFinal.DB.List<NFinal.DB.Struct> str, System.IO.TextWriter tw)
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
        /// <summary>
        /// 字符串反转义
        /// </summary>
        /// <param name="text">字符串</param>
        /// <returns>返回csharp中的字符串表示</returns>
        public void WriteString(string text,System.IO.TextWriter tw)
        {
            char[] temp_old = text.ToCharArray();
            for (int i = 0; i < temp_old.Length; i++)
            {
                switch (temp_old[i])
                {
                    case '\'': tw.Write("\\\'"); break;
                    case '\"': tw.Write("\\\""); break;
                    case '\\': tw.Write("\\\\"); break;
                    case '\0': tw.Write("\\0"); break;
                    case '\a': tw.Write("\\a"); break;
                    case '\b': tw.Write("\\b"); break;
                    case '\f': tw.Write("\\f"); break;
                    case '\n': tw.Write("\\n"); break;
                    case '\r': tw.Write("\\r"); break;
                    case '\t': tw.Write("\\t"); break;
                    case '\v': tw.Write("\\v"); break;
                    default: tw.Write(temp_old[i]); break;
                }
            }
        }
    }
}
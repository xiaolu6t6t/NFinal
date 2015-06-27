using System;
using System.Collections.Generic;
using System.Web;

namespace NFinal.DB
{
    public class NList<T> : System.Collections.Generic.List<T>
    {
        public string ToJson(IEnumerator<NFinal.DB.Struct> str)
        {
            System.IO.StringWriter tw = new System.IO.StringWriter();
            WriteJson(str, tw);
            string result = tw.ToString();
            tw.Close();
            return result;
        }
        public void WriteJson(IEnumerator<NFinal.DB.Struct> str, System.IO.TextWriter tw)
        {
            if (str == null)
            {
                tw.Write("null");
            }
            else
            {
                tw.Write("[");
                str.Reset();
                bool isFirst = true;
                while (str.MoveNext())
                {
                    if (!isFirst)
                    {
                        tw.Write(",");
                    }
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    str.Current.WriteJson(tw);
                }
                tw.Write("]");
            }
        }
    }
}
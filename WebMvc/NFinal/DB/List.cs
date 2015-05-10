using System;
using System.Collections.Generic;
using System.Web;

namespace NFinal.DB
{
    public class List<T> : System.Collections.Generic.List<T>
    {
        public void AddNewField(string filedName, Type t)
        { 
            
        }
        public string ToJson(IEnumerator<dynamic> str)
        {
            System.IO.StringWriter tw = new System.IO.StringWriter();
            WriteJson(str,tw);
            string result = tw.ToString();
            tw.Close();
            return result;
        }
        public void WriteJson(IEnumerator<dynamic> str, System.IO.TextWriter tw)
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
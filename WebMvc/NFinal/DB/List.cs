using System;
using System.Collections.Generic;
using System.Web;

namespace NFinal.DB
{
    public class List<T> : System.Collections.Generic.List<T>
    {
        /// <summary>
        /// 添加一个字段
        /// </summary>
        /// <param name="filedName">字段名</param>
        /// <param name="t">字段类型</param>
        public void AddNewField(string fieldName, Type t)
        { 
            
        }
        /// <summary>
        /// 添加dynamic字段
        /// </summary>
        /// <param name="fieldName"></param>
        public void AddNewField(string fieldName)
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
using System;
using System.Collections.Generic;
using System.Web;

namespace NFinal.DB
{
    public class SqlObject
    {
        public object obj = null;
        public SqlObject(object obj)
        {
            this.obj = obj;
        }
        public override string ToString()
        {
            if (obj != null)
            {
                return obj.ToString();
            }
            else
            {
                return null;
            }
        }
        public byte ToByte()
        {
            if (obj != null)
            {
                return Convert.ToByte(obj);
            }
            else
            {
                return 0;
            }
        }
        public int ToInt()
        { 
            if(obj!=null)
            {
                return Convert.ToInt32(obj);
            }
            else
            {
                return 0;
            }
        }
        public long ToLong()
        {
            if (obj != null)
            {
                return Convert.ToInt64(obj);
            }
            else
            {
                return 0;
            }
        }
        public float ToFloat()
        { 
            if(obj!=null)
            {
                return Convert.ToSingle(obj);
            }
            else
            {
                return 0;
            }
        }
        public double ToDouble()
        {
            if (obj != null)
            {
                return Convert.ToDouble(obj);
            }
            else
            {
                return 0;
            }
        }
        public decimal ToDecimal()
        {
            if (obj != null)
            {
                return Convert.ToDecimal(obj);
            }
            else
            {
                return 0;
            }
        }
        public DateTime ToDateTime()
        {
            if (obj != null)
            {
                return Convert.ToDateTime(obj);
            }
            else
            {
                return DateTime.Now;
            }
        }
    }
}
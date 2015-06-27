using System;
using System.Collections.Generic;
using System.Text;

namespace NFinal.DB.Coding
{

    /// <summary>
    /// 表信息
    /// </summary>
    public class Table
    {
        public string DbTypeEnumName="";
        public string UsingNameSpace="";
        public string FieldClassName = "";
        public string ParameterClassName = "";
        public string DbHelperClassName = "";
        public string name;
        public string nameCs;
        public string nameJs;
        public bool hasId;
        public Field id=null;
        public System.Collections.Generic.List<Field> fields = null;

        public Table()
        {
            name = "";
        }

        public Table(string name)
        {
            this.name = name;
        }
    }
}

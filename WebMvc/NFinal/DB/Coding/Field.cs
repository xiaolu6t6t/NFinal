using System;
using System.Collections.Generic;
using System.Text;

namespace NFinal.DB.Coding
{

    /// <summary>
    /// 数据库字段信息类
    /// </summary>
    public class Field
    {
        public string name;
        public string nameCs;
        public string nameJs;
        public string structFieldName;

        public int length;
        public int octLength;
        public int position;
        public bool allowNull;
        public bool isId;
        public bool hasDefault;
        public string defautlValue;
        public string dbType;
        public string simpleType;
        public string sqlType;
        public string localType;

        public CsTypeLink csTypeLink;
        public Field()
        {

        }
    }
}

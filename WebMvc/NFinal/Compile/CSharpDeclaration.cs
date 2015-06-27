using System;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

namespace NFinal.Compile
{
    public class CSharpDeclaration
    {
        //类型
        public string typeName;
        //编译器内部类型
        public string compileTypeName;
        //变量名
        public string varName;
        //表达式
        public string expression;
        //转换函数
        public string covertMethod;
    }
}
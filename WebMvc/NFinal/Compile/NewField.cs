using System;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

namespace NFinal.Compile
{
    public struct StructField
    {
        public string typeName;
        public string varName;
        public int index;
        public int length;
        public string csharpCode;

        public bool isString;
        public bool isSimple;
        public bool isList;
    }
    public class NewField
    {
        public string varName;
        public NewField(string varName)
        {
            this.varName = varName;
        }
        public bool IsString(string typeName)
        {
            bool isString = true;
            switch (typeName)
            {
                case "int": isString = false; break;
                case "float": isString = false; break;
                case "double": isString = false; break;
                case "long": isString = false; break;
                default: isString = true; break;
            }
            return isString;
        }
        public List<StructField> GetFields(ref string csharpCode,string MethodName)
        {
            string partern = varName + @"\s*.\s*AddNewField\s*\(""([^,""\(\)]+)""\s*,\s*typeof\(\s*([^()]+)\)\s*\)\s*;";
            Regex addNewFieldReg = new Regex(partern);
            MatchCollection addNewFieldMats = addNewFieldReg.Matches(csharpCode);
            StructField field;
            List<StructField> structFields = new List<StructField>();
            string note="";
            if (addNewFieldMats.Count>0)
            {
                partern =@"(List\s*<\s*dynamic\s*>)?(dynamic)?";
                Regex dynamicTypeReg = new Regex(partern);
                Match dynamicTypeMat;
                for (int i = 0; i < addNewFieldMats.Count; i++)
                {
                    field = new StructField();
                    field.index = addNewFieldMats[i].Index;
                    field.length = addNewFieldMats[i].Length;
                    field.varName = addNewFieldMats[i].Groups[1].Value;
                    field.typeName = addNewFieldMats[i].Groups[2].Value;
                    field.isString = IsString(field.typeName);
                    //原位置替换成注释语句
                    csharpCode = csharpCode.Remove(field.index,field.length);
                    note= string.Format("/*field:{0}.{1}*/",varName,field.varName);
                    csharpCode =csharpCode.Insert(field.index,note.PadRight(field.length));
                   
                    dynamicTypeMat = dynamicTypeReg.Match(field.typeName);
                    field.isSimple = true;
                    field.isList = false;
                    if (dynamicTypeMat.Success)
                    {
                        field.isSimple = false;
                        if (dynamicTypeMat.Groups[1].Success)
                        {
                            field.isList = true;
                            field.typeName = "NFinal.DB.NList<__" + MethodName + "_" + field.varName + "__>";
                        }
                        else if (dynamicTypeMat.Groups[2].Success)
                        {
                            field.typeName = "__" + MethodName + "_" + field.varName + "__";
                        }
                    }
                    structFields.Add(field);
                }
            }
            return structFields;
        }
    }
}
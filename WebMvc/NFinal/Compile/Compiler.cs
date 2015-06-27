using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NFinal.Compile
{
    /// <summary>
    /// Csharp代码分析类
    /// </summary>
    public class Compiler
    {
        /// <summary>
        /// 转换GET参数的代码
        /// </summary>
        /// <param name="type">参数中的类型</param>
        /// <param name="name">参数名</param>
        /// <returns></returns>
        private string BuildGetParameterCode(string type,bool isArray, string name,bool hasDefaultValue,string defaultValue)
        {
            string code = "";
            //切数组时用的字符串,字符串内部可以用%5E代替
            char splitChar = '^';
            if (!isArray)
            {
                switch (type.ToLower())
                {
                    case "string": code = string.Format("{0} {1}=get[\"{1}\"]==null?{2}:get[\"{1}\"];", type, name,hasDefaultValue?defaultValue:"null"); break;
                    case "int": code = string.Format("{0} {1}=get[\"{1}\"]==null?{2}:int.Parse(get[\"{1}\"]);", type, name,hasDefaultValue?defaultValue:"0"); break;
                    case "float": code = string.Format("{0} {1}=get[\"{1}\"]==null?{2}:float.Parse(get[\"{1}\"]);", type, name,hasDefaultValue?defaultValue:"0"); break;
                    case "double": code = string.Format("{0} {1}=get[\"{1}\"]==null?{2}:double.Parse(get[\"{1}\"]);", type, name,hasDefaultValue?defaultValue:"0"); break;
                    case "datetime": code = string.Format("{0} {1}=get[\"{1}\"]==null?{2}:DateTime.Parse(get[\"{1}\"]);", type, name,hasDefaultValue?defaultValue:"DateTime.Now"); break;
                    default: code= string.Format("/*对不起,参数{1}不支持{0}类型*/", type, name); break;
                }
            }
            else
            {
                switch (type.ToLower())
                {  
                    case "string": code = string.Format("string[] {1}=get[\"{1}\"]==null?{2}:get[\"{1}\"].Split('{3}');",type,name,hasDefaultValue?defaultValue:"null",splitChar);break;
                    case "int": code = string.Format("{0}[] {1}=get[\"{1}\"]==null?{2}:Array.ConvertAll<string, int>(get[\"{1}\"].Split('{3}'), delegate(string s) {{ return int.Parse(s); }});",type,name,hasDefaultValue?defaultValue:"null",splitChar);break;
                    case "float": code = string.Format("{0}[] {1}=get[\"{1}\"]==null?{2}:Array.ConvertAll<string, float>(get[\"{1}\"].Split('{3}'), delegate(string s) {{ return float.Parse(s); }});", type, name, hasDefaultValue ? defaultValue : "null", splitChar); break;
                    case "double": code = string.Format("{0}[] {1}=get[\"{1}\"]==null?{2}:Array.ConvertAll<string, double>(get[\"{1}\"].Split('{3}'), delegate(string s) {{ return double.Parse(s); }});", type, name, hasDefaultValue ? defaultValue : "null", splitChar); break;
                    case "datetime": code = string.Format("{0}[] {1}=get[\"{1}\"]==null?{2}:Array.ConvertAll<string, DateTime>(get[\"{1}\"].Split('{3}'), delegate(string s) {{ return DateTime.Parse(s); }});", type, name, hasDefaultValue ? defaultValue : "null", splitChar); break;
                    default: code = string.Format("/*对不起,参数{1}不支持{0}类型*/", type, name); break;
                }
            }
            return code;
        }

        /// <summary>
        /// 得到csharp代码中{}中的内容,如类内容,方法体等
        /// </summary>
        /// <param name="csharptCode">Csharp代码</param>
        /// <param name="index">{符号开始的地方</param>
        /// <returns></returns>
        private string GetContent(string csharptCode, int index)
        {
            char[] buffer = csharptCode.ToCharArray();
            StringBuilder sb = new StringBuilder();
            int begin=0;
            int begin_index = index+1;
            int end=0;
            int end_index = 0;
  
            for (int i = index; i < buffer.Length; i++)
            {
                if (buffer[i] == '{')
                {
                    begin++;
                }
                if(buffer[i]=='}')
                {
                    end++;
                }
                if (begin == end && begin != 0)
                {
                    end_index = i-1;
                    break;
                }
            }
            if (end_index > begin_index)
            {
                return csharptCode.Substring(begin_index, end_index - begin_index+1);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 取得类文件中的类,方法等信息
        /// </summary>
        /// <param name="fileName">类文件的路径</param>
        /// <param name="encoding">编码方式</param>
        /// <returns></returns>
        public FileData GetFileData(string appRoot,string app,string fileName, System.Text.Encoding encoding)
        {
            StreamReader sr = new StreamReader(fileName,encoding);
            string csharpCode = sr.ReadToEnd();
            sr.Close();
            CodeSegment codeSeg = new CodeSegment(appRoot, app);
            csharpCode= codeSeg.Parse(csharpCode);
            return GetFileData(fileName, csharpCode);
        }
        /// <summary>
        /// 把代码中的注释全部替换为空格
        /// </summary>
        /// <param name="csharpCode"></param>
        /// <returns></returns>
        public string DeleteComment(string csharpCode)
        {
            string parttern1 = @"/\*[^*]*\*+(?:[^/*][^*]*\*+)*/";
            string parttern2 = @"//[^\r\n]*";

            Regex reg1=new Regex(parttern1);
            MatchCollection mac1=reg1.Matches(csharpCode);
            for(int i=0;i<mac1.Count;i++)
            {
                csharpCode=csharpCode.Remove(mac1[i].Index,mac1[i].Length);
                csharpCode=csharpCode.Insert(mac1[i].Index,string.Empty.PadLeft(mac1[i].Length));
            }
            Regex reg2=new Regex(parttern2);
            MatchCollection mac2=reg2.Matches(csharpCode);
            for(int i=0;i<mac2.Count;i++)
            {
                csharpCode=csharpCode.Remove(mac2[i].Index,mac2[i].Length);
                csharpCode=csharpCode.Insert(mac2[i].Index,string.Empty.PadLeft(mac2[i].Length));
            }
            return csharpCode;
        }
        /// <summary>
        /// 获取类文件中的类,方法等信息
        /// </summary>
        /// <param name="fileName">类文件的路径</param>
        /// <param name="csharpCode">类文件中的代码</param>
        /// <returns></returns>
        public FileData GetFileData(string fileName,string csharpCode)
        {
            FileData fileData = new FileData();
            fileData.fileName =fileName;
            fileData.csharpCode=csharpCode;
            ClassData classData = null;
            MethodData methodData = null;
            ParameterData parameterData = null;
            string patern = @"namespace\s+(\S+)\s*\{";
            Regex reg=new Regex(patern);
            Match mFile,mParameter,mReturnVarName;
            MatchCollection mClass,mMethod;
            string[] parameters = null;
            //查找命名空间
            if (reg.IsMatch(csharpCode))
            {
                mFile = reg.Match(csharpCode);
                fileData.start = mFile.Index;
                fileData.length = mFile.Length;
                fileData.position=mFile.Index+mFile.Length;
                fileData.Content = GetContent(csharpCode, mFile.Index + mFile.Length - 1);
                fileData.nameSpace = mFile.Groups[1].Value;
                fileData.projectName = fileData.nameSpace.Split('.')[0];
                patern = @"public class\s+([^\s:]+)(?:\s*:\s*(\S+))?\s*\{";
                reg=new Regex(patern);
                mClass = reg.Matches(fileData.Content);
                //查找类
                if (mClass.Count>0)
                {
                    classData = null;
                    for (int i = 0; i < mClass.Count; i++)
                    {
                        classData = new ClassData();
                        classData.start = fileData.position + mClass[i].Index;
                        classData.length = mClass[i].Length;

                        classData.position =fileData.position+ mClass[i].Index + mClass[i].Length;
                        classData.Content = GetContent(fileData.Content, mClass[i].Index + mClass[i].Length - 1);

                        classData.name = mClass[i].Groups[1].Value;
                        classData.fullName = fileData.nameSpace +"."+ classData.name;
                        
                        classData.baseName = mClass[i].Groups[2].Value;
                        patern = @"(public|private|protected)\s+(?:(?:override|new)\s+)*(\S+)\s+(\S+)\s*\(([^\(\)]*)\)\s*\{";
                        reg = new Regex(patern);
                        mMethod = reg.Matches(classData.Content);
                        //查找方法
                        if (mMethod.Count>0)
                        {
                            methodData = null;
                            for (int j = 0; j < mMethod.Count; j++)
                            {
                                methodData = new MethodData();
                                
                                methodData.publicStr = mMethod[j].Groups[1].Value;
                                methodData.isPublic = methodData.publicStr == "public";
                                methodData.returnType = mMethod[j].Groups[2].Value;
                                methodData.returnTypeIndex = mMethod[j].Groups[2].Index;
    
                                methodData.name = mMethod[j].Groups[3].Value;
                                methodData.parameters = mMethod[j].Groups[4].Value;
                                methodData.parametersIndex =classData.position + mMethod[j].Groups[4].Index;
                                methodData.parametersLength = mMethod[j].Groups[4].Length;

                                if(!string.IsNullOrEmpty(methodData.parameters))
                                {
                                    //查找参数组
                                    parameters= methodData.parameters.Split(',');
                                    for (int k = 0; k < parameters.Length; k++)
                                    {
                                        patern = @"^\s*([a-zA-Z0-9_]+)(\s*\[\])?\s+([a-zA-Z0-9_]+)(\s*=\s*([^=]+))?\s*$";
                                        reg=new Regex(patern);
                                        mParameter = reg.Match(parameters[k]);
                                        if (mParameter.Success)
                                        {
                                            parameterData = new ParameterData();
                                            parameterData.type= mParameter.Groups[1].Value;
                                            parameterData.isArray = mParameter.Groups[2].Success;
                                            parameterData.name= mParameter.Groups[3].Value;
                                            parameterData.hasDefaultValue = mParameter.Groups[5].Success;
                                            parameterData.defaultValue = mParameter.Groups[5].Value;
                                            parameterData.getParamterCode = BuildGetParameterCode(parameterData.type,
                                                parameterData.isArray ,parameterData.name,parameterData.hasDefaultValue,parameterData.defaultValue);
                                            methodData.parameterNames += parameterData.name + ",";
                                            if (parameterData.hasDefaultValue)
                                            {
                                                methodData.parameterTypeAndNames += parameters[k].Remove(mParameter.Groups[4].Index, mParameter.Groups[4].Length) + ",";
                                            }
                                            else
                                            {
                                                methodData.parameterTypeAndNames += parameters[k] + ",";
                                            }
                                            methodData.parameterDataList.Add(parameterData);
                                        }
                                    }
                                    methodData.parameterNames = methodData.parameterNames.TrimEnd(',');
                                    methodData.parameterTypeAndNames = methodData.parameterTypeAndNames.TrimEnd(',');
                                }
                                methodData.start = classData.position + mMethod[j].Index;
                                methodData.length = mMethod[j].Length;

                                methodData.position = classData.position + mMethod[j].Index + mMethod[j].Length;
                                methodData.Content = GetContent(classData.Content, mMethod[j].Index+ mMethod[j].Length - 1);

                                patern = @"return\s+([^\s;]+)\s*;";
                                reg = new Regex(patern, RegexOptions.RightToLeft);
                                mReturnVarName = reg.Match(methodData.Content);
                                if (mReturnVarName.Success)
                                {
                                    methodData.returnVarName = mReturnVarName.Groups[1].Value;
                                }
                                classData.MethodDataList.Add(methodData);
                            }
                        }
                        fileData.ClassDataList.Add(classData);
                    }
                }
            }
            return fileData;
        }
    }
    
    /// <summary>
    /// csharp文件实体类
    /// </summary>
    public class FileData
    {
        public int start = 0;
        public int length = 0;
        public int position = 0;
        public string fileName = string.Empty;
        public string name = string.Empty;
        public string nameSpace = string.Empty;
        public string projectName = string.Empty;
        public string csharpCode = string.Empty;
        public string Content = string.Empty;
        
        public List<ClassData> ClassDataList = new List<ClassData>();
        public ClassData GetClassData(string className)
        {
            if (ClassDataList.Count > 0)
            {
                for (int i = 0; i < ClassDataList.Count; i++)
                {
                    if (ClassDataList[i].name == className)
                    {
                        return ClassDataList[i];
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 类实体类
    /// </summary>
    public class ClassData
    {
        //类头开始,及长度
        public int start = 0;
        public int length = 0;

        //类内容开始,及内容
        public int position = 0;
        public string Content = string.Empty;

        public string name = string.Empty;
        public string baseName = string.Empty;
        
        public string fullName = string.Empty;
        //relativeName="/Manage/IndexController"
        public string relativeName = string.Empty;
        //RelativeDotName=".Manage.IndexController"
        public string relativeDotName = string.Empty;
        
        public List<MethodData> MethodDataList=new List<MethodData>();
    }

    /// <summary>
    /// 方法实体类
    /// </summary>
    public class MethodData
    {
        //方法头开始,及长度
        public int start = 0;
        public int length = 0;

        //方法体开始,及内容
        public int position = 0;
        public string Content = string.Empty;

        public string publicStr = string.Empty;
        public bool isPublic = false;
        public string returnType = string.Empty;
        public int returnTypeIndex = 0;
        public int returnTypeLength = 0;
        public string returnVarName = string.Empty;
        public string name=string.Empty;
        public string parameters = string.Empty;
        public string parameterNames = string.Empty;
        public string parameterTypeAndNames = string.Empty;
        public int parametersIndex = 0;
        public int parametersLength = 0;
        
        

        //数据库方法
        public List<DbFunctionData> dbFunctions = new List<DbFunctionData>();
        //View方法
        public List<ViewData> views = new List<ViewData>();
        //函数参数
        public List<ParameterData> parameterDataList=new List<ParameterData>();
    }

    /// <summary>
    /// 方法参数类
    /// </summary>
    public class ParameterData
    {
        public int position = 0;
        public string type = string.Empty;
        public bool isArray = false;
        public string name = string.Empty;
        public bool hasDefaultValue = false;
        public string defaultValue = string.Empty;
        public string getParamterCode = string.Empty;
    }
}
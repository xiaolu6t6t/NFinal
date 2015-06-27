using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;

namespace NFinal.Compile
{
    //单独生成
    public class AutoCompleteCompiler
    {
        public AutoCompleteCompiler()
        {
        }
        public void Compile(string baseName, MethodData methodData,string StructDatas, List<ViewData> viewDatas,string nameSpace,string ClassName, Config config)
        {
            ViewData viewData = null;
            //模板目录,
            string tplDir = "";
            //模板文件
            string tplFileName = "";
            for (int i = 0; i < viewDatas.Count; i++)
            {
                viewData = viewDatas[i];
                bool IsVarName = false;
                //获取模板样式变量或字符串
                if (string.IsNullOrEmpty(viewData.templateStyle))
                {
                    //此处当字符串来算.
                    IsVarName = false;
                    viewData.templateStyle = "\"" + config.defaultStyle.Trim('/') + "\"";
                }
                else
                {
                    if (viewData.templateStyle.IndexOf('\"') > -1)
                    {
                        IsVarName = false;
                    }
                    else
                    {
                        IsVarName = true;
                    }
                }
                //如果是字符串则直接取值就行了.
                string[] templateStyle = null;
                string[] styles = Directory.GetDirectories(Frame.MapPath(config.Views));
                int defaultTemplateStyleIndex = 0;
                string[] sharpCodes = null;
                if (styles.Length > 0)
                {
                    sharpCodes = new string[styles.Length];
                    templateStyle = new string[styles.Length];
                    for (int j = 0; j < styles.Length; j++)
                    {
                        templateStyle[j] = new DirectoryInfo(styles[j]).Name;
                        //如果不是变量而是字符串
                        if (!IsVarName)
                        {
                            if (viewData.templateStyle.Trim('\"') == templateStyle[j])
                            {
                                defaultTemplateStyleIndex = j;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        //如果参数是字符串
                        else
                        {
                            if (templateStyle[j] == config.defaultStyle.Trim('/'))
                            {
                                defaultTemplateStyleIndex = j;
                            }
                        }
                        tplDir = config.ChangeControllerName(config.Views + templateStyle[j] + '/', (nameSpace + '.' + ClassName).Replace('.', '/')) + "/";

                        if (!string.IsNullOrEmpty(viewData.template))
                        {
                            //如果参数为/IndexController/Index.aspx
                            if (viewData.template.IndexOf('/') == 0)
                            {
                                tplFileName = Frame.MapPath(config.Views + templateStyle[j] + viewData.template);
                            }
                            //如果参数为IndexController/Index.aspx
                            else if (viewData.template.IndexOf('/') > 0)
                            {
                                tplFileName = Frame.MapPath(tplDir.Substring(0, tplDir.Length - ClassName.Length - 1) + viewData.template);
                            }
                            //如果参数为Index.aspx
                            else
                            {
                                tplFileName = Frame.MapPath(tplDir + viewData.template);
                            }
                        }
                        //如果参数为空
                        else
                        {
                            tplFileName = Frame.MapPath(tplDir + methodData.name + ".aspx");
                        }
                        tplDir = Path.GetDirectoryName(tplFileName);
                        if (!Directory.Exists(tplDir))
                        {
                            Directory.CreateDirectory(tplDir);
                        }
                        //如果模板存在,则重写自动提示类
                        if (File.Exists(tplFileName))
                        {
                            string tplFileClassName = tplFileName + ".cs";
                            //重写自动提示类
                            StreamReader tplFileReader = new StreamReader(tplFileName, System.Text.Encoding.UTF8);
                            string tplFileCodeString = tplFileReader.ReadToEnd();
                            tplFileReader.Close();
                            string aspxPageHeaderPattern = @"<%@\s+(Page|Control)[^%]+%>";
                            string aspxPageAttrInheritsPattern = @"Inherits\s*=\s*""([^""]+)""";
                            string aspxPageAttrCodePattern = @"Code(?:Behind|File)\s*=\s*""([^""]+)""";

                            Regex aspxPageHeaderReg = new Regex(aspxPageHeaderPattern);
                            Match aspxPageHeaderMat = aspxPageHeaderReg.Match(tplFileCodeString);
                            if (aspxPageHeaderMat.Success)
                            {
                                string aspxPageHeaderString = aspxPageHeaderMat.Value;
                                Regex aspxPageAttrInheritsReg = new Regex(aspxPageAttrInheritsPattern);
                                Match aspxPageAttrInheritsMat = aspxPageAttrInheritsReg.Match(aspxPageHeaderString);
                                Regex aspxPageAttrCodeReg = new Regex(aspxPageAttrCodePattern);
                                Match aspxPageAttrCodeMat = aspxPageAttrCodeReg.Match(aspxPageHeaderString);
                                if (aspxPageAttrCodeMat.Success && aspxPageAttrInheritsMat.Success)
                                {
                                    string aspxPageAttrCodeFileName =Path.Combine(tplDir,aspxPageAttrCodeMat.Groups[1].Value);
                                    string aspxPageInheritsString = aspxPageAttrInheritsMat.Groups[1].Value;
                                    string aspxPageNameSpace =aspxPageInheritsString.Substring(0,aspxPageInheritsString.LastIndexOf('.'));
                                    string aspxPageClassName = aspxPageInheritsString.Substring(aspxPageInheritsString.LastIndexOf('.')+1);
                                    //如果模板样式是变量
                                    VTemplate.Engine.TemplateDocument doc = new VTemplate.Engine.TemplateDocument(Frame.MapPath("/NFinal/Template/AutoComplete.tpl"), System.Text.Encoding.UTF8);
                                    doc.SetValue("aspxPageNameSpace",aspxPageNameSpace);
                                    doc.SetValue("aspxPageClassName",aspxPageClassName);
                                    doc.SetValue("baseName",baseName);
                                    doc.SetValue("StructDatas", StructDatas);
                                    
                                    doc.SetValue("parameterDataList", methodData.parameterDataList);
                                    //去掉重复的声明
                                    List<DbFunctionData> functionDataList=new List<DbFunctionData>();
                                    for(int m=0;m<methodData.dbFunctions.Count;m++)
                                    {
                                        bool hasDbFunctionValue=false;
                                        for (int n = 0; n < functionDataList.Count; n++)
                                        {
                                            if (functionDataList[n].varName == methodData.dbFunctions[m].varName)
                                            {
                                                hasDbFunctionValue=true;
                                            }
                                        }
                                        if (hasDbFunctionValue == false && methodData.dbFunctions[m].type == "var")
                                        {
                                            functionDataList.Add(methodData.dbFunctions[m]);
                                        }
                                    }
                                    CSharpDeclarationParser csharpDeclarationParser = new CSharpDeclarationParser();
                                    List<CSharpDeclaration> csharpDeclarationList= csharpDeclarationParser.Parse(methodData.Content);
                                    
                                    doc.SetValue("methodName",methodData.name);
                                    doc.SetValue("functionDataList", functionDataList);
                                    doc.SetValue("csharpDeclarationList", csharpDeclarationList);
                                    doc.RenderTo(tplFileClassName, System.Text.Encoding.UTF8);
                                }
                            }
                        }
                        else
                        {
                            sharpCodes[j] = "";
                        }
                    }

                    StringWriter swTemplate = new StringWriter();
                    //如果参数是变量,而非字符串
                    if (IsVarName)
                    {
                        swTemplate.Write("switch(");
                        swTemplate.Write(viewData.templateStyle);
                        swTemplate.Write(")\r\n\t\t\t{\r\n");

                        if (styles.Length > 0)
                        {
                            for (int j = 0; j < styles.Length; j++)
                            {
                                if (j != defaultTemplateStyleIndex)
                                {
                                    swTemplate.Write("\t\t\t\tcase \"");
                                    swTemplate.Write(templateStyle[j]);
                                    swTemplate.Write("\":\r\n\t\t\t\t{\r\n");
                                    swTemplate.Write(sharpCodes[j]);
                                    swTemplate.Write("\t\t\t\t\tbreak;\r\n\t\t\t\t}\r\n");
                                }
                            }
                        }

                        if (styles.Length > 0)
                        {
                            for (int j = 0; j < styles.Length; j++)
                            {
                                if (j == defaultTemplateStyleIndex)
                                {
                                    swTemplate.Write("\t\t\t\tdefault :\r\n\t\t\t\t{\r\n");
                                    swTemplate.Write(sharpCodes[j]);
                                    swTemplate.Write("\t\t\t\t\tbreak;\r\n\t\t\t\t}\r\n");
                                }
                            }
                        }
                        swTemplate.Write("\t\t\t}\r\n");
                    }
                    else
                    {
                        if (styles.Length > 0)
                        {
                            for (int j = 0; j < styles.Length; j++)
                            {
                                if (j == defaultTemplateStyleIndex)
                                {
                                    swTemplate.Write("\r\n");
                                    swTemplate.Write(sharpCodes[j]);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
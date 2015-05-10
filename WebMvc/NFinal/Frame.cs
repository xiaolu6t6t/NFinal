using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using NFinal.Compile;

namespace NFinal
{
    public class Frame
    {
        public Config config;
        public static string appRoot;
        public static string AssemblyTitle;
        public static string name="NFinal";
        public GenConfig genConfig;


        public Frame(Config conf)
        {
            this.config = conf;
            genConfig = GenConfig.Load(Frame.MapPath(conf.APP + "compile.xml"));
        }
        public static void Set(string appRoot)
        {
            Frame.appRoot = appRoot;
            string[] fileNames = Directory.GetFiles(appRoot, "*.csproj");
            if (fileNames.Length > 0)
            {
                AssemblyTitle = Path.GetFileNameWithoutExtension(fileNames[0]);
            }
            else
            {
                string temp;
                temp = appRoot.Trim('\\');
                AssemblyTitle = temp.Substring(temp.LastIndexOf('\\') + 1);
            }
        }
        /// <summary>
        /// 把基于网站根目录的绝对路径改为相对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MapPath(string url)
        {
            return appRoot + url.Trim('/').Replace('/', '\\');
        }
        /// <summary>
        /// 把路径转为命名空间
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public string GetNameSpace(string folder)
        {
            return NFinal.Frame.AssemblyTitle+"."+folder.Trim('/').Replace('/','.');
        }

        /// <summary>
        /// 从web.config中读取所有的连接字符串信息
        /// </summary>
        public static void CreateDB(string appRoot)
        {
            string configFileName =appRoot + "Web.config";
            if (File.Exists(configFileName))
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(configFileName);
                System.Xml.XmlNode root = doc.DocumentElement.SelectSingleNode("/configuration");
                System.Xml.XmlNodeList nodeList = root.SelectNodes("connectionStrings/add[@connectionString]");
                Config.ConnectionStrings.Clear();
                if (nodeList.Count > 0)
                {
                    System.Xml.XmlElement ele = null;
                    foreach (System.Xml.XmlNode node in nodeList)
                    {
                        ele = node as System.Xml.XmlElement;
                        var connectionString = new NFinal.DB.ConnectionString
                        {
                            name = ele.Attributes["name"].Value,
                            value = ele.Attributes["connectionString"].Value,
                            provider = ele.Attributes["providerName"].Value
                        };
                        if (connectionString.provider.ToLower().IndexOf("mysql") > -1)
                        {
                            connectionString.type = NFinal.DB.DBType.MySql;
                        }
                        else if (connectionString.provider.ToLower().IndexOf("sqlclient") > -1)
                        {
                            connectionString.type = NFinal.DB.DBType.SqlServer;
                        }
                        else if (connectionString.provider.ToLower().IndexOf("sqlite") > -1)
                        {
                            connectionString.type = NFinal.DB.DBType.Sqlite;
                        }
                        else if (connectionString.provider.ToLower().IndexOf("oracle") > -1)
                        {
                            connectionString.type = NFinal.DB.DBType.Oracle;
                        }
                        else
                        {
                            connectionString.type = NFinal.DB.DBType.Unknown;
                        }
                        Config.ConnectionStrings.Add(connectionString);
                    }
                }
            }

        }
        public static void GetDB()
        {
            Compile.SqlAnalyse analyse=new SqlAnalyse();
            analyse.GetDB(Frame.appRoot);
        }
        public void SetDBFile()
        {
            if (Config.ConnectionStrings.Count > 0)
            {
                string dbTplFileName = MapPath("/NFinal/DB/DB.tpl");

                VTemplate.Engine.TemplateDocument doc = null;
                for (int i = 0; i < Config.ConnectionStrings.Count; i++)
                {
                    doc = new VTemplate.Engine.TemplateDocument(dbTplFileName, System.Text.Encoding.UTF8);
                    doc.SetValue("project", Frame.AssemblyTitle);
                    doc.SetValue("app", config.APP.Trim('/'));
                    doc.SetValue("database", Config.ConnectionStrings[i].name);
                    doc.SetValue("name", Config.ConnectionStrings[i].name);
                    doc.SetValue("connectionString", Config.ConnectionStrings[i].value);
                    doc.SetValue("providerName", Config.ConnectionStrings[i].provider);
                    doc.SetValue("dbType", Config.ConnectionStrings[i].type.ToString());
                    doc.RenderTo(MapPath(config.APP + "DB/" + Config.ConnectionStrings[i].name + ".cs"));
                }
                string conStrFileName = MapPath("/NFinal/DB/AppConnectionString.tpl");
                doc = new VTemplate.Engine.TemplateDocument(conStrFileName, System.Text.Encoding.UTF8);
                doc.SetValue("project", Frame.AssemblyTitle);
                doc.SetValue("app", config.APP.Trim('/'));
                doc.SetValue("connectionStrings", Config.ConnectionStrings);
                doc.RenderTo(MapPath(config.APP + "ConnectionString.cs"));
            }
        }
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="folders"></param>
        private void CreateFolders( string[] folders)
        {
            string folder = "/";
            for (int i = 0; i < folders.Length; i++)
            {
                folder = MapPath(folders[i]);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
        }
        //自动创建项目
        public void CreateApp()
        {
            //创建目录
            string[] folders = { config.Controller,
                                   config.Common, 
                                   config.Views,
                                   config.Views+config.defaultStyle,
                                   config.Views+config.defaultStyle+"IndexController/",
                                   config.BLL,
                                   config.DAL,
                                   config.Web ,
                                   config.Content,
                                   config.ContentJS,
                                   config.ContentImages,
                                   config.ContentCss,
                                   config.DB
                               };
            CreateFolders(folders);
            //生成首页
            string fileName = MapPath(config.Controller + "IndexController.cs");
            if (!File.Exists(fileName))
            {
                VTemplate.Engine.TemplateDocument doc = new VTemplate.Engine.TemplateDocument(MapPath("/NFinal/Template/IndexController.cs.tpl"),System.Text.Encoding.UTF8);
                doc.SetValue("nameSpace", Frame.AssemblyTitle + "."+config.Controller.Trim('/').Replace('/','.'));
                doc.RenderTo(fileName);
            }
            //生成首页
            fileName = MapPath(config.Views +config.defaultStyle+ "Index"+config.ControllerSuffix+"/" + "Index.aspx");
            if (!File.Exists(fileName))
            {
                File.Copy(MapPath("/NFinal/Template/Index.aspx.tpl"), fileName);
            }
        }
       
        //在某段位置替换成另一段代码
        public int Replace(ref string str,int index,int length,string rep)
        {
            if (length > 0)
            {
                str = str.Remove(index, length);
            }
            if (index > 0)
            {
                str = str.Insert(index, rep);
            }
            return rep.Length - length; 
        }
        public void createDAL()
        {
            string[] fileNames;
            //获取要生成的文件
            if (genConfig != null)
            {
                if (genConfig.bllFiles.Count > 0)
                {
                    fileNames = new string[genConfig.bllFiles.Count];
                    for (int i = 0; i < fileNames.Length; i++)
                    {
                        fileNames[i] = MapPath(genConfig.bllFiles[i]);
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (!Directory.Exists(MapPath(config.BLL)))
                {
                    return;
                }
                //获取所有的bll文件
                fileNames = Directory.GetFiles(MapPath(config.BLL), "*.cs", SearchOption.AllDirectories);
                if (fileNames == null || fileNames.Length < 1)
                {
                    return;
                }
            }

            //生成的代码
            string compileCode = "";
            //类名称
            string ClassName = "";

            int relative_position = 0;

            if (fileNames.Length>0)
            {
                Compiler com = new Compiler();
                FileData fileData = null;
                ClassData classData = null;
                MethodData methodData = null;
                StreamWriter sw = null;

                string compileFileName = "";
                string compileDir = "";
                string methodContent = "";

                for (int i = 0; i < fileNames.Length; i++)
                {
                    relative_position = 0;
                    fileData = com.GetFileData(fileNames[i],System.Text.Encoding.UTF8);

                    ClassName = Path.GetFileNameWithoutExtension(fileNames[i]);
                    classData = fileData.GetClassData(ClassName);
                    compileCode = fileData.csharpCode.ToString();
                    //相对
                    compileDir = config.ChangeBLLName(config.DAL.Trim('/'), fileData.nameSpace.Replace('.', '/'));
                    //绝对
                    compileFileName = MapPath(compileDir+"\\" + ClassName + ".cs");
                    //更改命名空间
                    relative_position += Replace(ref compileCode, relative_position + fileData.start, fileData.length, "namespace " + (Frame.AssemblyTitle +"/"+ compileDir.TrimEnd('/')).Replace('/', '.') + "\r\n{");
                    if (classData.MethodDataList.Count > 0)
                    {
                        
                        for (int j = 0; j < classData.MethodDataList.Count; j++)
                        {
                            
                            methodData = classData.MethodDataList[j];
                            methodContent = methodData.Content;
                            SqlCompiler sqlCompiler = new NFinal.Compile.SqlCompiler();
                            
                            //从代码中分析出数据库相关函数
                            List<DbFunctionData> dbFunctions = sqlCompiler.Compile(methodContent);

                            if (dbFunctions.Count > 0)
                            {
                                SqlAnalyse analyse = new SqlAnalyse();
                                //与数据库相结合,从sql语句中分析出所有的表信息,列信息
                                methodData.dbFunctions = analyse.FillFunctionDataList(NFinal.DB.Coding.DB.DbStore, dbFunctions);
                            }
                            //数据库函数替换
                            int content_relative_position = 0;
                            if (dbFunctions.Count > 0)
                            {
                                bool hasSameVarName = false;
                                List<string> varNames = new List<string>();
                                //添加struct类型
                                for (int s = 0; s < dbFunctions.Count; s++)
                                {
                                    //去除重复项
                                    if (varNames.Count > 0)
                                    {
                                        hasSameVarName = false;
                                        for (int c = 0; c < varNames.Count; c++)
                                        {
                                            //如果发现重复项,则跳过循环
                                            if (varNames[c] == dbFunctions[s].varName)
                                            {
                                                hasSameVarName = true;
                                                break;
                                            }
                                        }
                                        if (hasSameVarName)
                                        {
                                            continue;
                                        }
                                    }
                                    varNames.Add(dbFunctions[s].varName);
                                    //分析出sql返回的List<dynamic>和dynamic类型是否有用AddNewField(string fileName,Type t);添加相关的类型
                                    NewField newFiled = new NewField(dbFunctions[s].varName);
                                    List<NFinal.Compile.StructField> structFieldList = newFiled.GetFields(ref methodContent, methodData.name);
                                    //添加struct字段
                                    string StructData = sqlCompiler.SetMagicStruct(methodData.name, dbFunctions[s],structFieldList, appRoot);
                                    if (!string.IsNullOrEmpty(StructData))
                                    {
                                        compileCode = compileCode.Insert(methodData.start + relative_position, StructData);
                                        relative_position += StructData.Length;
                                    }
                                }
                                //修正函数返回类型,提高执行效率
                                if (methodData.returnType.IndexOf("dynamic") > -1)
                                {
                                    string returnTypeString="";
                                    if (new System.Text.RegularExpressions.Regex(@"List\s*<\s*dynamic\s*>").IsMatch(methodData.returnType))
                                    {
                                        returnTypeString = string.Format("System.Collections.Generic.List<__{0}_{1}__>", methodData.name,methodData.returnVarName);
                                        
                                    }
                                    else
                                    {
                                        returnTypeString = string.Format("__{0}_{1}__", methodData.name, methodData.returnVarName);
                                    }
                                    relative_position += Replace(ref compileCode,
                                            methodData.returnTypeIndex+relative_position+classData.position,
                                            methodData.returnType.Length,
                                            returnTypeString);
                                }
                                //更换函数内的数据库操作函数
                                content_relative_position += sqlCompiler.SetMagicFunction(methodData.name,
                                    ref methodContent,
                                    content_relative_position,
                                    methodData.dbFunctions,
                                    appRoot);
                                relative_position += Replace(ref compileCode, relative_position + methodData.position, methodData.Content.Length, methodContent);
                            }
                        }
                    }
                    //如果文件夹不存在则新建
                    if (!Directory.Exists(MapPath(compileDir)))
                    {
                        Directory.CreateDirectory(MapPath(compileDir));
                    }
                    //写DAL层.class文件
                    sw = new StreamWriter(compileFileName, false, System.Text.Encoding.UTF8);
                    sw.Write(compileCode);
                    sw.Close();
                }
            }
        }
 
        //建立编译文件
        public void createCompile()
        {
            string[] fileNames;
            //获取要生成的文件
            if (genConfig != null)
            {
                if (genConfig.controllerFiles.Count > 0)
                {
                    fileNames = new string[genConfig.controllerFiles.Count];
                    for (int i = 0; i < fileNames.Length; i++)
                    {
                        fileNames[i] = MapPath(genConfig.controllerFiles[i]);
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                if(!Directory.Exists(MapPath(config.Controller)))
                {
                    return;
                }
                //获取所有的controls文件
                fileNames = Directory.GetFiles(MapPath(config.Controller), "*.cs", SearchOption.AllDirectories);
                if (fileNames == null || fileNames.Length < 1)
                {
                    return;
                }
            }
            
            //生成的代码
            string compileCode = "";
            //类名称
            string ClassName = "";

            //前面的字符串部分添加或删除后，后面的代码的相对位置
            int relative_postion=0;

            if(fileNames.Length>0)
            {
                Compiler com = new Compiler();
                FileData fileData = null;
                ClassData classData = null;
                MethodData methodData=null;
                StreamWriter sw = null;
                VTemplate.Engine.TemplateDocument swDebug = null;
                VTemplate.Engine.TemplateDocument swAspx = null;

                string compileFileName = "";
                string compileDir = "";
                string debugFileName="";
                string methodContent="";
                for (int i = 0; i < fileNames.Length; i++)
                {
                    fileData = com.GetFileData(fileNames[i],System.Text.Encoding.UTF8);
                    
                    ClassName = Path.GetFileNameWithoutExtension(fileNames[i]);
                    classData = fileData.GetClassData(ClassName);
                    
                    
                    if (classData.MethodDataList.Count > 0)
                    {
                        
                        for (int j = 0; j < classData.MethodDataList.Count; j++)
                        {
                            //只有公开方法才能访问
                            if (classData.MethodDataList[j].isPublic)
                            {
                                relative_postion = 0;
                                compileCode = fileData.csharpCode.ToString();
                                methodData = classData.MethodDataList[j];
                                //相对
                                compileDir = config.ChangeControllerName(config.Web+config.defaultStyle, (fileData.nameSpace + '.' + ClassName).Replace('.', '/')) + "/";
                                //绝对
                                compileFileName = MapPath(compileDir + methodData.name + ".cs");
                                if (!Directory.Exists(Path.GetDirectoryName(compileFileName)))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(compileFileName));
                                }
                                //调试文件
                                debugFileName = MapPath(compileDir + methodData.name + ".html");
                                //输出调试文件
                                if (!File.Exists(debugFileName))
                                {
                                    swDebug = new VTemplate.Engine.TemplateDocument(MapPath("/NFinal/Template/Debug.tpl"), System.Text.Encoding.UTF8);
                                    swDebug.SetValue("Url", config.ChangeControllerName(config.APP, (fileData.nameSpace + '.' + ClassName).Replace('.', '/')) + "/" + methodData.name + ".htm");
                                    swDebug.RenderTo(debugFileName, System.Text.Encoding.UTF8);
                                }
                                relative_postion += Replace(ref compileCode, relative_postion + fileData.start, fileData.length, "namespace " + (Frame.AssemblyTitle + compileDir.TrimEnd('/')).Replace('/', '.') + "\r\n{");
                                relative_postion += Replace(ref compileCode, relative_postion + classData.start, classData.length, "public class " +
                                                methodData.name + "Action " + (string.IsNullOrEmpty(classData.baseName) ? "" : " : " + classData.baseName) + "\r\n\t{"
                                                //添加初始化函数
                                                +"\r\n\t\tpublic "+ methodData.name + "Action(System.IO.TextWriter tw):base(tw){}"
                                                +"\r\n\t\tpublic "+ methodData.name + "Action(string fileName) : base(fileName) {}");
                                //循环内部所有方法
                                for (int k = 0; k < classData.MethodDataList.Count; k++)
                                {
                                    methodData =classData.MethodDataList[k];
                                    //如果两个相等,则进行替换
                                    if (j==k || (!classData.MethodDataList[k].isPublic))
                                    {
                                        #region "替换原有方法"
                                        //排除非公开和非基类的方法,替换原有方法体
                                        //if (methodData.isPublic)
                                        {
                                            methodContent=methodData.Content;
                                            SqlCompiler sqlCompiler =  new NFinal.Compile.SqlCompiler();
                                            //从代码中分析出数据库相关函数
                                            List<DbFunctionData> dbFunctions = sqlCompiler.Compile(methodContent);
                                            
                                            if (dbFunctions.Count > 0)
                                            {
                                                SqlAnalyse analyse = new SqlAnalyse();
                                                //与数据库相结合,从sql语句中分析出所有的表信息,列信息
                                                methodData.dbFunctions = analyse.FillFunctionDataList(NFinal.DB.Coding.DB.DbStore, dbFunctions);
                                            }
                                             //数据库函数替换
                                            int content_relative_position = 0;
                                            if (dbFunctions.Count > 0)
                                            {
                                                bool hasSameVarName = false;
                                                List<string> varNames = new List<string>();
                                                //添加struct类型
                                                for (int s = 0; s < dbFunctions.Count; s++)
                                                {
                                                    //去除重复项
                                                    if (varNames.Count > 0)
                                                    {
                                                        hasSameVarName = false;
                                                        for (int c = 0; c < varNames.Count; c++)
                                                        {
                                                            //如果发现重复项,则跳过循环
                                                            if (varNames[c] == dbFunctions[s].varName)
                                                            {
                                                                hasSameVarName = true ;
                                                                break;
                                                            }
                                                        }
                                                        if (hasSameVarName)
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                    varNames.Add(dbFunctions[s].varName);
                                                    //分析出sql返回的List<dynamic>和dynamic类型是否有用AddNewField(string fileName,Type t);添加相关的类型
                                                    NewField newFiled = new NewField(dbFunctions[s].varName);
                                                    List<NFinal.Compile.StructField> structFieldList = newFiled.GetFields(ref methodContent, methodData.name);
                                                    //添加struct字段
                                                    string StructData = sqlCompiler.SetMagicStruct(methodData.name, dbFunctions[s],structFieldList, appRoot);
                                                    if (!string.IsNullOrEmpty(StructData))
                                                    {
                                                        compileCode = compileCode.Insert(methodData.start+relative_postion, StructData);
                                                        relative_postion += StructData.Length;
                                                    }
                                                }
                                                content_relative_position += sqlCompiler.SetMagicFunction(methodData.name,
                                                    ref methodContent,
                                                    content_relative_position,
                                                    methodData.dbFunctions,
                                                    appRoot);
                                            }
                                            //从代码中分析出views函数
                                            NFinal.Compile.ViewCompiler viewCompiler = new NFinal.Compile.ViewCompiler();
                                            List<ViewData> views = viewCompiler.Compile(methodContent);
                                            //模版替换
                                            if (views.Count > 0)
                                            {
                                                content_relative_position = 0;
                                                content_relative_position = viewCompiler.SetMagicFunction(ref methodContent,
                                                    content_relative_position,
                                                    fileData.nameSpace,
                                                    ClassName,
                                                    methodData.name, views, config);
                                            }
                                            relative_postion+= Replace(ref compileCode,relative_postion+ methodData.position,methodData.Content.Length, methodContent);
                                            
                                            

                                        }
                                        #endregion
                                    }
                                    //如果两个不相等
                                    else
                                    {
                                        compileCode = compileCode.Remove(relative_postion+ classData.MethodDataList[k].start,
                                            classData.MethodDataList[k].length+
                                            classData.MethodDataList[k].Content.Length+1);//去掉最后一个}
                                        relative_postion -= classData.MethodDataList[k].length +
                                            classData.MethodDataList[k].Content.Length + 1;
                                    }
                                }
                                //写aspx页面的自动提示层
                                
                                //写Web层.class文件
                                sw = new StreamWriter(compileFileName, false, System.Text.Encoding.UTF8);
                                sw.Write(compileCode);
                                sw.Close();
                            }
                        }
                    }
                }
            }
        }
        //创建入口文件
        public void createIndex()
        {
            string page = "Default";
            VTemplate.Engine.TemplateDocument doc_aspx = new VTemplate.Engine.TemplateDocument(MapPath("/NFinal/Index/Index.aspx.cs.tpl"),
                System.Text.Encoding.UTF8);
            doc_aspx.SetValue("assm", NFinal.Frame.AssemblyTitle);
            doc_aspx.SetValue("page",page);
            doc_aspx.SetValue("app",config.APP.Trim('/'));
            doc_aspx.RenderTo(MapPath(page+".aspx.cs"));

            doc_aspx = new VTemplate.Engine.TemplateDocument(MapPath("/NFinal/Index/Index.aspx.designer.cs.tpl"),
                System.Text.Encoding.UTF8);
            doc_aspx.SetValue("assm", NFinal.Frame.AssemblyTitle);
            doc_aspx.SetValue("page", page);
            doc_aspx.SetValue("app", config.APP.Trim('/'));
            doc_aspx.RenderTo(MapPath(page+ ".aspx.designer.cs"));

            doc_aspx = new VTemplate.Engine.TemplateDocument(MapPath("/NFinal/Index/Index.aspx.tpl"),
                System.Text.Encoding.UTF8);
            doc_aspx.SetValue("assm", NFinal.Frame.AssemblyTitle);
            doc_aspx.SetValue("page", page);
            doc_aspx.SetValue("app", config.APP.Trim('/'));
            doc_aspx.RenderTo(MapPath(page + ".aspx"));
        }
        //创建分组路由器
        public void CreateRouter()
        {
            string[] fileNames = Directory.GetFiles(MapPath(config.Controller),"*.cs",SearchOption.AllDirectories);
           
            string ClassName = null;
            Compiler compiler=new Compiler();
            FileData fileData=null;
            ClassData classData=null;

            List<ClassData> classDataList = new List<ClassData>();
            string temp=null;
            if (fileNames.Length > 0)
            {
                for (int i = 0; i < fileNames.Length; i++)
                {

                    fileData = compiler.GetFileData(fileNames[i],System.Text.Encoding.UTF8);
                    ClassName = Path.GetFileNameWithoutExtension(fileNames[i]);
                    classData = fileData.GetClassData(ClassName);
                    temp = (Frame.AssemblyTitle + config.Controller).TrimEnd('/');
                    //relativeName="/Manage/IndexController"
                    //RelativeDotName=".Manage.IndexController"
                    classData.relativeName =classData.fullName.Substring(temp.Length,classData.fullName.Length-temp.Length).Replace('.','/');
                    classData.relativeDotName = classData.fullName.Substring(temp.Length, classData.fullName.Length - temp.Length);
                    classDataList.Add(classData);
                }
            }
            
            VTemplate.Engine.TemplateDocument doc = new VTemplate.Engine.TemplateDocument(MapPath("NFinal/Template/Router.tpl"), System.Text.Encoding.UTF8);
            doc.SetValue("defaultStyle",config.defaultStyle.Trim('/'));
            doc.SetValue("classDataList", classDataList);
            doc.SetValue("namespace", config.GetFullName(fileData.projectName,config.APP));
            doc.RenderTo(MapPath(config.APP+"Router.cs"), System.Text.Encoding.UTF8);
        }
        public static void CreateMain(string[] apps)
        {
            //创建主路由
            VTemplate.Engine.TemplateDocument doc = null;
            doc=new VTemplate.Engine.TemplateDocument(MapPath("NFinal/Template/Main.tpl"), System.Text.Encoding.UTF8);
            doc.SetValue("project", NFinal.Frame.AssemblyTitle);
            doc.SetValue("apps", apps);
            doc.SetValue("ControllerSuffix", "Controller");
            doc.RenderTo(MapPath("NFinal/Main.cs"), System.Text.Encoding.UTF8);
        }
        public static string[] GetWebConfig()
        {
            string webConfigFileName= MapPath("/Web.config");
            XmlDocument doc = new XmlDocument();
            doc.Load(webConfigFileName);
            XmlNode appsNode = doc.DocumentElement.SelectSingleNode("appSettings/add[@key='Apps']");
            string[] Apps = null;

            if (appsNode != null && appsNode.Attributes.Count > 0 && appsNode.Attributes["value"] != null)
            {
                Apps = appsNode.Attributes["value"].Value.Split(',');
            }
            else
            {
                Apps =new string[] {"App"};
            }
            return Apps;
        }
        
    }

}
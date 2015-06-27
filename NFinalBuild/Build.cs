using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace NFinalBuild
{
    public class Build
    {

        //将相对根目录的路径转为绝对路径
        public static string MapPath(string url)
        {
            return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName + url.Replace('/', '\\');
        }

        public string BuildNFinal(string projectFile,string versionText)
        {
            string projectRoot = Path.GetDirectoryName(projectFile) + "\\";
            string webconfigFile = projectRoot + "Web.config";
            System.Xml.XmlDocument xmlConfig = new System.Xml.XmlDocument();
            string[] Apps = null;
            if (File.Exists(webconfigFile))
            {
                xmlConfig.Load(webconfigFile);
                XmlNode appsNode = xmlConfig.DocumentElement.SelectSingleNode("appSettings/add[@key='Apps']");
                if (appsNode != null && appsNode.Attributes.Count > 0 && appsNode.Attributes["value"] != null)
                {
                    Apps = appsNode.Attributes["value"].Value.Split(',');
                }
            }
            if(Apps==null)
            {
                return "Web.config中找不到app配置项";
            }
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(projectFile);
            string nameSpace = doc.DocumentElement.Attributes["xmlns"].Value;
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.NameTable);
            namespaceManager.AddNamespace("x", nameSpace);
            Microsoft.Build.Utilities.TargetDotNetFrameworkVersion version;
            //如果versionText不存在就取工程文件中的版本
            if (string.IsNullOrEmpty(versionText))
            {
                XmlNode versionNode = doc.DocumentElement.SelectSingleNode("x:PropertyGroup/x:TargetFrameworkVersion", namespaceManager);
                versionText = versionNode.InnerText;
            }
            switch (versionText)
            {
                case "v1.1": version = Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.Version11; break;
                case "v2.0": version = Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.Version20; break;
                case "v3.0": version = Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.Version30; break;
                case "v3.5": version = Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.Version35; break;
                case "v4.0": version = Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.Version40; break;
                //case "v4.5": version = Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.Version45; break;
                default: version = Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.VersionLatest; break;
            }
            bool supportOracle = false;
            bool isNet2 = false;
            List<string> references = new List<string>();
            XmlNodeList referenceNodes = doc.DocumentElement.SelectNodes("x:ItemGroup/x:Reference[@Include]", namespaceManager);
            XmlNode hintPathNode = null;
            string dllPath = null;
            string referencePath=null ;//= Microsoft.Build.Utilities.ToolLocationHelper.GetPathToDotNetFrameworkReferenceAssemblies(version);
            string frameworkPath = null;// Microsoft.Build.Utilities.ToolLocationHelper.GetPathToDotNetFramework(version);
            ///reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\mscorlib.dll" 
            
            ///reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Core.dll" 
            
            if (version == Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.Version11
                || version == Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.Version20
                || version == Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.Version30
                || version == Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.Version35)
            {
                supportOracle = false;
                isNet2 = true;
                referencePath = Microsoft.Build.Utilities.ToolLocationHelper.GetPathToDotNetFramework(
                    Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.Version20);
                frameworkPath = Microsoft.Build.Utilities.ToolLocationHelper.GetPathToDotNetFramework(version);
                references.Add(Path.Combine(Microsoft.Build.Utilities.ToolLocationHelper.GetPathToDotNetFramework(
                    Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.Version20), "mscorlib.dll"));
                
            }
            else
            {
                referencePath = Microsoft.Build.Utilities.ToolLocationHelper.GetPathToDotNetFrameworkReferenceAssemblies(version);
                frameworkPath = Microsoft.Build.Utilities.ToolLocationHelper.GetPathToDotNetFramework(version);
                references.Add(Path.Combine(referencePath, "mscorlib.dll"));
                references.Add(Path.Combine(referencePath, "System.Core.dll"));
                supportOracle = true;
                isNet2 = false;
            }
            
            for (int i = 0; i < referenceNodes.Count; i++)
            {
                hintPathNode = referenceNodes[i].SelectSingleNode("x:HintPath", namespaceManager);

                //引入
                //如果是.net内部的dll文件
                if (hintPathNode == null)
                {
                    dllPath = Path.Combine(referencePath, referenceNodes[i].Attributes["Include"].Value.Split(',')[0] + ".dll");
                    if (File.Exists(dllPath))
                    {
                        references.Add(dllPath);
                    }
                }
                //如果是外部引用
                else
                {
                    dllPath = Path.Combine(projectRoot+"bin\\", Path.GetFileName(hintPathNode.InnerText));

                    //如果是oracle这个dll,则要看是否支持来引入
                    if (Path.GetFileName(dllPath) == "Oracle.ManagedDataAccess.dll")
                    {
                        if (!supportOracle)
                        {
                            if (File.Exists(dllPath))
                            {
                                File.Delete(dllPath);
                            }
                            //如果不支持oracle则直接跳过,不引入该dll文件
                            continue;
                        }
                        else
                        {
                            if (!File.Exists(dllPath))
                            {
                                dllPath = projectRoot + "NFinal\\Resource\\Oracle.ManagedDataAccess.dll";
                            }
                        }
                    }
                    references.Add(dllPath);
                }
            }
            string cscExeFileName = null;
            if (isNet2)
            {
                cscExeFileName = Path.Combine(Microsoft.Build.Utilities.ToolLocationHelper.GetPathToDotNetFramework(
                    Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.Version35), "csc.exe");
            }
            else
            { 
                cscExeFileName = Path.Combine(frameworkPath, "csc.exe");
            }
            StringBuilder commandText = new StringBuilder();
            //cs0472,由于“decimal”类型的值永不等于“decimal?”类型的“null”，该表达式的结果始终为“false”
            //0219,warning CS0219: 变量“a”已赋值，但其值从未使用过
            //如果不支持oracle则不编译oracle相关代码
            StringBuilder Def = new StringBuilder();
            if (supportOracle)
            {
                Def.Append(";ORACLE");
            }
            if (isNet2)
            {
                Def.Append(";NET2");
            }
            commandText.Append(string.Format("/noconfig /unsafe+ /nowarn:0219,1701,1702,0472 /nostdlib+ /errorreport:prompt /warn:4 /define:TRACE{0} ", Def.ToString()));
            for (int i = 0; i < references.Count; i++)
            {
                commandText.Append(string.Format("/reference:\"{0}\" ", references[i]));
            }
            commandText.Append(string.Format("/debug:pdbonly /optimize- /out:\"{0}obj\\Release\\WebMvc.dll\" ", projectRoot));
            commandText.Append("/target:library /utf8output ");
            //if(version==Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.Version40)
            //{
            //    commandText.Append(string.Format("\"{0}\" ",Path.Combine(projectRoot,"NFinal\\Compile\\Build\\.NETFramework,Version=v4.0.AssemblyAttributes.txt")));
            //}
            //else if(version==Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.Version45)
            //{
            //    commandText.Append(string.Format("\"{0}\" ", Path.Combine(projectRoot, "NFinal\\Compile\\Build\\.NETFramework,Version=v4.5.AssemblyAttributes.txt")));
            //}
            //else if(version==Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.VersionLatest)
            //{

            //    commandText.Append(string.Format("\"{0}\" ",Path.Combine(projectRoot,"NFinal\\Compile\\Build\\.NETFramework,Version=v4.5.1.AssemblyAttributes.txt")));
            //}

            //添加App下的代码
            for (int i = 0; i < Apps.Length; i++)
            {
                string[] webFileNames = Directory.GetFiles(projectRoot + Apps[i] + "\\Web\\", "*.cs", SearchOption.AllDirectories);
                AddFiles(ref commandText, webFileNames);
                string[] classFileNames = Directory.GetFiles(projectRoot + Apps[i] + "\\Common\\", "*.cs", SearchOption.AllDirectories);
                AddFiles(ref commandText, classFileNames);
                string[] conentFileNames = Directory.GetFiles(projectRoot + Apps[i] + "\\Content\\", "*.cs", SearchOption.AllDirectories);
                AddFiles(ref commandText, conentFileNames);
                string[] dalFileNames = Directory.GetFiles(projectRoot + Apps[i] + "\\Models\\DAL\\", "*.cs", SearchOption.AllDirectories);
                AddFiles(ref commandText, dalFileNames);
                //string[] dbFileNames = Directory.GetFiles(projectRoot + Apps[i] + "\\Models\\", "*.cs", SearchOption.TopDirectoryOnly);
                //AddFiles(ref commandText, dbFileNames);
                string[] appFiles = new string[] {
                    projectRoot+Apps[i]+"\\Models\\ConnectionStrings.cs",
                    projectRoot+Apps[i]+"\\Router.cs",
                    projectRoot+Apps[i]+"\\Config.cs",
                    projectRoot+Apps[i]+"\\Extension.cs",
                    projectRoot+Apps[i]+"\\WebCompiler.aspx.cs",
                    projectRoot+Apps[i]+"\\WebCompiler.aspx.designer.cs"
                };
                AddFiles(ref commandText, appFiles);
            }
            //添加所有的NFinal下的代码
            bool buildAll=true;
            if (buildAll)
            {
                string[] NFinalCompiles = Directory.GetFiles(projectRoot + "NFinal\\Compile\\", "*.cs", SearchOption.AllDirectories);
                AddFiles(ref commandText, NFinalCompiles);
                string[] NFinalDBCodings = Directory.GetFiles(projectRoot+"NFinal\\DB\\Coding\\","*.cs",SearchOption.AllDirectories);
                AddFiles(ref commandText, NFinalDBCodings);
                string[] NFinalVTemplates = Directory.GetFiles(projectRoot +"NFinal\\VTemplate\\","*.cs",SearchOption.AllDirectories);
                AddFiles(ref commandText,NFinalVTemplates);
                string[] NFinalCommons = Directory.GetFiles(projectRoot+"NFinal\\Common\\","*.cs",SearchOption.AllDirectories);
                AddFiles(ref commandText,NFinalCommons);
                string[] NFinalTemplate = Directory.GetFiles(projectRoot+"NFinal\\Template\\","*.cs",SearchOption.AllDirectories);
                AddFiles(ref commandText,NFinalTemplate);
                //string[] NFinalControlls = Directory.GetFiles(projectFile +"NFinal\\Controll\\","*.cs",SearchOption.AllDirectories);
                //AddFiles(ref commandText,NFinalControlls);
                //string[] NFinalContents = Directory.GetFiles(projectFile+"NFinal\\Content\\","*.cs",SearchOption.AllDirectories);
                //AddFiles(ref commandText,NFinalContents);
                //添加NFinal下的代码
                string[] nfDbFileNames = new string[]{
                    projectRoot+"NFinal\\Application.cs",
                    projectRoot+"NFinal\\Config.cs",
                    projectRoot+"NFinal\\Frame.cs",
                    projectRoot+"NFinal\\Builder.cs",
                    projectRoot+"NFinal\\DB\\SqlObject.cs",
                    projectRoot+"NFinal\\DB\\NList.cs",
                    projectRoot+"NFinal\\DB\\NStruct.txt",
                    projectRoot+"NFinal\\DB\\DBType.cs",
                    projectRoot+"NFinal\\DB\\ConnectionString.cs",
                    projectRoot+"NFinal\\Session\\Session.cs",
                    projectRoot+"NFinal\\Handler\\Handler.cs",
                    projectRoot+"NFinal\\Handler\\HandlerFactory.cs",
                    projectRoot+"NFinal\\Handler\\HttpAsyncHandler.cs",
                    projectRoot+"NFinal\\BaseAction.cs",
                    projectRoot+"NFinal\\Main.cs",
                    projectRoot+"NFinal\\Resource\\fileTree\\connectors\\editor.ashx.cs",
                    projectRoot+"NFinal\\Resource\\fileTree\\connectors\\jqueryFileTree.ashx.cs"
                };
                AddFiles(ref commandText, nfDbFileNames);
            }
            else
            {
                //添加NFinal下的代码
                string[] nfDbFileNames = new string[]{
                    projectRoot+"NFinal\\DB\\SqlObject.cs",
                    projectRoot+"NFinal\\DB\\NList.cs",
                    projectRoot+"NFinal\\DB\\NStruct.txt",
                    projectRoot+"NFinal\\Session\\Session.cs",
                    projectRoot+"NFinal\\Handler\\Handler.cs",
                    projectRoot+"NFinal\\Handler\\HandlerFactory.cs",
                    projectRoot+"NFinal\\Handler\\HttpAsyncHandler.cs",
                    projectRoot+"NFinal\\BaseAction.cs",
                    projectRoot+"NFinal\\Main.cs",
                    projectRoot+"NFinal\\Resource\\fileTree\\connectors\\editor.ashx.cs",
                    projectRoot+"NFinal\\Resource\\fileTree\\connectors\\jqueryFileTree.ashx.cs"
                };
                AddFiles(ref commandText, nfDbFileNames);
            }
            //urlRewriter
            string[] urlRewriterFileNames = Directory.GetFiles(projectRoot + "NFinal\\UrlRewriter\\", "*.cs", SearchOption.AllDirectories);
            AddFiles(ref commandText, urlRewriterFileNames);
            //litJson
            string[] litJsonFileNames = Directory.GetFiles(projectRoot + "NFinal\\LitJson\\", "*.cs", SearchOption.AllDirectories);
            AddFiles(ref commandText, litJsonFileNames);
            
            string propertyFileName = Path.Combine(projectRoot, "Properties\\Settings.Designer.cs");
            if (File.Exists(propertyFileName))
            {
                commandText.Append(string.Format("\"{0}\" ", propertyFileName));
            }
            propertyFileName = Path.Combine(projectRoot, "Properties\\AssemblyInfo.cs");
            if (File.Exists(propertyFileName))
            {
                commandText.Append(string.Format("\"{0}\" ", propertyFileName));
            }
            string commandLine = commandText.ToString();
            return ExeCommand(cscExeFileName, commandLine);
        }
        public void AddFiles(ref StringBuilder sb, string[] fileNames)
        {
            if (fileNames != null && fileNames.Length > 0)
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    sb.Append(string.Format("\"{0}\" ", fileNames[i]));
                }
            }
        }
        public string ExeCommand(string exeFileName,string commandText)
        {
            Process p = new Process();
            p.StartInfo.FileName = exeFileName;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.Arguments = commandText;
            p.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
            p.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
            p.StartInfo.CreateNoWindow = true;
            string strOutput = null;
            try
            {
                p.Start();
                //p.StandardInput.WriteLine(commandText);
                //p.StandardInput.WriteLine("exit");
                strOutput = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();
            }
            catch (Exception e)
            {
                strOutput = e.Message;
            }
            return strOutput;
        }
        public void AddFolders(System.Xml.XmlDocument doc,XmlNamespaceManager namespaceManager,string[] apps)
        {
            string app;
            for (int i = 0; i < apps.Length; i++)
            {
                app = apps[i];
                System.IO.Directory.GetDirectories(MapPath("/"+app+""));
            }
        }
    }
}

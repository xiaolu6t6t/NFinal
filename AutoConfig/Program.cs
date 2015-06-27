using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace AutoConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            string root = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName + "\\";
            FileInfo[] projFiles = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.GetFiles("*.csproj");
            if (projFiles.Length > 0)
            {
                Project proj = new Project(projFiles[0].FullName);
                //proj.Save();
                //return;
                Console.WriteLine("请输入要添加的模块名称:");
                string AppName= Console.ReadLine();
                Config.IISVersion version = Config.GetIISVersion();

                double versionNum = 0;
                if (version == Config.IISVersion.Unknown)
                {
                    Console.WriteLine("本机器未安装IIS,请手动输入IIS版本:(如:6,7,7.5,8)");
                    try
                    {
                        versionNum = Convert.ToDouble(Console.ReadLine());
                        if (versionNum < 7)
                        {
                            version = Config.IISVersion.IIS6;
                        }
                        else if (versionNum >= 7 && versionNum < 8)
                        {
                            version = Config.IISVersion.IIS7;
                        }
                        else if (versionNum >= 8)
                        {
                            version = Config.IISVersion.IIS8;
                        }
                    }
                    catch {
                        version = Config.IISVersion.IIS7;
                    }
                }
                Console.WriteLine("开始引入dll");
                //添加相关dll文件
                proj.AddLibrary();
                proj.AddFiles("NFinal"); 
                
                Console.WriteLine("开始配置Web.config");
                string[] apps = Config.SetWebConfig(version, root, AppName);

                Config config = new Config(root);
                //配置全局
                Console.WriteLine("配置路由");
                config.ModifyGlobal(apps);
                
                //初始化Main函数
                Console.WriteLine("初始化Main函数");
                config.InitMain(apps);
                Console.WriteLine("初始化数据库");
                //初始化数据库
                config.InitAppData();
                Console.WriteLine("创建主页");
                //创建主页文件
                config.InitIndexHTML();

                //如果已经配置过,则不需要重新生成
                string AppDir = root + "\\" + AppName;
                if (Directory.Exists(AppDir))
                {
                    Console.WriteLine(AppName+"已经存在.");
                    Console.ReadKey();
                    return;
                }
                Console.WriteLine("\r\n开始配置" + AppName);
                //App
                Directory.CreateDirectory(root + "\\" + AppName + "\\");
                //folders
                config.InitFolder(AppName);
                Console.WriteLine("生成Config.cs");
                //App/Config.cs
                config.InitConfig(AppName);
                Console.WriteLine("生成Extension.cs");
                //App/Extension.cs
                config.InitExtension(AppName);
                Console.WriteLine("生成Router.cs");
                //App/Router.cs
                config.InitRouter(AppName);
                Console.WriteLine("生成WebCompiler.aspx");
                //App/WebCompiler.cs
                config.InitWebCompiler(AppName);
                Console.WriteLine("生成Common层");
                //Common
                config.InitCommon(AppName);
                Console.WriteLine("生成Content层");
                //Content
                config.InitContent(AppName);
                Console.WriteLine("生成Controllers层");
                //Controllers
                config.InitControllers(AppName);
                Console.WriteLine("生成Models层");
                //Models
                config.InitModels(AppName);
                Console.WriteLine("生成Views层");
                //Views
                config.InitViews(AppName);
                Console.WriteLine("生成Web层");
                //Web
                config.InitWeb(AppName);
                Console.WriteLine(AppName + "配置完成\r\n");
                //添加模块文件
                proj.AddModule(AppName);
                proj.Save();

                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("请把NFinal放到项目根目录下");
                Console.ReadKey();
            }
        }
        public static string[] MergeArray(string[] a, string[] b)
        {
            List<string> student = new List<string>();
            foreach (string s1 in a)
            {
                student.Add(s1);
            }
            foreach (string s2 in b)
            {
                bool flag = true;
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i] == s2)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    student.Remove(s2);
                }
            }
            string[] c1 = student.ToArray();
            return c1;
        } 
    }
}

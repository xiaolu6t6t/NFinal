using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Configuration;

namespace WebCompiler
{
    class Program
    {
        public static Assembly GetAssembly(string root)
        {
            string[] dllFiles = Directory.GetFiles(root + "bin\\", "*.dll");
            Assembly ass = null;
            Type t = null;
            //引入相关dll文件
            Console.WriteLine("查找含有NFinal.Frame的dll文件");
            if (dllFiles.Length > 0)
            {
                for (int i = 0; i < dllFiles.Length; i++)
                {
                    try
                    {
                        ass = Assembly.LoadFile(dllFiles[i]);
                        t = ass.GetType("NFinal.Frame", false);
                        if (t != null)
                        {
                            return ass;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return null;
        }
        public static void WebCompile()
        {
            string root = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName + "\\";
            string[] dllFiles = Directory.GetFiles(root + "bin\\", "*.dll");
            //获取dll文件
            Console.WriteLine("获取dll文件");
            Assembly ass = GetAssembly(root);
            if (ass == null) return;
            Type FrameType = ass.GetType("NFinal.Frame", true);
            //开始执行
            if (FrameType != null)
            {
                Console.WriteLine("开始生成");
                object Frame = Activator.CreateInstance(FrameType, root);
                Console.WriteLine("获取Web.Config中的配置信息");
                MethodInfo GetApps = FrameType.GetMethod("GetApps");
                string[] Apps=(string[])GetApps.Invoke(Frame, null);
                Console.WriteLine("获取数据库信息");
                MethodInfo GetDB = FrameType.GetMethod("GetDB");
                GetDB.Invoke(Frame, null);
                Type dataUtility = ass.GetType("NFinal.DB.Coding.DataUtility");
                Type DB = ass.GetType("NFinal.DB.Coding.DB");
                FieldInfo DbStore = DB.GetField("DbStore");
                object db = DbStore.GetValue(null);
                Console.WriteLine("创建Main函数");
                MethodInfo CreateMain = FrameType.GetMethod("CreateMain");
                CreateMain.Invoke(Frame,new object[]{Apps});
                Console.WriteLine("获取项目名称");
                FieldInfo AssemblyTitle= FrameType.GetField("AssemblyTitle");
                string AssemblyString= (string)AssemblyTitle.GetValue(Frame);
                if (Apps != null)
                {
                    for (int i = 0; i < Apps.Length; i++)
                    {
                        string App = Apps[i];
                        Console.WriteLine(App + ":生成开始");
                        Console.WriteLine(App+":获取配置");
                        Type configSet= ass.GetType(string.Format("{0}.{1}.Config", AssemblyString, App));
                        object set = configSet.GetField("set").GetValue(Frame);
                        Type config = ass.GetType("NFinal.Config");
                        Console.WriteLine(App + ":初始化");
                        MethodInfo Init = config.GetMethod("Init");
                        Init.Invoke(set, new string[]{App});
                        Console.WriteLine(App + ":创建Application");
                        Type Application = ass.GetType("NFinal.Application");
                        object application = Activator.CreateInstance(Application, set);
                        Console.WriteLine(App + ":创建框架");
                        MethodInfo CreateApp = Application.GetMethod("CreateApp");
                        CreateApp.Invoke(application,null);
                        Console.WriteLine(App + ":创建Models");
                        MethodInfo CreateModelsFile = Application.GetMethod("CreateModelsFile");
                        CreateModelsFile.Invoke(application,null);
                        Console.WriteLine(App + ":创建路由");
                        MethodInfo CreateRouter = Application.GetMethod("CreateRouter");
                        CreateRouter.Invoke(application,null);
                        Console.WriteLine(App + ":创建DAL");
                        MethodInfo CreateDAL = Application.GetMethod("CreateDAL");
                        CreateDAL.Invoke(application,null);
                        Console.WriteLine(App + ":创建Compile");
                        MethodInfo CreateCompile = Application.GetMethod("CreateCompile");
                        CreateCompile.Invoke(application,null);
                        Console.WriteLine(App + ":生成结束");
                    }
                }
                Console.WriteLine("生成结束");
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("开始编译");
            //编译文件
            WebCompile();
            Console.WriteLine("编译完成");
        }
    }
}

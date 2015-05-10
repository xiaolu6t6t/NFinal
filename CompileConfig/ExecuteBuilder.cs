using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace CompileConfig
{
    class ExecuteBuilder
    {
        public static void WebCompile()
        {
            string root = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName + "\\";
            string[] dllFiles = Directory.GetFiles(root + "bin\\", "*.dll");
            Assembly ass = null;
            Type t = null;
            string dllFile = null;
            Type BuilderType = null;
            //引入相关dll文件
            Console.WriteLine("查找含有NFinal.Builder的dll文件");
            if (dllFiles.Length > 0)
            {
                for (int i = 0; i < dllFiles.Length; i++)
                {
                    try
                    {
                        ass = Assembly.LoadFile(dllFiles[i]);
                        t = ass.GetType("NFinal.Builder", false);
                        if (t != null)
                        {
                            dllFile = dllFiles[i];
                            BuilderType = t;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            //开始执行
            Console.WriteLine("查找结束");
            if (BuilderType != null)
            {
                Console.WriteLine("获取Builder对象");
                object builder = Activator.CreateInstance(BuilderType, root);
                MethodInfo method = BuilderType.GetMethod("Create");
                Console.WriteLine("执行");
                method.Invoke(builder, null);
                Console.WriteLine("完毕");
            }
        }
    }
}

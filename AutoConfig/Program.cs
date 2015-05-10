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
                Console.WriteLine("开始引入dll");
                Project proj = new Project();
                //添加相关dll文件
                proj.AddLibrary(projFiles[0].FullName);
                Console.WriteLine("开始配置Web.config");
                
                
                //编译文件
                Config config = new Config(root);
                //初始化Main函数
                config.InitMain();
                //修改Web.config文件
                Config.SetWebConfig(Config.IsNewIIS(),root);
                //WebCompile();
                Console.WriteLine("配置完成");
                //Console.ReadKey();
            }
            else
            {
                Console.WriteLine("请把NFinal放到项目根目录下");
                //Console.ReadKey();
            }
        }
    }
}

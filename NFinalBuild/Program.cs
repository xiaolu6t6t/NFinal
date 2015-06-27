using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NFinalBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Default;
            string root = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName + "\\";
            FileInfo[] projFiles = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.GetFiles("*.csproj");
            if (projFiles.Length > 0)
            {
                string projectFileName = projFiles[0].FullName;
                string projectName = Path.GetFileNameWithoutExtension(projectFileName);
                Console.WriteLine("开始清理");
                string fileName = root + "obj\\Release\\" + projectName + ".dll";
                Console.WriteLine("删除" + fileName);
                File.Delete(fileName);
                fileName = root + "obj\\Release\\" + projectName + ".pdb";
                Console.WriteLine("删除"+fileName);
                File.Delete(fileName);
                Console.WriteLine("开始编译");
                Build build = new Build();
                string version = null;
                if (args!=null && args.Length>0)
                {
                    version = args[0];
                }
                string result = build.BuildNFinal(projectFileName,version);
                string[] errors = result.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine(result);
                Console.WriteLine("复制文件到bin目录下");
                fileName = root + "obj\\Release\\" + projectName + ".dll";
                if (File.Exists(fileName))
                {
                    File.Copy(fileName, root + "bin\\" + projectName + ".dll", true);
                    Console.WriteLine("文件" + projectName + ".dll");
                }
                else
                {
                    Console.WriteLine("生成出错,无法复制");
                }
                fileName = root + "obj\\Release\\" + projectName + ".pdb";
                if (File.Exists(fileName))
                {
                    File.Copy(fileName, root + "bin\\" + projectName + ".pdb", true);
                    Console.WriteLine("文件" + projectName + ".pdb");
                }
                fileName = root + "Web.config";
                if (File.Exists(fileName))
                {
                    File.Copy(fileName, root + "bin\\" + projectName + ".dll.config", true);
                    Console.WriteLine("文件" + projectName + ".dll.config");
                }
                if (args==null || args.Length<1)
                {
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("请把NFinal放到项目根目录下");
            }
        }
    }
}

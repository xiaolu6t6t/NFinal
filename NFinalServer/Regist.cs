using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace NFinalServer
{
    class Regist
    {

        public void AddFileContextMenuItem(string itemName, string associatedProgramFullPath)
        {
            //创建项：shell 
            RegistryKey shellKey = Registry.ClassesRoot.OpenSubKey(@"*\shell", true);
            if (shellKey == null)
            {
                shellKey = Registry.ClassesRoot.CreateSubKey(@"*\shell");
            }
            //创建项：右键显示的菜单名称
            RegistryKey rightCommondKey = shellKey.CreateSubKey(itemName);
            RegistryKey associatedProgramKey = rightCommondKey.CreateSubKey("command");

            //创建默认值：关联的程序
            associatedProgramKey.SetValue(string.Empty, associatedProgramFullPath);

            //刷新到磁盘并释放资源
            associatedProgramKey.Close();
            rightCommondKey.Close();
            shellKey.Close();
        }

        public void AddDirectory()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory+"NFinalServer.exe";
            //查看是否已经添加
            RegistryKey NFinalKey = Registry.ClassesRoot.OpenSubKey(@"directory\shell\NFinalServer", false);
            if (NFinalKey == null)
            {
                //创建项：shell 
                RegistryKey shellKey = Registry.ClassesRoot.OpenSubKey(@"directory\shell", true);
                if (shellKey == null)
                {
                    shellKey = Registry.ClassesRoot.CreateSubKey(@"*\shell");
                }

                //创建项：右键显示的菜单名称
                RegistryKey rightCommondKey = shellKey.CreateSubKey("NFinalServer");
                rightCommondKey.SetValue("", "用NFinalServer加载此目录为网站");
                RegistryKey associatedProgramKey = rightCommondKey.CreateSubKey("command");

                //创建默认值：关联的程序
                associatedProgramKey.SetValue("", string.Format("\"{0}\" \"%1\"", path));


                //刷新到磁盘并释放资源
                associatedProgramKey.Close();
                rightCommondKey.Close();
                shellKey.Close();
            }
            else
            {
                NFinalKey.Close();
            }
        }
        public void DeleteDirectory()
        {
            string path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            //查看是否已经添加
            RegistryKey NFinalKey = Registry.ClassesRoot.OpenSubKey(@"directory\shell\NFinalServer", false);
            if (NFinalKey != null)
            {
                Registry.ClassesRoot.DeleteSubKeyTree(@"directory\shell\NFinalServer", false);
            }
        }
    }
}

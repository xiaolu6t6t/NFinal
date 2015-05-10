using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Release
{
    class Program
    {
        static void Main(string[] args)
        {
            string root = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName + "\\";
            Releaser releaser = new Releaser(root);
            releaser.Main();
        }
    }
}

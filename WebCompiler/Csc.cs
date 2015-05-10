using System;
using System.Collections.Generic;
using System.Text;

namespace WebCompiler
{
    public enum DotNetVersion
    {
        v2,v3,v35,v4,v45,v451
    }
    class Csc
    {
        public string CscPath = @"C:\Program Files (x86)\MSBuild\12.0\bin\Csc.exe";
        public string GetDirectory(DotNetVersion version)
        {
            string path="";
            switch(version)
            {
                case DotNetVersion.v2:path=@"C:\Windows\Microsoft.NET\Framework\v2.0.50727\";break;
                case DotNetVersion.v3:path=@"C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.0\";break;
                case DotNetVersion.v35:path=@"C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.5\";break;
                case DotNetVersion.v4:path=@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\";break;
                case DotNetVersion.v45:path=@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\";break;
                case DotNetVersion.v451:path=@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\";break;
            }
            return path;
        }
        public string GetReference(string[] referenceFiles,DotNetVersion version)
        {
            string referenceFile="";
            string result = "";
            string path = GetDirectory(version);
            for (int i = 0; i < referenceFiles.Length;i++ )
            {
                referenceFile = referenceFiles[i];
                result += string.Format(" /reference:\"{0}{1}\" ",path ,referenceFile);
            }
            return result;
        }
       
        public void Run(DotNetVersion version)
        {
            string args = CscPath;
            args+= " /noconfig /unsafe+ /nowarn:0219,1701,1702,2008 /nostdlib+ /errorreport:prompt /warn:4 /define:TRACE /errorendlocation /preferreduilang:zh-CN /highentropyva- ";
            string[] references= new string[]{"",""};
            args+=GetReference(references,version);
            args += @"/debug:pdbonly /optimize+ /out:obj\Release\WebMvc.dll /target:library /utf8output ";
            string[] csharpFiles=new string[2];
            for(int i=0;i<csharpFiles.Length;i++)
            {
                args +=string.Format(" {0} ",csharpFiles[i]);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

namespace NFinal.Compile
{
    /// <summary>
    /// csharp中参数解析类
    /// </summary>
    public class CSharpCompiler
    {
        public List<CSharpDeclaration> Parser(string csharpCode)
        {
            string partern = @"(?:\{|;)\s*(\S+)\s+(\S+)\s*(?:=|;)";
            Regex reg = new Regex(partern);
            MatchCollection mac = reg.Matches(partern);
            List<CSharpDeclaration> declarations = new List<CSharpDeclaration>();
            CSharpDeclaration declaration = null; 
            if (mac.Count > 0)
            {
                for(int i=0;i<mac.Count;i++)
                {
                    declaration = new CSharpDeclaration();
                    declaration.typeName= mac[i].Groups[1].Value;
                    declaration.varName = mac[i].Groups[2].Value;
                }
                declarations.Add(declaration);
            }
            return declarations;
        }
        public void SetViewsCsharpFile(string fileName)
        {
            
        }
    }
}
using System;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

namespace NFinal.Compile
{
    /// <summary>
    /// csharp中参数解析类
    /// </summary>
    public class CSharpDeclarationParser
    {
        public List<CSharpDeclaration> Parse(string csharpCode)
        {
            string partern = @"[^_0-9a-zA-Z](([_0-9a-zA-Z\.]+)\s*(\[\s*\]|\<\s*[_0-9a-zA-Z\.]+\s*\>|\<\s*[_0-9a-zA-Z\.]+\s*,\s*[_0-9a-zA-Z\.]+\s*\>)?)\s+([_0-9a-zA-Z]+)\s*(?:;|=([^;]+);)";
            Regex reg = new Regex(partern,RegexOptions.Multiline);
            MatchCollection mac = reg.Matches(csharpCode);
            List<CSharpDeclaration> declarations = new List<CSharpDeclaration>();
            CSharpDeclaration declaration = null; 
            if (mac.Count > 0)
            {
                for(int i=0;i<mac.Count;i++)
                {
                    declaration = new CSharpDeclaration();
                    declaration.typeName= mac[i].Groups[1].Value;
                    declaration.varName = mac[i].Groups[4].Value;
                    if (mac[i].Groups[5].Success)
                    {
                        declaration.expression = mac[i].Groups[5].Value;
                    }
                    else
                    {
                        declaration.expression = null;
                    }
                    if (declaration.typeName != "var")
                    {
                        declarations.Add(declaration);
                    }
                }
            }
            return declarations;
        }
    }
}
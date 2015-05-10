using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

/*******************************************
 * 压缩jsp,html中的代码，去掉所有空白符、换行符
 * @author  bearrui(ak-47)
 * @version 0.1
 * @date     2010-5-13
 *******************************************/
namespace NFinal.Template
{
    /// <summary>
    /// Html代码压缩类
    /// </summary>
    public class HtmlCompressor
    {
        private static String tempPreBlock = "%%%HTMLCOMPRESS~PRE&&&";
        private static String tempTextAreaBlock = "%%%HTMLCOMPRESS~TEXTAREA&&&";
        private static String tempScriptBlock = "%%%HTMLCOMPRESS~SCRIPT&&&";
        private static String tempStyleBlock = "%%%HTMLCOMPRESS~STYLE&&&";
        private static String tempJspBlock = "%%%HTMLCOMPRESS~JSP&&&";

        private static Regex commentPattern = new Regex("<!--\\s*[^\\[].*?-->",   RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static Regex itsPattern = new Regex(">\\s+?<",  RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static Regex prePattern = new Regex("<pre[^>]*?>.*?</pre>",  RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static Regex taPattern = new Regex("<textarea[^>]*?>.*?</textarea>",  RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static Regex jspPattern = new Regex("<%([^-@][\\w\\W]*?)%>",   RegexOptions.IgnoreCase | RegexOptions.Multiline);
        // <script></script>
        private static Regex scriptPattern = new Regex("(?:<script\\s*>|<script type=['\"]text/javascript['\"]\\s*>)(.*?)</script>",  RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static Regex stylePattern = new Regex("<style[^>()]*?>(.+)</style>",  RegexOptions.IgnoreCase | RegexOptions.Multiline);

        // 单行注释，
        private static Regex signleCommentPattern = new Regex("//.*");
        // 字符串匹配
        private static Regex stringPattern = new Regex("(\"[^\"\\n]*?\"|'[^'\\n]*?')");
        // trim去空格和换行符
        private static Regex trimPattern = new Regex("\\n\\s*",  RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static Regex trimPattern2 = new Regex("\\s*\\r",  RegexOptions.IgnoreCase | RegexOptions.Multiline);
        // 多行注释
        private static Regex multiCommentPattern = new Regex("/\\*.*?\\*/",  RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private static String tempSingleCommentBlock = "%%%HTMLCOMPRESS~SINGLECOMMENT&&&";  // //占位符
        private static String tempMulitCommentBlock1 = "%%%HTMLCOMPRESS~MULITCOMMENT1&&&";  // /*占位符
        private static String tempMulitCommentBlock2 = "%%%HTMLCOMPRESS~MULITCOMMENT2&&&";  // */占位符


        public static String compress(String fileName, System.Text.Encoding encoding)
        {
            StreamReader sr = new StreamReader(fileName, encoding);
            string html= sr.ReadToEnd();
            sr.Close();
            return compress(html);
        }
        public static String compress(String html)
        {
            if (html == null || html.Length == 0)
            {
                return html;
            }

            List<String> preBlocks = new List<String>();
            List<String> taBlocks = new List<String>();
            List<String> scriptBlocks = new List<String>();
            List<String> styleBlocks = new List<String>();
            List<String> jspBlocks = new List<String>();

            String result = html;

            //preserve inline java code
            Match jspMatcher = jspPattern.Match(result);

            while (jspMatcher.Success)
            {
                jspBlocks.Add(jspMatcher.Value);
                jspMatcher = jspMatcher.NextMatch();
            }
            result = jspPattern.Replace(result, tempJspBlock);

            //preserve PRE tags
            Match preMatcher = prePattern.Match(result);
            while (preMatcher.Success)
            {
                preBlocks.Add(preMatcher.Value);
                preMatcher = preMatcher.NextMatch();
            }
            result = prePattern.Replace(result, tempPreBlock);

            //preserve TEXTAREA tags
            Match taMatcher = taPattern.Match(result);
            while (taMatcher.Success)
            {
                taBlocks.Add(taMatcher.Value);
                taMatcher = taMatcher.NextMatch();
            }
            result = taPattern.Replace(result, tempTextAreaBlock);

            //preserve SCRIPT tags
            Match scriptMatcher = scriptPattern.Match(result);
            while (scriptMatcher.Success)
            {
                scriptBlocks.Add(scriptMatcher.Value);
                scriptMatcher = scriptMatcher.NextMatch();
            }
            result = scriptPattern.Replace(result, tempScriptBlock);

            // don't process inline css 
            Match styleMatcher = stylePattern.Match(result);
            while (styleMatcher.Success)
            {
                styleBlocks.Add(styleMatcher.Value);
                styleMatcher = styleMatcher.NextMatch();
            }
            result = stylePattern.Replace(result, tempStyleBlock);

            //process pure html
            result = processHtml(result);

            //process preserved blocks
            result = processPreBlocks(result, preBlocks);
            result = processTextareaBlocks(result, taBlocks);
            result = processScriptBlocks(result, scriptBlocks);
            result = processStyleBlocks(result, styleBlocks);
            result = processJspBlocks(result, jspBlocks);

            preBlocks = taBlocks = scriptBlocks = styleBlocks = jspBlocks = null;

            return result.Trim();
        }

        private static String processHtml(String html)
        {
            String result = html;

            //remove comments
            //		if(removeComments) {
            result = commentPattern.Replace(result, "");
            //		}

            //remove inter-tag spaces
            //		if(removeIntertagSpaces) {
            result = itsPattern.Replace(result, "><");
            //		}

            //remove multi whitespace characters
            //		if(removeMultiSpaces) {
            result = new Regex("\\s{2,}").Replace(result, " ");

            return result;
        }

        private static String processJspBlocks(String html, List<String> blocks)
        {
            String result = html;
            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i] = compressJsp(blocks[i]);
            }
            //put preserved blocks back
            Regex regex = new Regex(tempJspBlock);
            for (int i = 0; i < blocks.Count; i++)
            {
                result = regex.Replace(result, blocks[i], 1);
            }

            return result;
        }
        private static String processPreBlocks(String html, List<String> blocks)
        {
            String result = html;

            //put preserved blocks back

            Regex regex = new Regex(tempPreBlock);
            for (int i = 0; i < blocks.Count; i++)
            {
                result = regex.Replace(result, blocks[i], 1);
            }

            return result;
        }

        private static String processTextareaBlocks(String html, List<String> blocks)
        {
            String result = html;

            //put preserved blocks back
            Regex regex = new Regex(tempTextAreaBlock);
            for (int i = 0; i < blocks.Count; i++)
            {
                result = regex.Replace(result, blocks[i], 1);
            }

            return result;
        }

        private static String processScriptBlocks(String html, List<String> blocks)
        {
            String result = html;

            //		if(compressJavaScript) {
            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i] = compressJavaScript(blocks[i]);
            }
            //		}

            //put preserved blocks back
            Regex regex = new Regex(tempScriptBlock);
            for (int i = 0; i < blocks.Count; i++)
            {
                result=regex.Replace(result, blocks[i], 1);
            }

            return result;
        }

        private static String processStyleBlocks(String html, List<String> blocks)
        {
            String result = html;

            //		if(compressCss) {
            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i] = compressCssStyles(blocks[i]);
            }
            //		}

            //put preserved blocks back
            Regex regex = new Regex(tempStyleBlock);
            for (int i = 0; i < blocks.Count; i++)
            {
                result = regex.Replace(result, blocks[i], 1);
            }

            return result;
        }

        private static String compressJsp(String source)
        {
            //check if block is not empty
            Match jspMatcher = jspPattern.Match(source);
            if (jspMatcher.Success)
            {
                String result = compressJspJs(jspMatcher.Groups[1].Value);
                return (new StringBuilder(source.Substring(0, jspMatcher.Groups[1].Index)).Append(result).Append(source.Substring(jspMatcher.Groups[1].Index + jspMatcher.Groups[1].Length))).ToString();
            }
            else
            {
                return source;
            }
        }
        private static String compressJavaScript(String source)
        {
            //check if block is not empty
            Match scriptMatcher = scriptPattern.Match(source);
            if (scriptMatcher.Success)
            {
                String result = compressJspJs(scriptMatcher.Groups[1].Value);
                return (new StringBuilder(source.Substring(0, scriptMatcher.Groups[1].Index)).Append(result).Append(source.Substring(scriptMatcher.Groups[1].Index + scriptMatcher.Groups[1].Length))).ToString();
            }
            else
            {
                return source;
            }
        }

        private static String compressCssStyles(String source)
        {
            //check if block is not empty
            Match styleMatcher = stylePattern.Match(source);
            if (styleMatcher.Success)
            {
                // 去掉注释，换行
                String result = multiCommentPattern.Replace(styleMatcher.Groups[1].Value, "");
                result = trimPattern.Replace(result, "");
                result = trimPattern2.Replace(result, "");
                return (new StringBuilder(source.Substring(0, styleMatcher.Groups[1].Index)).Append(result).Append(source.Substring(styleMatcher.Groups[1].Index + styleMatcher.Groups[1].Length))).ToString();
            }
            else
            {
                return source;
            }
        }

        private static String compressJspJs(String source)
        {
            String result = source;
            // 因注释符合有可能出现在字符串中，所以要先把字符串中的特殊符好去掉
            Match stringMatcher = stringPattern.Match(result);
            while (stringMatcher.Success)
            {
                String tmpStr = stringMatcher.Value;

                if (tmpStr.IndexOf("//") != -1 || tmpStr.IndexOf("/*") != -1 || tmpStr.IndexOf("*/") != -1)
                {
                    String blockStr = tmpStr.Replace("//", tempSingleCommentBlock);
                    blockStr = new Regex("/\\*").Replace(blockStr, tempMulitCommentBlock1);
                    blockStr = new Regex("\\*/").Replace(blockStr, tempMulitCommentBlock2);
                    result = result.Replace(tmpStr, blockStr);
                }
                stringMatcher = stringMatcher.NextMatch();
            }
            // 去掉注释
            result = signleCommentPattern.Replace(result, "");
            result = multiCommentPattern.Replace(result, "");
            result = trimPattern2.Replace(result, "");
            result = trimPattern.Replace(result, " ");
            // 恢复替换掉的字符串
            result = result.Replace(tempSingleCommentBlock, "//").Replace(tempMulitCommentBlock1, "/*")
                    .Replace(tempMulitCommentBlock2, "*/");

            return result;
        }
    }
}
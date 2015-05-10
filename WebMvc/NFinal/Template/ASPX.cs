using System;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

namespace NFinal.Template
{
    /// <summary>
    /// ASPX模板引擎
    /// </summary>
    public class ASPX
    {
        /// <summary>
        /// 当某变量为空时设置默认值
        /// </summary>
        /// <param name="val">变量</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public string getString(string val, string def)
        {
            return string.IsNullOrEmpty(val) ? def : val;
        }
        /// <summary>
        /// 获取模板渲染后的字符串
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="encoding">文件的编码方式</param>
        /// <returns></returns>
        public string GetRenderText(string fileName, System.Text.Encoding encoding, int tab)
        {
            StringWriter sw = new StringWriter();
            Render(sw, fileName, tab);
            return sw.ToString();
        }
        /// <summary>
        /// 处理reference标签,实现模板引用
        /// </summary>
        /// <param name="template">模板字符串</param>
        /// <param name="deepth">深度</param>
        /// <returns></returns>
        public string Reference(string template, int deepth)
        {
            Regex reg = new Regex(@"<%@\s+Reference\s+\S+\s*=\s*""([^\s""]+)""\s*%>");
            MatchCollection mc = reg.Matches(template);
            string fileName;
            string content;
            int relative_position = 0;
            if (mc.Count > 0 && deepth < 6)
            {
                deepth++;
                for (int i = 0; i < mc.Count; i++)
                {
                    Match m = mc[i];
                    fileName = Frame.MapPath(m.Groups[1].Value.TrimStart('~'));
                    if (File.Exists(fileName))
                    {
                        StreamReader sr = new StreamReader(fileName, System.Text.Encoding.UTF8);
                        content = sr.ReadToEnd();
                        template = template.Remove(relative_position + m.Index, m.Length);
                        template = template.Insert(relative_position + m.Index, content);
                        relative_position += content.Length - m.Length;
                    }
                }
                template = Reference(template, deepth);
            }
            return template;
        }
        /// <summary>
        /// 获取完整的模板字符串
        /// </summary>
        /// <param name="template">模板字符串</param>
        /// <returns>模板字符串,包含引用模板的信息</returns>
        public string GetAllTemplateFromReference(string template)
        {
            return template;
        }
        public Dictionary<string, string> GetRegistWebControls(string template)
        {
            string pattern = @"<%@\s+Register\s+Src=""([^""\s]+)""\s+TagPrefix=""([^""\s]+)""\s+TagName=""([^""\s]+)""\s*%>";
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
            Dictionary<string, string> dicControls = new Dictionary<string, string>();
            MatchCollection mac = reg.Matches(template);
            if (mac.Count > 0)
            {
                for (int i = 0; i < mac.Count; i++)
                {
                    dicControls.Add(mac[i].Groups[2].Value + ":" + mac[i].Groups[3].Value, mac[i].Groups[1].Value);
                }
            }
            return dicControls;
        }
        public string TransWebControls(string template, int deepth)
        {
            Dictionary<string, string> dicControls = GetRegistWebControls(template);
            string pattern = @"<([^<\s:>]+:[^<\s:>]+)\s+runat=""[^""\s]+""\s+id=""[^""\s]+""\s*/>";
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection mac = reg.Matches(template);
            string fileName;
            string content;
            Match m;
            int relative_position = 0;
            if (mac.Count > 0 && deepth < 6)
            {
                for (int i = 0; i < mac.Count; i++)
                {
                    m = mac[i];
                    if (dicControls.ContainsKey(mac[i].Groups[1].Value))
                    {
                        fileName = Frame.MapPath(dicControls[mac[i].Groups[1].Value].TrimStart('~'));
                        StreamReader sr = new StreamReader(fileName, System.Text.Encoding.UTF8);
                        content = sr.ReadToEnd();
                        template = template.Remove(relative_position + m.Index, m.Length);
                        template = template.Insert(relative_position + m.Index, content);
                        relative_position += content.Length - m.Length;
                    }
                    //如果找不到注册的组件名,则删除
                    else
                    {
                        template = template.Remove(relative_position + m.Index, m.Length);
                        relative_position -= m.Length;
                    }
                }
                template = TransWebControls(template, deepth);
            }
            return template;
        }
        /// <summary>
        /// 渲染模板
        /// </summary>
        /// <param name="writer">写操作类</param>
        /// <param name="template">模板字符串</param>
        /// <returns></returns>
        public string Render(TextWriter writer, string template, int tab)
        {
            int deepth = 0;
            //template= Reference(template,deepth);
            deepth = 0;
            template = TransWebControls(template, deepth);
            string text = "";
            string section = @"(<%[^%>]+%>)";
            Regex reg = new Regex(section);
            MatchCollection matches = reg.Matches(template);
            //开始
            int text_start = 0;
            //结束
            int text_end = 0;
            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    text_end = m.Index - 1;
                    if (m.Value[2] == '@')
                    {

                        //添加写入html的源码字符串
                        if (text_end - text_start >= 0)
                        {
                            text = template.Substring(text_start, text_end - text_start + 1);
                            if (!IsNullOrWhiteSpace(text))
                            {
                                WriteTab(writer, tab);
                                text = BuildWriteCode(text);
                                writer.WriteLine(text);
                            }
                        }
                        //如果<%@ Reference VirtualPath="" %>

                        //让开头指向结尾
                        text_start = m.Index + m.Length;
                    }
                    else if (m.Value[2] == '=')
                    {
                        //添加写入html的源码字符串
                        if (text_end - text_start >= 0)
                        {
                            text = template.Substring(text_start, text_end - text_start + 1);
                            if (!IsNullOrWhiteSpace(text))
                            {
                                WriteTab(writer, tab);
                                text = BuildWriteCode(text);
                                writer.WriteLine(text);
                            }
                        }
                        //替换掉<%%>,转为源码

                        if (m.Value.IndexOf("}") > 0)
                        {
                            tab--;
                        }
                        WriteTab(writer, tab);
                        writer.WriteLine(BuildWriteVar(m.Value.Replace("<%=", "").Replace("%>", "").Trim()));
                        if (m.Value.IndexOf("{") > 0)
                        {
                            tab++;
                        }
                        //让开头指向结尾
                        text_start = m.Index + m.Length;
                    }
                    else
                    {

                        //添加写入html的源码字符串
                        if (text_end - text_start >= 0)
                        {
                            text = template.Substring(text_start, text_end - text_start + 1);
                            if (!IsNullOrWhiteSpace(text))
                            {
                                WriteTab(writer, tab);
                                text = BuildWriteCode(text);
                                writer.WriteLine(text);
                            }
                        }

                        if (m.Value.IndexOf("}") > 0)
                        {
                            tab--;
                        }
                        //替换掉<%%>,转为源码
                        WriteTab(writer, tab);
                        writer.WriteLine(m.Value.Replace("<%", "").Replace("%>", "").Trim());
                        if (m.Value.IndexOf("{") > 0)
                        {
                            tab++;
                        }
                        //让开头指向结尾
                        text_start = m.Index + m.Length;
                    }
                }
                text_end = template.Length - 1;
                text = template.Substring(text_start, text_end - text_start + 1);
                if (!IsNullOrWhiteSpace(text))
                {
                    WriteTab(writer, tab);
                    text = BuildWriteCode(text);
                    writer.WriteLine(text);
                }
            }
            writer.Close();
            return writer.ToString();
        }
        /// <summary>
        /// 输入代码中的缩进
        /// </summary>
        /// <param name="tw">写类</param>
        /// <param name="tab">table符的数量</param>
        /// <param name="isFirstLine">是否是第一行</param>
        public void WriteTab(TextWriter tw, int tab)
        {
            if (tab > 0)
            {
                for (int i = 0; i < tab; i++)
                {
                    tw.Write("\t");
                }
            }
        }
        /// <summary>
        /// 返回写变量的csharp代码
        /// </summary>
        /// <param name="text">内容</param>
        /// <returns></returns>
        public string BuildWriteVar(string text)
        {
            //text = ReserveString(text);//输出csharp无需转义
            text = string.Format("Write({0});", text.Trim());
            return text;
        }
        public bool IsNullOrWhiteSpace(string text)
        {
            char[] space = new char[] { ' ', '\r', '\n', '\t', '\f', '\v' };
            return string.IsNullOrEmpty(text.Trim(space));
        }
        /// <summary>
        /// 返回输出字符串的csharp代码
        /// </summary>
        /// <param name="text">字符串</param>
        /// <returns></returns>
        public string BuildWriteCode(string text)
        {
            
            text = ReserveString(text);
            text = string.Format("Write(\"{0}\");", text.Trim());
            return text;
        }
        /// <summary>
        /// 字符串反转义
        /// </summary>
        /// <param name="text">字符串</param>
        /// <returns>返回csharp中的字符串表示</returns>
        public string ReserveString(string text)
        {
            char[] temp_old = text.ToCharArray();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < temp_old.Length; i++)
            {
                switch (temp_old[i])
                {
                    case '\'': sb.Append("\\\'"); break;
                    case '\"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\0': sb.Append("\\0"); break;
                    case '\a': sb.Append("\\a"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    case '\v': sb.Append("\\v"); break;
                    default: sb.Append(temp_old[i]); break;
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 渲染模板
        /// </summary>
        /// <param name="context">页面类</param>
        /// <param name="fileName">模板文件名</param>
        public void Render(HttpContext context, string fileName, int tab)
        {
            Render(context.Response.Output, context.Server.MapPath(fileName), tab);
        }
        /// <summary>
        /// 渲染模板到指定文件
        /// </summary>
        /// <param name="fileName">模板文件路径</param>
        /// <param name="outFileName">输出文件路径</param>
        public void RenderTo(string fileName, string outFileName, int tab)
        {
            if (!Directory.Exists(Path.GetDirectoryName(outFileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outFileName));
            }
            StreamWriter sw = new StreamWriter(outFileName, false, System.Text.Encoding.UTF8);
            Render(sw, fileName, tab);
        }
    }
}
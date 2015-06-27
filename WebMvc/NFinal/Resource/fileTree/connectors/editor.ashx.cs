using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Diagnostics;
using Microsoft.Win32;

namespace NFinal.Resource.fileTree.connectors
{
    /// <summary>
    /// editor 的摘要说明
    /// </summary>
    public class editor : IHttpHandler
    {
        private string ExeCommand(string exeFileName, string commandText)
        {
            Process p = new Process();
            p.StartInfo.FileName = exeFileName;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.Arguments = commandText;
            p.StartInfo.StandardOutputEncoding = System.Text.Encoding.Default;
            p.StartInfo.StandardErrorEncoding = System.Text.Encoding.Default;
            p.StartInfo.CreateNoWindow = true;
            string strOutput = null;
            try
            {
                p.Start();
                strOutput = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();
            }
            catch (Exception e)
            {
                strOutput = e.Message;
            }
            return strOutput;
        }
        /// <summary>
        /// 获取已安装的所有NET版本
        /// </summary>
        /// <returns></returns>
        public string[] GetDotNetVersions()
        {
            RegistryKey NDPKey= Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
            if (NDPKey != null)
            {
                return NDPKey.GetSubKeyNames();
            }
            else
            {
                return null;
            }
        }
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string method = context.Request.Form["method"];
            switch (method)
            {
                case "load":
                    {
                        string file = context.Request.Form["file"];
                        if (File.Exists(context.Server.MapPath(file)))
                        {
                            StreamReader sr = new StreamReader(context.Server.MapPath(file), System.Text.Encoding.UTF8);
                            context.Response.Write(sr.ReadToEnd());
                            sr.Close();
                        }
                    } break;
                case "save":
                    {
                        string file =context.Request.Form["file"];
                        if (File.Exists(context.Server.MapPath(file)))
                        {
                            StreamWriter sw = new StreamWriter(context.Server.MapPath(file), false, System.Text.Encoding.UTF8);
                            sw.Write(HttpUtility.UrlDecode(context.Request.Form["content"]));
                            sw.Close();
                            context.Response.ContentType = "application/json";
                            context.Response.Write("true");
                        }
                        else
                        {
                            context.Response.ContentType = "application/json";
                            context.Response.Write("false");
                        } break;
                    }
                case "apps": {
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    string fileName=context.Server.MapPath("/Web.config");
                    if (File.Exists(fileName))
                    {
                        doc.Load(fileName);
                        //读取Apps中的值
                        System.Xml.XmlNode appsNode = doc.DocumentElement.SelectSingleNode("appSettings/add[@key='Apps']");
                        if (appsNode != null && appsNode.Attributes.Count > 0 && appsNode.Attributes["value"] != null)
                        {
                            context.Response.Write(appsNode.Attributes["value"].Value);
                        }
                        else
                        {
                            context.Response.Write("");
                        }
                    }
                    else
                    {
                        context.Response.Write("");
                    }
                } break;
                case "version":
                    {
                        List<string> clr = new List<string>();
                        string[] versions = GetDotNetVersions();
                        for (int i = 0; i < versions.Length; i++)
                        {
                            if (versions[i].IndexOf("v3.5") > -1)
                            {
                                clr.Add("v3.5");
                            }
                            else if(versions[i].IndexOf("v4.0")>-1)
                            {
                                clr.Add("v4.0");
                            }
                        }
                        context.Response.Write(string.Join(",",clr.ToArray()));
                } break;
                case "build":
                    {
                        string version = context.Request.Form["version"];
                        context.Response.Write(ExeCommand(context.Server.MapPath("/NFinal/NFinalBuild.exe"), version));
                    } break;
                default:
                    {

                    } break;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
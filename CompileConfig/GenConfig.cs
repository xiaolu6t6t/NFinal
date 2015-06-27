using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CompileConfig
{
    public class GenConfig
    {
        public List<string> controllerFiles = new List<string>();
        public List<string> bllFiles = new List<string>();
        public void Save(TreeView tv, string app)
        {
            FilePath fp = new FilePath();
            TreeNode[] td= tv.Nodes.Find(fp.MapPath(app + "/Controllers"),true );
            if (td.Length > 0)
            {
                controllerFiles.Clear();
                AddFiles(td[0], ref controllerFiles);
            }
            td = tv.Nodes.Find(fp.MapPath(app+"/BLL"),true);
            if (td.Length > 0)
            {
                bllFiles.Clear();
                AddFiles(td[0], ref bllFiles);
            }
            XmlSerializer ser = new XmlSerializer(typeof(GenConfig));
            StreamWriter sw=new StreamWriter(fp.MapPath(app+"/compile.xml"),false,System.Text.Encoding.UTF8);
            ser.Serialize(sw, this);
            sw.Close();
        }
        public void Load(TreeView tv, string app)
        {
            FilePath fp = new FilePath();
            if (!File.Exists(fp.MapPath(app + "/compile.xml"))) 
            {
                return;
            }
            
            XmlSerializer ser=new XmlSerializer(typeof(GenConfig));
            StreamReader sr=new StreamReader(fp.MapPath(app+"/compile.xml"),System.Text.Encoding.UTF8);
            GenConfig config=(GenConfig)ser.Deserialize(sr);
            sr.Close();
            if(config.controllerFiles.Count>0)
            {
                TreeNode[] tns;
                for (int i = 0; i < config.controllerFiles.Count; i++)
                {
                    tns = tv.Nodes.Find(fp.MapPath(config.controllerFiles[i]), true);
                    if (tns != null && tns.Length > 0)
                    {
                        tns[0].Checked = true;
                    }
                }
            }
            if (config.bllFiles.Count > 0)
            {
                TreeNode[] tns;
                for (int i = 0; i < config.bllFiles.Count; i++)
                {
                    tns = tv.Nodes.Find(fp.MapPath(config.bllFiles[i]), true);
                    if (tns != null && tns.Length > 0)
                    {
                        tns[0].Checked = true;
                    }
                }
            }
        }
        public string[] GetApps()
        {
            FilePath fp = new FilePath();
            string configFileName = fp.MapPath("/Web.config");
            if (File.Exists(configFileName))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configFileName);
                XmlNode AppsNode = doc.DocumentElement.SelectSingleNode("appSettings/add[@key='Apps']");
                if (AppsNode != null && AppsNode.Attributes.Count > 0 && AppsNode.Attributes["value"] != null)
                {
                    return AppsNode.Attributes["value"].Value.Split(',');
                }
            }
            return null;
        }
        public void AddFiles(TreeNode node,ref List<string>files)
        {
            if (node.Nodes.Count > 0)
            {
                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    if (node.Nodes[i].Checked )
                    {
                        if(node.Nodes[i].FullPath.EndsWith(".cs"))
                        {
                            files.Add(node.Nodes[i].FullPath);
                        }
                    }
                    AddFiles(node.Nodes[i], ref files);
                }
            }
        }
    }
}

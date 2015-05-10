using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace CompileConfig
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            LoadControllers();
            GenConfig config = new GenConfig();
            string[] apps = config.GetApps();
            if (apps != null && apps.Length > 0)
            {
                for (int i = 0; i < apps.Length; i++)
                {
                    config.Load(trvControllers, "App");
                }
            }
        }
        public string[] GetApps(string configFileName)
        {
            if (File.Exists(configFileName))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configFileName);
                XmlNode AppsNode = doc.DocumentElement.SelectSingleNode("appSettings/add[@key='Apps']");
                if (AppsNode != null && AppsNode.Attributes.Count > 0 && AppsNode.Attributes["value"]!=null)
                {
                    return AppsNode.Attributes["value"].Value.Split(',');
                }
            }
            return null;
        }
        public void LoadControllers()
        {
            trvControllers.Nodes.Clear();
            FilePath fp = new FilePath();
            string[] apps=GetApps(fp.MapPath("/Web.config"));
            string rootPath,bllPath;
            TreeNodeCollection tnc = trvControllers.Nodes;
            TreeNode node=null;
            if (apps != null)
            {
                for (int i = 0; i < apps.Length; i++)
                {
                    node = tnc.Add(apps[i],apps[i]);
                    rootPath = fp.MapPath(apps[i]+"/Controllers");
                    TreeNode tr = node.Nodes.Add( rootPath,"Controllers");   
                    AddFileNames(rootPath, tr.Nodes);
                    AddFolders(rootPath, tr.Nodes);

                    bllPath = fp.MapPath(apps[i] + "/BLL");
                    tr = node.Nodes.Add( bllPath,"BLL");
                    AddFileNames(bllPath, tr.Nodes);
                    AddFolders(bllPath, tr.Nodes);
                }
            }
        }
        public void LoadSetting(string settingFileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(settingFileName);
            doc.DocumentElement.SelectNodes("");
        }
        public void AddFileNames(string rootPath, TreeNodeCollection tnc)
        {
            string[] fileNames;
            
            fileNames = Directory.GetFiles(rootPath);
            if (fileNames != null)
            {
                for (int j = 0; j < fileNames.Length; j++)
                {
                    tnc.Add(fileNames[j], Path.GetFileName(fileNames[j]));
                }
            }
        }
        public void AddFolders(string rootPath, TreeNodeCollection tnc)
        {
            string[] folders;
            TreeNode node=null;
            folders = Directory.GetDirectories(rootPath);
            if (folders != null)
            {
                for (int j = 0; j < folders.Length; j++)
                {
                    node = tnc.Add(folders[j], Path.GetFileName(folders[j]));
                    AddFileNames(folders[j], node.Nodes);
                }
            }
        }
        private void trvControllers_AfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            SetChecked(node,node.Checked);
        }
        public void SetChecked(TreeNode node, bool isChecked)
        {
            if (node != null && node.Nodes != null)
            {
                //node.Checked = isChecked;
                if (node.Nodes.Count > 0)
                {
                    for (int i = 0; i < node.Nodes.Count; i++)
                    {
                        node.Nodes[i].Checked = isChecked;
                        //SetChecked(node, isChecked);
                    }
                }
            }
        }

        private void trvControllers_AfterSelect(object sender, TreeViewEventArgs e)
        {
            MessageBox.Show(e.Node.Name);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            GenConfig config = new GenConfig();
            string[] apps = config.GetApps();
            if (apps!=null && apps.Length > 0)
            {
                for (int i = 0; i < apps.Length; i++)
                {
                    config.Save(trvControllers, "App");
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            LoadControllers();
            GenConfig config = new GenConfig();
            string[] apps = config.GetApps();
            if (apps != null && apps.Length > 0)
            {
                for (int i = 0; i < apps.Length; i++)
                {
                    config.Load(trvControllers, "App");
                }
            }
        }

        private void btnAutoGenerateRun_Click(object sender, EventArgs e)
        {
            ExecuteBuilder.WebCompile();
        }
    }
}

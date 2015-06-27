using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace NFinalServer
{
    public partial class frmMain : Form
    {
        string serverPath = string.Empty;
        int port=5001;
        Cassini.Server _server;
        string virtRoot = "/";
        public frmMain(string[] args)
        {
            InitializeComponent();
            Random rand = new Random();
            port = 5000 + rand.Next(3000);
            txtPort.Text = port.ToString();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            if (args != null && args.Length>0 && args[0]!=null)
            {
                serverPath = args[0];
                
                if (Directory.Exists(serverPath))
                {
                    StartServer();
                    Process.Start("http://localhost:" + port.ToString() + "/");
                }
            }
            else
            {
                serverPath = null;
            }
        }
        private void StartServer()
        {
            try
            {
                txtServerPath.Text = serverPath;
                _server = new Cassini.Server(port, virtRoot, serverPath);
                _server.Start();
                btnStart.Enabled = false;
                btnStop.Enabled = true;
            }
            catch
            {
     
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            serverPath = txtServerPath.Text;
            StartServer();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                _server.Stop();
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
            catch
            { }
        }
        private void btnExplorer_Click(object sender, EventArgs e)
        {
            Process.Start("http://localhost:" + port.ToString() + "/");
        }

        private void btnCopyURL_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("http://localhost:" + port.ToString() + "/");
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            Regist reg = new Regist();
            reg.AddDirectory();
        }

        private void btnDeleteFolder_Click(object sender, EventArgs e)
        {
            Regist reg = new Regist();
            reg.DeleteDirectory();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 参数设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void 退出程序ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _server.Stop();
            Application.Exit();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.ShowInTaskbar = false;
                this.notifyIcon1.Icon = this.Icon;
                this.Hide();
            }
        }

        private void btnServerPath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (Directory.Exists(folderBrowserDialog1.SelectedPath))
                {
                    serverPath = folderBrowserDialog1.SelectedPath;
                    txtServerPath.Text = serverPath;
                }
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            _server.Stop();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            this.notifyIcon1.Icon = this.Icon;
            this.Show();
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(serverPath))
            {
                this.ShowInTaskbar = false;
                this.notifyIcon1.Icon = this.Icon;
                this.Hide();
            }
        }
        
    }
}

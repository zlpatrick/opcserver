using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace OPCServerProject
{
    public partial class LogInfo : Form
    {
        public LogInfo()
        {
            InitializeComponent();
            loadLogList();
        }

        private void loadLogList()
        {
            string[] loglist = Directory.GetFiles("LogFiles");
            if (loglist.Length > 0)
            {
                Array.Sort(loglist);
                for (int i = 0; i < loglist.Length; i++)
                {
                    this.treeView1.Nodes["rootNode"].Nodes.Add(loglist[i].Replace("LogFiles\\", ""));
                }
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string fileName = e.Node.Text;
            if (fileName.Equals("所有日志"))
                return;
            this.textBox1.Text = "";
            string[] contents = File.ReadAllLines("LogFiles\\" + fileName);
            foreach (string line in contents)
            {
                this.textBox1.Text += line + "\r\n";
            }
        }
    }


}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OPCServerProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MonitorOPCServer monitorServer = MonitorOPCServer.getInstance();
            string ip = this.textBox1.Text;
            int port = -1;
            try
            {
                port = Convert.ToInt32(this.textBox2.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("端口需要填写一个数字");
                return;
            }
            monitorServer.startMonitor(ip,port);
            this.button1.Enabled = false;
            this.button2.Enabled = true;
        }

        private void oPC标签管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OPCLabel labelDialog = new OPCLabel();
            labelDialog.ShowDialog();
        }

        private void 数据库管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DBManagement dbmanagement = new DBManagement();
            dbmanagement.ShowDialog();
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("OPC数据监控服务器 v1.0");
        }

        private void 日志管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogInfo log = new LogInfo();
            log.ShowDialog();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            MonitorOPCServer.getInstance().shoutdownMonitor();
            this.button1.Enabled = true;
            this.button2.Enabled = false;
        }
    }
}

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

        UInt32[] TagHandle = new UInt32[100];

        private void button1_Click(object sender, EventArgs e)
        {
            MonitorOPCServer monitorServer = MonitorOPCServer.getInstance();
            monitorServer.startMonitor();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string exepath;
            OPClib.WriteNotificationDelegate WriteCallback = new OPClib.WriteNotificationDelegate(MyWriteCallback);
            exepath = Application.StartupPath + "\\OPCServerProject.exe";

            OPClib.UpdateRegistry("{57E9743C-0678-419c-B28B-7508417DAC8C}",
                                        "My OPC Server",
                                        "My OPC Server",
                                        exepath);
            OPClib.SetVendorInfo("OPCServer.Com");
            OPClib.InitWTOPCsvr("{57E9743C-0678-419c-B28B-7508417DAC8C}", 1000);

            OPClib.EnableWriteNotification(WriteCallback, true);
        }

        public void MyWriteCallback(UInt32 hItem, ref Object Value, ref UInt32 ResultCode)
        {
            ResultCode = 0;
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TagHandle[0] = OPClib.CreateTag("Tag.1", "test1", 192, true);
            TagHandle[1] = OPClib.CreateTag("Tag.2", "test2", 192, true);
            TagHandle[2] = OPClib.CreateTag("Tag.3", "test3", 192, true);
            TagHandle[3] = OPClib.CreateTag("Tag.4", "test4", 192, true);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            OPClib.UpdateTag(TagHandle[0], "test11", 192);
            OPClib.UpdateTag(TagHandle[1], "test12", 192);
            OPClib.UpdateTag(TagHandle[2], "test13", 192);
            OPClib.UpdateTag(TagHandle[3], "test14", 192);
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
    }
}

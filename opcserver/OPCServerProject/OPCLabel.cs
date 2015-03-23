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
    public partial class OPCLabel : Form
    {
        List<string> labelContent = new List<string>();
        public OPCLabel()
        {
            InitializeComponent();
            loadLabels();
        }

        private void loadLabels()
        {
            if (!File.Exists("label.properties"))
            {
                File.Create("label.properties");
            }
            string[] labels = File.ReadAllLines("label.properties");
            
            if (labels.Length > 0)
            {
                for (int i = 0; i < labels.Length; i++)
                {
                    if (labels[i].Equals(""))
                        continue;
                    labelContent.Add(labels[i]);
                    string labelLine = labels[i];
                    string[] prop = labelLine.Split('=');
                    string equipment = prop[0];
                    string[] labelText = prop[1].Split(',');

                    TreeNode node = this.treeView1.Nodes.Add(equipment);
                    for (int j = 0; j < labelText.Length; j++)
                        node.Nodes.Add(labelText[j]);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddLabel addLabel = new AddLabel();
            DialogResult result = addLabel.ShowDialog();
            if (result == DialogResult.OK)
            {
                TreeNode node = this.treeView1.Nodes.Add(addLabel.equipmentName);
                if (addLabel.autoCreate)
                {
                    node.Nodes.Add("GPRS-Level");
                    node.Nodes.Add("AI1/AC1");
                    node.Nodes.Add("AI2/AC2");
                    node.Nodes.Add("AI3/AC3");
                    node.Nodes.Add("AI4/AC4");
                    node.Nodes.Add("AI5/AC5");
                    node.Nodes.Add("AI6/AC6");
                    node.Nodes.Add("DI1");
                    node.Nodes.Add("DI2");
                    node.Nodes.Add("DI3");
                    node.Nodes.Add("DI4");
                    node.Nodes.Add("DI5");
                    node.Nodes.Add("DI6");
                    List<string> appendLine = new List<string>();
                    appendLine.Add(addLabel.equipmentName + "=GPRS-Level,AI1/AC1,AI2/AC2,AI3/AC3,AI4/AC4,AI5/AC5,AI6/AC6,DI1,DI2,DI3,DI4,DI5,DI6\n");
                    File.AppendAllLines("label.properties", appendLine);
                }
                else
                {
                    List<string> appendLine = new List<string>();
                    appendLine.Add(addLabel.equipmentName + "=\n");
                    File.AppendAllLines("label.properties", appendLine);
                }
            }
        }

        private void sssToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectLabel select = new SelectLabel();
            DialogResult result = select.ShowDialog();
            TreeNode selectedNode = this.treeView1.SelectedNode;
            if (result == DialogResult.OK)
            {
               
                selectedNode.Nodes.Add(select.labelName);
            }
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
    }
}

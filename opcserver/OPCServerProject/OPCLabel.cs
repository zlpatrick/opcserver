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
        Dictionary<string, string> labelContent = new Dictionary<string, string>();
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
                    
                    string labelLine = labels[i];
                    string[] prop = labelLine.Split('=');
                    string equipment = prop[0];
                    string[] labelText = prop[1].Split(',');
                    labelContent.Add(equipment,labels[i]);
                    TreeNode node = this.treeView1.Nodes.Add(equipment);
                    for (int j = 0; j < labelText.Length; j++)
                    {
                        if (!labelText[j].Equals(""))
                            node.Nodes.Add(labelText[j]);
                    }
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
                bool existed = false;
                foreach (TreeNode sub in selectedNode.Nodes)
                {
                    if (sub.Text.Equals(select.labelName))
                    {
                        existed = true;
                        break;
                    }
                }

                if (!existed)
                {
                    selectedNode.Nodes.Add(select.labelName);
                    string allValue = "";
                    foreach (TreeNode subnode in selectedNode.Nodes)
                    {
                        allValue += subnode.Text + ",";
                    }
                    allValue = allValue.Substring(0, allValue.Length - 1);
                    labelContent[selectedNode.Text] = selectedNode.Text + "=" + allValue;
                    File.WriteAllLines("label.properties", labelContent.Values.ToArray());
                }
                else
                {
                    MessageBox.Show("此设备已经添加了您选择的标签");
                }
            }
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = this.treeView1.SelectedNode;
            if (selectedNode.Parent == null)
            {
                labelContent.Remove(selectedNode.Text);
                selectedNode.Remove();
            }
            else
            {
                TreeNode parent = selectedNode.Parent;
                selectedNode.Remove();
                string allValue = "";
                foreach (TreeNode subnode in parent.Nodes)
                {
                    allValue += subnode.Text+",";
                }
                allValue = allValue.Substring(0, allValue.Length - 1);
                labelContent[parent.Text] = parent.Text + "="+ allValue;
            }
            File.WriteAllLines("label.properties", labelContent.Values.ToArray());
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (this.treeView1.SelectedNode != null && this.treeView1.SelectedNode.Parent != null)
            {
                sssToolStripMenuItem.Enabled = false;
            }
            else
            {
                sssToolStripMenuItem.Enabled = true;
            }
        }
    }
}

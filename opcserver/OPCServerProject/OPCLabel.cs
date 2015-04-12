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
                File.Create("label.properties").Close();
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
                        {
                            string[] labelSubElement = labelText[j].Split('|');
                            string labelName = labelSubElement[0];
                            string labelType = labelSubElement[1];
                            string inputOutput = labelSubElement[2];
                            TreeNode subNode = new TreeNode(labelName + "(" + labelType + ")");
                            subNode.Tag = labelName + "|" + labelType + "|" + inputOutput;
                            node.Nodes.Add(subNode);
                        }
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
                    TreeNode subNode = new TreeNode("GPRS-Level(string)");
                    subNode.Tag = "GPRS-Level|string|input";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("AI1/AC1(int)");
                    subNode.Tag = "AI1/AC1|int|input";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("AI2/AC2(int)");
                    subNode.Tag = "AI2/AC2|int|input";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("AI3/AC3(int)");
                    subNode.Tag = "AI3/AC3|int|input";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("AI4/AC4(int)");
                    subNode.Tag = "AI4/AC4|int|input";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("AI5/AC5(int)");
                    subNode.Tag = "AI5/AC5|int|input";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("AI6/AC6(int)");
                    subNode.Tag = "AI6/AC6|int|input";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("DI1(bool)");
                    subNode.Tag = "DI1|bool|input";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("DI2(bool)");
                    subNode.Tag = "DI2|bool|input";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("DI3(bool)");
                    subNode.Tag = "DI3|bool|input";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("DI4(bool)");
                    subNode.Tag = "DI4|bool|input";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("DI5(bool)");
                    subNode.Tag = "DI5|bool|input";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("DI6(bool)");
                    subNode.Tag = "DI6|bool|input";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("DO1(bool)");
                    subNode.Tag = "DO1|bool|output";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("DO2(bool)");
                    subNode.Tag = "DO2|bool|output";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("DO3(bool)");
                    subNode.Tag = "DO3|bool|output";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("DO4(bool)");
                    subNode.Tag = "DO4|bool|output";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("DO5(bool)");
                    subNode.Tag = "DO5|bool|output";
                    node.Nodes.Add(subNode);

                    subNode = new TreeNode("DO6(bool)");
                    subNode.Tag = "DO6|bool|output";
                    node.Nodes.Add(subNode);

                   
                    List<string> appendLine = new List<string>();
                    string single = addLabel.equipmentName + "=GPRS-Level|string|input,AI1/AC1|int|input,AI2/AC2|int|input,AI3/AC3|int|input,"+
                        "AI4/AC4|int|input,AI5/AC5|int|input,AI6/AC6|int|input,DI1|bool|input,DI2|bool|input,DI3|bool|input,DI4|bool|input,"+
                        "DI5|bool|input,DI6|bool|input,DO1|bool|output,DO2|bool|output,DO3|bool|output,DO4|bool|output,DO5|bool|output,DO6|bool|output\n";
                    appendLine.Add(single);
                    labelContent.Add(addLabel.equipmentName, single);
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
                    TreeNode newNode = new TreeNode(select.labelName + "(" + select.labelType + ")");
                    string addedValue = select.labelName + "|" + select.labelType + "|" + select.inputOutput;
                    newNode.Tag = addedValue;
                    if (selectedNode.Nodes.Count == 0)
                    {
                        if(!labelContent.ContainsKey(selectedNode.Text))
                            labelContent.Add(selectedNode.Text, selectedNode.Text +"="+addedValue);
                        else
                            labelContent[selectedNode.Text] = labelContent[selectedNode.Text] + addedValue;
                    }
                    else
                    {
                        labelContent[selectedNode.Text] = labelContent[selectedNode.Text] + "," + addedValue;
                    }
                    selectedNode.Nodes.Add(newNode);
                    File.WriteAllLines("label.properties", labelContent.Values.ToArray());
                }
                else
                {
                    MessageBox.Show("此设备已经添加了您指定的标签");
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
                    allValue += subnode.Tag+",";
                }
                if(allValue.Length>0)
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

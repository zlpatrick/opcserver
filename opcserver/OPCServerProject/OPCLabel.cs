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
    public partial class OPCLabel : Form
    {
        public OPCLabel()
        {
            InitializeComponent();
            loadLabels();
        }

        private void loadLabels()
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddLabel addLabel = new AddLabel();
            DialogResult result = addLabel.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.treeView1.Nodes.Add(addLabel.equipmentName);
            }
        }
    }
}

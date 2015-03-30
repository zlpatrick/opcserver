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
    public partial class SelectLabel : Form
    {

        public string labelName
        {
            get
            {
                return this.comboBox1.SelectedItem.ToString();
            }
            set
            {
                this.comboBox1.SelectedText = value;
            }
        }
 
        public SelectLabel()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

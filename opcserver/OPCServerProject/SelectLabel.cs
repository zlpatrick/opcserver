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
                return this.comboBox1.Text;
            }
            set
            {
                this.comboBox1.Text = value;
            }
        }

        public string labelType
        {
            get
            {
                return this.comboBox2.SelectedItem.ToString() ;
            }
            set
            {
                this.comboBox2.SelectedItem = value;
            }
        }

        public string inputOutput
        {
            get
            {
                return this.comboBox3.SelectedItem.ToString();
            }
            set
            {
                this.comboBox3.SelectedItem = value;
            }
        }
 
        public SelectLabel()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text == null || this.comboBox2.Text == null || this.comboBox3.Text == null)
            {
                MessageBox.Show("选择不完整");
                return;
            }
            if (this.comboBox1.Text.Equals("") || this.comboBox2.Text.Equals("") || this.comboBox3.Text.Equals(""))
            {
                MessageBox.Show("选择不完整");
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

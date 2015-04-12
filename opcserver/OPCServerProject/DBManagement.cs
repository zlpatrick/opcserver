using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Sql;
using System.IO;

namespace OPCServerProject
{
    public partial class DBManagement : Form
    {
        public DBManagement()
        {
            InitializeComponent();
            loadProperties();
        }

        private void loadProperties()
        {
            if (!File.Exists("alert.properties"))
            {
                File.Create("alert.properties").Close();
            }
            else
            {
                string alert = File.ReadAllText("alert.properties");
                this.textBox5.Text = alert;
            }

            if (!File.Exists("dbmapping.properties"))
            {
                File.Create("dbmapping.properties").Close();
            }
            else
            {
                string[] mappings = File.ReadAllLines("dbmapping.properties");
                for (int i = 0; i < mappings.Length; i++)
                {
                    string[] values = mappings[i].Split(',');
                    this.dataGridView1.Rows.Add(new string[] { values[0], values[1] });
                }
            }

            if (!File.Exists("dbconn.properties"))
            {
                File.Create("dbconn.properties").Close();
            }
            else
            {
                string[] values = File.ReadAllLines("dbconn.properties");
                if (values.Length > 0)
                {
                    this.textBox1.Text = values[0];
                    this.textBox2.Text = values[1];
                    this.textBox3.Text = values[2];
                    this.textBox4.Text = values[3];
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddDBMapping dialog = new AddDBMapping();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.dataGridView1.Rows.Add(new string[] { dialog.equipmentName, dialog.dbTableName });
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            File.WriteAllText("alert.properties", this.textBox5.Text);

            List<string> allMapping = new List<string>();
            for(int i = 0;i<this.dataGridView1.Rows.Count;i++)
            {
                allMapping.Add(this.dataGridView1.Rows[i].Cells[0].Value.ToString()
                    + "," + this.dataGridView1.Rows[i].Cells[1].Value.ToString());
            }
            File.WriteAllLines("dbmapping.properties", allMapping.ToArray());

            string[] dbconfig = new string[4];
            dbconfig[0] = this.textBox1.Text;
            dbconfig[1] = this.textBox2.Text;
            dbconfig[2] = this.textBox3.Text;
            dbconfig[3] = this.textBox4.Text;
            File.WriteAllLines("dbconn.properties", dbconfig);
            MessageBox.Show("信息保存成功");
        }
    }
}

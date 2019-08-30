using RLU.Classifier.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RLU.ClassifierFrontend
{
    public partial class Form3 : Form
    {
        private IClassifierProvider classifierProvider = new ClassifierProvider(new DatabaseLayer());
        private LibraryTag[] tags;
        private LibraryTag selected;
        public Form3()
        {
            tags = classifierProvider.GetTags("");
            InitializeComponent();
            listBox1.Items.Clear();
            listBox1.Items.AddRange(tags);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            tags = classifierProvider.GetTags(textBox1.Text.Trim());
            listBox1.Items.Clear();
            listBox1.Items.AddRange(tags);
        }

        private void label4_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            label4.Visible = false;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                label4.Visible = true;
            }
        }

        private void Form3_Click(object sender, EventArgs e)
        {
            label1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            selected = (LibraryTag)listBox1.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show("pick a tag");
            }
            else
            {
                button1.Visible = false;
                textBox1.Visible = false;
                label4.Visible = false;
                listBox1.Visible = false;

                button2.Visible = true;
                button3.Visible = true;
                textBox2.Visible = true;
                label2.Visible = true;


                label2.Text = selected.name;
                foreach (string s in classifierProvider.GetDisplayNamesForTag(selected.id))
                {
                    textBox2.Text += (s + "\r\n");
                }
                // MessageBox.Show(selected.ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // submit

            // add all new display names
            classifierProvider.AddDisplayNamesToTag(selected.id, textBox2.Text.Trim());
            button2_Click(sender, e);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // back
            button1.Visible = true;
            textBox1.Visible = true;
            label4.Visible = true;
            listBox1.Visible = true;

            button2.Visible = false;
            button3.Visible = false;
            textBox2.Visible = false;
            label2.Visible = false;
            tags = classifierProvider.GetTags("");
            selected = null;
        }
    }
}

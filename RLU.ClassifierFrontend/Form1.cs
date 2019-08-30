using RLU.Classifier.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RLU.ClassifierFrontend
{
    public partial class Form1 : Form
    {
        private string path = "";
        private IClassifierProvider classifierProvider = new ClassifierProvider(new DatabaseLayer());
        public Form1()
        {
            InitializeComponent();
        }

        private void panel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetFormats().Any(f => f == DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            path = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
            if (classifierProvider.CheckPath(path))
            {
                label2.Visible = false;
                label5.Visible = true;
                panel1.BorderStyle = BorderStyle.FixedSingle;
                panel1.AllowDrop = false;

                label3.Text = "Path: " + path;
                label3.Visible = true;
                Change.Visible = true;
            }
            else
            {
                throw new Exception("Invalid path");
            }

        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            // autocomplete while typing tags
            AutoCompleteStringCollection data = new AutoCompleteStringCollection();
            data.AddRange(classifierProvider.GetTags("").AsParallel().Select(x => x.name).ToArray());
            textBox2.AutoCompleteCustomSource = data;
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            string tag = textBox2.Text.Trim();
            if (path == "")
            {
                MessageBox.Show("Please select a file/folder");
            }
            else if (tag == "")
            {
                MessageBox.Show("Please enter a valid tag");
            }
            else
            {
                try
                {
                    classifierProvider.TagFiles(path, tag);
                    textBox2.Text = "";
                    label4.Visible = true;
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        

        private void Change_Click(object sender, EventArgs e)
        {
            path = "";
            textBox2.Text = "";
            label4.Visible = true;
            label5.Visible = false;
            label2.Visible = true;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.AllowDrop = true;
            Change.Visible = false;
            label3.Visible = false;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            label4.Visible = false;
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                label4.Visible = true;
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            textBox2.Focus();
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            label1.Focus();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.Enter == e.KeyData)
            {
                button3_Click(sender, e);
            }
        }
    }
}

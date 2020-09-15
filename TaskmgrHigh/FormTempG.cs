using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskmgrHigh
{
    public partial class FormTempG : Form
    {
        public FormTempG()
        {
            InitializeComponent();
        }

        GPUTemp gt = new GPUTemp();

        private void FormTempG_Shown(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            this.TopMost = true;
            adjust();
        }

        private void FormTempG_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            adjust();
        }

        void adjust()
        {
            var data = gt.Update();
            if(data.Count > 0)
            {
                this.Size = new Size(300, 70 + data.Count * 25);
                this.textBox1.Text = "";
                for(int i = 0;i < data.Count; i++)
                {
                    this.textBox1.Text += data[i].type + " → " + data[i].temp + "\r\n";
                }
                this.textBox1.Text = this.textBox1.Text.Remove(this.textBox1.Text.LastIndexOf('\r'), 2);
            }
            else
            {
                MessageBox.Show("No GPU Detected");
                this.Close();
            }
        }
    }
}

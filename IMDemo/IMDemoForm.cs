using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace IMDemo
{
    public partial class IMDemoForm : Form
    {
        public IMClient client;
        

        public IMDemoForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client = new IMClient(this.IPBox.Text, int.Parse(this.PortBox.Text));
            if (client.Connect())
            {
                new Thread(client.Start).Start();
                MessageBox.Show("连接成功");
            }
            else
            {
                MessageBox.Show("连接失败");
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (client != null)
                client.disconnect();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (client != null)
                client.SendMessage(this.SendMsgBox.Text);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (client != null && client.messages.Count > 0)
                listView1.Items.Add((string)client.messages.Dequeue());
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TestThirdGame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("ThirdGame.dll")]
        public static extern IntPtr OpenLongzhu();

        [DllImport("ThirdGame.dll")]
        public static extern IntPtr GetMemberInfo();
        
        private void button1_Click(object sender, EventArgs e)
        {
            IntPtr ret = GetMemberInfo();
            MessageBox.Show(ret.ToString(), "会员卡信息获取调用返回");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            IntPtr ret = OpenLongzhu();
            MessageBox.Show(ret.ToString(), "打开龙管家窗体调用返回");
        }


        public static string getSHA1HashFromFile(string fileName)
        {
            FileStream file = null;
            try
            {
                file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
               
            }
            finally
            {
                if (file != null)
                    file.Close();
            }

            return null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFIle = new OpenFileDialog();
            if (openFIle.ShowDialog() == DialogResult.OK)
            {
                System.IO.FileInfo file = new System.IO.FileInfo(openFIle.FileName);
                textBox2.Text = file.Length.ToString();
                textBox1.Text = getSHA1HashFromFile(openFIle.FileName);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            client = new SocketHelper.TcpClients();
            client.InitSocket(textBox3.Text, int.Parse(textBox4.Text));
            client.Start();
        }

        SocketHelper.TcpClients client;
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

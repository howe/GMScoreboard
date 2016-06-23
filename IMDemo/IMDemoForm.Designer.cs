namespace IMDemo
{
    partial class IMDemoForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.IPBox = new System.Windows.Forms.TextBox();
            this.PortBox = new System.Windows.Forms.TextBox();
            this.PortLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SendMsgBox = new System.Windows.Forms.TextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP";
            // 
            // IPBox
            // 
            this.IPBox.Location = new System.Drawing.Point(36, 13);
            this.IPBox.Name = "IPBox";
            this.IPBox.Size = new System.Drawing.Size(97, 21);
            this.IPBox.TabIndex = 1;
            this.IPBox.Text = "119.29.56.137";
            // 
            // PortBox
            // 
            this.PortBox.Location = new System.Drawing.Point(174, 13);
            this.PortBox.Name = "PortBox";
            this.PortBox.Size = new System.Drawing.Size(38, 21);
            this.PortBox.TabIndex = 3;
            this.PortBox.Text = "9999";
            // 
            // PortLabel
            // 
            this.PortLabel.AutoSize = true;
            this.PortLabel.Location = new System.Drawing.Point(139, 17);
            this.PortLabel.Name = "PortLabel";
            this.PortLabel.Size = new System.Drawing.Size(29, 12);
            this.PortLabel.TabIndex = 2;
            this.PortLabel.Text = "Port";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(218, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(334, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "连接";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(558, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(162, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "断开";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(558, 50);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(162, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "发送";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // SendMsgBox
            // 
            this.SendMsgBox.Location = new System.Drawing.Point(15, 50);
            this.SendMsgBox.Name = "SendMsgBox";
            this.SendMsgBox.Size = new System.Drawing.Size(537, 21);
            this.SendMsgBox.TabIndex = 7;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(15, 78);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(705, 241);
            this.listView1.TabIndex = 8;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // IMDemoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 338);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.SendMsgBox);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.PortBox);
            this.Controls.Add(this.PortLabel);
            this.Controls.Add(this.IPBox);
            this.Controls.Add(this.label1);
            this.Name = "IMDemoForm";
            this.Text = "IMDemo测试客户端V0.1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox IPBox;
        private System.Windows.Forms.TextBox PortBox;
        private System.Windows.Forms.Label PortLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox SendMsgBox;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Timer timer1;
    }
}


namespace GMScoreboard
{
    partial class ClientTipForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.autoCloseTimer = new System.Windows.Forms.Timer(this.components);
            this.header = new System.Windows.Forms.Panel();
            this.icon = new System.Windows.Forms.PictureBox();
            this.headText = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.PictureBox();
            this.title = new System.Windows.Forms.Label();
            this.linkText = new System.Windows.Forms.Label();
            this.content = new System.Windows.Forms.LinkLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.header.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // autoCloseTimer
            // 
            this.autoCloseTimer.Interval = 12000;
            this.autoCloseTimer.Tick += new System.EventHandler(this.autoCloseTimer_Tick);
            // 
            // header
            // 
            this.header.BackColor = System.Drawing.Color.BurlyWood;
            this.header.Controls.Add(this.icon);
            this.header.Controls.Add(this.headText);
            this.header.Controls.Add(this.closeButton);
            this.header.Location = new System.Drawing.Point(0, 0);
            this.header.Name = "header";
            this.header.Size = new System.Drawing.Size(300, 30);
            this.header.TabIndex = 0;
            // 
            // icon
            // 
            this.icon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.icon.Image = global::GMScoreboard.Properties.Resources.jule_18x18;
            this.icon.Location = new System.Drawing.Point(11, 5);
            this.icon.Name = "icon";
            this.icon.Size = new System.Drawing.Size(18, 18);
            this.icon.TabIndex = 2;
            this.icon.TabStop = false;
            this.icon.MouseEnter += new System.EventHandler(this.mouseEnter);
            this.icon.MouseLeave += new System.EventHandler(this.mouseLeave);
            // 
            // headText
            // 
            this.headText.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.headText.AutoSize = true;
            this.headText.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.headText.Location = new System.Drawing.Point(30, 6);
            this.headText.Name = "headText";
            this.headText.Size = new System.Drawing.Size(56, 17);
            this.headText.TabIndex = 1;
            this.headText.Text = "头部信息";
            this.headText.MouseEnter += new System.EventHandler(this.mouseEnter);
            this.headText.MouseLeave += new System.EventHandler(this.mouseLeave);
            // 
            // closeButton
            // 
            this.closeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.closeButton.ErrorImage = global::GMScoreboard.Properties.Resources.close;
            this.closeButton.Image = global::GMScoreboard.Properties.Resources.close;
            this.closeButton.InitialImage = global::GMScoreboard.Properties.Resources.close;
            this.closeButton.Location = new System.Drawing.Point(270, 0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(30, 30);
            this.closeButton.TabIndex = 0;
            this.closeButton.TabStop = false;
            this.closeButton.Click += new System.EventHandler(this.closeButton_click);
            this.closeButton.MouseEnter += new System.EventHandler(this.mouseEnter);
            this.closeButton.MouseLeave += new System.EventHandler(this.mouseLeave);
            // 
            // title
            // 
            this.title.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.title.Cursor = System.Windows.Forms.Cursors.Hand;
            this.title.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.title.Location = new System.Drawing.Point(12, 37);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(276, 24);
            this.title.TabIndex = 1;
            this.title.Text = "消息标题";
            this.title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.title.Click += new System.EventHandler(this.gotoDetail);
            this.title.MouseEnter += new System.EventHandler(this.mouseEnter);
            this.title.MouseLeave += new System.EventHandler(this.mouseLeave);
            // 
            // linkText
            // 
            this.linkText.AutoSize = true;
            this.linkText.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkText.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.linkText.ForeColor = System.Drawing.SystemColors.Highlight;
            this.linkText.Location = new System.Drawing.Point(231, 179);
            this.linkText.Name = "linkText";
            this.linkText.Size = new System.Drawing.Size(57, 12);
            this.linkText.TabIndex = 3;
            this.linkText.Text = "查看详情";
            this.linkText.Click += new System.EventHandler(this.gotoDetail);
            this.linkText.MouseEnter += new System.EventHandler(this.mouseEnter);
            this.linkText.MouseLeave += new System.EventHandler(this.mouseLeave);
            // 
            // content
            // 
            this.content.ActiveLinkColor = System.Drawing.Color.Moccasin;
            this.content.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.content.Cursor = System.Windows.Forms.Cursors.Hand;
            this.content.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.content.ForeColor = System.Drawing.Color.Gray;
            this.content.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.content.LinkColor = System.Drawing.Color.DimGray;
            this.content.Location = new System.Drawing.Point(14, 72);
            this.content.Name = "content";
            this.content.Size = new System.Drawing.Size(274, 91);
            this.content.TabIndex = 4;
            this.content.TabStop = true;
            this.content.Text = "消息内容";
            this.content.UseCompatibleTextRendering = true;
            this.content.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.gotoDetail);
            this.content.Click += new System.EventHandler(this.gotoDetail);
            this.content.MouseEnter += new System.EventHandler(this.mouseEnter);
            this.content.MouseLeave += new System.EventHandler(this.mouseLeave);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(30, 30);
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            // 
            // ClientTipForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(300, 200);
            this.Controls.Add(this.content);
            this.Controls.Add(this.linkText);
            this.Controls.Add(this.title);
            this.Controls.Add(this.header);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ClientTipForm";
            this.ShowInTaskbar = false;
            this.Text = "ClientTipForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ClientTipForm_Load);
            this.MouseEnter += new System.EventHandler(this.mouseEnter);
            this.MouseLeave += new System.EventHandler(this.mouseLeave);
            this.header.ResumeLayout(false);
            this.header.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer autoCloseTimer;
        private System.Windows.Forms.Panel header;
        private System.Windows.Forms.PictureBox closeButton;
        private System.Windows.Forms.Label headText;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.Label linkText;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.LinkLabel content;
        private System.Windows.Forms.PictureBox icon;
    }
}
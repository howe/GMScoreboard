namespace GMScoreboard
{
    partial class ScoreboardForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScoreboardForm));
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.clientTipTimer = new System.Windows.Forms.Timer(this.components);
            this.lolMonitorTimer = new System.Windows.Forms.Timer(this.components);
            this.lolGameMonitorTimer = new System.Windows.Forms.Timer(this.components);
            this.transparentPanel1 = new GMScoreboard.UI.TransparentPanel();
            this.homeTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.Margin = new System.Windows.Forms.Padding(0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(345, 607);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.ScrollBarsEnabled = false;
            this.webBrowser1.Size = new System.Drawing.Size(345, 718);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "网吧英雄联盟竞技大师";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // clientTipTimer
            // 
            this.clientTipTimer.Enabled = true;
            this.clientTipTimer.Interval = 1000;
            this.clientTipTimer.Tick += new System.EventHandler(this.clientTipTimer_Tick);
            // 
            // lolMonitorTimer
            // 
            this.lolMonitorTimer.Enabled = true;
            this.lolMonitorTimer.Interval = 5000;
            this.lolMonitorTimer.Tick += new System.EventHandler(this.lolMonitorTimer_Tick);
            // 
            // lolGameMonitorTimer
            // 
            this.lolGameMonitorTimer.Enabled = true;
            this.lolGameMonitorTimer.Interval = 5000;
            this.lolGameMonitorTimer.Tick += new System.EventHandler(this.lolGameMonitorTimer_Tick);
            // 
            // transparentPanel1
            // 
            this.transparentPanel1.BackColor = System.Drawing.Color.Transparent;
            this.transparentPanel1.Location = new System.Drawing.Point(50, 0);
            this.transparentPanel1.Name = "transparentPanel1";
            this.transparentPanel1.Size = new System.Drawing.Size(245, 50);
            this.transparentPanel1.TabIndex = 1;
            this.transparentPanel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.transparentPanel1_MouseDown);
            this.transparentPanel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.transparentPanel1_MouseMove);
            this.transparentPanel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.transparentPanel1_MouseUp);
            // 
            // homeTimer
            // 
            this.homeTimer.Tick += new System.EventHandler(this.homeTimer_Tick);
            // 
            // ScoreboardForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 718);
            this.Controls.Add(this.transparentPanel1);
            this.Controls.Add(this.webBrowser1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "ScoreboardForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScoreboardForm_FormClosing);
            this.Load += new System.EventHandler(this.ScoreboardForm_Load);
            this.Shown += new System.EventHandler(this.ScoreboardForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Timer clientTipTimer;
        private System.Windows.Forms.Timer lolMonitorTimer;
        private System.Windows.Forms.Timer lolGameMonitorTimer;
        private UI.TransparentPanel transparentPanel1;
        private System.Windows.Forms.Timer homeTimer;
    }
}

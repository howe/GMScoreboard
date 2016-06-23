namespace GMScoreTestCliect
{
    partial class FrmBrowser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmBrowser));
            this.web_Browser = new System.Windows.Forms.WebBrowser();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // web_Browser
            // 
            this.web_Browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.web_Browser.Location = new System.Drawing.Point(0, 0);
            this.web_Browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.web_Browser.Name = "web_Browser";
            this.web_Browser.ScrollBarsEnabled = false;
            this.web_Browser.Size = new System.Drawing.Size(315, 718);
            this.web_Browser.TabIndex = 0;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "网吧英雄联盟竞技大师";
            this.notifyIcon1.Visible = true;
            // 
            // FrmBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 718);
            this.Controls.Add(this.web_Browser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmBrowser";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FrmBrowser";
            this.Load += new System.EventHandler(this.FrmBrowser_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser web_Browser;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
    }
}
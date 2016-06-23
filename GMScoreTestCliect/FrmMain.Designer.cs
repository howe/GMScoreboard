namespace GMScoreTestCliect
{
    partial class FrmMain
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
            this.txt_height = new System.Windows.Forms.TextBox();
            this.txt_width = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_SetWeb = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.btnBrowser = new System.Windows.Forms.Button();
            this.grp_setting = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chk_ShowError = new System.Windows.Forms.CheckBox();
            this.chk_ShowWebConect = new System.Windows.Forms.CheckBox();
            this.chk_ScrollBar = new System.Windows.Forms.CheckBox();
            this.txt_funName = new System.Windows.Forms.TextBox();
            this.btn_runScript = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_args = new System.Windows.Forms.TextBox();
            this.grp_setting.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_height
            // 
            this.txt_height.Location = new System.Drawing.Point(177, 17);
            this.txt_height.Name = "txt_height";
            this.txt_height.Size = new System.Drawing.Size(100, 21);
            this.txt_height.TabIndex = 2;
            this.txt_height.Text = "718";
            // 
            // txt_width
            // 
            this.txt_width.Location = new System.Drawing.Point(36, 17);
            this.txt_width.Name = "txt_width";
            this.txt_width.Size = new System.Drawing.Size(100, 21);
            this.txt_width.TabIndex = 2;
            this.txt_width.Text = "345";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(142, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "高度";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "长度";
            // 
            // btn_SetWeb
            // 
            this.btn_SetWeb.Location = new System.Drawing.Point(283, 15);
            this.btn_SetWeb.Name = "btn_SetWeb";
            this.btn_SetWeb.Size = new System.Drawing.Size(75, 23);
            this.btn_SetWeb.TabIndex = 0;
            this.btn_SetWeb.Text = "设置";
            this.btn_SetWeb.UseVisualStyleBackColor = true;
            this.btn_SetWeb.Click += new System.EventHandler(this.btn_SetWeb_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "路径";
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(36, 41);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(241, 21);
            this.txtUrl.TabIndex = 4;
            this.txtUrl.Validated += new System.EventHandler(this.txtUrl_Validated);
            // 
            // btnBrowser
            // 
            this.btnBrowser.Location = new System.Drawing.Point(283, 39);
            this.btnBrowser.Name = "btnBrowser";
            this.btnBrowser.Size = new System.Drawing.Size(75, 23);
            this.btnBrowser.TabIndex = 0;
            this.btnBrowser.Text = "...";
            this.btnBrowser.UseVisualStyleBackColor = true;
            this.btnBrowser.Click += new System.EventHandler(this.btnBrowser_Click);
            // 
            // grp_setting
            // 
            this.grp_setting.Controls.Add(this.chk_ScrollBar);
            this.grp_setting.Controls.Add(this.txt_args);
            this.grp_setting.Controls.Add(this.txt_funName);
            this.grp_setting.Controls.Add(this.chk_ShowWebConect);
            this.grp_setting.Controls.Add(this.chk_ShowError);
            this.grp_setting.Controls.Add(this.label5);
            this.grp_setting.Controls.Add(this.groupBox1);
            this.grp_setting.Controls.Add(this.label4);
            this.grp_setting.Controls.Add(this.btn_runScript);
            this.grp_setting.Dock = System.Windows.Forms.DockStyle.Left;
            this.grp_setting.Location = new System.Drawing.Point(0, 0);
            this.grp_setting.Name = "grp_setting";
            this.grp_setting.Size = new System.Drawing.Size(378, 312);
            this.grp_setting.TabIndex = 1;
            this.grp_setting.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtUrl);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txt_height);
            this.groupBox1.Controls.Add(this.txt_width);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnBrowser);
            this.groupBox1.Controls.Add(this.btn_SetWeb);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(3, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(372, 75);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // chk_ShowError
            // 
            this.chk_ShowError.AutoSize = true;
            this.chk_ShowError.Location = new System.Drawing.Point(11, 108);
            this.chk_ShowError.Name = "chk_ShowError";
            this.chk_ShowError.Size = new System.Drawing.Size(72, 16);
            this.chk_ShowError.TabIndex = 6;
            this.chk_ShowError.Text = "脚本错误";
            this.chk_ShowError.UseVisualStyleBackColor = true;
            this.chk_ShowError.CheckedChanged += new System.EventHandler(this.chk_ShowError_CheckedChanged);
            // 
            // chk_ShowWebConect
            // 
            this.chk_ShowWebConect.AutoSize = true;
            this.chk_ShowWebConect.Location = new System.Drawing.Point(11, 130);
            this.chk_ShowWebConect.Name = "chk_ShowWebConect";
            this.chk_ShowWebConect.Size = new System.Drawing.Size(72, 16);
            this.chk_ShowWebConect.TabIndex = 6;
            this.chk_ShowWebConect.Text = "右键菜单";
            this.chk_ShowWebConect.UseVisualStyleBackColor = true;
            this.chk_ShowWebConect.CheckedChanged += new System.EventHandler(this.chk_ShowError_CheckedChanged);
            // 
            // chk_ScrollBar
            // 
            this.chk_ScrollBar.AutoSize = true;
            this.chk_ScrollBar.Location = new System.Drawing.Point(11, 152);
            this.chk_ScrollBar.Name = "chk_ScrollBar";
            this.chk_ScrollBar.Size = new System.Drawing.Size(60, 16);
            this.chk_ScrollBar.TabIndex = 6;
            this.chk_ScrollBar.Text = "滚动条";
            this.chk_ScrollBar.UseVisualStyleBackColor = true;
            this.chk_ScrollBar.CheckedChanged += new System.EventHandler(this.chk_ShowError_CheckedChanged);
            // 
            // txt_funName
            // 
            this.txt_funName.Location = new System.Drawing.Point(73, 172);
            this.txt_funName.Name = "txt_funName";
            this.txt_funName.Size = new System.Drawing.Size(207, 21);
            this.txt_funName.TabIndex = 4;
            this.txt_funName.Validated += new System.EventHandler(this.txtUrl_Validated);
            // 
            // btn_runScript
            // 
            this.btn_runScript.Location = new System.Drawing.Point(73, 274);
            this.btn_runScript.Name = "btn_runScript";
            this.btn_runScript.Size = new System.Drawing.Size(75, 24);
            this.btn_runScript.TabIndex = 0;
            this.btn_runScript.Text = "执行脚本";
            this.btn_runScript.UseVisualStyleBackColor = true;
            this.btn_runScript.Click += new System.EventHandler(this.btn_runScript_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 175);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "脚本名称";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 202);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "脚本参数";
            // 
            // txt_args
            // 
            this.txt_args.Location = new System.Drawing.Point(73, 199);
            this.txt_args.Multiline = true;
            this.txt_args.Name = "txt_args";
            this.txt_args.Size = new System.Drawing.Size(207, 69);
            this.txt_args.TabIndex = 4;
            this.txt_args.Validated += new System.EventHandler(this.txtUrl_Validated);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 312);
            this.Controls.Add(this.grp_setting);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GMSorceBoard测试";
            this.grp_setting.ResumeLayout(false);
            this.grp_setting.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox txt_height;
        private System.Windows.Forms.TextBox txt_width;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_SetWeb;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnBrowser;
        private System.Windows.Forms.GroupBox grp_setting;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chk_ScrollBar;
        private System.Windows.Forms.CheckBox chk_ShowWebConect;
        private System.Windows.Forms.CheckBox chk_ShowError;
        private System.Windows.Forms.TextBox txt_funName;
        private System.Windows.Forms.Button btn_runScript;
        private System.Windows.Forms.TextBox txt_args;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
    }
}


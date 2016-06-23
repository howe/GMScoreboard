using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GMScoreTestCliect
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();

            chk_ShowError_CheckedChanged(null, null);
        }

        /// <summary>
        /// 获取窗体句柄
        /// </summary>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        /// <summary>
        /// 设置目标窗体大小，位置
        /// </summary>
        /// <param name="hWnd">目标句柄</param>
        /// <param name="x">目标窗体新位置X轴坐标</param>
        /// <param name="y">目标窗体新位置Y轴坐标</param>
        /// <param name="nWidth">目标窗体新宽度</param>
        /// <param name="nHeight">目标窗体新高度</param>
        /// <param name="BRePaint">是否刷新窗体</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool BRePaint);

        FrmBrowser frmBrowser = new FrmBrowser();

        private void btn_SetWeb_Click(object sender, EventArgs e)
        {
            try
            {
                int height = 0, width = 0;

                if (!int.TryParse(txt_height.Text, out height))
                {
                    MessageBox.Show("请输入正确高度！");
                    return;
                }

                if (!int.TryParse(txt_width.Text, out width))
                {
                    MessageBox.Show("请输入正确宽度！");
                    return;
                }

                frmBrowser.Size = new Size(width, height);
                frmBrowser.TopMost = true;
                frmBrowser.Show();
              
                IntPtr intptr = FindWindow(null, frmBrowser.Text);

                if (intptr.ToInt32() > 0)
                {
                    MoveWindow(intptr, Screen.PrimaryScreen.WorkingArea.Width - frmBrowser.Width - 4, 3, frmBrowser.Width, frmBrowser.Height, true);
                }

                frmBrowser.TopMost = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Title = "打开网页内容";
                openFile.Filter = "网页文件|*.html;*.HTML|All Files (*.*)|*.*";
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    txtUrl.Text = openFile.FileName;
                    txtUrl_Validated(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //
        }

        private void txtUrl_Validated(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUrl.Text))
            {
                return;
            }
            if (frmBrowser.webBrowser.Url == null ||
                frmBrowser.webBrowser.Url.LocalPath != txtUrl.Text)
            {
                frmBrowser.webBrowser.Url = new Uri(txtUrl.Text);
                chk_ShowError_CheckedChanged(null, null);
            }
        }

        private void chk_ShowError_CheckedChanged(object sender, EventArgs e)
        {
            frmBrowser.webBrowser.ScriptErrorsSuppressed = !chk_ShowError.Checked;
            frmBrowser.webBrowser.ScrollBarsEnabled = chk_ScrollBar.Checked;
            frmBrowser.webBrowser.IsWebBrowserContextMenuEnabled = chk_ShowWebConect.Checked;
        }

        private void btn_runScript_Click(object sender, EventArgs e)
        {
            try
            {
                if (frmBrowser.webBrowser.Url==null)
                {
                    return;
                }

                object[] value = txt_args.Text.Split(new char[] { ',', '，', ';', '$' }, StringSplitOptions.RemoveEmptyEntries);

                frmBrowser.webBrowser.Document.InvokeScript(txt_funName.Text, value);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

using GMScoreboard.Util;
using GMScoreboard.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using GMScoreboard.Monitor;
using System.Collections;
using GMScoreboard.Properties;

namespace GMScoreboard
{
    public partial class ScoreboardForm : Form
    {
        public static string CLIENT_TIP_URL = "http://gmaster.youzijie.com/scoreboard/clienttip/getTip";
        public static string REPORT_ERROR_URL = "http://gmaster.youzijie.com/report/error";

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow([MarshalAs(UnmanagedType.LPTStr)] string lpClassName, [MarshalAs(UnmanagedType.LPTStr)] string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string lpsz1, string lpsz2);

        [DllImport("ThirdGame.dll")]
        public static extern IntPtr OpenLongzhu();

        private ScoreBoardV3Config config;
        private int lolClientId = 0;

        public delegate void OpenIMPrivateForm(IMPrivateForm form);

        // 鼠标位置和左键标记
        private Point mouseOff;
        private bool leftFlag;
        private bool isUnFix;

        public ScoreboardForm(ScoreBoardV3Config config)
        {
            InitializeComponent();

            // 设置窗体和浏览器属性
            this.config = config;
            this.Location = new Point(config.locationX, config.locationY);

            IMConversation.initialMainForm(this);
        }

        public void invokeScript(string func, object[] args)
        {
            this.webBrowser1.Document.InvokeScript(func, args);
        }

        private void doOpenIMPrivateForm(IMPrivateForm form)
        {
            form.Show();
            form.WindowState = System.Windows.Forms.FormWindowState.Normal;
            form.Activate();
        }

        private void ScoreboardForm_Load(object sender, EventArgs e)
        {
            // 固定在桌面
            if (Process.GetProcessesByName("BarChargeClient").Length == 0)
            {
                LogUtil.log("Fix on desktop.");
                SetParent(this.Handle, FindWindowEx(FindWindow("Progman", null),
                    IntPtr.Zero, "shelldll_defview", null));
            }
        }

        private void clientTipTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (FullScreenUtil.isFullScreen())
                    return;

                ClientTip tip = HeartBeatTask.getClientTip();
                if (tip != null && tip.errCode == 0 && (config.maxTip--) > 0)
                    new ClientTipForm(tip).Show();

            }
            catch (Exception ex)
            {
                LogUtil.log("Error occurs during loading client tip.", ex);
            }
        }

        private void ScoreboardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
            this.notifyIcon1.ShowBalloonTip(30, null, "双击可以显示本网吧英雄榜", ToolTipIcon.Info);
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        /// <summary>
        /// 5秒一次轮询判断LolClient是否已经重启过 如果重启则重新刷新页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lolMonitorTimer_Tick(object sender, EventArgs e)
        {
            // 条件进程存在才启动窗体
            Process[] pros = Process.GetProcessesByName(config.processKeyword);
            if (pros == null || pros.Length < 1)
                return;

            if (!this.isUnFix && Process.GetProcessesByName("BarChargeClient").Length != 0)  //取消固定在桌面顶部
            {
                LogUtil.log("unFix on desktop.");
                SetParent(this.Handle, IntPtr.Zero);
                this.isUnFix = true;
            }

            // 如果进程 ID 换了则重新刷新
            if (pros[0].Id != lolClientId)
            {
                // 如果当前进程的玩家信息未抓取则不做动作
                LolPlayer lolPlayer = GMDataTask.getPlayerInfoById(pros[0].Id);
                if (lolPlayer == null || string.IsNullOrEmpty(lolPlayer.playerName))
                    return;

                // 初次启动
                if (lolClientId == 0)
                {
                    LogUtil.log("Open main window.");

                    ScriptEvent se = new ScriptEvent(this);
                    se.openIMPrivateForm += new OpenIMPrivateForm(doOpenIMPrivateForm);

                    this.webBrowser1.ObjectForScripting = se;
                    this.webBrowser1.Navigate(config.indexUrl + "&lolClientId=" + pros[0].Id, null, null, "shopId:" + config.shopId);
                    this.Show();
                    this.notifyIcon1.Visible = true;

                    LogUtil.log("Disable lolMonitor timer.");
                }
                else
                {
                    this.webBrowser1.Document.InvokeScript("logout");
                    this.webBrowser1.Refresh();
                    IMConversation.initialMainForm(this);
                }

                lolClientId = pros[0].Id;

            }
        }

        private void ScoreboardForm_Shown(object sender, EventArgs e)
        {
            this.Hide();
            this.notifyIcon1.Visible = false;
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //////////////////////失败重新加载/////////////////////
            if (string.IsNullOrEmpty(webBrowser1.DocumentText)
                || webBrowser1.DocumentText.Contains("404 Not Found"))
            {
                LogUtil.log("webBrowser1 try reload url:" + e.Url);

                try
                {
                    var url = e.Url.ToString();
                    var count = 1;

                    if (url.Contains("?"))
                    {
                        var idx = url.IndexOf("reload=");

                        if (idx != -1)
                        {
                            count = int.Parse(url.Substring(idx + "reload=".Length, 1));

                            if (count < 5)      //重新加载最多5次
                                url = url.Replace($"reload={count}", $"reload={count + 1}");
                            else
                            {
                                LogUtil.log("stop webBrowser1 max try reload url:" + e.Url);
                                return;
                            }
                        }
                        else
                            url += "&reload=1";
                    }
                    else
                        url += "?reload=1";

                    webBrowser1.DocumentText = "正在加载,请稍等..." + count;
                    homeTimer.Interval = count * 1000;
                    homeTimer.Tag = new Uri(url);
                    homeTimer.Enabled = true;
                }
                catch (Exception ex)
                {
                    LogUtil.log("webBrowser1 reload error:" + ex.Message);
                }
                return;
            }
            ////////////////////失败重新加载/////////////////////

            string md5 = HttpClient.computeMD5(this.webBrowser1.DocumentText);
            LogUtil.log("webBrowser1_DocumentCompleted expected md5:" + config.docMd5 + ", real md5:" + md5 + ", size:" + this.webBrowser1.DocumentText.Length);

            if (config.docMd5 != null && !config.docMd5.Trim().Equals("") && !config.docMd5.Trim().Equals(md5))
            {
                LogUtil.log(this.webBrowser1.DocumentText);
                HttpClient.post(QueryParam.create("type", 1001).add("content", "different md5 " + md5).catQueryString(REPORT_ERROR_URL), this.webBrowser1.DocumentText);
            }
        }

        private void transparentPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = Control.MousePosition;   //得到变量的值
                leftFlag = true;                    //点击左键按下时标注为true;
            }
        }

        private void transparentPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouseSet = Control.MousePosition;

            if (leftFlag && !mouseOff.Equals(mouseSet))
            {
                var tmp = new Point(mouseSet.X - mouseOff.X, mouseSet.Y - mouseOff.Y);
                var tmpLocation = Location;

                tmpLocation.Offset(tmp);
                Location = tmpLocation;
                mouseOff = mouseSet;
            }
        }

        private void transparentPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                leftFlag = false;//释放鼠标后标注为false;
            }
        }

        /// <summary>
        /// 如果是游戏退出默认弹出窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lolGameMonitorTimer_Tick(object sender, EventArgs e)
        {
            if (LolMonitor.needShowForm() == 1)
            {
                this.Show();
            }
        }

        private void lolGame_Exited(object sender, EventArgs e)
        {
            lolGameMonitorTimer.Enabled = true;
        }

        //404刷新网页
        private void homeTimer_Tick(object sender, EventArgs e)
        {
            homeTimer.Enabled = false;
            webBrowser1.Navigate((sender as System.Windows.Forms.Timer).Tag as Uri);
        }

        // 供JS调用
        [System.Runtime.InteropServices.ComVisible(true)]
        public class ScriptEvent
        {
            private ScoreboardForm mainForm;
            private DateTime lastOpenTime = DateTime.Now;
            private string lastOpenUserId = "";

            public ScriptEvent(ScoreboardForm form)
            {
                this.mainForm = form;
            }

            public void messageBox(string content, string title)
            {
                MessageBox.Show(content, title);
            }

            public void openBrowser(string url)
            {
                System.Diagnostics.Process.Start("explorer.exe", url);
            }

            public void open(string browser, string url)
            {
                openBrowser(url);
            }

            public void openWinForm()
            {
                this.mainForm.Show();
            }

            public void closeWinForm()
            {
                this.mainForm.Hide();
                this.mainForm.notifyIcon1.ShowBalloonTip(30, null, "双击可以显示本网吧英雄榜", ToolTipIcon.Info);
            }

            public void enableDebug()
            {
                this.mainForm.webBrowser1.ScriptErrorsSuppressed = false;
                this.mainForm.webBrowser1.IsWebBrowserContextMenuEnabled = true;
            }

            public string getLgjSBarID()
            {
                return ConfigUtil.sBarID;
            }

            public string getLgjMemberCardNo()
            {
                return ConfigUtil.cardNo;
            }

            public string getLgjMemberCardType()
            {
                return ConfigUtil.cardType;
            }

            public string getLgjCashServerIP()
            {
                return ConfigUtil.serverIP;
            }

            public string getLgjMemberRealName()
            {
                return ConfigUtil.realName;
            }

            // IM 查询未读消息列表
            public int getUnreadCount()
            {
                return IMConversation.getUnreadCount();
            }

            // IM 保存接收到的消息
            public void receiveIMMessage(string fromUserId, string serverName, string playerName, string avatar, string sumary, object message)
            {
                IMConversation.receiveIMMessage(fromUserId, serverName, playerName, avatar, sumary, message);
            }

            // IM 打开单聊窗口
            public event OpenIMPrivateForm openIMPrivateForm;
            public void openIMDialogWindow(string toUserId, string toServerName, string toPlayerName, string toAvatar)
            {
                // 1秒钟内禁止重复点击
                if (toUserId != null && toUserId.Equals(lastOpenUserId) && (DateTime.Now.Ticks - lastOpenTime.Ticks) < 1000)
                {
                    return;
                }
                lastOpenUserId = toUserId;
                lastOpenTime = DateTime.Now;
                openIMPrivateForm(IMConversation.openDialog(toUserId, toServerName, toPlayerName, toAvatar));
            }

            // IM 获取对话列表
            public string getIMConversationList()
            {
                return IMConversation.getConversationListJSON();
            }

            // 提示消息接口
            public void showTips(string balloonTip)
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(Resources.MSG);
                player.Play();
                if (balloonTip != null && balloonTip.Length != 0)
                {
                    mainForm.notifyIcon1.ShowBalloonTip(30, null, balloonTip, ToolTipIcon.Info);
                }
            }

            // 获取当前会话的历史记录
            public object getHistoryMessage(string toUserId, int index)
            {
                List<object> list = IMConversation.getHistoryMessages(toUserId);
                return list.Count > index ? list[index] : null;
            }
            public int getHistoryMessageCount(string toUserId)
            {
                return IMConversation.getHistoryMessages(toUserId).Count;
            }

            // APP的KVDB接口
            public void Add(string key, object obj)
            {
                IMConversation.globalData.Add(key, obj);
            }

            public object get(string key)
            {
                return IMConversation.globalData[key];
            }

            public void remove(string key)
            {
                IMConversation.globalData.Remove(key);
            }

            public bool containsKey(string key)
            {
                return IMConversation.globalData.ContainsKey(key);
            }

            public bool containsValue(object val)
            {
                return IMConversation.globalData.ContainsValue(val);
            }

            public void clear()
            {
                IMConversation.globalData.Clear();
            }

            public ICollection values()
            {
                return IMConversation.globalData.Values;
            }

            // 获取网吧信息
            public string getShopId()
            {
                return ConfigUtil.shopId;
            }

            public string getHostName()
            {
                return ConfigUtil.hostName;
            }

            // 获取游戏信息
            public LolPlayer getPlayerInfo()
            {
                Process[] processes = Process.GetProcessesByName("LolClient");
                if (processes == null || processes.Length == 0)
                    return null;
                return GMDataTask.getPlayerInfoById(processes[0].Id);
            }

            public string getQQ()
            {
                var playerInfo = GMDataTask.getNowPlayer();

                if (playerInfo != null)
                    return playerInfo.qq;
                else
                    return null;
            }

            public string getServerID()
            {
                var playerInfo = GMDataTask.getNowPlayer();

                if (playerInfo != null)
                    return playerInfo.nodeId;
                else
                    return null;
            }

            public string getNickName()
            {
                var playerInfo = GMDataTask.getNowPlayer();

                if (playerInfo != null)
                    return playerInfo.playerName;
                else
                    return null;
            }

            public string getProfileIconID()
            {
                var playerInfo = GMDataTask.getNowPlayer();

                if (playerInfo != null)
                    return playerInfo.profileIconId;
                else
                    return null;
            }

            public string getTier()
            {
                var playerInfo = GMDataTask.getNowPlayer();

                if (playerInfo != null)
                    return playerInfo.tier;
                else
                    return null;
            }
        }
    }
}

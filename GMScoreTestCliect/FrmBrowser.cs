
using GMScoreTestCliect.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GMScoreTestCliect
{
    public partial class FrmBrowser : Form
    {
        public FrmBrowser()
        {
            InitializeComponent();

            webBrowser = this.web_Browser;
        }

        public WebBrowser webBrowser;
        // 供JS调用
        [System.Runtime.InteropServices.ComVisible(true)]
        public class ScriptEvent
        {
            private FrmBrowser mainForm;
            private DateTime lastOpenTime = DateTime.Now;
            private string lastOpenUserId = "";

            public ScriptEvent(FrmBrowser form)
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
                this.mainForm.webBrowser.ScriptErrorsSuppressed = false;
                this.mainForm.webBrowser.IsWebBrowserContextMenuEnabled = true;
            }

            public string getLgjSBarID()
            {
                //return ConfigUtil.sBarID;
                return "getLgjSBarID";
            }

            public string getLgjMemberCardNo()
            {
                //return ConfigUtil.cardNo;
                return "getLgjMemberCardNo";
            }

            public string getLgjMemberCardType()
            {
                //return ConfigUtil.cardType;
                return "getLgjMemberCardType";
            }

            public string getLgjCashServerIP()
            {
                //return ConfigUtil.serverIP;
                return "getLgjCashServerIP";
            }

            public string getLgjMemberRealName()
            {
                //return ConfigUtil.realName;
                return "getLgjMemberRealName";
            }

            // IM 查询未读消息列表
            public int getUnreadCount()
            {
                //return IMConversation.getUnreadCount();
                return 0;
            }

            // IM 保存接收到的消息
            public void receiveIMMessage(string fromUserId, string serverName, string playerName, string avatar, string sumary, object message)
            {
                //IMConversation.receiveIMMessage(fromUserId, serverName, playerName, avatar, sumary, message);
            }

            public void openIMDialogWindow(string toUserId, string toServerName, string toPlayerName, string toAvatar)
            {
                // 1秒钟内禁止重复点击
                if (toUserId != null && toUserId.Equals(lastOpenUserId) && (DateTime.Now.Ticks - lastOpenTime.Ticks) < 1000)
                {
                    return;
                }
                lastOpenUserId = toUserId;
                lastOpenTime = DateTime.Now;
               // openIMPrivateForm(IMConversation.openDialog(toUserId, toServerName, toPlayerName, toAvatar));
            }

            // IM 获取对话列表
            public string getIMConversationList()
            {
                //return IMConversation.getConversationListJSON();
                return "getIMConversationList";
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
                //List<object> list = IMConversation.getHistoryMessages(toUserId);
                //return list.Count > index ? list[index] : null;
                return null;
            }
            public int getHistoryMessageCount(string toUserId)
            {
                //return IMConversation.getHistoryMessages(toUserId).Count;
                return 0;
            }

            // APP的KVDB接口
            public void Add(string key, object obj)
            {
                //IMConversation.globalData.Add(key, obj);
            }

            public object get(string key)
            {
                // return IMConversation.globalData[key];
                return "";
            }

            public void remove(string key)
            {
                //IMConversation.globalData.Remove(key);
            }

            public bool containsKey(string key)
            {
                // return IMConversation.globalData.ContainsKey(key);
                return false;
            }

            public bool containsValue(object val)
            {
                // return IMConversation.globalData.ContainsValue(val);
                return false;
            }

            public void clear()
            {
                //IMConversation.globalData.Clear();
            }

            public ICollection values()
            {
                //return IMConversation.globalData.Values;
                return null;
            }

            // 获取网吧信息
            public string getShopId()
            {
                //return ConfigUtil.shopId;
                return "shopId";
            }

            public string getHostName()
            {
                //return ConfigUtil.hostName;
                return "hostName";
            }

            // 获取游戏信息
            //public LolPlayer getPlayerInfo()
            //{
            //    Process[] processes = Process.GetProcessesByName("LolClient");
            //    if (processes == null || processes.Length == 0)
            //        return null;
            //    return GMDataTask.getPlayerInfoById(processes[0].Id);
            //}

            public string getQQ()
            {
                //var playerInfo = GMDataTask.getNowPlayer();

                //if (playerInfo != null)
                //    return playerInfo.qq;
                //else
                    return null;
            }

            public string getServerID()
            {
                //var playerInfo = GMDataTask.getNowPlayer();

                //if (playerInfo != null)
                //    return playerInfo.nodeId;
                //else
                    return null;
            }

            public string getNickName()
            {
                //var playerInfo = GMDataTask.getNowPlayer();

                //if (playerInfo != null)
                //    return playerInfo.playerName;
                //else
                    return null;
            }

            public string getProfileIconID()
            {
                //var playerInfo = GMDataTask.getNowPlayer();

                //if (playerInfo != null)
                //    return playerInfo.profileIconId;
                //else
                    return null;
            }

            public string getTier()
            {
                //var playerInfo = GMDataTask.getNowPlayer();

                //if (playerInfo != null)
                //    return playerInfo.tier;
                //else
                    return null;
            }
        }

        private void FrmBrowser_Load(object sender, EventArgs e)
        {
            ScriptEvent se = new ScriptEvent(this);
 
            this.webBrowser.ObjectForScripting = se;
        }
    }
}

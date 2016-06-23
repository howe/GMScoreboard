using GMScoreboard.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GMScoreboard
{
    public partial class IMPrivateForm : Form
    {
        private readonly static string CHAT_URL = "http://gmaster.youzijie.com/html/nb-scoreboardv4/chat.html?toUserId=";

        private string toUserId;
        private string serverName;
        private string playerName;
        private string avatar;

        private ScriptEvent se;

        // 鼠标位置和左键标记
        private Point mouseOff;
        private bool leftFlag;

        // 闪动任务栏
        [DllImport("user32.dll")]
        public static extern bool FlashWindowEx(ref FLASHWINFO pwfi);
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }
        public enum falshType : uint
        {
            FLASHW_STOP = 0, //停止闪烁
            FALSHW_CAPTION = 1, //只闪烁标题
            FLASHW_TRAY = 2, //只闪烁任务栏
            FLASHW_ALL = 3, //标题和任务栏同时闪烁
            FLASHW_PARAM1 = 4,
            FLASHW_PARAM2 = 12,
            FLASHW_TIMER = FLASHW_TRAY | FLASHW_PARAM1, //无条件闪烁任务栏直到发送停止标志或者窗口被激活，如果未激活，停止时高亮
            FLASHW_TIMERNOFG = FLASHW_TRAY | FLASHW_PARAM2 //未激活时闪烁任务栏直到发送停止标志或者窗体被激活，停止后高亮
        }
        public static bool flashTaskBar(IntPtr hWnd, falshType type)
        {
            FLASHWINFO fInfo = new FLASHWINFO();
            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;//要闪烁的窗口的句柄，该窗口可以是打开的或最小化的
            fInfo.dwFlags = (uint)type;//闪烁的类型
            fInfo.uCount = UInt32.MaxValue;//闪烁窗口的次数
            fInfo.dwTimeout = 0; //窗口闪烁的频度，毫秒为单位；若该值为0，则为默认图标的闪烁频度
            return FlashWindowEx(ref fInfo);
        }
        public void doNotifyNewMessage()
        {
            flashTaskBar(this.Handle, falshType.FLASHW_TIMERNOFG);
        }


        public IMPrivateForm(string toUserId, string serverName, string playerName, string avatar)
        {
            this.toUserId = toUserId;
            this.serverName = serverName;
            this.playerName = playerName;
            this.avatar = avatar;

            InitializeComponent();
            this.se = new ScriptEvent(this, toUserId);
            this.webBrowser1.ObjectForScripting = se;
            this.webBrowser1.Navigate(CHAT_URL + toUserId + "&serverName=" + serverName + "&playerName=" + playerName + "&avatar=" + avatar);
            
        }

        public void invokeScript(string func, object[] args)
        {
            this.webBrowser1.Document.InvokeScript(func, args);
        }

        // 供JS调用
        [System.Runtime.InteropServices.ComVisible(true)]
        public class ScriptEvent
        {
            private string toUserId;
            private IMPrivateForm form;

            public ScriptEvent(IMPrivateForm form, string toUserId)
            {
                this.form = form;
                this.toUserId = toUserId;
            }

            // 发送消息
            public void sendIMMessage(string sumary, object message)
            {
                IMConversation.sendIMMessage(toUserId, sumary, message);
            }

            // 关闭窗体
            public void closeWinForm()
            {
                form.Close();
            }
            public void hideWinForm()
            {
                form.WindowState = FormWindowState.Minimized;
            }

            // 获取当前会话的历史记录
            public object getHistoryMessage(int index)
            {
                List<object> list = IMConversation.getHistoryMessages(toUserId);
                return list.Count > index ? list[index] : null;
            }
            public int getHistoryMessageCount()
            {
                return IMConversation.getHistoryMessages(toUserId).Count;
            }

            // 获取当前玩家基本信息
            public string getServerName()
            {
                return form.serverName;
            }

            public string getUserId()
            {
                return form.toUserId;
            }

            public string getPlayerName()
            {
                return form.playerName;
            }

            public string getAvatar()
            {
                return form.avatar;
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
        }

        private void IMPrivateForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            IMConversation.closeDialog(toUserId);
        }

        private void transparentPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y); //得到变量的值
                leftFlag = true;                  //点击左键按下时标注为true;
            }
        }

        private void transparentPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOff.X, mouseOff.Y);  //设置移动后的位置
                Location = mouseSet;
            }
        }

        private void transparentPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                leftFlag = false;//释放鼠标后标注为false;
            }
        }

        private void IMPrivateForm_Load(object sender, EventArgs e)
        {
            this.Text = "与 " + playerName + " 的私聊";
        }

    }
}

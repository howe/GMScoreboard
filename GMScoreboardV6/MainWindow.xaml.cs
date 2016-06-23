using GMScoreboardV6.Com;
using GMScoreboardV6.Models;
using GMScoreboardV6.Monitor;
using GMScoreboardV6.Utils;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GMScoreboardV6
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly string PRIZE_URL = "http://gmaster.youzijie.com/scoreboardV6/getGameInfos";
        static readonly string USERINFO_URL = "http://gmaster.youzijie.com/scoreboardV6/getUserInfos";
        static readonly string PersonCenter = "http://gmaster.youzijie.com/html/accountv6/user/profile.html";

        readonly System.Windows.Forms.NotifyIcon m_NotifyIcon =
            new System.Windows.Forms.NotifyIcon();
        readonly Hashtable m_WebCache = new Hashtable();
        PersonalCenter personCenter = null;
        WebBrowser chatWeb; WebBrowser lotteryWeb;
        ItemsControl prizeBox; ItemsControl awardBox;

        public MainWindow()
        {
            InitializeComponent();
            App.ConfigV6.MainWindow = this;
            this.Visibility = Visibility.Hidden;
            ThreadPool.QueueUserWorkItem(BackLogic, null);  //后台更新线程
            m_NotifyIcon.MouseDoubleClick += M_NotifyIcon_MouseDoubleClick;
            m_NotifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(
                System.Windows.Forms.Application.ExecutablePath);
        }

        private void M_NotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top = 0;
            this.Left = SystemParameters.WorkArea.Width - this.Width;

#if !DEBUG
            this.TryFixedDesktop();                     //固定到桌面
#else
            this.Topmost = true;
            this.ShowInTaskbar = true;
#endif

            //按后端设置进行tab排序
            try
            {
                var sortMenu = App.ConfigV6.menuTabs.OrderBy(m => m.index).ToArray();

                foreach (var menu in sortMenu)
                {
                    var item = (ImgTabItem)tabControl.Resources[menu.name];

                    LogUtil.log("add to tabControl:" + menu.index + ","
                        + menu.name + ":" + (item != null));

                    item.Title = menu.title;
                    item.IsSelected = menu.index == 0;

                    if (menu.type == 1)     //1是webUrl
                    {
                        var web = (WebBrowser)item.Content;

                        LogUtil.log("tabItem.Content is null:" + (web == null));
                        web.ObjectForScripting = new ScriptEvent(this);
                        web.Source = new Uri(menu.content);
                    }
                    tabControl.Items.Add(item);
                }

                chatWeb = tabControl.FindName("chatWeb") as WebBrowser;
                lotteryWeb = tabControl.FindName("lotteryWeb") as WebBrowser;
                prizeBox = tabControl.FindName("prizeBox") as ItemsControl;
                awardBox = tabControl.FindName("awardBox") as ItemsControl;

                clientTipTimer.Interval = TimeSpan.FromSeconds(1);
                clientTipTimer.Tick += ClientTipTimer_Tick;
                clientTipTimer.Start();
            }
            catch (Exception ex)
            {
                LogUtil.log("tabItem add to tabControl error." + ex.Message);
            }
        }

        private void ClientTipTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                clientTipTimer.Stop();

                if (FullScreenUtil.isFullScreen())
                    return;

                ClientTip tip = HeartBeatTask.getClientTip();
                if (tip != null && tip.errCode == 0 && (App.ConfigV6.maxTip--) > 0)
                    new ClientTipForm(tip).Show();
            }
            catch (Exception ex)
            {
                LogUtil.log("Error occurs during loading client tip.", ex);
            }
            finally
            {
                clientTipTimer.Start();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void person_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (personCenter == null)
            {
                personCenter = new PersonalCenter("个人中心", PersonCenter, 718, 345);
            }

            personCenter.ShowInTaskbar = false;
            personCenter.Left = this.Left - personCenter.Width;
            personCenter.Top = this.Top;
            personCenter.Topmost = true;

            personCenter.Refsah();
            personCenter.Show();
            personCenter.Topmost = false;
        }

        private void WebBrowser_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            var webBrowser = sender as WebBrowser;
            var fi = typeof(WebBrowser).GetField("_axIWebBrowser2",
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (fi != null)
            {
                object browser = fi.GetValue(webBrowser);

                if (browser != null)
                {
                    browser.GetType().InvokeMember("Silent", BindingFlags.SetProperty,
                        null, browser, new object[] { true });
                }
            }
        }

        private void giftBtn_Click(object sender, RoutedEventArgs e)
        {
            var player = GMDataTask.getNowPlayer();

            if (string.IsNullOrEmpty(player.qq))
            {
                return;
            }

            MyAward myAward = new MyAward(player.qq);
            
            myAward.ShowInTaskbar = false;
            myAward.Title = "我的奖励";
            myAward.Left = this.Left + 25;
            myAward.Top = this.Top + 80;
            myAward.Topmost = true;
            myAward.ShowDialog();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            m_NotifyIcon.ShowBalloonTip(30, null, "双击我可以再显示竞技大师",
                System.Windows.Forms.ToolTipIcon.Info);
        }

        private bool TryFixedDesktop()
        {
            var handle = WinApi.GetWindowHandle(this);
            var desktop = WinApi.FindWindowEx(WinApi.FindWindow("Progman", null),
                IntPtr.Zero, "shelldll_defview", null);

            if (WinApi.GetChildCount(desktop) == 1)
            {
                WinApi.SetParent(handle, desktop);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void BackLogic(object state)
        {
            int clientId = 0;
            string playerName = null;
            int uv = 0, pv = 0;
            long refreshTick = 0;

            while (true)
            {
                var player = GMDataTask.getNowPlayer();
                var nowTick = DateTime.Now.Ticks / 10000;
                var nowClientId = 0;
                var process = Process.GetProcessesByName(App.ConfigV6.processKeyword);

                if (process != null && process.Length > 0) nowClientId = process[0].Id;

                if (this.Visibility == Visibility.Hidden && !m_NotifyIcon.Visible) //首个玩家
                {
                    if (player != null && !string.IsNullOrEmpty(player.playerName))
                    {
                        clientId = nowClientId;
                        playerName = player.playerName;

                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (!string.IsNullOrEmpty(player.qq))
                            {
                                ShowUserInfo(ref uv);
                                ShowPrize(ref pv);

                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    #region 
                                    try
                                    {
                                        lotteryWeb.InvokeScript("logout");
                                    }
                                    catch (Exception exception)
                                    {

                                    }

                                    try
                                    {
                                        lotteryWeb.Refresh();
                                    }
                                    catch (Exception exception)
                                    {

                                       
                                    }

                                    try
                                    {
                                        chatWeb.InvokeScript("logout");
                                    }
                                    catch (Exception exception)
                                    {


                                    }

                                    try
                                    {
                                        chatWeb.Refresh();
                                        
                                    }
                                    catch (Exception exception) 
                                    {

                                    }
                                    #endregion
                                }));
                            }

                            m_NotifyIcon.Visible = true;
                            this.Show();
                        }));
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                }

                if (player != null && nowClientId != 0 && nowClientId != clientId
                    && !string.IsNullOrEmpty(player.playerName)
                    && !playerName.Equals(player.playerName))   //切换玩家
                {
                    clientId = nowClientId;
                    playerName = player.playerName;
                    refreshTick = nowTick;
                    uv = 0; pv = 0;

                    ShowUserInfo(ref uv);
                    ShowPrize(ref pv);

                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        #region 
                        try
                        {
                            lotteryWeb.InvokeScript("logout");
                        }
                        catch (Exception exception)
                        {

                        }

                        try
                        {
                            lotteryWeb.Refresh();
                        }
                        catch (Exception exception)
                        {


                        }

                        try
                        {
                            chatWeb.InvokeScript("logout");
                        }
                        catch (Exception exception)
                        {


                        }

                        try
                        {
                            chatWeb.Refresh();

                        }
                        catch (Exception exception)
                        {

                        }
                        #endregion

                    }));
                }

                if (nowTick - refreshTick >= App.ConfigV6.refreshInterval)  //定时刷新
                {
                    refreshTick = nowTick;
                    ShowUserInfo(ref uv);
                    ShowPrize(ref pv);
                }

                Thread.Sleep(33);   //sleep
            }
        }

        private void ShowUserInfo(ref int v)
        {
            var player = GMDataTask.getNowPlayer();
            var ret = HttpClient.post(QueryParam.create("shopId", ConfigUtil.shopId)
                .add("hostName", ConfigUtil.hostName).add("qq", player.qq)
                .catQueryString(USERINFO_URL), null);

            //LogUtil.log("load userInfo ret:" + ret);

            var userInfo = JsonHelper.Deserialize<WebRetInfo<UserInfo>>(ret);

            if (userInfo != null)
                v = userInfo.v;

            if (userInfo.body != null)
            {
                this.Dispatcher.BeginInvoke(new Action<UserInfo, Models.LolPlayer>((x, y) =>
                {
                    barName.Text = x.barName;
                    hostName.Text = x.hostName;
                    goldBox.Text = x.balance.ToString();
                    nickBox.Text = y.playerName;

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri($"http://img.lolbox.duowan.com/profileIcon/profileIcon{y.profileIconId}.jpg");
                    bitmap.EndInit();
                    faceBox.Source = bitmap;
                }), userInfo.body, player);
            }
        }

        private void ShowPrize(ref int v)
        {
            var player = GMDataTask.getNowPlayer();
            var ret = HttpClient.post(QueryParam.create("shopId", ConfigUtil.shopId)
                .add("hostName", ConfigUtil.hostName)
                .add("v", v).add("qq", player.qq)
                .catQueryString(PRIZE_URL), null);

            //LogUtil.log("load prizeConfig ret:" + ret);

            var config = JsonHelper.Deserialize<WebRetInfo<PrizeConfig>>(ret);

            if (config != null)
                v = config.v;

            if (config.body != null)
            {
                this.Dispatcher.BeginInvoke(new Action<object, object>((x, y) =>
                {
                    prizeBox.ItemsSource = (ActivieConfig[])x;
                    awardBox.ItemsSource = (AwardConfig[])y;
                }), (object)config.body.activies, (object)config.body.awards);
            }
        }

        System.Windows.Threading.DispatcherTimer clientTipTimer = new System.Windows.Threading.DispatcherTimer();

        #region 前端接口

        // 供JS调用
        [System.Runtime.InteropServices.ComVisible(true)]
        public class ScriptEvent
        {
            private Window mainForm;
            private DateTime lastOpenTime = DateTime.Now;

            private PersonalCenter showOtherWindow = null;
            public ScriptEvent(Window form)
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
                if (this.mainForm.Visibility == Visibility.Hidden)
                {
                    return;
                }

                this.mainForm.Show();
            }

            public void closeWinForm()
            {
                this.mainForm.Hide();

                MainWindow mainwindow = mainForm as MainWindow;

                if (mainwindow == null)
                {
                    return;
                }

                mainwindow.m_NotifyIcon.ShowBalloonTip(30, null, "双击我可以再显示竞技大师",
                    System.Windows.Forms.ToolTipIcon.Info);
            }

            public void enableDebug()
            {
                //this.mainForm.webBrowser1.ScriptErrorsSuppressed = false;
                //this.mainForm.webBrowser1.IsWebBrowserContextMenuEnabled = true;
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

            // APP的KVDB接口
            public void Add(string key, object obj)
            {
                MainWindow mainwindow = mainForm as MainWindow;

                if (mainwindow == null)
                {
                    return;
                }
                mainwindow.m_WebCache.Add(key, obj);
            }

            public object get(string key)
            {
                MainWindow mainwindow = mainForm as MainWindow;

                if (mainwindow == null)
                {
                    return "";
                }
                return mainwindow.m_WebCache[key];
            }

            public void remove(string key)
            {
                MainWindow mainwindow = mainForm as MainWindow;

                if (mainwindow == null)
                {
                    return;
                }

                mainwindow.m_WebCache.Remove(key);
            }

            public bool containsKey(string key)
            {
                MainWindow mainwindow = mainForm as MainWindow;

                if (mainwindow == null)
                {
                    return false;
                }

                return mainwindow.m_WebCache.ContainsKey(key);
            }

            public bool containsValue(object val)
            {
                MainWindow mainwindow = mainForm as MainWindow;

                if (mainwindow == null)
                {
                    return false;
                }
                return mainwindow.m_WebCache.ContainsValue(val);
            }

            public void clear()
            {
                MainWindow mainwindow = mainForm as MainWindow;

                if (mainwindow == null)
                {
                    return;
                }
                mainwindow.m_WebCache.Clear();
            }

            public ICollection values()
            {
                MainWindow mainwindow = mainForm as MainWindow;

                if (mainwindow == null)
                {
                    return new Hashtable();
                }
                return mainwindow.m_WebCache.Values;
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
            public Models.LolPlayer getPlayerInfo()
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

            public void refreshUserInfo()
            {
                MainWindow mainwindow = mainForm as MainWindow;

                if (mainwindow == null)
                {
                    return;
                }

                int v = 0;

                mainwindow.ShowUserInfo(ref v);
            }

            public void UpdateBalance(string balanceMoney)
            {
                MainWindow mainwindow = mainForm as MainWindow;

                if (mainwindow == null)
                {
                    return;
                }

                mainwindow.Dispatcher.Invoke(new Action<TextBlock, string>((text, balance) =>
                {
                    text.Text = balance;
                }));
            }

            public void OpenNewWindow(string title, string url, int height, int width)
            {
                if (showOtherWindow == null)
                {
                    showOtherWindow = new PersonalCenter(title, url, height, width);
                }

                showOtherWindow.ShowInTaskbar = false;
                showOtherWindow.Left = App.ConfigV6.MainWindow.Left - width;
                showOtherWindow.Top = App.ConfigV6.MainWindow.Top;
                showOtherWindow.Topmost = true;
                showOtherWindow.Icon = App.ConfigV6.MainWindow.Icon;
                showOtherWindow.Refsah(url);
                showOtherWindow.Show();
                showOtherWindow.Topmost = false;
            }
        }

        #endregion
    }
}
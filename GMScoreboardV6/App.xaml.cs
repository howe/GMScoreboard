using GMScoreboardV6.Models;
using GMScoreboardV6.Monitor;
using GMScoreboardV6.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using WpfApplication = System.Windows.Application;

namespace GMScoreboardV6
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : WpfApplication
    {
        static string CONFIG_URL = "http://gmaster.youzijie.com/configV2/getGMScoreboardConfigV6";
        static string CheckRank_Url = "http://gmaster.youzijie.com/html/GMscoreboardv6/rank.html?";
        static readonly int SLEEP_TIME = 5000;
        static Mutex s_Mutex;       //进程唯一
        static bool s_IsOne;        //进程唯一

        public static ScoreBoardV6Config ConfigV6 { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                // 防止重复运行
                s_Mutex = new Mutex(true, @"Global\jule_gmscoreboard", out s_IsOne);
                if (!s_IsOne) { WpfApplication.Current.Shutdown(); return; }

                // 初始化日志
                LogUtil.initialLogUtil();
                LogUtil.log("GMScoreboardV6 start.");

                // .net 2.0 以下版本退出
                if (Environment.Version.Major < 2)
                {
                    LogUtil.log("GMScoreboardV6 Exit for unmatch .net version.");
                    Thread.Sleep(3000); System.Environment.Exit(0);
                }

                // 非龙管家版本优先
                if (!ConfigUtil.getCurrentExeName().Contains("GM") && (Process.GetProcessesByName("GMScoreboardV6").Length > 0
                    || Process.GetProcessesByName("GMScoreboard").Length > 0
                    || Process.GetProcessesByName("GMaster").Length > 0))
                {
                    LogUtil.log("Existed first. Exit.");
                    Thread.Sleep(3000); System.Environment.Exit(0);
                }

                // 设置程序浏览器内核
                WinUtil.setRegKeyAndValue(Registry.CurrentUser, @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", ConfigUtil.getCurrentExeName() + ".exe", int.MaxValue);

                //设置SSL支持
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\3\", "1609", 0);

                // 清理IE缓存文件
                WinUtil.CleanIETempFiles();

                // 加载配置
                ConfigUtil.initial();
                App.ConfigV6 = loadGMScoreboardConfig();                    //供全局使用
                LogUtil.log("Loaded ScoreBoardV6Config.");

                SearchLolPath(App.ConfigV6);

                // 启动LOL进程监控
                new Thread(new PluginTask(App.ConfigV6.lolClientPaths).preparePlugins).Start();
                new Thread(LolMonitor.monitorGameTask).Start();
                new Thread(LolMonitor.monitorClientTask).Start();
                new Thread(new HeartBeatTask(App.ConfigV6).beat).Start();
            }
            catch (Exception ex)
            {
                LogUtil.log("Main thread exit.", ex);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (s_IsOne)        //已获取互斥体
            {
                s_Mutex.ReleaseMutex();
                s_Mutex.Close();
            }
        }

        public static ScoreBoardV6Config loadGMScoreboardConfig()
        {
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;

            while (true)
            {
                try
                {
                    QueryParam param = QueryParam.create("screenHeight", screenHeight).add("screenWidth", screenWidth).
                        add("cardNo", ConfigUtil.cardNo).add("serverIp", ConfigUtil.serverIP).add("cardType", ConfigUtil.cardType)
                        .add("realName", ConfigUtil.realName);
                    string ret = HttpClient.post(param.catQueryString(CONFIG_URL), null);

                    LogUtil.log("LoadConfigV6:return " + ret);

                    var config = JsonHelper.Deserialize<WebRetInfo<ScoreBoardV6Config>>(ret);

                    if (config != null && config.errCode == 0)
                    {
                        ConfigUtil.shopId = "" + config.body.shopId;
                        ConfigUtil.agentId = config.body.agentId;
                        ConfigUtil.isDebug = config.body.isDebug;
                        return config.body;
                    }
                    else if (config != null && config.errCode == 999)
                    {
                        LogUtil.log("LoadConfig:application exit.");
                        Thread.Sleep(3000); System.Environment.Exit(0);
                    }
                }
                catch (Exception e)
                {
                    LogUtil.log("Error occurs during loading config.", e);
                }

                Thread.Sleep(SLEEP_TIME);
            }
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogUtil.log("run exception.", e.Exception);
        }

        private void GetAward_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as System.Windows.Controls.Button;

            var award = btn.DataContext as AwardConfig;

            if (award != null)
            {
                GetAward(award.awardId.ToString());
                return;
            }

            var myAward = btn.DataContext as Prizes;

            if (myAward != null)
            {
                GetAward(myAward.awardId.ToString());
                return;
            }
        }

        private void GetAward(string awardId)
        {
            try
            {
                var webclient = new WebClient();
                var ret = webclient.DownloadString(QueryParam.create("billid", awardId)
                    .add("supid", 1).add("callback", "JSON_CALLBACK")
                    .catQueryString("http://" + ConfigUtil.serverIP
                    + ":8089/lolgameaddmoney.htm"));

                if (!string.IsNullOrEmpty(ret) && ret.IndexOf('1') != -1)
                {
                    MsgBox.Show(MainWindow.Icon, "竞技大师", "领取奖励成功！", "知道了");
                }
                else
                {
                    MsgBox.Show(MainWindow.Icon, "竞技大师", "你已领取过了！", "知道了");
                }
            }
            catch (Exception)
            {
                MsgBox.Show(MainWindow.Icon, "竞技大师", "网络错误！", "知道了");
            }
        }

        PersonalCenter checkRank = new PersonalCenter();
        private void btnCheckRank_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as System.Windows.Controls.Button;

            if (btn == null)
            {
                return;
            }

            var activeConfig = btn.DataContext as ActivieConfig;

            if (activeConfig==null)
            {
                return;
            }
            
            string target = GetRankName(activeConfig.ruleId);

            string url = string.Format(
                "{0}shopId={1}&hostName={2}&fromSec=true&target={3}", 
                CheckRank_Url,
                ConfigUtil.shopId, ConfigUtil.hostName, target);

            checkRank = new PersonalCenter("排行榜", url, 720, 345);
            
            if (App.ConfigV6.PersonalCenter != null)
            {
                App.ConfigV6.PersonalCenter.Hide();
            }

            checkRank.ShowInTaskbar = false;
            checkRank.Left = App.ConfigV6.MainWindow.Left - checkRank.Width;
            checkRank.Top = App.ConfigV6.MainWindow.Top;
            checkRank.Topmost = true;
            checkRank.Icon = MainWindow.Icon;
            checkRank.Refsah(url);
            checkRank.Show();
            checkRank.Topmost = false;
        }

        public static string GetRankName(string rankid)
        {
            //1031 杀人王
            //1032 实力王
            //1033 耐力王
            string rankType = string.Empty;

            switch (rankid)
            {
                case "1031":
                    rankType = "killrank";
                    break;
                case "1032":
                    rankType = "winrank";
                    break;
                case "1033":
                    rankType = "timerank";
                    break;
                default:
                    rankType = "winrank";
                    break;
            }

            return rankType;
        }

        static void SearchLolPath(ScoreBoardV6Config config)
        {
            // LOL路径查找策略
            try
            {
                #region 1 查注册表
                string lolPath = ScoreBoardV6Config.GetLolClientPath();

                #endregion

                #region 2 校验上报路径中是否存在

                bool needSearchDrice = false;

                if (!string.IsNullOrEmpty(lolPath) &&
                    !config.lolClientPaths.Contains(lolPath))
                {
                    Utils.HttpClient.post(LolMonitor.REPORT_LOLCLIENTPATH, lolPath);
                    config.lolClientPaths += string.Concat(lolPath, "|");
                }

                if (string.IsNullOrEmpty(lolPath))
                {
                    string[] lolPathArray = config.lolClientPaths.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                    needSearchDrice = true;

                    foreach (var item in lolPathArray)
                    {
                        if (Directory.Exists(item))
                        {
                            needSearchDrice = false;
                            break;
                        }
                    }
                }

                #endregion

                #region 3 路径扫描

                if (needSearchDrice)
                {
                    List<string> lolPaths = DriveInfoSearchFile.GetPathByFile("LolClient.exe");

                    foreach (var item in lolPaths)
                    {
                        if (!App.ConfigV6.lolClientPaths.Contains(item))
                        {
                            HttpClient.post(LolMonitor.REPORT_LOLCLIENTPATH, item);
                            App.ConfigV6.lolClientPaths += string.Concat(item, "|");
                        }
                    }
                }

                #endregion

            }
            catch (Exception exception)
            {
                LogUtil.log("Error occurs during SearchLolPath.", exception);
            }
        }
    }
}
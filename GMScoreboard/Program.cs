using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using GMScoreboard.Model;
using GMScoreboard.Util;
using System.Threading;
using System.Diagnostics;
using GMScoreboard.Monitor;
using System.Drawing;
using Microsoft.Win32;

namespace GMScoreboard
{
    static class Program
    {

        static string CONFIG_URL = "http://gmaster.youzijie.com/configV2/getGMScoreboardConfigV4";
        static readonly int SLEEP_TIME = 5000;

        /// <summary>
        /// 入口函数 启动监控器 启动窗口
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                // 初始化日志
                LogUtil.initialLogUtil();
                LogUtil.log("GMScoreboardV5 start.");

                // .net 2.0 以下版本退出
                if (Environment.Version.Major < 2)
                {
                    LogUtil.log("GMScoreboardV5 Exit for unmatch .net version.");
                    Thread.Sleep(3000); System.Environment.Exit(0);
                }

                // 防止重复运行
                string exeName = ConfigUtil.getCurrentExeName();
                LogUtil.log("Check Duplicated process " + exeName);
                if (Process.GetProcessesByName(exeName).Length > 1)
                {
                    System.Environment.Exit(0);
                }

                // 非龙管家版本优先
                if (!ConfigUtil.getCurrentExeName().Contains("GM") && (Process.GetProcessesByName("GMScoreboardV5").Length > 0 || Process.GetProcessesByName("GMScoreboard").Length > 0 || Process.GetProcessesByName("GMaster").Length > 0))
                {
                    LogUtil.log("Existed first. Exit.");
                    Thread.Sleep(3000); System.Environment.Exit(0);
                }

                // 设置程序浏览器内核
                WinUtil.setRegKeyAndValue(Registry.CurrentUser, @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", ConfigUtil.getCurrentExeName() + ".exe", 8888);

                //设置SSL支持
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\3\", "1609", 0);

                // 清理IE缓存文件
                WinUtil.CleanIETempFiles();

                // 加载配置
                ConfigUtil.initial();
                ScoreBoardV3Config config = loadGMScoreboardConfig();

                LogUtil.log("Loaded ScoreBoardV5Config.");

                SearchLolPath(config);

                new Thread(new PluginTask(config.lolClientPaths).preparePlugins).Start();
                new Thread(LolMonitor.monitorGameTask).Start();
                new Thread(LolMonitor.monitorClientTask).Start();
                new Thread(new HeartBeatTask(config).beat).Start();
                new Thread(new PatchTask(config.patch).autoPatch).Start();

                // 启动主界面
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ScoreboardForm(config));
            }
            catch (Exception e)
            {
                LogUtil.log("Main thread exit.", e);
            }
        }

        /// <summary>
        /// 加载配置项 直至加载成功或者999退出程序  否则一直循环获取 15秒一次
        /// </summary>
        public static ScoreBoardV3Config loadGMScoreboardConfig()
        {
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;

            while (true)
            {
                try
                {
                    QueryParam param = QueryParam.create("screenHeight", screenHeight).add("screenWidth", screenWidth).
                        add("cardNo", ConfigUtil.cardNo).add("serverIp", ConfigUtil.serverIP).add("cardType", ConfigUtil.cardType).add("realName", ConfigUtil.realName);
                    string ret = HttpClient.post(param.catQueryString(CONFIG_URL), null);
                    LogUtil.log("LoadConfig:return " + ret);

                    ScoreBoardV3Config config = ScoreBoardV3Config.fromXML(ret);

                    if (config != null && config.errCode == 0)
                    {
                        ConfigUtil.shopId = "" + config.shopId;
                        ConfigUtil.agentId = config.agentId;
                        ConfigUtil.isDebug = config.isDebug;
                        return config;
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

        static void SearchLolPath(ScoreBoardV3Config config)
        {
            // LOL路径查找策略
            try
            {
                #region 1 查注册表
                string lolPath = ScoreBoardV3Config.GetLolClientPath();

                #endregion

                #region 2 校验上报路径中是否存在

                bool needSearchDrice = false;

                if (!string.IsNullOrEmpty(lolPath) &&
                    !config.lolClientPaths.Contains(lolPath))
                {
                    HttpClient.post(LolMonitor.REPORT_LOLCLIENTPATH, lolPath);
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
                    List<string> lolPaths = new FileSearchUtil().GetPathByFile("LolClient.exe");

                    foreach (var item in lolPaths)
                    {
                        HttpClient.post(LolMonitor.REPORT_LOLCLIENTPATH, item);
                        config.lolClientPaths += string.Concat(item, "|");
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

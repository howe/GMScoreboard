using GMScoreboard.Model;
using GMScoreboard.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace GMScoreboard.Monitor
{
    public class HeartBeatTask
    {

        private static readonly string CLIENT_TIP_URL = "http://gmaster.youzijie.com/scoreboard/clienttip/getTip";
        //TODO:修改提交地址
        //http://gmaster.youzijie.com/scoreboard/clienttip/report
        private static readonly string REPORT_LOG_URL = "http://wbmgmt.youzijie.com/scoreboard/clienttip/report";

        private static readonly Object lck = new Object();
        private static Queue tips = new Queue();

        private ScoreBoardV3Config config;
        public HeartBeatTask(ScoreBoardV3Config config)
        {
            this.config = config;
        }

        public void beat()
        {
            LogUtil.log("HeartBeatTask.beat stated. Interval is " + config.refreshInterval);
            while (true)
            {
                try
                {
                    // 龙管家版本预备动作
                    if ("lgj".Equals(ConfigUtil.agentId) && LgjInfoUtil.genMemberInfo() == 1 && LgjInfoUtil.readLgjInfos() > 0)
                    {
                        LogUtil.log("HeartBeat LGJ Userstatus changed.");
                    }

                    QueryParam qparam = QueryParam.create("t", Environment.TickCount);

                    // 查询游戏进程和账户信息
                    Process[] processes = Process.GetProcessesByName("League of Legends");
                    if (processes != null && processes.Length > 0)
                    {
                        qparam.add("lolGameId", processes[0].Id);
                        qparam.add("lolGameStartTime", LogUtil.toUnixTime(processes[0].StartTime));
                    }

                    // 查询大厅进程和账户信息
                   processes = Process.GetProcessesByName("LolClient");
                    if (processes != null && processes.Length > 0)
                    {
                        qparam.add("lolClientId", processes[0].Id);
                        qparam.add("lolClientStartTime",  LogUtil.toUnixTime(processes[0].StartTime));

                        LolPlayer player = GMDataTask.getPlayerInfoById(processes[0].Id);
                        if (player != null)
                            qparam.add("qq", player.qq).add("nodeId", player.nodeId).add("playerName", ConfigUtil.UrlEncode(player.playerName))
                                .add("tier", player.tier).add("profileIconId", player.profileIconId);
                    }

                    string ret = HttpClient.post(qparam.catQueryString(CLIENT_TIP_URL), null);
                    LogUtil.log("LoadClientTip:return " + ret);
                    ClientTip tip = ClientTip.fromXML(ret);
                    
                    // 退出信号
                    if (tip != null && tip.errCode == 999)
                    {
                        LogUtil.log("HeartBeat received exit code. Exit.");
                        Thread.Sleep(3000); System.Environment.Exit(0);
                        return;
                    }

                    // 重启信号
                    if (tip != null && tip.errCode == 998)
                    {
                        LogUtil.log("HeartBeat received restart code. Exit.");
                        System.Windows.Forms.Application.Restart();
                    }

                    // 重启母程序并退出信号
                    if (tip != null && tip.errCode == 997)
                    {
                        LogUtil.log("HeartBeat received restart GMMaster. Exit.");
                        System.Diagnostics.Process.Start(ConfigUtil.getCurProcessDir() + "GMaster.exe");
                        System.Environment.Exit(0);
                    }

                    // 日志上送信息
                    if (tip != null && tip.errCode == 996)
                    {
                        LogUtil.log("HeartBeat received report log.");

                        using (StreamReader reader = new StreamReader(new FileStream(LogUtil.LOG_FILE, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), Encoding.UTF8))
                        {
                            HttpClient.post(REPORT_LOG_URL, reader.ReadToEnd());
                        }
                    }

                    // 弹 TIP
                    if (tip != null && tip.errCode == 0 && tip.link.StartsWith("http://gmaster.youzijie.com"))
                    {
                        lock (lck)
                        {
                            tips.Enqueue(tip);
                        }
                    }

                    // 执行任务
                    if (tip != null && tip.errCode == 0 && tip.taskType > 0)
                    {
                        if (tip.taskType == 1)
                        {
                            new Thread(new TGPTask(1).runTask).Start();
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.log("Error occurs during loading client tip.", ex);
                }

                // 间歇心跳
                Thread.Sleep(config.refreshInterval);
            }
        }

        public static void addClientTip(ClientTip tip)
        {
            lock (lck)
            {
                tips.Enqueue(tip);
            }
        }

        public static ClientTip getClientTip()
        {
            lock(lck)
            {
                if (tips.Count > 0)
                {
                    return (ClientTip)tips.Dequeue();
                }
            }
            return null;
        }
    }
}

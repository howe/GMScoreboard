using GMScoreboardV6.Models;
using GMScoreboardV6.Utils;
using System;
using System.Diagnostics;
using System.Threading;

namespace GMScoreboardV6.Monitor
{
    public class LolMonitor
    {
        static Object lok = new Object();
        static Boolean needShow = false;

        static string REPORT_STATUS = "http://gmaster.youzijie.com/reportV5/status";
        public static string REPORT_LOLCLIENTPATH = "http://gmaster.youzijie.com/reportV5/lolClientPath";

        public static void monitorGameTask()
        {
            LogUtil.log("LolMonitor.monitorGameTask started.");
            while (true)
            {
                Process[] processes = Process.GetProcessesByName("League of Legends");
                if (processes != null && processes.Length > 0)
                {
                    processes[0].EnableRaisingEvents = true;
                    processes[0].Exited += new EventHandler(lolGame_Exited);
                    LogUtil.log("LolMonitor.monitorGameTask League of Legends starts.");
                    break;
                }
                Thread.Sleep(1000);
            }

            findCurPlayerAndReportStatus("2", 0);
        }

        public static void reportMainModule()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName("CrossProxy");
                if (processes != null && processes.Length > 0)
                {
                    Process p = processes[0];
                    HttpClient.post(REPORT_LOLCLIENTPATH, p.MainModule.FileName);
                }
            }
            catch (Exception e)
            {
                LogUtil.log("Error occurs during reportMainModule.", e);
            }
        }

        public static void monitorClientTask()
        {
            LogUtil.log("LolMonitor.monitorClientTask started.");
            int lolClientId = 0;

            while (true)
            {
                Process[] processes = Process.GetProcessesByName("LolClient");
                if (processes != null && processes.Length > 0)
                {
                    processes[0].EnableRaisingEvents = true;
                    processes[0].Exited += new EventHandler(lolClient_Exited);
                    lolClientId = processes[0].Id;
                    new Thread(new GMDataTask(lolClientId, processes[0].StartTime).monitorGMDataLog).Start();
                    LogUtil.log("LolMonitor.monitorClientTask LolClient starts.");
                    break;
                }
                Thread.Sleep(1000);
            }

            findCurPlayerAndReportStatus("1", lolClientId);
            reportMainModule();
        }

        static void lolClient_Exited(object sender, EventArgs e)
        {
            new Thread(monitorClientTask).Start();
            Process p = (Process)sender;
            findCurPlayerAndReportStatus("4", p.Id);
            LogUtil.log("LolMonitor.lolClient_Exited reported.");
        }

        static void lolGame_Exited(object sender, EventArgs e)
        {
            HeartBeatTask.addClientTip(new ClientTip()
            {
                headText = "等待游戏结果...",
                title = "请不要跳过等待",
                content = "英雄请不要跳过等待，正在计算您的战绩，以及奖品哦！",
            });

            new Thread(monitorGameTask).Start();
            findCurPlayerAndReportStatus("3", 0);
            LogUtil.log("LolMonitor.lolGame_Exited reported.");
            lock (lok)
            {
                needShow = true;
            }
        }

        static void findCurPlayerAndReportStatus(string status, int lolClientId)
        {
            try
            {
                if (lolClientId == 0)
                {
                    Process[] processes = Process.GetProcessesByName("LolClient");
                    lolClientId = processes[0].Id;
                }

                Thread.Sleep(30000);
                Models.LolPlayer player = GMDataTask.getPlayerInfoById(lolClientId);

                QueryParam param = QueryParam.create("lolClientId", player.lolClientId).add("lolClientStartTime", player.lolClientStartTime).add("tier", player.tier)
                    .add("qq", player.qq).add("nodeId", player.nodeId).add("status", status).add("profileIconId", player.profileIconId).add("playerName", ConfigUtil.UrlEncode(player.playerName));

                string ret = HttpClient.post(param.catQueryString(REPORT_STATUS), player.playerName);
                if (!"success".Equals(ret))
                {
                    Thread.Sleep(5000);
                    HttpClient.post(param.add("retry", 1).catQueryString(REPORT_STATUS), player.playerName);
                }
            }
            catch (Exception e)
            {
                LogUtil.log("Error occurs during reporting player status.", e);
            }

        }

        public static int needShowForm()
        {
            if (needShow)
            {
                lock (lok)
                {
                    needShow = false;
                }

                return 1;
            }
            else
            {
                return 0;
            }
        }

    }
}

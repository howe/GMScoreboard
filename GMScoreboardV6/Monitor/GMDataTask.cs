using GMScoreboardV6.Models;
using GMScoreboardV6.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;

namespace GMScoreboardV6.Monitor
{
    /// <summary>
    /// GMData 插件类 用于启动GMdATA并监控日志的输出
    /// </summary>
    public class GMDataTask
    {

        static string REPORT_GAME = "http://gmaster.youzijie.com/reportV5/gamelog";
        static readonly string REPORT_NAME = "http://gmaster.youzijie.com/reportV5/playerName";
        static readonly string REPORT_TIER = "http://gmaster.youzijie.com/reportV5/playerTier";
        static readonly string REPORT_ERROR_AMFDAT = "http://gmaster.youzijie.com/reportV5/errorAMF";

        static Hashtable playerInfos = Hashtable.Synchronized(new Hashtable());

        public static LolPlayer getPlayerInfoById(int id)
        {
            return (LolPlayer)playerInfos[id];
        }

        public static LolPlayer getNowPlayer()
        {
            return player;
        }

        private int lolClientId;
        private string lolClientStartTime;
        private int seq = 1;
        private string logDir = "";
        private static LolPlayer player = null;

        public GMDataTask(int id, DateTime st)
        {
            logDir = "C://" + id + "//";
            lolClientStartTime = LogUtil.toUnixTime(st).ToString();
            lolClientId = id;
            player = new LolPlayer();
            playerInfos.Add(id, player);
        }

        /// <summary>
        /// 监控实例对应的LolClient进程号对应目录和输出
        /// </summary>
        public void monitorGMDataLog()
        {

            LogUtil.log("GMDataTask.monitorGMDataLog starts. LolClientId:" + lolClientId);

            // 等待注入生成的文件
            while (!System.IO.Directory.Exists(logDir))
                Thread.Sleep(1000);

            // 生成区服和QQ信息
            fetchCrossProxyInfo();

            while (true)
            {
                if (System.IO.File.Exists(logDir + seq + ".dat"))
                {
                    Thread.Sleep(500);  // 等待文件完全生成
                    try
                    {
                        // AMF3解码
                        var dat = File.ReadAllBytes(logDir + seq + ".dat");
                        var buf = new List<byte>();
                        buf.AddRange(dat);
                        var len = buf.Count;
                        var amf3 = new AMF3();
                        buf = amf3.封包处理(buf);
                        amf3.置数据(buf);
                        var data = amf3.read_elem();
                        string json = amf3.变体型转换文本(data);

                        LogUtil.log("Get dat file and decode amf3 " + logDir + seq + ".dat" + " result len:" + json.Length);
                        seq++;
                        if (string.IsNullOrEmpty(json) || json == "null"|| json== "{ }")
                        {
                            continue;
                        }
                        
                        using (FileStream fs = new FileStream(logDir + seq + ".txt", FileMode.Create))
                        {
                            using (StreamWriter sw = new StreamWriter(fs))
                            {
                                sw.Write(json);
                                sw.Flush();
                            }
                        }

                        // 用户信息提取
                        if (json.Contains("internalName"))
                        {
                            handlerPlayerInfo(json);
                        }
                        // 段位信息提取
                        else if (json.Contains("summonerLeagues"))
                        {
                            handlerPlayerTierInfo(json);
                        }
                        // 赛事信息提取
                        else if (json.Contains("teamPlayerParticipantStats"))
                        {
                            handlerMatchInfo(json);
                        }
                    }
                    catch (Exception e)
                    {
                        LogUtil.log("Error occurs during handling dat File. " + seq + ".dat", e);
                        if (ConfigUtil.isDebug == 1)
                            HttpClient.postBytes(QueryParam.create("lolClientId", lolClientId).add("seq", seq).catQueryString(REPORT_ERROR_AMFDAT), File.ReadAllBytes(logDir + seq + ".dat"));
                    }

                    // 空读次数清零并递增序号
                   

                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        ///  处理玩家信息的消息包 保存并上报
        /// </summary>
        /// <param name="json"></param>
        private void handlerPlayerInfo(string json)
        {
            string name = findValue(json, "internalName");
            string profileIconId = findValue(json, "profileIconId");
            if (name.StartsWith("sum"))
                findValue(json.Substring(json.IndexOf("internalName")), "name");

            string acctId = findValue(json, "acctId");

            // 输出玩家数据
            LogUtil.log("GMDataTask.handlerPlayerInfo playerInfo found name and icon " + name + " " + profileIconId);

            // 如果玩家信息已存在则忽略
            if (player.playerName != null && player.playerName.Trim().Length > 0)
            {
                LogUtil.log("GMDataTask.handlerPlayerInfo playerInfo found name and icon. But existed playerName.");
            }
            else
            {
                // 如果玩家名称不为空先上报
                if (!string.IsNullOrEmpty(name))
                {
                    HttpClient.post(QueryParam.create("qq", player.qq).add("nodeId", player.nodeId).add("lolClientId", player.lolClientId)
                        .add("lolClientStartTime", lolClientStartTime).catQueryString(REPORT_NAME), name);

                    string tier = string.Empty, rank = string.Empty;
                    if (LOLServerMang.GetTierRank(acctId, player.nodeId, ref tier, ref rank))
                    {
                        player.tier = tier;
                        player.rank = rank;
                    }

                }
                // 保存至本地
                lock (player)
                {
                    player.playerName = name;
                    player.profileIconId = profileIconId;
                }
            }
        }

        /// <summary>
        ///  处理玩家信息的消息包
        /// </summary>
        /// <param name="json"></param>
        private void handlerPlayerTierInfo(string json)
        {
            string[] sArray = Regex.Split(json, "previousDayLeaguePosition", RegexOptions.IgnoreCase);
            foreach (string s in sArray)
            {
                if (s.Contains("\"tier\"") && !s.Contains("TEAM-"))
                {
                    json = s;
                    break;
                }
            }

            string tier = findValue(json, "tier");
            if (tier == null || tier.Trim().Length == 0)
                tier = "UNRANKED";
            string rank = findValue(json, "rank");

            // 输出玩家段位
            LogUtil.log("GMDataTask.handlerPlayerTierInfo playerInfo found tier and rank " + tier + rank);
            if (player.tier != null && player.tier.Trim().Length > 0)
            {
                LogUtil.log("GMDataTask.handlerPlayerTierInfo playerInfo found tier and rank. But existed tier.");
            }
            else
            {
                lock (player)
                {
                    player.tier = tier;
                    player.rank = rank;
                }
            }

            if (ConfigUtil.isDebug == 1)
                HttpClient.post(QueryParam.create("qq", player.qq).add("nodeId", player.nodeId).add("lolClientId", player.lolClientId).catQueryString(REPORT_TIER), json);
        }

        /// <summary>
        /// 获取联合登录信息并保存至cross.txt文件
        /// </summary>
        private void fetchCrossProxyInfo()
        {
            string qq = "";
            string nodeId = "";
            string lolClientId = "";
            string lolClientStartTime = "";

            try
            {
                // 获取两个主进程的ID
                Thread.Sleep(1000);
                Process[] lolprocesses = Process.GetProcessesByName("LolClient");
                Process[] crossprocess = Process.GetProcessesByName("CrossProxy");

                Process lolprocess = lolprocesses[0];
                Process crossProcess = crossprocess[0];

                // 设置主进程信息
                lolClientId = lolprocess.Id + "";
                lolClientStartTime = LogUtil.toUnixTime(lolprocess.StartTime) + "";

                // 获取QQ号和区服
                string line = "";
                using (ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + crossProcess.Id))
                {
                    foreach (ManagementObject mo in mos.Get())
                    {
                        line += mo["CommandLine"];
                    }
                }

                LogUtil.log("GMDataTask.fetchCrossProxyInfo read commandline for crossproxy:" + line);

                if (line.Contains(".exe"))
                    line = line.Substring(line.IndexOf(".exe") + 4);

                int qindex = line.IndexOf("-q") + 3;
                int iindex = line.IndexOf("-i");
                int sindex = line.IndexOf("-s") + 3;
                int gindex = line.IndexOf("-g");

                if (qindex > 0 && iindex > qindex)
                {
                    qq = line.Substring(qindex, iindex - qindex);
                }
                if (sindex > 0 && gindex > sindex)
                {
                    nodeId = line.Substring(sindex, gindex - sindex);
                }
                if (qq != null && qq.Trim().Length > 0)
                {
                    qq = qq.Trim();
                }
                if (nodeId != null && nodeId.Trim().Length > 0)
                {
                    nodeId = nodeId.Trim();
                }
            }
            catch (Exception e)
            {
                LogUtil.log("Error occurs during fetchUinAndAreaId.", e);
            }

            try
            {
                lock (player)
                {
                    player.qq = qq;
                    player.nodeId = nodeId;
                    player.lolClientId = lolClientId;
                    player.lolClientStartTime = lolClientStartTime;
                }

                LogUtil.log("GMDataTask.fetchCrossProxyInfo playerInfo found qq and nodeId " + qq + " " + nodeId);
            }
            catch (Exception e)
            {
                LogUtil.log("Error occurs during fetchUinAndAreaId.", e);
            }

        }

        /// <summary>
        /// 处理比赛结果的消息包
        /// </summary>
        /// <param name="json"></param>
        private void handlerMatchInfo(string json)
        {
            LogUtil.log("GMDataTask.handlerMatchInfo report matchData." + lolClientId);

            var count = 16;
            var sleep = 1000;
            var success = false;

            do
            {
                string ret = HttpClient.post(QueryParam.create("lolClientId", player.lolClientId).add("nodeId", player.nodeId).add("qq", player.qq).add("tier", player.tier).add("playerName", ConfigUtil.UrlEncode(player.playerName)).catQueryString(REPORT_GAME), json);

                if ((success = "success".Equals(ret)) == false)
                {
                    Thread.Sleep(sleep);
                    sleep *= 2;
                }
            }
            while (--count > 0 && !success);
        }

        private string findValue(string json, string key)
        {
            try
            {
                key = "\"" + key + "\"";
                int i = json.IndexOf(key);
                if (i > 0)
                {
                    string s = json.Substring(i + key.Length).TrimStart();
                    if (s.StartsWith(":"))
                        s = s.Substring(1).TrimStart();

                    if (s.StartsWith("\""))
                    {
                        return s.Substring(1, s.IndexOf("\",") - 1);
                    }
                    else
                    {
                        return s.Substring(0, s.IndexOf(","));
                    }
                }
            }
            catch (Exception e)
            {
                LogUtil.log("Error occurs during GMDataTask.findValue.", e);
            }

            return "";

        }
    }
}
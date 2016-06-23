using GMScoreboard.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

/// 调用方法 new Thread(LolBattleMonitor.monitorBattleInfo).Start(); 
/// 用于获取用户LOL路径下的比赛结果数据 V5版本暂不调用
namespace GMScoreboard.Monitor
{
    public class LolBattleMonitor
    {

        static string REPORT_BATTLEINFO = "http://gmaster.youzijie.com/reportV2/battleinfo/";
        static string RICHDATA_DIR = null;
        static DateTime latestTime = DateTime.Now;

        public static void monitorBattleInfo()
        {
            LogUtil.log("LolBattleMonitor.monitorBattleInfo started.");

            while (initialLolDir() == null)
            {
                Thread.Sleep(60000);
            }

            while (true)
            {
                Thread.Sleep(30000);

                try
                {
                    string dir = findLatestDir(RICHDATA_DIR);
                    if (dir == null)
                        continue;

                    string file = findLatestFile(dir);
                    if (file == null)
                        continue;

                    FileInfo fi = new FileInfo(file);
                    if (fi.LastWriteTime.CompareTo(latestTime) > 0)
                    {
                        string info = readBattleInfo(file);
                        string qq = dir.Substring(dir.LastIndexOf("\\") + 1);
                        reportBattleInfo(qq, info);
                        latestTime = fi.LastWriteTime;
                    }
                                        
                } 
                catch (Exception e)
                {
                    LogUtil.log("Error occurs during looping battle dir.", e);
                }
            }
        }

        private static string findLatestDir(string dir)
        {
            // 遍历目录
            string[] dirs = Directory.GetDirectories(dir);
            if (dirs == null || dirs.Length == 0)
                return null;

            DateTime dt = DateTime.MinValue;
            string latestDir = null;

            foreach (string d in dirs)
            {
                
                DirectoryInfo di = new DirectoryInfo(d);
                if (di.LastWriteTime.CompareTo(dt) > 0)
                {
                    dt = di.LastWriteTime;
                    latestDir = d;
                }
            }

            return latestDir;
        }

        private static string findLatestFile(string dir)
        {

            // 遍历目录
            string[] files = Directory.GetFiles(dir);
            if (files == null || files.Length == 0)
                return null;

            DateTime dt = DateTime.MinValue;
            string latestFile = null;

            foreach (string file in files)
            {

                FileInfo fi = new FileInfo(file);
                if (file.Contains("BattleInfo") && fi.LastWriteTime.CompareTo(dt) > 0)
                {
                    dt = fi.LastWriteTime;
                    latestFile = file;
                }
            }

            return latestFile;
        }

        /// <summary>
        /// 初始化 LOL 的目录
        /// </summary>
        private static string initialLolDir()
        {
            try
            {
                // 初始化程序目录
                if (RICHDATA_DIR == null)
                {
                    Process[] processes = Process.GetProcessesByName("LolClient");
                    if (processes != null && processes.Length > 0)
                        RICHDATA_DIR = processes[0].MainModule.FileName.Replace(@"Air\LolClient.exe", @"Cross\Apps\GameRecorder\RichData");

                }

                if (RICHDATA_DIR == null)
                {
                    Process[] processes = Process.GetProcessesByName("League of Legends.exe");
                    if (processes != null && processes.Length > 0)
                        RICHDATA_DIR = processes[0].MainModule.FileName.Replace(@"Game\League of Legends.exe", @"Cross\Apps\GameRecorder\RichData");
                }
            } 
            catch (Exception e)
            {
                LogUtil.log("Error occurs during initializing lol dir.");
            }

            return RICHDATA_DIR;
        }

        static void reportBattleInfo(string qq, string info)
        {
            try 
            {
                QueryParam param = QueryParam.create("qq", qq).add("realName", ConfigUtil.realName);;
                HttpClient.post(param.catQueryString(REPORT_BATTLEINFO), info);
            }
            catch (Exception e)
            {
                LogUtil.log("Error occurs during reporting battle info.", e);
            }
        }

        static string readBattleInfo(string file)
        {
            StringBuilder sb = new StringBuilder();
            StreamReader sr = null;
            try
            {
                FileStream logFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                sr = new StreamReader(logFile, Encoding.UTF8);
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("StartTime"))
                        return line;
                }
            } 
            catch (Exception e)
            {
                LogUtil.log("Error occurs during reading battleinfo file.", e);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
            return "";
        }
    }
}

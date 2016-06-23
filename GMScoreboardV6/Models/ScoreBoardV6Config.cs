using GMScoreboardV6.Utils;
using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace GMScoreboardV6.Models
{
    [DataContract]
    public class ScoreBoardV6Config
    {
        [DataMember(Name = "errCode")]
        public int errCode { get; set; }

        [DataMember(Name = "msg")]
        public string msg { get; set; }

        [DataMember(Name = "shopId")]
        public int shopId { get; set; }

        [DataMember(Name = "locationX")]
        public int locationX { get; set; }

        [DataMember(Name = "locationY")]
        public int locationY { get; set; }

        [DataMember(Name = "browser")]
        public string browser { get; set; }

        [DataMember(Name = "barName")]
        public string barName { get; set; }

        [DataMember(Name = "processKeyword")]
        public string processKeyword { get; set; }

        [DataMember(Name = "indexUrl")]
        public string indexUrl { get; set; }

        [DataMember(Name = "patch")]
        public string patch { get; set; }

        [DataMember(Name = "docMd5")]
        public string docMd5 { get; set; }

        [DataMember(Name = "title")]
        public string title { get; set; }

        [DataMember(Name = "agentId")]
        public string agentId { get; set; }

        [DataMember(Name = "lolClientPaths")]
        public string lolClientPaths { get; set; }

        [DataMember(Name = "bossIp")]
        public string bossIp { get; set; }

        [DataMember(Name = "bossPort")]
        public int bossPort { get; set; }

        [DataMember(Name = "maxTip")]
        public int maxTip { get; set; }

        [DataMember(Name = "isDebug")]
        public int isDebug { get; set; }

        [DataMember(Name = "refreshInterval")]
        public int refreshInterval { get; set; }

        [DataMember(Name = "menuTabs")]
        public MenuConfig[] menuTabs { get; set; }

        [DataMember(Name = "homeIndex")]
        public int homeIndex { get; set; }

        [DataMember(Name = "time")]
        public long time { get; set; }

        [DataMember(Name = "MainWindow")]
        public MainWindow MainWindow { get; set; }

        [DataMember(Name = "PersonalCenter")]
        public PersonalCenter PersonalCenter { get; set; }

        [DataMember(Name = "CheckRank")]
        public PersonalCenter CheckRank { get; set; }

        [DataMember(Name = "UseInfo")]
        public UserInfo UseInfo { get; set; }

        public static string GetLolClientPath()
        {
            string lolpath = string.Empty;

            try
            {
                RegistryKey key = Registry.LocalMachine;
                RegistryKey software = key.OpenSubKey("SOFTWARE\\Tencent\\LOL", true);
                if (software == null)
                {
                    return lolpath;
                }

                #region InstallPath

                var InstallPath = software.GetValue("InstallPath");

                if (InstallPath == null ||
                    string.IsNullOrEmpty(InstallPath.ToString()))
                {
                    return lolpath;
                }
                else
                {
                    lolpath = string.Concat(InstallPath.ToString(), "\\Air\\");

                    if (Directory.Exists(lolpath))
                    {
                        return lolpath;
                    }

                    lolpath = "";
                }

                #endregion

                #region setup

                if (string.IsNullOrEmpty(lolpath))
                {
                    var setup = software.GetValue("setup");

                    if (setup == null || string.IsNullOrEmpty(setup.ToString()))
                    {
                        return lolpath;
                    }
                    else
                    {
                        lolpath = string.Concat(InstallPath.ToString().Replace("tcls\\client.exe", ""), "\\Air\\");

                        if (Directory.Exists(lolpath))
                        {
                            return lolpath;
                        }

                        lolpath = "";
                    }
                }

                #endregion

            }
            catch (Exception exception)
            {
                LogUtil.log("Exception：ScoreBoardV3Config.GetLolClientPath,ErrMsg: " + exception.Message);
                lolpath = "";
                return lolpath;
            }

            return lolpath;
        }
    }
}
using GMScoreboard.Util;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace GMScoreboard.Model
{
    [XmlRoot("ScoreBoardV3Config")]
    public class ScoreBoardV3Config
    {
        [XmlElement("errCode")]
        public int errCode { get; set; }

        [XmlElement("msg")]
        public string msg { get; set; }

        [XmlElement("shopId")]
        public int shopId { get; set; }

        [XmlElement("locationX")]
        public int locationX { get; set; }

        [XmlElement("locationY")]
        public int locationY { get; set; }

        [XmlElement("agentId")]
        public string agentId { get; set; }

        [XmlElement("barName")]
        public string barName { get; set; }

        [XmlElement("title")]
        public string title { get; set; }

        [XmlElement("browser")]
        public string browser { get; set; }

        [XmlElement("processKeyword")]
        public string processKeyword { get; set; }

        [XmlElement("indexUrl")]
        public string indexUrl { get; set; }

        [XmlElement("patch")]
        public string patch { get; set; }

        [XmlElement("maxTip")]
        public int maxTip { get; set; }

        [XmlElement("docMd5")]
        public string docMd5 { get; set; }

        [XmlElement("refreshInterval")]
        public int refreshInterval { get; set; }

        [XmlElement("isDebug")]
        public int isDebug { get; set; }

        [XmlElement("lolClientPaths")]
        public string lolClientPaths { get; set; }

        public static ScoreBoardV3Config fromXML(string xml)
        {

            if (xml == null || xml.Trim().Equals(""))
                return null;

            using (StringReader rdr = new StringReader(xml))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ScoreBoardV3Config));
                ScoreBoardV3Config config = (ScoreBoardV3Config)serializer.Deserialize(rdr);
                return config;
            }
        }

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
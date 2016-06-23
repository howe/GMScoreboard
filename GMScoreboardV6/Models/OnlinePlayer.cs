using System.Collections.Generic;
using System.Windows.Media;

namespace GMScoreboardV6.Models
{
    public class PlayersItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string playerName { get; set; }
        /// <summary>
        /// 艾欧尼亚
        /// </summary>
        public string serverName { get; set; }
        /// <summary>
        /// 在线
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string hostName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string qq { get; set; }
        /// <summary>
        /// 无评级
        /// </summary>
        public string tier { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lolClientTime { get; set; }

        public string ServerNameTier { get { return string.Format("{0} {1}", serverName, tier); } }
        public Dictionary<string, string> ShowServerNameRun
        {
            get
            {
                var dic = new Dictionary<string, string>();
                dic.Add(serverName, "#FFFF962E");
                dic.Add(tier, "#999999");
                dic.Add(hostName, "#FFFF962E");
                return dic;
            }
        }
    }

    public class Data
    {
        /// <summary>
        /// 
        /// </summary>
        public int interval { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PlayersItem> players { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int totalPlayer { get; set; }
    }

    public class PlayerRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public int errCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int retCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dtag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Data data { get; set; }
    }
}

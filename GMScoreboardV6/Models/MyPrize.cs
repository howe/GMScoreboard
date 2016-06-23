using System;
using System.Collections.Generic;

namespace GMScoreboardV6.Models
{

    public class Prizes
    {
        public string awardId { get; set; }
        public string bizId { get; set; }
        public string shopId { get; set; }
        public string isWin { get; set; }
        public string status { get; set; }
        public string prizeId { get; set; }
        public string prizeName { get; set; }
        public string playerName { get; set; }
        public string serverName { get; set; }
        public string tier { get; set; }
        public string qq { get; set; }
        public string hostName { get; set; }
        public string ruleId { get; set; }
        public string ruleName { get; set; }
        public string grantTime { get; set; }
        public string createTime { get; set; }
        public string updateTime { get; set; }
        public string lgjAutoChargeAmount { get; set; }
        public string championNameEn { get; set; }
        public string heroImage
        {
            get
            {
                return string.IsNullOrEmpty(championNameEn) ?
                     "http://cdn.youzijie.com/wb/img/julelogo.png" :
                     string.Format("http://gmaster.youzijie.com/img/nb-center/hero/{0}.png", championNameEn);
                //http://gmaster.youzijie.com/img/nb-center/hero/LeeSin.png
                // return "http://img.lolbox.duowan.com/profileIcon/profileIcon25.jpg";
            }
        }
        public string heroImageToolTip
        {
            get { return string.Format("服务器：{0}\r\n  角色：{1}\r\n  段位：{2}", serverName, playerName, tier); }
        }
        public string awardImage { get { return GetAwardImage(); } }
        /// <summary>
        /// 是否领取奖励
        /// </summary>
        public System.Windows.Visibility IsGetAward
        {
            get
            {
                if (this.status.Trim() == "0")
                {
                    return System.Windows.Visibility.Visible;
                }
                return System.Windows.Visibility.Hidden;
            }
        }

        public string ShowCreateTime
        {
            get
            {
                var time = DateTime.Now;

                DateTime.TryParse(this.createTime, out time);

                TimeSpan tmSpan = DateTime.Now - time;

                if (tmSpan.TotalDays > 365)
                {
                    return "一年前";
                }
                if (tmSpan.TotalDays > 30)
                {
                    return "一月前";
                }

                if (tmSpan.TotalDays > 7)
                {
                    return "一周前";
                }

                if (tmSpan.TotalDays > 0)
                {
                    return tmSpan.TotalDays.ToString("N0") + "天前";
                }

                if (tmSpan.TotalHours > 0)
                {
                    return tmSpan.TotalHours.ToString("N0") + "小时前";
                }

                if (tmSpan.TotalSeconds > 0)
                {
                    return tmSpan.TotalSeconds.ToString("N0") + "秒前";
                }

                return "刚刚";
            }
        }
        string GetAwardImage()
        {
            //1031 杀人王
            //1032 实力王
            //1033 耐力王
            string rankTypeUrl = string.Empty;

            switch (this.ruleId)
            {
                //smallGod
                //smallKill
                //smallKillMatch
                //smallTimeMatch
                //smallWin
                //smallWinMatch
                case "1002":
                    rankTypeUrl = "smallKill";
                    break;
                case "1003":
                    rankTypeUrl = "bigKill";
                    break;
                case "1031":
                    rankTypeUrl = "smallKillMatch";
                    break;
                
                default:
                    rankTypeUrl = "smallKill";
                    break;
            }

            return string.Format("http://cdn.youzijie.com/wb/img/scoreboardv4/small/{0}.png", rankTypeUrl);
        }
    }

    public class Body
    {
        public List<Prizes> prizes { get; set; }
        public string unreceivePrizeCount { get; set; }
    }

    public class MyPrizes
    {
        public int errCode { get; set; }
        public int retCode { get; set; }
        public string msg { get; set; }
        public string dtag { get; set; }
        public string v { get; set; }
        public Body body { get; set; }
    }
}

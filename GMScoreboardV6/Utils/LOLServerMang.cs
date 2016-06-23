using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GMScoreboardV6.Utils
{
    public static class LOLServerMang
    {
        public static Dictionary<string, string> LOLServer = new Dictionary<string, string>();

        static void InitLoloServer()
        {
            if (LOLServer.Count > 0)
            {
                return;
            }

            LOLServer = new Dictionary<string, string>();

            #region 
            LOLServer.Add("257", "1");
            LOLServer.Add("258", "2");
            LOLServer.Add("513", "3");
            LOLServer.Add("769", "4");
            LOLServer.Add("1025", "5");
            LOLServer.Add("514", "6");
            LOLServer.Add("1281", "7");
            LOLServer.Add("1537", "8");
            LOLServer.Add("770", "9");
            LOLServer.Add("1793", "10");
            LOLServer.Add("2049", "11");
            LOLServer.Add("1026", "12");
            LOLServer.Add("2305", "13");
            LOLServer.Add("2561", "14");
            LOLServer.Add("2817", "15");
            LOLServer.Add("1282", "16");
            LOLServer.Add("3073", "17");
            LOLServer.Add("3329", "18");
            LOLServer.Add("3585", "19");
            LOLServer.Add("1538", "20");
            LOLServer.Add("65539", "21");
            LOLServer.Add("3841", "22");
            LOLServer.Add("4097", "23");
            LOLServer.Add("4353", "24");
            LOLServer.Add("4609", "25");
            LOLServer.Add("1794", "26");
            LOLServer.Add("4865", "27");

            #endregion
        }

        public static bool GetTierRank(string acctId, string nodeId, ref string tier, ref string rank)
        {
            try
            {


                if (string.IsNullOrEmpty(acctId) || string.IsNullOrEmpty(nodeId))
                {
                    LogUtil.log("LOLServerMang.GetTierRank  return no data");
                    return false;
                }

                InitLoloServer();

                string areaId = string.Empty;

                if (!LOLServer.TryGetValue(nodeId, out areaId))
                {
                    LogUtil.log("LOLServerMang.GetTierRank not ContainsKey:" + nodeId);
                    return false;
                }

                string url = string.Format("http://apps.game.qq.com/lol/api/v2013/userinfo.php?p1={0}&p2={1}&d1=retBaseinfo&r={2}&_=1466582719077", acctId, areaId, new Random().NextDouble());

                string result = HttpClient.Get(url);

                if (string.IsNullOrEmpty(result))
                {
                    return false;
                }

                LogUtil.log("LOLServerMang.GetTierRank Result:" + result);

                var regTier = new Regex(@"tier"":""(.*?)""");

                if (regTier.IsMatch(result))
                {
                    tier = regTier.Match(result).Groups[1].Value;
                }

                var regrank = new Regex(@"rank"":""(.*?)""");

                if (regTier.IsMatch(result))
                {
                    rank = regrank.Match(result).Groups[1].Value;
                }

                return true;
            }
            catch (Exception exception)
            {
                LogUtil.log("LOLServerMang.GetTierRank Exception:", exception);
                return false;
            }
        }
    }


    public class Msg
    {
        public string id { get; set; }
        public string area { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public string lasted { get; set; }
        public string tier { get; set; }
        public string rank { get; set; }
    }

    public class BaseInfo
    {
        public string status { get; set; }
        public Msg msg { get; set; }
    }
}

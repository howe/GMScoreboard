using GMScoreboardV6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMScoreboardV6.Utils
{
    public class ChartRoomMng
    {
        static string getOnlinePlayersUrl = "http://gmaster.youzijie.com/scoreboardV4/getOnlinePlayers";
        public static PlayerRoot GetOnlinePlayers()
        {
            var playerRoot = new PlayerRoot();

            if (string.IsNullOrEmpty(ConfigUtil.shopId))
            {
                return playerRoot;
            }

            try
            {
                var ret = HttpClient.post(QueryParam.create("shopId", ConfigUtil.shopId).catQueryString(getOnlinePlayersUrl), null);

                playerRoot = JsonHelper.Deserialize<PlayerRoot>(ret);

                
                if (playerRoot==null)
                {
                    playerRoot = new PlayerRoot();
                }
            }
            catch (Exception exception)
            {
                playerRoot = new PlayerRoot();
            }
            finally { }

            return playerRoot;
        }
    }
}

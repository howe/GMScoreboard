using GMScoreboardV6.Models;
using System;

namespace GMScoreboardV6.Utils
{
    public class MyAwardMng
    {
        static string getMyAwardsUrl = "http://gmaster.youzijie.com/scoreboardV6/getMyAwards";

        public static MyPrizes GetMyPrizes(string qq)
        {
            MyPrizes myPrices = new MyPrizes();

            if (string.IsNullOrEmpty(qq))
            {
                return myPrices;
            }

            try
            {
                var ret = HttpClient.post(QueryParam.create("shopId", ConfigUtil.shopId)
                .add("hostName", ConfigUtil.hostName).add("qq", qq)
                .catQueryString(getMyAwardsUrl), null);

                myPrices = JsonHelper.Deserialize<MyPrizes>(ret);

                if (myPrices.errCode != 0)
                {
                    LogUtil.log("MyAwardMng.GetAwardList getMyAwards ret Error:" + ret);
                    myPrices = new MyPrizes();
                }
            }
            catch (Exception exception)
            {
                LogUtil.log("MyAwardMng.GetAwardList Exception:", exception);
            }

            return myPrices;
        }
    }
}




using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using GMScoreboard.Util;
using System.Diagnostics;

namespace GMScoreboard.Monitor
{
    public class TGPTask
    {

        private readonly static string GETTASK_URL = @"http://gmaster.youzijie.com/reportV2/tgptask/getTask";
        private readonly static string GETCOOKIE_URL = @"http://gmaster.youzijie.com/reportV2/tgptask/getCookie";
        private readonly static string REPORT_URL = @"http://gmaster.youzijie.com/reportV2/tgpdata/report";

        public int launchType { get; set; }
        public int time { get; set; }
        private int taskId = new Random().Next();
        
        public TGPTask(int ltype)
        {
            this.launchType = ltype;
        }

        public void runTask()
        {
            LogUtil.log("TGPTask.runTask. launchType:" + launchType);
            if (Process.GetProcessesByName("TGPHelper").Length > 0)
            {
                return;
            }

            Thread.Sleep(5000);

            string cookie = postToGMServer(GETCOOKIE_URL, "", "", null);
            if (cookie == null || cookie.Trim().Equals(""))
            {
                return;
            }

            string gmret = postToGMServer(GETTASK_URL, "", cookie, null);
            while (gmret != null && gmret.StartsWith("http"))
            {
                string tgpret = getFromTGP(gmret, cookie);
                gmret = postToGMServer(REPORT_URL, gmret, cookie, tgpret);
            }

            if ("repeat".Equals(gmret))
            {
                Thread.Sleep(30000);
                new Thread(runTask).Start();
            }
        }

        public string getFromTGP(string url, string authcookie)
        {
            string ret = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {

                request = WebRequest.Create(url) as HttpWebRequest;
                request.Headers["Cookie"] = authcookie;
                request.KeepAlive = false;
                request.ContentType = "application/text;";
                request.Method = "GET";
                
                response = request.GetResponse() as HttpWebResponse;
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    ret = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return ret;
        }

        public string postToGMServer(string url, string gmret, string tgpcookie, string body)
        {
            
            string ret = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {

                request = WebRequest.Create(url) as HttpWebRequest;
                request.Headers["sBarID"] = ConfigUtil.sBarID;
                request.Headers["shopId"] = ConfigUtil.shopId;
                request.Headers["hostName"] = ConfigUtil.hostName;
                request.Headers["launchType"] = launchType + "";
                request.Headers["version"] = ConfigUtil.cv;
                request.Headers["tgpcookie"] = tgpcookie;
                request.Headers["taskId"] = ConfigUtil.shopId + ConfigUtil.hostName + taskId;
                request.Headers["times"] = (time++) + "";
                request.KeepAlive = false;
                request.ContentType = "application/text;";
                request.Method = "POST";

                if (body != null)
                {
                    byte[] data = Encoding.UTF8.GetBytes(body);
                    using (Stream stream = request.GetRequestStream())
                        stream.Write(data, 0, data.Length);
                }

                response = request.GetResponse() as HttpWebResponse;
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    ret = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
                if (response != null)
                {
                    response.Close();
                }
            }

            return ret;
        }

    }

}

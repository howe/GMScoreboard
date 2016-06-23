using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace GMScoreboard.Util
{
    public class HttpClient
    {
        static Encoding encoding = Encoding.UTF8;

        static HttpClient()
        {
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = 512;
            ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidationCallback;
        }

        static bool RemoteCertificateValidationCallback(
            Object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static string computeMD5(string text)
        {
            try
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(text));

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                LogUtil.log("Error occurs during computing file.", ex);
            }

            return null;
        }

        public static byte[] get(string url)
        {
            byte[] ret = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {
                request = createHttpRequest(url, "GET");
                response = request.GetResponse() as HttpWebResponse;

                using (Stream stream = response.GetResponseStream())
                {
                    ret = new byte[response.ContentLength];
                    stream.Read(ret, 0, ret.Length);
                }
            }
            catch (Exception e)
            {
                LogUtil.log("Error occurs during http get. " + url + " \n" + e.StackTrace, e);
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

            LogUtil.log("HttpClient.get " + url + " ret:" + ret);
            return ret;
        }

        public static string post(string url, string body)
        {

            string ret = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {
                request = createHttpRequest(url, "POST");
                if (body != null)
                {
                    byte[] data = encoding.GetBytes(body);
                    using (Stream stream = request.GetRequestStream())
                        stream.Write(data, 0, data.Length);
                }

                response = request.GetResponse() as HttpWebResponse;
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, encoding);
                    ret = reader.ReadToEnd();
                }

            }
            catch (Exception e)
            {
                LogUtil.log("Error occurs during http post. " + url + " \n" + e.StackTrace, e);
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

            LogUtil.log("HttpClient.post " + url + " ret:" + ret);
            return ret;
        }


        public static string postBytes(string url, byte[] data)
        {

            string ret = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {
                request = createHttpRequest(url, "POST");
                using (Stream stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                response = request.GetResponse() as HttpWebResponse;
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, encoding);
                    ret = reader.ReadToEnd();
                }

            }
            catch (Exception e)
            {
                LogUtil.log("Error occurs during http post. " + url + " \n" + e.StackTrace, e);
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

            LogUtil.log("HttpClient.post " + url + " ret:" + ret);
            return ret;
        }

        private static HttpWebRequest createHttpRequest(string url, string method)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            request.Headers["sBarID"] = ConfigUtil.sBarID;
            request.Headers["cardNo"] = ConfigUtil.cardNo;
            request.Headers["cardType"] = ConfigUtil.cardType;
            request.Headers["realName"] = ConfigUtil.UrlEncode(ConfigUtil.realName);

            request.Headers["shopId"] = ConfigUtil.shopId;
            request.Headers["token"] = ConfigUtil.token;

            request.Headers["hostName"] = ConfigUtil.hostName;
            request.Headers["cpu"] = ConfigUtil.cpu;
            request.Headers["mac"] = ConfigUtil.mac;

            request.Headers["bootId"] = ConfigUtil.bootId;
            request.Headers["bootTime"] = ConfigUtil.bootTime;
            request.Headers["systemStartTime"] = ConfigUtil.systemStartTime;

            request.Headers["cv"] = ConfigUtil.cv;
            request.Headers["netversion"] = Environment.Version.ToString();
            request.Headers["osversion"] = Environment.OSVersion.ToString();

            //request.KeepAlive = false;
            request.ContentType = "application/text;";
            request.Method = method;

            return request;
        }

    }


    public class QueryParam
    {
        private Hashtable param = new Hashtable();

        private QueryParam()
        {
        }

        public static QueryParam create(string key, object value)
        {
            return new QueryParam().add(key, value);
        }

        public QueryParam add(string key, object value)
        {
            param.Add(key, value);
            return this;
        }

        public string catQueryString(string url)
        {
            if (!url.EndsWith("?"))
            {
                url = url + "?";
            }

            foreach (string k in param.Keys)
            {
                url = url + k + "=" + param[k].ToString() + "&";
            }

            return url;
        }
    }
}

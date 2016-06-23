using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GMScoreboardV6.Utils
{
    public class LgjInfoUtil
    {
        private static readonly string REPORT_LGJSTATUS = "http://gmaster.youzijie.com/reportV2/lgj/userstatus";

        private static readonly string MEMINFO_64 = @"C:\Windows\SysWOW64\MemberInfo.kv";
        private static readonly string MEMINFO_32 = @"C:\Windows\System32\MemberInfo.kv";
        private static MemoryStream memberInfo;                                 //内存中的会员卡信息

        /// <summary>
        /// 获取会员卡信息
        /// </summary>
        [DllImport("ThirdGame.dll")]
        public static extern int GetMemberInfo();                               //返回1生成kv文件成功

        [DllImport("ThirdGame.dll")]
        public static extern int GetMemberInfo2(byte[] buf, uint len);          //返回1成功

        /// <summary>
        /// 生成龙管家信息文件
        /// </summary>
        public static int genMemberInfo()
        {
            var buf = new byte[1024];
            var retryCount = 3;

            while (retryCount-- > 0)
            {
                try
                {
                    if (GetMemberInfo2(buf, (uint)buf.Length) == 1)
                    {
                        memberInfo = new MemoryStream(buf);
                        return 1;
                    }
                }
                catch (Exception e) { LogUtil.log("GetMemberInfo2 " + e.Message); }
                System.Threading.Thread.Sleep((4 - retryCount) * 1000);
            }

            retryCount = 3;
            while (retryCount-- > 0)
            {
                try
                {
                    if (GetMemberInfo() == 1)
                    {
                        return 1;
                    }
                }
                catch (Exception e) { LogUtil.log("GetMemberInfo " + e.Message); }
                System.Threading.Thread.Sleep((4 - retryCount) * 1000);
            }
            return 0;                                                   //0为失败
        }

        /// <summary>
        /// 读取龙管家信息文件, 返回负数表示失败  非负数表示成功  返回非负数表示变更属性的个数
        /// </summary>
        public static int readLgjInfos()
        {
            // 生成会员卡信息
            StreamReader sr = null;

            try
            {
                string line = "", sBarID = "", serverIP = "", cardNo = "", cardType = "", realName = "";
                FileStream logFile = null;

                if (System.IO.File.Exists(MEMINFO_64))
                {
                    logFile = new FileStream(MEMINFO_64, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                }
                else if (System.IO.File.Exists(MEMINFO_32))
                {
                    logFile = new FileStream(MEMINFO_32, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                }

                if (memberInfo == null && logFile == null)
                {
                    LogUtil.log("get LGJ MemberInfo file is fail!");
                    return -1;
                }

                LogUtil.log("Loaded LGJ MemberInfo file.");
                sr = memberInfo != null ? new StreamReader(memberInfo, Encoding.UTF8)
                    : new StreamReader(logFile, Encoding.UTF8);

                while (line != null)
                {
                    line = sr.ReadLine();
                    if (line != null && line.Split(new char[] { '=' }).Length > 1)
                    {
                        string[] ss = line.Split(new char[] { '=' });
                        if ("sBarID".Equals(ss[0]))
                        {
                            sBarID = ss[1].Replace("\0", "");
                        }
                        else if ("serverIP".Equals(ss[0]))
                        {
                            serverIP = ss[1].Replace("\0", "");
                        }
                        else if ("cardNo".Equals(ss[0]))
                        {
                            cardNo = ss[1].Replace("\0", "");
                        }
                        else if ("cardType".Equals(ss[0]))
                        {
                            cardType = ss[1].Replace("\0", "");
                        }
                        else if ("realName".Equals(ss[0]))
                        {
                            realName = ss[1].Replace("\0", "");
                        }
                    }
                }

                LogUtil.log("Loaded LGJ MemberInfo sBarID:" + sBarID + ", serverIP:" + serverIP + ", cardNo:" + cardNo + ", cardType:" + cardType + ", realName:" + realName);
                if (isEmpty(sBarID) || isEmpty(cardNo) || isEmpty(cardType) || isEmpty(serverIP))
                {
                    return -1;
                }
                else
                {
                    int ret = 0;
                    if (!sBarID.Equals(ConfigUtil.sBarID))
                    {
                        ConfigUtil.sBarID = sBarID;
                        ret++;
                    }
                    if (!serverIP.Equals(ConfigUtil.serverIP))
                    {
                        ConfigUtil.serverIP = serverIP;
                        ret++;
                    }
                    if (!cardNo.Equals(ConfigUtil.cardNo))
                    {
                        ConfigUtil.cardNo = cardNo;
                        ret++;
                    }
                    if (!cardType.Equals(ConfigUtil.cardType))
                    {
                        ConfigUtil.cardType = cardType;
                        ret++;
                    }
                    if (!realName.Equals(ConfigUtil.realName))
                    {
                        ConfigUtil.realName = realName;
                    }
                    if (ret > 0)
                    {
                        string res = HttpClient.post(QueryParam.create("realName", ConfigUtil.realName).add("serverIp", ConfigUtil.serverIP).catQueryString(REPORT_LGJSTATUS), null);
                        LogUtil.log("Upload lgj user status " + res);
                    }
                    return ret;
                }

            }
            catch (Exception e)
            {
                LogUtil.log("Error occurs during getting lgj memberinfo from kv file. " + e.Message);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }

            return -1;
        }

        private static bool isEmpty(string s)
        {
            return s == null || s.Trim().Equals("");
        }

    }
}
using GMScoreboard.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Resources;
using System.Text;

namespace GMScoreboard.Util
{
    public class ConfigUtil
    {
        // 龙管家模块配置文件
        private static readonly string RWY_MODULE_FILE_32 = @"C:\Windows\System32\Rwymoudle.dat";
        private static readonly string RWY_MODULE_FILE_64 = @"C:\Windows\SysWOW64\Rwymoudle.dat";
        private static readonly string RWY_FEEMODULE_FILE_32 = @"C:\Windows\System32\RwyFeemoudle.dat";
        private static readonly string RWY_FEEMODULE_FILE_64 = @"C:\Windows\SysWOW64\RwyFeemoudle.dat";

        private static readonly string CONFIG_FILE = "config.properties";

        // 客户端启动信息
        public static string bootId = new Random(100).Next() + "";
        public static string bootTime = Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0)).TotalSeconds * 1000) + "";
        public static string systemStartTime = Convert.ToInt64((DateTime.Now.AddMilliseconds(0 - Environment.TickCount) - new DateTime(1970, 1, 1, 8, 0, 0)).TotalSeconds * 1000) + "";

        // 机器信息
        public static readonly string hostName = System.Net.Dns.GetHostName();
        public static readonly string mac = getMacByNetworkInterface();
        public static readonly string cpu = getCPUId();

        // 龙管家网吧信息
        public static string sBarID = "";

        // 龙管家用户信息
        public static string cardNo = "";
        public static string serverIP = "";
        public static string cardType = "";
        public static string realName = "";

        // 版本信息
        public static readonly string cv = "5445";
        public static int isDebug = 0;

        // 竞技大师配置信息
        public static string shopId = "";
        public static string token = "";
        public static string agentId = "";

        public static void initial()
        {
            // 读取竞技大师配置
            String configfile = getCurProcessDir() + CONFIG_FILE;
            if (File.Exists(configfile))
            {
                string[] lines = File.ReadAllLines(configfile);
                foreach (string line in lines)
                {
                    LogUtil.log("ConfigUtil.initial readline:" + line);
                    if (line.StartsWith("shopId=", true, null))
                        shopId = line.Substring(7);
                    if (line.StartsWith("token=", true, null))
                        token = line.Substring(6);
                }
            }

            // 读取龙管家sBarID
            if (string.IsNullOrEmpty(sBarID) && File.Exists(RWY_MODULE_FILE_64))
                sBarID = readRWYConfig(RWY_MODULE_FILE_64);
            if (string.IsNullOrEmpty(sBarID) && File.Exists(RWY_MODULE_FILE_32))
                sBarID = readRWYConfig(RWY_MODULE_FILE_32);
            if (string.IsNullOrEmpty(sBarID) && File.Exists(RWY_FEEMODULE_FILE_64))
                sBarID = readRWYConfig(RWY_FEEMODULE_FILE_64);
            if (string.IsNullOrEmpty(sBarID) && File.Exists(RWY_FEEMODULE_FILE_32))
                sBarID = readRWYConfig(RWY_FEEMODULE_FILE_32);

            // 如果没有配置文件 或者 是龙管家的网吧 都调用DLL获取龙管家完整信息
            if (string.IsNullOrEmpty(shopId) || !string.IsNullOrEmpty(sBarID))
            {
                LgjInfoUtil.genMemberInfo();
                LgjInfoUtil.readLgjInfos();
            }
        }

        public static string readRWYConfig(string file)
        {
            foreach (string line in File.ReadAllLines(file))
            {
                if (line.StartsWith("sBarID=", true, null))
                    return line.Substring(7);
            }
            return "";
        }

        public static string getCurProcessDir()
        {
            string path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            int index = path.LastIndexOf("\\");
            path = path.Substring(0, index);
            return path + "\\";
        }

        private static string getMacByNetworkInterface()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                string mac = ni.GetPhysicalAddress().ToString();
                mac.Replace(":", "");
                return mac;
            }
            return "unknownmac";
        }

        private static string getCPUId()
        {
            string cpuInfo = "";
            try
            {
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                    return cpuInfo.ToString();
                }
            }
            catch (Exception e)
            {
                LogUtil.log("Error occurs during getCPUId", e);
            }

            return "unknowncpu";
        }

        public string getOSVersion()
        {
            Version ver = System.Environment.OSVersion.Version;
            string strClient = "";
            if (ver.Major == 5 && ver.Minor == 1)
            {
                strClient = "Win XP";
            }
            else if (ver.Major == 6 && ver.Minor == 0)
            {
                strClient = "Win Vista";
            }
            else if (ver.Major == 6 && ver.Minor == 1)
            {
                strClient = "Win 7";
            }
            else if (ver.Major == 5 && ver.Minor == 0)
            {
                strClient = "Win 2000";
            }
            else
            {
                strClient = "未知";
            }
            return strClient;
        }

        public static string getCurrentExeName()
        {
            string file = Process.GetCurrentProcess().MainModule.FileName;
            return file.Substring(file.LastIndexOf('\\') + 1).Replace(".exe", "").Replace(".EXE", "");
        }

        public static string UrlEncode(string str)
        {
            if (str == null || str.Trim().Length == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }
    }
}

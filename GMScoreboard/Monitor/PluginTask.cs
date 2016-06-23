using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using GMScoreboard.Util;
using GMScoreboard.Properties;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Management;
using GMScoreboard.Model;
using System.Collections;
using System.Text.RegularExpressions;

namespace GMScoreboard.Monitor
{
    /// <summary>
    /// GMData 插件类 用于启动GMdATA并监控日志的输出
    /// </summary>
    public class PluginTask
    {
        private string paths = "";

        [DllImport("kernel32.dll")]
        public static extern int WinExec(string exeName, int operType);

        public PluginTask(string paths)
        {
            this.paths = paths;
        }

        public void preparePlugins()
        {
            LogUtil.log("PluginTask.preparePlugins is run!");

            string[] ps = paths.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string p in ps)
            {
                try
                {
                    if (Directory.Exists(p))
                    {
                        LogUtil.log("PluginTask.preparePlugins:" + p);

                        string airdll = p + "AirAIR.dll";
                        string gmdata = p + "gmdata.exe";

                        // 写 AirAIR.dll
                        try
                        {
                            if (File.Exists(airdll))
                            {
                                new FileInfo(airdll).IsReadOnly = false;
                                File.Delete(airdll);
                            }
                            File.WriteAllBytes(airdll, Resources.AirAIR);
                            new FileInfo(airdll).Attributes = FileAttributes.ReadOnly
                                | FileAttributes.System | FileAttributes.Hidden;
                            LogUtil.log("PluginTask success write airdll in " + airdll);
                        }
                        catch (Exception e)
                        {
                            LogUtil.log("Error occurs during PluginTask  write airdll in "+airdll , e);
                        }

                        // 写 gmdata.exe
                        try
                        {
                            if (File.Exists(gmdata))
                            {
                                new FileInfo(gmdata).IsReadOnly = false;
                                File.Delete(gmdata);
                            }
                            File.WriteAllBytes(gmdata, Resources.gmdata);
                            new FileInfo(gmdata).Attributes = FileAttributes.ReadOnly
                                | FileAttributes.System | FileAttributes.Hidden;
                            startGMData(gmdata);    //运行gmdata
                            //Thread.Sleep(3000);
                            //LogUtil.log("gmdata.exe is run? ret:" + (Process.GetProcessesByName("gmdata").Length > 0));
                            LogUtil.log("PluginTask success write and run gmdata in " + gmdata);
                        }
                        catch (Exception e)
                        {
                            LogUtil.log("Error occurs during PluginTask write gmdata in " + gmdata, e);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogUtil.log("Error occurs during PluginTask. " + paths, e);
                }
            }
        }

        public static void startGMData(string path)
        {
            try
            {
                if (Process.GetProcessesByName("gmdata").Length > 0)
                    return;

                int ptr = WinExec(path, 0);
                LogUtil.log("Error occurs during gmdata run ret:" + ptr);
            }
            catch (Exception e)
            {
                LogUtil.log("Error occurs during run gmdata. " + e.Message);
            }
        }
    }
}
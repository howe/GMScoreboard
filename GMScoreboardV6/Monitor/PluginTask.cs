using GMScoreboardV6.Properties;
using GMScoreboardV6.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace GMScoreboardV6.Monitor
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
                            File.WriteAllBytes(airdll, Resources.airair);
                            new FileInfo(airdll).Attributes = FileAttributes.ReadOnly
                                | FileAttributes.System | FileAttributes.Hidden;
                            LogUtil.log("PluginTask success write airdll in " + airdll);
                        }
                        catch (Exception e)
                        {
                            LogUtil.log("Error occurs during PluginTask  write airdll in " + airdll, e);
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
                LogUtil.log("gmdata run success ret:" + ptr);
            }
            catch (Exception e)
            {
                LogUtil.log("Error occurs during run gmdata. " + e.Message);
            }
        }
    }
}
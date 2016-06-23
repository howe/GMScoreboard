using GMScoreboard.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace TestAMF3
{

    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            int seq = 1;
            string logDir = @"C:\110\";
            while (true)
            {
                if (System.IO.File.Exists(logDir + seq + ".dat"))
                {
                    Thread.Sleep(500);  // 等待文件完全生成
                    try
                    {
                        // AMF3解码
                        var dat = File.ReadAllBytes(logDir + seq + ".dat");
                        var buf = new List<byte>();
                        buf.AddRange(dat);
                        var len = buf.Count;
                        var amf3 = new AMF3();
                        buf = amf3.封包处理(buf);
                        amf3.置数据(buf);
                        var data = amf3.read_elem();
                        string json = amf3.变体型转换文本(data);
                        if (json.Contains("previousDayLeaguePosition"))
                        {

                            string[] sArray = Regex.Split(json, "previousDayLeaguePosition", RegexOptions.IgnoreCase);
                            foreach (string s in sArray)
                            {
                                if (s.Contains("\"tier\"") && !s.Contains("TEAM-"))
                                {
                                    json = s;
                                    break;
                                }
                            }
                        }
                        File.WriteAllText(logDir + seq + ".txt", json);
                            
                    }
                    catch (Exception e)
                    {
                    }

                    // 空读次数清零并递增序号
                    seq++;


                    

                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        static string airDir = "";
        static void findAirDir(string dir)
        {
            if (!string.IsNullOrEmpty(airDir))
                return;

            try
            {
                foreach(string d in Directory.GetDirectories(dir))
                {
                    if(d.Split(new char[] { '\\' }).Length > 6)
                    {
                        break;
                    }
                    else if("Air".Equals(d) && "英雄联盟".Equals(dir))
                    {
                        airDir = d;
                        break;
                    }
                    else
                    {
                        findAirDir(dir);
                    }
                }
            }
            catch(Exception e)
            {

            }
        }
    }
}

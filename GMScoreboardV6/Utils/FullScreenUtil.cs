using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace GMScoreboardV6.Utils
{
    public class FullScreenUtil
    {
        [StructLayout(LayoutKind.Sequential)] 
        public struct RECT 
        {
            public int Left;    
            public int Top;    
            public int Right;    
            public int Bottom; 
        } 
         
        //取得前台窗口句柄函数 
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow(); 

        //取得桌面窗口句柄函数 
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow(); 

        //取得Shell窗口句柄函数 
        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow(); 

        //取得窗口大小函数 
        [DllImport("user32.dll", SetLastError = true)] 
        private static extern int GetWindowRect(IntPtr hwnd, out RECT rc); 
        
        public static bool isFullScreen()
        {
            // 桌面窗口 Shell窗口句柄 当前窗口
            IntPtr desktopHandle = GetDesktopWindow();
            IntPtr shellHandle = GetShellWindow();
            IntPtr hWnd = GetForegroundWindow(); 
 
            // 取得前台窗口句柄并判断是否全屏 
            RECT appBounds;
            Rectangle screenBounds;

            if (hWnd != null && !hWnd.Equals(IntPtr.Zero) && !hWnd.Equals(desktopHandle) && !hWnd.Equals(shellHandle)) 
            {
                // 取得窗口大小 
                GetWindowRect(hWnd, out appBounds); 

                // 判断是否全屏
                screenBounds = Screen.FromHandle(hWnd).Bounds; 
                return (appBounds.Bottom - appBounds.Top) == screenBounds.Height && (appBounds.Right - appBounds.Left) == screenBounds.Width;

            }

            return false;
        }
        
    }

    public class WinUtil
    {

        /// <summary>
        /// 设置注册表
        /// </summary>
        public static void setRegKeyAndValue(RegistryKey root, string key, string name, object value)
        {
            RegistryKey reg = null;
            try
            {
                reg = root.OpenSubKey(key, true);
                if (reg == null)
                {
                    reg = Registry.LocalMachine.CreateSubKey(key);
                }
                reg.SetValue(name, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (reg != null)
                    reg.Close();
            }
        }

        /// <summary>
        /// 清除文件夹
        /// </summary>
        /// <param name="path">文件夹路径</param>
        static void FolderClear(string path)
        {
            System.IO.DirectoryInfo diPath = new System.IO.DirectoryInfo(path);
            foreach (System.IO.FileInfo fiCurrFile in diPath.GetFiles())
            {
                try
                {
                    System.IO.File.Delete(fiCurrFile.FullName);
                }
                catch (Exception e)
                {
                    LogUtil.log("Error occurs during delete cache files.", e);
                }
            }
            foreach (System.IO.DirectoryInfo diSubFolder in diPath.GetDirectories())
            {
                FolderClear(diSubFolder.FullName); // Call recursively for all subfolders
            }
        }

        /// <summary>
        /// 执行命令行
        /// </summary>
        /// <param name="cmd"></param>
        static void RunCmd(string cmd)
        {
            ProcessStartInfo p = new ProcessStartInfo();
            p.FileName = "cmd.exe";
            p.Arguments = "/c " + cmd;
            p.WindowStyle = ProcessWindowStyle.Hidden;  // Use a hidden window
            Process.Start(p);
        }

        /// <summary>
        /// 删除IE临时文件
        /// </summary>
        public static void CleanIETempFiles()
        {
            try
            {
                FolderClear(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
            }
            catch (Exception e1)
            {
                // ignore
            }
                
            try
            {
                RunCmd("RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 8");
            }
            catch (Exception e2)
            {
                // ignore
            }
        }

    }
}

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace GMScoreboardV6.Utils
{
    static class WinApi
    {
        public static IntPtr GetWindowHandle(Window window)
        {
            return new WindowInteropHelper(window).Handle;
        }

        public static int GetChildCount(IntPtr handle)
        {
            var wnd = IntPtr.Zero;
            var count = 0;

            while ((wnd = FindWindowEx(handle, wnd, null, null)) != IntPtr.Zero)
                count++;

            return count;
        }

        [DllImport("user32")]
        public static extern IntPtr SetParent(IntPtr childWnd, IntPtr parentWnd);

        [DllImport("user32")]
        public static extern IntPtr FindWindow(string className, string windowName);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr parentWnd, IntPtr afterWnd, string className, string windowName);

       public static void SuppressScriptErrors(System.Windows.Controls.WebBrowser webBrowser, bool hide)
        {
            try
            {
                webBrowser.Navigating += (s, e) =>
                {
                    var fiComWebBrowser = typeof(System.Windows.Controls.WebBrowser).GetField("_axIWebBrowser2", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    if (fiComWebBrowser == null)
                        return;

                    object objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
                    if (objComWebBrowser == null)
                        return;

                    objComWebBrowser.GetType().InvokeMember("Silent", System.Reflection.BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
                };
            }
            catch (Exception exception)
            {
                LogUtil.log("Exception WinApi.SuppressScriptErrors ", exception);
            }
        }
    }
}
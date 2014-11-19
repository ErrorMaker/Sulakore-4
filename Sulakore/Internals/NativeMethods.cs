using System;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Sulakore
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hwnd, uint msg, int wparam, IntPtr lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        private static readonly RegistryKey ProxyRegistry;
        private static bool settingsReturn, refreshReturn;
        private const string ProxyServerFormat = "127.0.0.1:{0}";

        static NativeMethods()
        {
            ProxyRegistry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
        }

        public static void DisableProxy()
        {
            ProxyRegistry.SetValue("ProxyEnable", 0);
            ProxyRegistry.SetValue("ProxyOverride", "<-loopback>");
            RefreshIESettings();
        }
        public static void EnableProxy(int port)
        {
            string proxyServer = string.Format(ProxyServerFormat, port);
            ProxyRegistry.SetValue("ProxyServer", proxyServer);

            ProxyRegistry.SetValue("ProxyEnable", 1);
            ProxyRegistry.SetValue("ProxyOverride", "<-loopback>;<local>");
            RefreshIESettings();
        }

        private static void RefreshIESettings()
        {
            settingsReturn = InternetSetOption(IntPtr.Zero, 39, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, 37, IntPtr.Zero, 0);
        }
    }
}
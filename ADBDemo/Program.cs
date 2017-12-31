using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ADBDemo
{
    static class Program
    {
        internal static string AdbPath = @"C:\Android\SDK\platform-tools\adb.exe";
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ADBJump
{
    public class EventArgsOutput : EventArgs
    {
        public string Output;
    }

    public enum OutputStatus { NotRun, Busy, Success, Error };   //cmd状态

    class ADBHelper : IDisposable
    {
        internal string AdbPath = @"C:\Android\SDK\platform-tools\adb.exe";
        private Process CmdProcess = null;
        
        public string OutputData = string.Empty;               //最后一个正确的输出结果
        public string OutputError = string.Empty;              //最后一个错误的输出结果
        public OutputStatus CmdStatus = OutputStatus.NotRun;   //最后一个命令的运行状态

        public byte[] bytesOutputfixed;

        HiPerfTimer timer = null;
        public event EventHandler Output;

        public ADBHelper()
        {
            timer = new HiPerfTimer();
            //https://msdn.microsoft.com/zh-cn/library/system.diagnostics.process.beginoutputreadline(v=vs.110).aspx
            //http://blog.csdn.net/hcj116/article/details/7772262
            /*
            CmdProcess = new Process();
            CmdProcess.StartInfo.FileName = "cmd.exe";           //启动cmd
            CmdProcess.StartInfo.UseShellExecute = false;        //自定义shell
            CmdProcess.StartInfo.RedirectStandardInput = true;   //重定向标准输入   
            CmdProcess.StartInfo.RedirectStandardOutput = true;  //重定向标准输出   
            CmdProcess.StartInfo.RedirectStandardError = true;   //重定向错误输出   
            CmdProcess.StartInfo.CreateNoWindow = true;          //不显示原始窗口
            CmdProcess.OutputDataReceived += CmdProcess_ProcessOutDataReceived;
            CmdProcess.ErrorDataReceived += CmdProcess_ErrorDataReceived;
            CmdProcess.Start();
            cmdinput = CmdProcess.StandardInput;                 //重定向输入
            CmdProcess.BeginOutputReadLine();                    //开始读取输出信息。这是个关键的步骤，否则不触发任何输出事件
            CmdProcess.BeginErrorReadLine();
            //CmdProcess.WaitForExit();
            */
        }
        /*
        internal void CmdProcess_ProcessOutDataReceived(object sender, DataReceivedEventArgs e)
        {
            OutputData = e.Data;
            CmdStatus = OutputStatus.Success;
            if (Output!=null)
            {
                Output(null, new EventArgsOutput(){ Output = OutputData });
            }
            cmdIsBusy = false;
        }
        private void CmdProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            OutputError = e.Data;
            CmdStatus = OutputStatus.Error;
            
            if (Output != null)
            {
                Output(null, new EventArgsOutput() { Output = OutputData });
            }
            cmdIsBusy = false;
        }
        internal void WaitForFinished()
        {
            while(true)
            {
                if (cmdIsBusy)
                    Thread.Sleep(30);
                else
                    break;
            }
        }
        */

        #region Dispose
        public void Dispose()
        {
            this.Dispose(true);////释放托管资源
            GC.SuppressFinalize(this);//请求系统不要调用指定对象的终结器. //该方法在对象头中设置一个位，系统在调用终结器时将检查这个位
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)                                   //_isDisposed为false表示没有进行手动dispose
            {
                if (disposing)
                {
                    //清理托管资源
                    if (CmdProcess != null)
                    {
                        CmdProcess.Close();
                    }
                }
                //清理非托管资源
            }
            _isDisposed = true;
        }

        private bool _isDisposed;

        ~ADBHelper()
        {
            this.Dispose(false);//释放非托管资源，托管资源由终极器自己完成了
        }
        #endregion Dispose        

        private void runADBShell(string shellcommand)
        {
            CmdStatus = OutputStatus.Busy;
            OutputData = string.Empty;
            bytesOutputfixed = null;
            timer.Start();
            using (Process p = new Process())
            {
                p.StartInfo.FileName = AdbPath;             // @"C:\Android\sdk\platform-tools\adb.exe";
                p.StartInfo.Arguments = shellcommand;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;   //重定向标准输入   
                p.StartInfo.RedirectStandardOutput = true;  //重定向标准输出   
                p.StartInfo.RedirectStandardError = true;   //重定向错误输出   
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                //https://www.cnblogs.com/jackdong/archive/2011/03/31/2000740.html
                //OutputData = p.StandardOutput.ReadToEnd();  //这一句执行之后，BaseStream就被读取，无法再读。
                MemoryStream OutputStream = new MemoryStream();
                p.StandardOutput.BaseStream.CopyTo(OutputStream);
                OutputStream.Position = 0;
         
                bytesOutputfixed = Fix0d0d0a(OutputStream.ToArray());
                p.WaitForExit();
                p.Close();
            }
            timer.Stop();
            if (shellcommand.Contains("shell screencap -p"))
            {
                //System.IO.File.WriteAllBytes(@"D:\1.png", bytes);
                //System.IO.File.WriteAllBytes(@"D:\1fix.png", bytesfix);
            }
            else
                OutputData = System.Text.Encoding.ASCII.GetString(bytesOutputfixed).Replace((char)0x0a,' ');

            if (string.IsNullOrEmpty(OutputData)) OutputData = shellcommand + " ";
            if (Output != null)
            {
                Output(null, new EventArgsOutput() { Output = OutputData + "--> " + timer.Duration.ToString() + "\r\n" });
            }
            CmdStatus = OutputStatus.Success;
        }
        private byte[] Fix0d0d0a(byte[] bytes)
        {
            long length = bytes.Length;
            byte[] bytesfix = new byte[length];

            int idx = 0;
            int count = 0;
            for (int i = 0; i < length; i++)
            {
                byte b = bytes[i];
                if (i > 1 && b == 0x0a)
                {
                    if (bytes[i - 1] == 0x0d && bytes[i - 2] == 0x0d)
                    {
                        count++;
                        idx = idx - 2;
                        bytesfix[idx] = b;
                        idx++;
                    }
                }
                else
                {
                    bytesfix[idx] = b;
                    idx++;
                }               
            }
            byte[] bytesfinal = new byte[length-count*2];
            Buffer.BlockCopy(bytesfix, 0, bytesfinal, 0, bytesfinal.Length);            
            return bytesfinal;
        }
        /// <summary>
        /// 获取手机型号。默认为同步运行，isAsyn=false
        /// </summary>
        internal void DetectAndriod()
        {
            runADBShell("shell getprop ro.product.model");            
        }
        /// <summary>
        /// 获取手机分辨率。默认为同步运行，isAsyn=false
        /// </summary>
        internal void DetectAndriodScreenSize()
        {
            runADBShell("shell wm size");
        }

        internal void RunADBShellCommand(string shellcommandd)
        {
            runADBShell(shellcommandd);
        }
    }
}

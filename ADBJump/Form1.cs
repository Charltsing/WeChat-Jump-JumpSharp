using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

//http://www.cnblogs.com/dotnet-org-cn/p/8149693.html
namespace ADBJump
{
    public partial class Form1 : Form
    {
        ADBHelper ADB;

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            string txt = "辅助跳一跳: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            txt += "    故障判断：看调试窗口，1、手机型号，2、是否启动截图及返回数据大小";
            this.Text = txt;
            //Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void ADB_Output(object sender, EventArgs e)
        {
            if (rtbCmd == null) return;
            EventArgsOutput Data = e as EventArgsOutput;
            string output = Data.Output;
            if (string.IsNullOrEmpty(output)) return;
            Invoke(new MethodInvoker(delegate ()
            {                
                {
                    rtbCmd.AppendText(output);
                    rtbCmd.ScrollToCaret();
                }
            }));
        }

        int StartX = 0;
        int StartY = 0;

        int ResolutionX = 0;
        int ResolutionY = 0;
        double ResolutionXScale = 0;
        double ResolutionYScale = 0;

        bool HasAndroid = false;
        bool isBusy = false;
        bool isWaiting = false;
        int MaxCaptureCount = 5;
        int capturecount = 0;

        /// <summary>
        /// 设备后插入延时执行
        /// </summary>
        private System.Windows.Forms.Timer DeviceTimer = new System.Windows.Forms.Timer();

        private System.Windows.Forms.Timer CaptureTimer = new System.Windows.Forms.Timer();

        private void Form1_Load(object sender, EventArgs e)
        {
            DeviceTimer.Interval = 2000;                                                     //只需要执行一次
            DeviceTimer.Tick += DeviceTimer_Tick;

            CaptureTimer.Interval = 50;
            CaptureTimer.Tick += CaptureTimer_Tick;            

            ADB = new ADBHelper();
            ADB.Output += ADB_Output;

            HasAndroid = false;
            CheckHasAndroidModel();
            capturecount = 0;
            
        }

        private void DeviceTimer_Tick(object sender, EventArgs e)
        {
            DeviceTimer.Stop();
            DeviceTimer.Enabled = false;
            CheckHasAndroidModel();
        }

        private void CaptureTimer_Tick(object sender, EventArgs e)
        {
            if (!HasAndroid || isWaiting || isBusy) return;
            if (capturecount > MaxCaptureCount) return;
            isBusy = true;
            capturecount++;
            try
            {                
                CaptureAndriod();
            }
            catch { }
            isBusy = false;
        }

        private void CaptureAndriod()
        {
            //截屏获取图片
            ADB.RunADBShellCommand("shell screencap -p");
            if (ADB.bytesOutputfixed != null)
            {
                using (MemoryStream pngStream = new MemoryStream(ADB.bytesOutputfixed))
                {
                    HiPerfTimer timer = new HiPerfTimer();
                    timer.Start();
                    Bitmap bmpfix = DetectStartPoint(pngStream);                    
                    timer.Stop();
                    if (bmpfix != null)
                    {
                        string tmp = "Detect:" + timer.Duration.ToString() + "\r\n";
                        Invoke(new MethodInvoker(delegate ()
                        {
                            {
                                if (rtbCmd != null)
                                {
                                    rtbCmd.AppendText(tmp);
                                    rtbCmd.ScrollToCaret();
                                }
                            }
                        }));
                        pictureBox1.Invoke(new Action(() =>
                        {
                            pictureBox1.Image = bmpfix;
                            pictureBox1.Refresh();
                        }));
                    }
                }
            }
        }
        private Bitmap DetectStartPoint(MemoryStream pngStream)
        {
            if (!HasAndroid ) return null;
            Bitmap bmpfix=null;
            using (var img = Image.FromStream(pngStream))
            {
                Bitmap bmp = new Bitmap(img);
                CAPTCHA c = new CAPTCHA(bmp);
                StartX = (int)(c.DetectX / ResolutionXScale)-8;
                StartY = (int)(c.DetectY / ResolutionYScale)+6;
                string xyText = "(" + StartX.ToString() + "," + StartY.ToString() + ")";

                bmpfix = new Bitmap(c.bmp).Clone() as Bitmap;
                using (Graphics g = Graphics.FromImage(bmpfix))
                {
                    using (Font font = new Font("Arial", 36))
                    {
                        g.DrawString(xyText, font, Brushes.Red, new PointF(10, 30));
                    }
                }
            }
            return bmpfix;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x219)
            {
                Debug.WriteLine("WParam：{0} ,LParam:{1},Msg：{2}，Result：{3}", m.WParam, m.LParam, m.Msg, m.Result);
                if (m.WParam.ToInt32() == 7)                                               //设备插入或拔出
                {
                    if (!HasAndroid)
                        DeviceTimer.Start();
                    else
                        Close();
                }
            }
            try
            {
                base.WndProc(ref m);
            }
            catch { }
        }
        /// <summary>
        /// 检测是否存在手机
        /// </summary>
        private void CheckHasAndroidModel()
        {
            ADB.DetectAndriod();   //获取手机型号

            if (ADB.CmdStatus == OutputStatus.Success)
            {
                string text = ADB.OutputData;

                if (text.Contains("no devices") || string.IsNullOrWhiteSpace(text))
                {
                    HasAndroid = false;
                    toolStripStatusLabel2.Text = "未检测到设备";
                    Invoke(new MethodInvoker(delegate ()
                    {
                        {
                            if (rtbCmd != null)
                            {
                                rtbCmd.AppendText("Detect Andriod device failed!\r\n1、请在cmd运行adb shell getprop ro.product.model 检测手机USB调试模式是否正常！\r\n2、请在cmd运行adb shell screencap -p 查看是否返回数据。\r\n\r\n");
                                rtbCmd.ScrollToCaret();
                            }
                        }
                    }));
                }
                else
                {
                    HasAndroid = true;
                    DetectSize();
                    toolStripStatusLabel2.Text = text.Trim() + "(" + ResolutionX.ToString() + "x" + ResolutionY.ToString() + ")" +
                                                 " ," + ResolutionXScale.ToString() + "x" + ResolutionYScale.ToString() + "";
                    Invoke(new MethodInvoker(delegate ()
                    {
                        {
                            if (rtbCmd != null)
                            {
                                rtbCmd.AppendText("Detect Andriod device success.\r\n\r\n***** Start capture Andriod Screen *****\r\n\r\n");
                                rtbCmd.ScrollToCaret();
                            }
                        }
                    }));
                    CaptureTimer.Start();
                }
            }
            else
            {
                toolStripStatusLabel2.Text = "cmd error: " + ADB.OutputError ;
            }
        }
        private void DetectSize()
        {
            ADB.DetectAndriodScreenSize();                                  //获取手机分辨率
            if (ADB.CmdStatus == OutputStatus.Success)
            {
                string output = ADB.OutputData;
                string[] Resolution = output.Split('x');
                ResolutionX = GetNumberInt(Resolution[0]);
                ResolutionY = GetNumberInt(Resolution[1]);
                ResolutionXScale = (double)ResolutionX / pictureBox1.Width;
                ResolutionYScale = (double)ResolutionY / pictureBox1.Height;
            }
            else
            {
                toolStripStatusLabel2.Text = "cmd error: " + ADB.OutputError;
            }
        }       
        

        private void button1_Click(object sender, EventArgs e)
        {
            ADB.RunADBShellCommand("shell input keyevent  1 ");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ADB.RunADBShellCommand("shell input keyevent  3 ");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ADB.RunADBShellCommand("shell input keyevent 4 ");
        }        

        /// <summary>
        /// 黑人底部位置
        /// </summary>
        Point Start;
        /// <summary>
        /// 图案中心或者白点位置
        /// </summary>
        Point End;
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            capturecount = 0;
            var me = ((System.Windows.Forms.MouseEventArgs)(e));
            if (me.Button==MouseButtons.Left)              //按下左键是黑人底部坐标
            {
                Start = ((System.Windows.Forms.MouseEventArgs)(e)).Location;
            }
            else if (me.Button == MouseButtons.Right)      //按下右键键是终点坐标
            {
                if (Start.X==0)
                {
                    Start = new Point(StartX, StartY);
                }
                End = ((System.Windows.Forms.MouseEventArgs)(e)).Location;
                //计算两点直接的距离
                double value = Math.Sqrt(Math.Abs(Start.X - End.X) * Math.Abs(Start.X - End.X) + Math.Abs(Start.Y - End.Y) * Math.Abs(Start.Y - End.Y));
                Text = string.Format("距离：{0}，时间：{1}", value.ToString("0.00"), (3.999022243950134 * value).ToString("0.000"));
                //3.999022243950134  这个是我通过多次模拟后得到 我这个分辨率的最佳时间
                ADB.RunADBShellCommand(string.Format("shell input swipe 100 100 200 200 {0}", (3.999022243950134 * value).ToString("0")));
                
                Cursor = Cursors.WaitCursor;
                int delay = 2500;
                bool ret = int.TryParse(textBox1.Text, out delay);
                if (!ret)
                {
                    textBox1.Text="1200";
                }
                if (delay < 500) delay = 500;
                if (delay > 5000) delay = 5000;
                isWaiting = true;
                Thread.Sleep(delay);
                Cursor = Cursors.Default;
                isWaiting = false;                
                
                Start = new Point(0, 0);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                return;
            }
            if (e.KeyChar == 8)
            {
                e.Handled = false;
                return;
            }
            if (e.KeyChar < '0' || e.KeyChar > '9')
            {
                e.Handled = true;
                return;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            rtbCmd = null;
            CaptureTimer.Tick-= CaptureTimer_Tick;
            CaptureTimer.Stop();            
            ADB.Output -= ADB_Output;            
            CaptureTimer.Dispose();

            //杀掉adb进程
            var process = Process.GetProcesses().Where(pr => pr.ProcessName=="adb");
            foreach (var pk in process)
            {
                try
                {
                    pk.Kill();
                }
                catch
                { }
            }
        }

        public static int GetNumberInt(string str)
        {
            int result = 0;
            if (str != null && str != string.Empty)
            {
                // 正则表达式剔除非数字字符（不包含小数点.）
                str = Regex.Replace(str, @"[^\d.\d]", "");
                // 如果是数字，则转换为decimal类型
                if (Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$"))
                {
                    result = int.Parse(str);
                }
            }
            return result;
        }

        private void rtbCmd_MouseClick(object sender, MouseEventArgs e)
        {
            capturecount = 0;
        }
    }
}

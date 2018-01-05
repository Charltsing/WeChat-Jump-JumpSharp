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
            txt += "    故障判断：看调试窗口，1、手机型号及分辨率，2、是否启动截图及返回数据大小";
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
        bool isPause = false;
        string AndriodReleaseVer = string.Empty;

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
            capturecount = 0;

            ADB = new ADBHelper();
            ADB.Output += ADB_Output;

            HasAndroid = false;
            bool ret = ADB.CheckADB();
            if (ret)
            {
                CheckHasAndroidModel();
            }
            else
            {
                toolStripStatusLabel2.Text = "未找到adb文件！";
            }
            this.rtbCmd.Text = "操作说明：\r\n1、右键选目标点，自动跳。\r\n2、如果小黑人下面没有红点，先用左键点小黑人脚下再按右键。\r\n3、默认五次抓屏后停止，如果屏幕不同步请在调试窗口点一下鼠标左键。\r\n\r\n重要说明：\r\n  辅助工具只管跳，别的不负责。但是如果你不停歇连续跳得太多，或者右键跳得太准，或者与前一次成绩相差分数太大，腾讯会删除这个分数。所以，要考虑跳一会歇一会，不要连续跳中心点，也不要一次超上次得分太多。\r\n  遇到极为特殊的方块，该掉下就掉下去吧，别和腾讯对着干。\r\n\r\n";
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
            if (capturecount > MaxCaptureCount)
            {
                if (!isPause)
                {
                    Invoke(new MethodInvoker(delegate ()
                    {
                        {
                            if (rtbCmd != null)
                            {
                                rtbCmd.AppendText("\r\nCapture paused, wait for mouse click... \r\n\r\n");
                                rtbCmd.ScrollToCaret();
                            }
                        }
                    }));
                    isPause = true;
                    Application.DoEvents();
                }
                return;
            }
            isBusy = true;
            capturecount++;
            try
            {                
                CaptureAndriod();
            }
            catch
            { }
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
                    Bitmap bmpfix = null;
                    try
                    {
                        bmpfix = DetectStartPoint(pngStream);
                        //为什么有的手机截屏画面上下颠倒呢？？？
                        if (cbxRotate180.Checked) bmpfix.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        using (Graphics g = Graphics.FromImage(bmpfix))
                        {
                            using (Font font = new Font("Times New Roman", 36))
                            {
                                string xyText = "(" + StartX.ToString() + "," + StartY.ToString() + ")";
                                g.DrawString(xyText, font, Brushes.Red, new PointF(10, 30));
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Invoke(new MethodInvoker(delegate ()
                        {
                            {
                                if (rtbCmd != null)
                                {
                                    rtbCmd.AppendText(ex.ToString());
                                    rtbCmd.ScrollToCaret();
                                }
                            }
                        }));
                    }
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
                    else
                    {
                        Invoke(new MethodInvoker(delegate ()
                        {
                            {
                                if (rtbCmd != null)
                                {
                                    rtbCmd.AppendText("bmpfix is null!");
                                    rtbCmd.ScrollToCaret();
                                }
                            }
                        }));
                    }
                }
            }
            else
            {
                Invoke(new MethodInvoker(delegate ()
                {
                    {
                        if (rtbCmd != null)
                        {
                            rtbCmd.AppendText("bytesOutputfixed is null!");
                            rtbCmd.ScrollToCaret();
                        }
                    }
                }));
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

                bmpfix = new Bitmap(c.bmp).Clone() as Bitmap;
                
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
                string Phone = ADB.OutputData;

                if (Phone.Contains("no devices") || string.IsNullOrWhiteSpace(Phone))
                {
                    HasAndroid = false;
                    toolStripStatusLabel2.Text = "未检测到设备";
                    Invoke(new MethodInvoker(delegate ()
                    {
                        {
                            if (rtbCmd != null)
                            {
                                rtbCmd.AppendText("Detect Andriod device failed!\r\n1、请在cmd运行adb shell getprop ro.product.model 检测手机USB调试模式是否正常！\r\n2、请在cmd运行adb shell screencap -p 查看是否有返回数据。\r\n\r\n");
                                rtbCmd.ScrollToCaret();
                            }
                        }
                    }));
                }
                else
                {
                    DetectVer();
                    if (AndriodReleaseVer.StartsWith("4.0")) cbxRotate180.Checked = true;
                    DetectSize();
                    if (ResolutionX == 0 || ResolutionY == 0)
                    {
                        toolStripStatusLabel2.Text = Phone.Trim() + "(检测手机屏幕分辨率失败)";
                    }
                    else
                    {
                        HasAndroid = true;
                        string msg = Phone.Trim() + " Ver:" + AndriodReleaseVer + "(" + ResolutionX.ToString() + "x" + ResolutionY.ToString() + "), screen scale: " +
                                     ResolutionXScale.ToString() + "x" + ResolutionYScale.ToString();
                        toolStripStatusLabel2.Text = msg.Replace("\r", "");
                        rtbCmd.AppendText("Detect Andriod device success.\r\n\r\n***** Start capture Andriod Screen *****\r\n\r\n");
                        rtbCmd.ScrollToCaret();

                        CaptureTimer.Start();
                    }
                }
            }
            else
            {
                toolStripStatusLabel2.Text = "cmd error: " + ADB.OutputError ;
            }
        }
        private void DetectVer()
        {
            ADB.DetectAndriodReleaseVer();
            if (ADB.CmdStatus == OutputStatus.Success)
            {
                AndriodReleaseVer = ADB.OutputData;
            }
        }
        private void DetectSize()
        {
            ResolutionX = 0;
            ResolutionY = 0;
            ResolutionXScale = 0;
            ResolutionYScale = 0;

            ADB.DetectAndriodScreenSize();                                  //获取手机分辨率
            if (ADB.CmdStatus == OutputStatus.Success)
            {
                string output = ADB.OutputData;
                if (output.Contains("size"))
                {
                    int idx = output.IndexOf("Override size");
                    if (idx < 0) idx = output.IndexOf("Physical size");
                    if (idx >= 0)
                    {
                        output = output.Substring(idx + 15, output.Length - idx - 17);
                        string[] Resolution = output.Split('x');
                        ResolutionX = GetNumberInt(Resolution[0]);
                        ResolutionY = GetNumberInt(Resolution[1]);
                        ResolutionXScale = (double)ResolutionX / pictureBox1.Width;
                        ResolutionYScale = (double)ResolutionY / pictureBox1.Height;
                    }
                }
            }
            //有的手机不能用wm size，换一种方式
            if (ResolutionX == 0 || ResolutionY == 0)
            {
                ADB.DetectAndriodWindow();
                if (ADB.CmdStatus == OutputStatus.Success)
                {
                    string output = ADB.OutputData;
                    if (output.Contains("Display"))
                    {
                        //Display: init=540x960 base=540x960 cur=540x960 app=540x960 raw=540x960
                        //app是不是当前游戏的分辨率呢？？？
                        int idx = output.IndexOf("app=");
                        if (idx >= 0)
                        {
                            output = output.Substring(idx);
                            idx = output.IndexOf(" ");
                            output = output.Substring(4, idx - 4);  //去掉app=

                            string[] Resolution = output.Split('x');
                            ResolutionX = GetNumberInt(Resolution[0]);
                            ResolutionY = GetNumberInt(Resolution[1]);
                            ResolutionXScale = (double)ResolutionX / pictureBox1.Width;
                            ResolutionYScale = (double)ResolutionY / pictureBox1.Height;
                        }
                    }
                }
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
            isPause = false;
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
                
                //3.999022243950134  这个是我通过多次模拟后得到 我这个分辨率的最佳时间
                if (this.tbxPressTimeValue.Text.Trim().Length==0)
                {
                    tbxPressTimeValue.Text = "4.0";// "3.999022243950134";
                }
                double timevalue;
                bool ret;
                ret=double.TryParse(tbxPressTimeValue.Text,out timevalue);
                if (!ret)
                {
                    tbxPressTimeValue.Text = "4.0";
                    timevalue = 4.0d;
                }
                Text = string.Format("距离：{0}，时间：{1}", value.ToString("0.00"), (timevalue * value).ToString("0.000"));

                Random ran = new Random();
                string rndX = ran.Next(100, 300).ToString();
                string rndY = ran.Next(100, 600).ToString();
                string offsetx = ran.Next(10, 50).ToString();
                string offsety = ran.Next(10, 100).ToString();
                ADB.RunADBShellCommand(string.Format("shell input swipe {0} {1} {2} {3} {4}", 
                                                     rndX, rndY, (int.Parse(rndX) + int.Parse(offsetx)).ToString(), (int.Parse(rndY) + int.Parse(offsety)).ToString(), (timevalue * value).ToString("0")));
                
                Cursor = Cursors.WaitCursor;
                int delay = 2500;
                ret = int.TryParse(textBox1.Text, out delay);
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
            isPause = false;
        }

        private void btnDebugCapture_Click(object sender, EventArgs e)
        {
            isWaiting = true;
            Cursor = Cursors.WaitCursor;
            ADB.DebugCapture(System.Windows.Forms.Application.StartupPath);
            MessageBox.Show("截图图像保存成功。\r\n请在ADBJump目录下检查fix.png是否可以用看图软件打开。\r\n如果无法打开,请用二进制编辑器检查pull.png文件和fix.png文件的异同,并发给作者。\r\nQQ:564955427。");
            Cursor = Cursors.Default;
            isWaiting = false;
        }

        private void tbxPressTimeValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                return;
            }
            if (e.KeyChar == 8 || e.KeyChar == 46)
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
    }
}

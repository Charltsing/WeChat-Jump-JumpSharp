using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ADBJump
{
    class HiPerfTimer
    {
        //引用win32 api中的queryperformancecounter()方法
        //该方法用来查询任意时刻高精度计数器的实际值
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        //引用win32 api中的queryperformancefrequency()方法
        //该方法返回高精度计数器每秒的计数值,这对于同一台电脑，是同一个值，与CPU的频率有关~so，根据这个值，可以粗略看出一个cpu的好坏。
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long startTime, stopTime;
        private long Frequence;

        //构造函数
        public HiPerfTimer()
        {
            startTime = 0;
            stopTime = 0;
            if (QueryPerformanceFrequency(out Frequence) == false)
            {
                //不支持高性能计时器
                throw new System.ComponentModel.Win32Exception();
            }
        }

        //开始计时
        public void Start()
        {
            //让等待线程工作
            System.Threading.Thread.Sleep(0);
            QueryPerformanceCounter(out startTime);
        }

        //结束计时
        public void Stop()
        {
            QueryPerformanceCounter(out stopTime);
        }

        //返回计时结果(ms)
        public int Duration
        {
            get
            {
                return (int)((double)(stopTime - startTime) * 1000 / (double)Frequence);
            }
        }
    }
}

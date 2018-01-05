using System;
using System.Runtime.InteropServices;

namespace ADBJump
{
    public class UnixHelper
    {
        // *nix need to implement QueryPerformanceCounter and QueryPerformanceFrequency manually.
        [DllImport("libc")]
        static extern int uname(IntPtr buf);

        #region Darwin imports

        [DllImport("libc")]
        static extern int mach_timebase_info(out mach_timebase_info_data_t info);

        [DllImport("libc")]
        static extern UInt64 mach_absolute_time();
        
        #endregion

        [DllImport("libc")]
        static extern int gettimeofday(out timeval t, IntPtr p);

        static public string OS
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                if (p == 4 || p == 6 || p == 128)
                {
                    IntPtr buf = Marshal.AllocHGlobal(8192);
                    if (uname(buf) != 0)
                    {
                        Marshal.FreeHGlobal(buf);
                        return "generic-unix";
                    }
                    else
                    {
                        string os = Marshal.PtrToStringAnsi(buf);
                        Marshal.FreeHGlobal(buf);
                        if (os == "Darwin")
                        {
                            return "Darwin";
                        }
                        else
                        {
                            return "generic-unix";
                        }
                    }
                }
                else
                {
                    return "windows";
                }
            }
        }

        // TODO: This may be wrong on some platforms.
        private struct timeval {
            public long tv_sec;
            public int tv_usec;
        }

        private struct mach_timebase_info_data_t
        {
            public UInt32 numer;
            public UInt32 denom;
        }

        static private void NtQueryPerformanceCounter(out long counter, out long frequency)
        {
            timeval now;
            if (OS == "Darwin")
            {
                mach_timebase_info_data_t timebase;
                mach_timebase_info(out timebase);
                counter = (long)mach_absolute_time() * timebase.numer/ timebase.denom / 100;
            }
            else // Other *nix support this
            {
                gettimeofday(out now, IntPtr.Zero);
                counter = now.tv_sec * 10000000 + now.tv_usec * 10 + (long)((UInt64)(369 * 365 + 89) * 86400 * 10000000);
            }
            frequency = 10000000; // Hard-coded
        }

        static public bool QueryPerformanceCounter(out long counter)
        {
            long unused;
            NtQueryPerformanceCounter(out counter, out unused);
            return true;
        }

        static public bool QueryPerformanceFrequency(out long frequency)
        {
            long counter;
            NtQueryPerformanceCounter(out counter, out frequency);
            return true;
        }

        public UnixHelper()
        {
        }
    }
}

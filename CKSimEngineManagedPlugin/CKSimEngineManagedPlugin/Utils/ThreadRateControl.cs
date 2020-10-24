using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CKSimEngineManagedPlugin.Utils
{
    public class ThreadRateControl
    {
        private double StartTime = 0;
        private double EndTime = 0;
        private int ElapsedTimeMS = 0;
        private double PrevStartTime = 0;
        private Stopwatch stopwatch = Stopwatch.StartNew();

        public ThreadRateControl()
        {
            SetTimerResolutionFast();
        }

        ~ThreadRateControl()
        {
            RestoreTimerResolutionOnExit();
        }

        public void Start()
        {
            stopwatch.Reset();
            stopwatch.Start();
            StartTime = stopwatch.ElapsedMilliseconds / 1000.0;
            PrevStartTime = StartTime;
        }

        public void Stop()
        {
            stopwatch.Reset();
            stopwatch.Stop();
        }

        public void DoRateControl(int minLoopTime)
        {
            LoopTime = (StartTime - PrevStartTime) * 1000;
            do
            {
                EndTime = stopwatch.ElapsedMilliseconds / 1000.0;
                ElapsedTimeMS = (int)((EndTime - StartTime) * 1000);
                if (ElapsedTimeMS < minLoopTime)
                {
                    try
                    {
                        Thread.Sleep(Math.Abs(minLoopTime - ElapsedTimeMS));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            } while (ElapsedTimeMS < minLoopTime);
            PrevStartTime = StartTime;
            StartTime = stopwatch.ElapsedMilliseconds / 1000.0;
        }

        public double LoopTime { get; private set; } = 0;

        #region Windows Timer Resolution Update
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetTimerResolution(uint DesiredResolution, bool SetResolution, out uint CurrentResolution);
        private uint prevTimerResolution = 0;
        private readonly uint requestedTimerInterval = 4959U;
        private void SetTimerResolutionFast()
        {
            //Get current value of timer resolution
            NtSetTimerResolution(requestedTimerInterval, false, out prevTimerResolution);

            //Set windows timer interrupt to 0.4959ms if the current value is slower
            //Units in 100ns per unit
            if (prevTimerResolution > requestedTimerInterval)
            {
                NtSetTimerResolution(requestedTimerInterval, true, out prevTimerResolution);
            }
        }
        private void RestoreTimerResolutionOnExit()
        {
            NtSetTimerResolution(prevTimerResolution, true, out _);
        }
        #endregion
    }
}

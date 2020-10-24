using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKSimEngineManagedPlugin.Utils
{
    public class ElapsedTimer
    {
        private Stopwatch stopwatch = Stopwatch.StartNew();
        public void Start()
        {
            stopwatch.Restart();
        }

        public double HasElapsed()
        {
            return stopwatch.ElapsedMilliseconds;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CKSimEngineManagedPlugin;

namespace CKSimEngineManagedPluginTest
{
    class Program
    {
        static void Main(string[] args)
        {
            CKSimNetCode cr = CKSimNetCode.Instance;
            cr.SetAccelerometer(0, 0.3f);
            Console.ReadKey();
        }
    }
}

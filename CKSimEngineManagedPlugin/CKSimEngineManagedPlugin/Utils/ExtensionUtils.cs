using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroMQ;

namespace CKSimEngineManagedPlugin.Utils
{
    public static class ExtensionUtils
    {
        public static bool TryCloseSocket(this ZSocket zSock)
        {
            try
            {
                zSock?.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}

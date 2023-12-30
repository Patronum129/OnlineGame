using System;
using System.Net;
using System.Net.Sockets;

namespace Utilitys
{
    public class NetTool
    {
        public static IPAddress GetIPAddress()
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in ipEntry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }

            throw new Exception();
        }
    }
}
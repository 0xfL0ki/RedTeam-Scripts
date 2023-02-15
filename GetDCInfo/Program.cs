using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.InteropServices;

namespace GetDCInfo
{
   
    class Program
    {
        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int NetWkstaGetInfo(string servername, int level, out IntPtr bufptr);

        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
        public static extern int NetApiBufferFree(IntPtr Buffer);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WKSTA_INFO_100
        {
            public int wki100_platform_id;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string wki100_computername;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string wki100_langroup;
            public int wki100_ver_major;
            public int wki100_ver_minor;
        }

        static void Main(string[] args)
        {
            IntPtr pBuffer;
            int nStatus = NetWkstaGetInfo(null, 100, out pBuffer);

            if (nStatus == 0)
            {
                try
                {
                    WKSTA_INFO_100 wkstaInfo = (WKSTA_INFO_100)Marshal.PtrToStructure(pBuffer, typeof(WKSTA_INFO_100));
                    Console.WriteLine("Current domain name: " + wkstaInfo.wki100_langroup);

                    string domainControllerName = Environment.GetEnvironmentVariable("LOGONSERVER");
                    if (!string.IsNullOrEmpty(domainControllerName))
                    {
                        domainControllerName = domainControllerName.Substring(2);
                        Console.WriteLine("Domain controller name: " + domainControllerName);

                        IPAddress[] addresses = Dns.GetHostAddresses(domainControllerName);
                        foreach (IPAddress address in addresses)
                        {
                            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                Console.WriteLine("Domain controller IP address: " + address.ToString());
                            }
                        }
                    }
                }
                finally
                {
                    NetApiBufferFree(pBuffer);
                }
            }
            else
            {
                throw new System.ComponentModel.Win32Exception(nStatus);
            }
        }
    }

}

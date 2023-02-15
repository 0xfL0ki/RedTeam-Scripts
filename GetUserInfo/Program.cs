using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace GetUserInfo
{
    
    class Program
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool GetUserName(StringBuilder lpBuffer, ref int nSize);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool LookupAccountName(string lpSystemName, string lpAccountName, IntPtr Sid, ref int cbSid, StringBuilder ReferencedDomainName, ref int cchReferencedDomainName, out int peUse);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetComputerName(StringBuilder lpBuffer, ref int nSize);

        static void Main(string[] args)
        {
            // Get the username
            StringBuilder username = new StringBuilder(256);
            int usernameSize = username.Capacity;
            if (!GetUserName(username, ref usernameSize))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
            Console.WriteLine("Username: " + username.ToString());

            // Get the user group
            StringBuilder domainName = new StringBuilder(256);
            int domainNameSize = domainName.Capacity;
            int sidSize = 0;
            int peUse = 0;
            if (!LookupAccountName(null, username.ToString(), IntPtr.Zero, ref sidSize, domainName, ref domainNameSize, out peUse))
            {
                IntPtr sidPtr = Marshal.AllocHGlobal(sidSize);
                try
                {
                    if (!LookupAccountName(null, username.ToString(), sidPtr, ref sidSize, domainName, ref domainNameSize, out peUse))
                    {
                        throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(sidPtr);
                }
            }
            Console.WriteLine("UserGroup: " + domainName.ToString());

            // Get the hostname
            StringBuilder hostname = new StringBuilder(256);
            int hostnameSize = hostname.Capacity;
            if (!GetComputerName(hostname, ref hostnameSize))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
            Console.WriteLine("Hostname: " + hostname.ToString());

            //Console.ReadLine();
        }
    }

}

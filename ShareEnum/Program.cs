using System;
using System.Runtime.InteropServices;

namespace ShareEnum
{
    class Program
    {
        [DllImport("Netapi32.dll", SetLastError = true)]
        public static extern int NetShareEnum(
            string servername,
            int level,
            out IntPtr bufPtr,
            uint prefmaxlen,
            out int entriesread,
            out int totalentries,
            ref uint resumeHandle
        );

        [DllImport("Netapi32.dll")]
        public static extern int NetApiBufferFree(IntPtr buffer);

        [DllImport("Advapi32.dll", SetLastError = true)]
        public static extern int LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out IntPtr phToken
        );

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("Netapi32.dll", SetLastError = true)]
        static extern uint NetShareCheck(
        string servername,
        string netname,
        out SHARE_TYPE sharType,
        out uint permissions,
        IntPtr phToken
        );

        enum SHARE_TYPE : uint
        {
            STYPE_DISKTREE = 0,
            STYPE_PRINTQ = 1,
            STYPE_DEVICE = 2,
            STYPE_IPC = 3,
            STYPE_SPECIAL = 0x80000000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct SHARE_INFO_2
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string shi2_netname;
            public SHARE_TYPE shi2_type;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string shi2_remark;
            public uint shi2_permissions;
            public uint shi2_max_uses;
            public uint shi2_current_uses;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string shi2_path;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string shi2_passwd;
        }

        static void Main(string[] args)
        {
            // Set the server name to null to enumerate shares on the local computer.
            string serverName = null;
            int level = 2; // Level 2 returns information about the share, including its name, type, and path.

            IntPtr bufPtr;
            int entriesRead, totalEntries;
            uint resumeHandle = 0;

            int result = NetShareEnum(serverName, level, out bufPtr, uint.MaxValue, out entriesRead, out totalEntries, ref resumeHandle);

            if (result == 0)
            {
                Console.WriteLine($"Found {entriesRead} network shares:");

                // Cast the buffer pointer to a SHARE_INFO_2 struct and enumerate the shares.
                IntPtr currentPtr = bufPtr;
                for (int i = 0; i < entriesRead; i++)
                {
                    SHARE_INFO_2 share = (SHARE_INFO_2)Marshal.PtrToStructure(currentPtr, typeof(SHARE_INFO_2));

                    Console.WriteLine($"\nShare name: {share.shi2_netname}");
                    Console.WriteLine($"Share path: {share.shi2_path}");
                    Console.WriteLine($"Share type: {share.shi2_type}");

                    // Validate the current user's access to the share using the LogonUser function.
                    // Note that this requires the user to enter their password at runtime, which could be a security risk.
                    IntPtr userToken;
                    int logonResult = LogonUser(Environment.UserName, Environment.UserDomainName, null, 2, 0, out userToken);

                    if (logonResult != 0)
                    {
                        // Check the user's access to the share.
                        bool hasAccess = Convert.ToBoolean(NetShareCheck(serverName, share.shi2_netname, out SHARE_TYPE _, out uint _, userToken));

                        if (hasAccess)
                        {
                            Console.WriteLine("You have access to this share.");
                        }
                        else
                        {
                            Console.WriteLine("You do not have access to this share.");
                        }

                        // Close the user token handle.
                        CloseHandle(userToken);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to log on user {Environment.UserName} with error code {Marshal.GetLastWin32Error()}");
                    }

                    // Move the pointer to the next element in the buffer.
                    currentPtr = (IntPtr)(currentPtr.ToInt64() + Marshal.SizeOf(typeof(SHARE_INFO_2)));
                }

                // Free the buffer memory.
                NetApiBufferFree(bufPtr);
            }
            else
            {
                Console.WriteLine($"Error {result} occurred while enumerating shares.");
            }
        }
    }
}

       
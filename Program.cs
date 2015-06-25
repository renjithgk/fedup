using Microsoft.Win32;
using System;
using System.Threading;

namespace ConsoleApplication4
{
    class Program
    {
        static void Main(string[] args)
        {

            SystemEvents.SessionEnding += SystemEvents_SessionEnding;
            SystemEvents.SessionEnded += SystemEvents_SessionEnded;

            Thread.Sleep(TimeSpan.FromDays(3));
        }

        static void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
        {
            Helper.SystemRestarter.Abort();
        }

        static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            Helper.SystemRestarter.Abort();
        }

        //public static void Test()
        //{
        //    // Restart the current computer in 30 seconds and wait for applications to close.
        //    // Specify that the restart operation is planned because a consecuence of an installation.
        //    dynamic Success = SystemRestarter.Restart(null, 30, "System is gonna be restarted quickly, save all your data...!", SystemRestarter.Enums.InitiateShutdown_Force.Wait, SystemRestarter.Enums.ShutdownReason.MajorOperatingSystem | SystemRestarter.Enums.ShutdownReason.MinorInstallation, SystemRestarter.Enums.ShutdownPlanning.Planned);

        //    Console.WriteLine(string.Format("Restart operation initiated successfully?: {0}", Convert.ToString(Success)));

        //    // Abort the current operation.
        //    if (Success)
        //    {
        //        dynamic IsAborted = SystemRestarter.Abort();
        //        Console.WriteLine(string.Format("Restart operation aborted   successfully?: {0}", Convert.ToString(IsAborted)));
        //    }
        //    else
        //    {
        //        Console.WriteLine("There is any restart operation to abort.");
        //    }
        //    Console.ReadKey();

        //    // Shutdown the current computer instantlly and force applications to close.
        //    // ( When timeout is '0' the operation can't be aborted )
        //    SystemRestarter.Shutdown(null, 0, null, SystemRestarter.Enums.InitiateShutdown_Force.ForceSelf);

        //    // LogOffs the current user.
        //    SystemRestarter.LogOff(SystemRestarter.Enums.ExitwindowsEx_Force.Wait);

        //}
    }
}

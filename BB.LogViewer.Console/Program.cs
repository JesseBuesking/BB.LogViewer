using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace BB.LogViewer.Console
{
    public class Program
    {
        public static Timer checkLogsForNew;

        public static DateTime lastGeneratedDT;

        public static string logName;

        public static string machineName;

        public static HashSet<string> sources;

        public static HashSet<EventLogEntryType> logEntryTypes;

        static void Main(string[] args)
        {
            Program.logName = "Application";
            Program.machineName = ".";
            Program.sources = null;
            Program.lastGeneratedDT = DateTime.MinValue;
            Program.logEntryTypes = new HashSet<EventLogEntryType>
            {
                EventLogEntryType.FailureAudit,
                EventLogEntryType.Error,
                EventLogEntryType.Warning,
                EventLogEntryType.SuccessAudit,
                EventLogEntryType.Information
            };

            while (true)
            {
                var logs = Utilities.LogReader.GetLogs(Program.logName, Program.machineName, Program.sources,
                Program.logEntryTypes, ref Program.lastGeneratedDT);

                Utilities.LogReader.WriteLogsToConsole(logs);

                // argh blocking!!!
                Thread.Sleep(1000); // sleep for 1 second
            }

            //checkLogsForNew = new Timer(CheckLogsForNew, null, 0, (int)TimeSpan.FromSeconds(1).TotalMilliseconds);

            //Thread.Sleep(10000000);
        }

        //static void CheckLogsForNew(object state)
        //{
        //    var logs = Utilities.LogReader.GetLogs(Program.logName, Program.machineName, Program.sources, 
        //        Program.logEntryTypes, ref Program.lastGeneratedDT);

        //    Utilities.LogReader.WriteLogsToConsole(logs);
        //}
    }
}

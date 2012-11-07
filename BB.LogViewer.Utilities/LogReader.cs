using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BB.LogViewer.Utilities
{
    /// <summary>
    /// Methods to read logs.
    /// </summary>
    public static class LogReader
    {
        /// <summary>
        /// Get the specified logs.
        /// </summary>
        /// <param name="logName">The name of the log to use.</param>
        /// <param name="machineName">The machine to use.</param>
        /// <param name="sources">The sources to get logs from.</param>
        /// <param name="logEntryTypes">The type of logs to get.</param>
        /// <param name="lastGeneratedDT">The starting point in time to get logs from.</param>
        public static IOrderedEnumerable<EventLogEntry> GetLogs(string logName, string machineName,
            HashSet<string> sources, HashSet<EventLogEntryType> logEntryTypes, ref DateTime lastGeneratedDT)
        {
            DateTime minValue = lastGeneratedDT;

            EventLog eventLog = new EventLog(logName, machineName);

            var logs = eventLog.Entries.Cast<EventLogEntry>()
                .Where(logEntry => minValue < logEntry.TimeGenerated)
                .Where(logEntry => null == sources || sources.Contains(logEntry.Source, new InvariantCultureIgnoreCaseComparer()))
                .Where(logEntry => null == logEntryTypes || logEntryTypes.Contains(logEntry.EntryType))
                .OrderBy(logEntry => logEntry.TimeGenerated);

            lastGeneratedDT = DateTime.UtcNow;

            return logs;
        }

        /// <summary>
        /// Get the last <see cref="limit"/> logs.
        /// </summary>
        /// <param name="logName">The name of the log to use.</param>
        /// <param name="machineName">The machine to use.</param>
        /// <param name="sources">The sources to get logs from.</param>
        /// <param name="limit">How many logs to get.</param>
        /// <param name="logEntryTypes">The type of logs to get.</param>
        public static IOrderedEnumerable<EventLogEntry> Tail(string logName, string machineName, 
            HashSet<string> sources, int limit, HashSet<EventLogEntryType> logEntryTypes)
        {
            EventLog eventLog = new EventLog(logName, machineName);

            return eventLog.Entries.Cast<EventLogEntry>()
                .Where(logEntry => null == sources || sources.Contains(logEntry.Source, new InvariantCultureIgnoreCaseComparer()))
                .Where(logEntry => null == logEntryTypes || logEntryTypes.Contains(logEntry.EntryType))
                .OrderByDescending(logEntry => logEntry.TimeGenerated)
                .Take(limit)
                .OrderByDescending(logEntry => logEntry.TimeGenerated);
        }

        /// <summary>
        /// Writes the logs supplied to the console, coloring them.
        /// </summary>
        /// <param name="logs">The logs to write to the console.</param>
        public static void WriteLogsToConsole(IOrderedEnumerable<EventLogEntry> logs)
        {
            var defaultColor = Console.ForegroundColor;
            foreach (var log in logs)
            {
                switch (log.EntryType)
                {
                    case EventLogEntryType.FailureAudit:
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                    case EventLogEntryType.SuccessAudit:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                    case EventLogEntryType.Information:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case EventLogEntryType.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case EventLogEntryType.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                }

                Console.WriteLine("{0} -- {1} -- {2} -- {3} -- {4}", log.EntryType,
                    log.MachineName, log.TimeGenerated, log.Source, log.Message);

                Console.ForegroundColor = defaultColor;
            }
        }
    }

    /// <summary>
    /// Compares strings using StringComparison.InvariantCultureIgnoreCase.
    /// </summary>
    class InvariantCultureIgnoreCaseComparer : EqualityComparer<string>
    {
        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public override bool Equals(string s1, string s2)
        {
            return s1.Equals(s2, StringComparison.InvariantCultureIgnoreCase);
        }
        
        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public override int GetHashCode(string s)
        {
            return base.GetHashCode();
        }
    }
}

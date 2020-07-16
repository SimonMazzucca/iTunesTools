using System;
using System.IO;
using System.Reflection;
using System.Configuration;

namespace iTunesReporting
{
    public class Logger
    {
        #region Constants

        private const string LOG_FILE = "{0}.csv";
        private const string HEADER = "Date,Time,Log,ThreadID";
        private const string LOG_ENTRY = "\"{0}\",\"{1}\",\"{2}\",\"{3}\""; //TODO: soft code based on HEADER

        #endregion

        #region Private Fields

        private static readonly string _AppPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
        private static readonly string _Year = DateTime.Now.ToString("yyyy");

        #endregion

        #region Public Methods

        public static void LogActivity(string logEntry)
        {
            string fullLogEntry = string.Empty;
            string fullPath = GetFullLogPath();

            try
            {
                PopulateLogHeader(ref fullLogEntry, fullPath);
                PopulateLogEntry(ref fullLogEntry, logEntry);

                using (StreamWriter logWriter = new StreamWriter(fullPath, true))
                {
                    Console.WriteLine(logEntry);
                    logWriter.WriteLine(fullLogEntry, true);
                    logWriter.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("---------------------------");
                Console.WriteLine("Log error: " + ex.ToString());
            }
        }

        public static void LogException(Exception ex)
        {
            LogActivity("Exception: " + ex.ToString());
        }

        #endregion

        #region Private Methods

        private static void PopulateLogHeader(ref String fullLogEntry, string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                fullLogEntry = HEADER + "\n";
            }
        }

        private static void PopulateLogEntry(ref String fullLogEntry, string logEntry)
        {
            string dateStamp = DateTime.Now.ToString("MM/dd/yy");
            string timeStamp = DateTime.Now.ToString("HH:mm:ss");
            string log = logEntry.Replace("\"", "'").Replace("\r", " ");
            int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;

            fullLogEntry += string.Format(LOG_ENTRY, dateStamp, timeStamp, log, threadID);
        }

        private static string GetFullLogPath()
        {
            string dateStamp = DateTime.Now.ToString("yyyy.MM.dd");
            string fileName = string.Format(LOG_FILE, dateStamp);
            string result = Path.Combine(GetLogPath(), fileName);

            return result;
        }

        private static string GetLogPath()
        {
            string result = Path.Combine(_AppPath, _Year);

            result = result.Replace("file:\\", "");

            if (!Directory.Exists(result))
                Directory.CreateDirectory(result);

            return result;
        }

        #endregion
    }
}



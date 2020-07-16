using iTunesLib;
using iTunesReporting.Reports;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace iTunesReporting
{
    class Facade
    {
        const string CONFIG_FILE = "iTunesReporting.txt";
        const string APP_HEADER = "iTunesReporting - Version: {0}\n\n";

        readonly Char PARAM_SEPARATOR = ';';
        readonly Char VALUE_SEPARATOR = '=';

        List<Report> _Reports;

        public void Run()
        {
            WriteAppHeader();
            LoadConfiguration();
            RunTrackReports();
            WriteReports();

            //try
            //{
            //    WriteAppHeader();
            //    LoadConfiguration();
            //    RunTrackReports();
            //    WriteReports();
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogException(ex);
            //}
        }

        private void WriteAppHeader()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion; 

            Console.WriteLine(APP_HEADER, version);
        }

        private void LoadConfiguration()
        {
            StreamReader configFile = new StreamReader(CONFIG_FILE);
            _Reports = new List<Report>();

            string configAll = configFile.ReadToEnd();
            string[] lines = configAll.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (!line.StartsWith("[") && !string.IsNullOrEmpty(line))
                {
                    string command = GetCommand(line);
                    Dictionary<string, string> parameters = GetParameters(line);

                    if (command.Equals("RunOrphanFilesReport", StringComparison.InvariantCultureIgnoreCase))
                    {
                        OrphanFilesReport report = new OrphanFilesReport();
                        _Reports.Add(report);
                    }
                    else if (command.Equals("RunFilePathReport", StringComparison.InvariantCultureIgnoreCase))
                    {
                        LocationReport report = new LocationReport();
                        report.LoadParameters(parameters);
                        _Reports.Add(report);
                    }
                    else if (command.Equals("ArtistGenresReport", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ArtistGenresReport report = new ArtistGenresReport();
                        _Reports.Add(report);
                    }
                }
            }

        }

        private string GetCommand(string line)
        {
            string[] parts = line.Split(PARAM_SEPARATOR);
            string result = parts[0];

            return result;
        }

        private Dictionary<string, string> GetParameters(string line)
        {
            string[] parts = line.Split(PARAM_SEPARATOR);
            Dictionary<string, string> result = new Dictionary<string, string>();

            for (int i = 1; i < parts.Length; i++)
            {
                if (parts[i].Contains(VALUE_SEPARATOR))
                {
                    string[] keyValuePair = parts[i].Split(VALUE_SEPARATOR);
                    result.Add(keyValuePair[0], keyValuePair[1]);
                }
                else
                {
                    result.Add(parts[i], "");
                }
            }

            return result;
        }

        /// <summary>
        /// Conceivably I could have TrackReports that run PER track
        /// and OtherReports that are executed independently
        /// </summary>
        private void RunTrackReports()
        {
            const string STATUS = "{0} of {1} processed ({2}%)";
            const string STATUS_END = "{0} of {1} processed        ";

            iTunesApp _iTunesApp = new iTunesApp();
            IITTrackCollection allTracks = _iTunesApp.LibraryPlaylist.Tracks;
            IITFileOrCDTrack fileTrack = null;

            int current = 0;
            int total = allTracks.Count;
            int percent = 0;
            string status;

            foreach (IITTrack track in allTracks)
            {
                if (track is IITFileOrCDTrack)
                {
                    foreach (Report report in _Reports)
                    {
                        fileTrack = (IITFileOrCDTrack)track;
                        report.ProcessTrack(fileTrack);
                    }
                    current++;
                    percent = (int)(current * 100 / total);
                    status = string.Format(STATUS, current, total, percent);
                    Console.Write("\r" + status);
                }

                //if (percent == 5) break;
            }

            foreach (Report report in _Reports)
            {
                report.FinalizeReport();
            }

            status = string.Format(STATUS_END, current, total);
            Console.Write("\r" + status);
            Console.WriteLine("\n");

            _iTunesApp = null;
        }

        private void WriteReports()
        {
            foreach (Report report in _Reports)
            {
                report.Write();
            }
            Console.WriteLine("\n\nDone. Press any key to close window.");
        }

    }
}

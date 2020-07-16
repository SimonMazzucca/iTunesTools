using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iTunesLib;

namespace iTunesReporting.Reports
{
    public enum ReportTypes
    {
        LogAll,
        LogInPath,
        LogNotInPath
    }

    public class LocationReport : Report
    {
        public ReportTypes ReportType { get; set; }
        public string ReportTypeValue { get; set; }
        public string ReportTypeValueLower { get; set; }

        public override void LoadParameters(Dictionary<string, string> parameters)
        {
            //Assumes one and one only param
            string key = parameters.Keys.FirstOrDefault();
            string value = parameters.Values.FirstOrDefault();

            if (key.Equals("LogAll", StringComparison.InvariantCultureIgnoreCase))
                ReportType = ReportTypes.LogAll;
            else if (key.Equals("LogInPath", StringComparison.InvariantCultureIgnoreCase))
                ReportType = ReportTypes.LogInPath;
            else if (key.Equals("LogNotInPath", StringComparison.InvariantCultureIgnoreCase))
                ReportType = ReportTypes.LogNotInPath;

            ReportTypeValue = value;
            ReportTypeValueLower = value.ToLower();
        }

        public override void InitializeOutput()
        {
            OutputHeaders.Add("Artist");
            OutputHeaders.Add("Name");
            OutputHeaders.Add("Path");

            ReportName = "Location Report";
        }

        public override void ProcessTrack(IITFileOrCDTrack track)
        {
            switch (ReportType)
            {
                case ReportTypes.LogAll:
                    ProcessLogAll(track);
                    break;
                case ReportTypes.LogInPath:
                    ProcessLogInPath(track);
                    break;
                case ReportTypes.LogNotInPath:
                    ProcessLogLogNotInPath(track);
                    break;
            }
        }

        private void AddTrackToReport(IITFileOrCDTrack track)
        {
            List<string> row = GetTrackRow(track);
            OutputRows.Add(row);
        }

        private void ProcessLogAll(IITFileOrCDTrack track)
        {
            AddTrackToReport(track);
        }

        private void ProcessLogInPath(IITFileOrCDTrack track)
        {
            string location = track.Location;

            if (!string.IsNullOrEmpty(location) && location.ToLower().StartsWith(ReportTypeValueLower))
            {
                AddTrackToReport(track);
            }
        }

        private void ProcessLogLogNotInPath(IITFileOrCDTrack track)
        {
            string location = track.Location;

            if (!string.IsNullOrEmpty(location) && !location.ToLower().StartsWith(ReportTypeValueLower))
            { 
                AddTrackToReport(track);
            }
        }

        private List<string> GetTrackRow(IITFileOrCDTrack track)
        {
            List<string> result = new List<string>();

            result.Add(track.Artist);
            result.Add(track.Name);
            result.Add(track.Location);

            return result;
        }

    }
}

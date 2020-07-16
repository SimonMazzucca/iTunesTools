using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iTunesLib;

namespace iTunesReporting.Reports
{
    //TODO: rename OrphanTracksReport
    class OrphanFilesReport : Report
    {

        public override void LoadParameters(Dictionary<string, string> parameters)
        {
            throw new NotImplementedException();
        }

        public override void InitializeOutput()
        {
            OutputHeaders.Add("Artist");
            OutputHeaders.Add("Name");

            ReportName = "Orphan Files Report";
        }

        public override void ProcessTrack(IITFileOrCDTrack track)
        {
            if (string.IsNullOrEmpty(track.Location))
            {
                List<string> row = GetTrackRow(track);
                OutputRows.Add(row);
            }
        }

        //public override void Run()
        //{

        //    try
        //    { 
        //        //Move to base constructor ?
        //        _iTunesApp = new iTunesApp();
        //        OutputRows = new List<List<string>>();
        //        IITFileOrCDTrack fileTrack;
        //        //LoadOutputHeaders();

        //        IITTrackCollection allTracks = _iTunesApp.LibraryPlaylist.Tracks;

        //        foreach (IITTrack track in allTracks)
        //        {
        //            if (track is IITFileOrCDTrack)
        //            {
        //                fileTrack = (IITFileOrCDTrack)track;
        //                if (string.IsNullOrEmpty(fileTrack.Location))
        //                {
        //                    Console.WriteLine(track.Name);
        //                    OutputCount++;
        //                    List<string> row = GetTrackRow(fileTrack);
        //                    OutputRows.Add(row);
        //                }
        //            }
        //            else
        //            {
        //                //Console.WriteLine("Not a file track: " + track.Name);
        //                break;
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogException(ex);
        //    }

        //}

        private List<string> GetTrackRow(IITFileOrCDTrack track)
        {
            List<string> result = new List<string>();

            result.Add(track.Artist);
            result.Add(track.Name);

            return result;
        }

    }
}

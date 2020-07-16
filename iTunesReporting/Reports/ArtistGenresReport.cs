using iTunesLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iTunesReporting.Reports
{
    class ArtistGenresReport : Report
    {

        IDictionary<string, IList<string>> _GenresByArtist = new Dictionary<string, IList<string>>();

        public override void LoadParameters(Dictionary<string, string> parameters)
        {
            throw new NotImplementedException();
        }

        public override void InitializeOutput()
        {
            OutputHeaders.Add("Artist");
            OutputHeaders.Add("Genre");
            OutputHeaders.Add("Path");
            OutputHeaders.Add("Kind");

            ReportName = "Artist Genres";
        }

        public override void ProcessTrack(IITFileOrCDTrack track)
        {
            if (track.Kind == ITTrackKind.ITTrackKindFile)
            if (string.IsNullOrEmpty(track.Artist))
            {
                //List<string> row = GetTrackRow(track.Name, "No artist", track.Location, track.Category + "_" + track.sourceID);
                //OutputRows.Add(row);
            }
            else if (string.IsNullOrEmpty(track.Genre))
            {
                //List<string> row = GetTrackRow(track.Name, "No genre", track.Location, track.Category + "_" + track.sourceID);
                //OutputRows.Add(row);
            }
            else if (_GenresByArtist.ContainsKey(track.Artist))
            {
                if (!_GenresByArtist[track.Artist].Contains(track.Genre))
                {
                    _GenresByArtist[track.Artist].Add(track.Genre);
                }
            }
            else
            {
                _GenresByArtist.Add(
                    track.Artist,
                    new List<string>() { track.Genre }
                );
            }
        }

        public override void FinalizeReport()
        {
            IEnumerable<KeyValuePair<string, IList<string>>> multipleGenres = _GenresByArtist.Where(x => x.Value.Count > 1);

            foreach (var item in multipleGenres)
            {
                List<string> row = GetTrackRow(item.Key, string.Join(",", item.Value.ToArray()), "", "");
                OutputRows.Add(row);
            }
        }

        private List<string> GetTrackRow(params string[] args)
        {
            List<string> result = new List<string>(args);
            return result;
        }

    }
}

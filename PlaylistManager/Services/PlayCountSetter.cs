using iTunesLib;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlaylistManager.Services
{
    public class PlayCountSetter : BaseService, IService
    {
        public void Run(string playlistName)
        {
            string latest = GetLatestPlaylistLog();
            IDictionary<string, int> playcountByArtist = GetPlaycountByArtist(latest);

            if (ConfirmPlayCountCorrect(playcountByArtist))
                SetPlayCount(playcountByArtist, playlistName);
            else
                Console.WriteLine("Abort");
        }

        private void SetPlayCount(IDictionary<string, int> playcountByArtist, string playlistName)
        {
            iTunesApp itunes = new iTunesApp();
            IITLibraryPlaylist mainLibrary = itunes.LibraryPlaylist;
            IITPlaylist playlist = itunes.LibrarySource.Playlists.GetPlaylist(playlistName);
            IITTrackCollection tracks = playlist.Tracks;

            for (int currTrackIndex = 1; currTrackIndex <= tracks.Count; currTrackIndex++)
            {
                IITTrack fileTrack = tracks[currTrackIndex];
                if (playcountByArtist.ContainsKey(fileTrack.Artist))
                    fileTrack.PlayedCount = playcountByArtist[fileTrack.Artist];
                else
                    Console.WriteLine("Skipped: {0} - {1}", fileTrack.Artist, fileTrack.Name);
            }
        }

        private bool ConfirmPlayCountCorrect(IDictionary<string, int> playcountByArtist)
        {
            foreach (string artist in playcountByArtist.Keys)
                Console.WriteLine("{0}: {1}", playcountByArtist[artist], artist);

            Console.WriteLine("Does this seem right? [Y|N]");
            ConsoleKeyInfo decision = Console.ReadKey(true);
            return (decision.KeyChar == 'y' || decision.Key == ConsoleKey.Enter);
        }

        private IDictionary<string, int> GetPlaycountByArtist(string latest)
        {
            IDictionary<string, int> toReturn = new Dictionary<string, int>();
            TextFieldParser parser = new TextFieldParser(latest);

            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");

            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                string artist = fields[0];
                int playcount = Int32.Parse(fields[3]) + 1;

                toReturn[artist] = playcount;
            }

            return toReturn;
        }

        private string GetLatestPlaylistLog()
        {
            string[] files = Directory.GetFiles(_settings.PlaylistPath, "*.csv");

            if (files == null || files.Length == 0)
                throw new Exception("Playlist folder is empty");

            string latest = files.ToList().OrderByDescending(f => f).FirstOrDefault();

            return latest;
        }
    }
}

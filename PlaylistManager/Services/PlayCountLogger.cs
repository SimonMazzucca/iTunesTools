using iTunesLib;
using System;
using System.IO;

namespace PlaylistManager.Services
{
    public class PlayCountLogger : BaseService, IService
    {

        public void Run(string playlistName)
        {
            iTunesApp itunes = new iTunesApp();
            IITLibraryPlaylist mainLibrary = itunes.LibraryPlaylist;
            IITPlaylist playlist = itunes.LibrarySource.Playlists.GetPlaylist(playlistName);
            IITTrackCollection tracks = playlist.Tracks;

            string csvFilePath = GetFullCsvFilePath(playlistName);

            if (CheckIfUserWantsToAbort(csvFilePath))
                return;

            using (StreamWriter csvFile = new StreamWriter(csvFilePath))
            {
                for (int currTrackIndex = 1; currTrackIndex <= tracks.Count; currTrackIndex++)
                {
                    IITTrack fileTrack = tracks[currTrackIndex];
                    Console.WriteLine("{0},{1},{2},{3}", fileTrack.Artist, fileTrack.TrackNumber, fileTrack.Name, fileTrack.PlayedCount);
                    csvFile.WriteLine("\"{0}\",\"{1}\",\"{2}\",\"{3}\"", fileTrack.Artist, fileTrack.TrackNumber, fileTrack.Name, fileTrack.PlayedCount);
                }
            }
        }

        private bool CheckIfUserWantsToAbort(string csvFilePath)
        {
            if (File.Exists(csvFilePath))
            {
                Console.WriteLine("File already exists. Replace? [Y|N]");
                ConsoleKeyInfo decision = Console.ReadKey(true);
                if (decision.KeyChar == 'y')
                {
                    File.Delete(csvFilePath);
                    Console.WriteLine("File deleted: {0}", csvFilePath);
                }
                else
                {
                    Console.WriteLine("Abort");
                    return true;
                }
            }

            return false;
        }

        private string GetFullCsvFilePath(string playlistName)
        {
            string name = String.Format("{0:yyyy.MM.dd} - {1}.csv", DateTime.Now, playlistName);
            string toReturn = Path.Combine(_settings.PlaylistPath, name);

            return toReturn;
        }

    }
}

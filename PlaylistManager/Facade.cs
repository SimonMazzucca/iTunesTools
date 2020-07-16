using iTunesLib;
using PlaylistManager.Utilities;
using System;
using System.IO;

namespace PlaylistManager
{
    public class Facade
    {

        Settings _settings;

        public Facade()
        {
            ISettingsFileAccess fileAccess = new SettingsFileAccess();
            SettingsFacade facade = new SettingsFacade(fileAccess);
            _settings = facade.GetSettings();
        }

        public void Run()
        {

            iTunesApp itunes = new iTunesApp();
            IITLibraryPlaylist mainLibrary = itunes.LibraryPlaylist;
            IITPlaylist playlist = itunes.LibrarySource.Playlists.GetPlaylist(_settings.Playlist);
            IITTrackCollection tracks = playlist.Tracks;

            string csvFilePath = GetFullCsvFilePath();

            //if (CheckIfUserWantsToAbort(csvFilePath))
            //    return;

            File.Delete(csvFilePath);
            using (StreamWriter csvFile = new StreamWriter(csvFilePath))
            {
                //Start from 0?
                for (int currTrackIndex = 1; currTrackIndex <= tracks.Count; currTrackIndex++)
                {
                    IITTrack fileTrack = tracks[currTrackIndex];
                    Console.WriteLine("{0},{1},{2},{3}", fileTrack.Artist, fileTrack.TrackNumber, fileTrack.Name, fileTrack.PlayedCount);
                    csvFile.WriteLine("{0},{1},{2},{3}", fileTrack.Artist, fileTrack.TrackNumber, fileTrack.Name, fileTrack.PlayedCount);
                }
            }
        }

        private bool CheckIfUserWantsToAbort(string csvFilePath)
        {
            if (File.Exists(csvFilePath))
            {
                Console.WriteLine("File already exists. Replace? [Y|N]");
                ConsoleKeyInfo decision = Console.ReadKey();
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

        private string GetFullCsvFilePath()
        {
            string name = String.Format("{0:yyyy.MM.dd} - {1}.csv", DateTime.Now, _settings.Playlist);
            string toReturn = Path.Combine(_settings.PlaylistPath, name);

            return toReturn;
        }
    }
}

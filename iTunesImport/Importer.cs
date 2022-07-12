using iTunesLib;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace iTunesImport
{

    public class Importer
    {

        #region Constants

        private const string TRACKS_PATH = @"{0}:\!Songs";
        private const string PLAYLIST_PATH = @"{0}:\";
        // Private Const PLAYLIST_PATH As String = "{0}:\M\Mazzucca, Simon"

        private const string LOG_PATH = @"C:\Temp";
        private const string LOG_FILE = "iTunesImport Report.txt";
        private const bool DEBUG_MODE = false;

        #endregion

        #region Definitions

        public enum ModeType
        {
            Import,
            FindDuplicates
        }

        #endregion

        #region Fields

        private string _CurrentFolder;
        private string _CurrentFile;
        private iTunesApp _iTunesApp;

        private StreamWriter _LogWriter;
        private ModeType _Mode = ModeType.Import;
        private string _CurrentPlaylist;

        private bool _SomeFilesMissing;

        private List<int> _IndexesOfPlaylistTracks = new List<int>();
        private List<string> _BadTracks = new List<string>();

        private string _MusicDrive;
        private string _TracksPath;
        private string _PlaylistPath;

        #endregion

        #region Private Properties

        private ModeType Mode
        {
            get
            {
                return _Mode;
            }
            set
            {
                _Mode = value;
            }
        }

        private string CurrentPlaylist
        {
            get
            {
                return _CurrentPlaylist;
            }
            set
            {
                _CurrentPlaylist = value;
            }
        }

        #endregion

        #region Events

        public event StatusChangedEventHandler StatusChanged;

        public delegate void StatusChangedEventHandler();
        public event FinishedEventHandler Finished;

        public delegate void FinishedEventHandler();

        #endregion

        #region Constructor/Destructor

        public Importer()
        {
            _MusicDrive = ConfigurationManager.AppSettings["MusicDrive"].ToString();
            _TracksPath = string.Format(TRACKS_PATH, _MusicDrive);
            _PlaylistPath = string.Format(PLAYLIST_PATH, _MusicDrive);
        }

        ~Importer()
        {
        }

        #endregion

        #region Public Properties

        public string CurrentFolder
        {
            get
            {
                return _CurrentFolder;
            }
            set
            {
                _CurrentFolder = value;
            }
        }

        public string CurrentFile
        {
            get
            {
                return _CurrentFile;
            }
            set
            {
                _CurrentFile = value;
            }
        }

        public bool SomeFilesMissing
        {
            get
            {
                return _SomeFilesMissing;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 09.28.16: moved _iTunesApp creation from constructor to here to allow app to open/close iTunes freely
        /// </summary>
        public void ImportLibrary()
        {
            Mode = ModeType.Import;

            _iTunesApp = new iTunesApp();

            LogInitialize();
            ProcessTracks();
            ProcessPlaylists();
            LogOrphanTracks();
            LogBadTracks();

            _iTunesApp.Quit();
            _iTunesApp = null;

            Finished?.Invoke();
        }

        public void FindDuplicates()
        {
            Mode = ModeType.FindDuplicates;

            _iTunesApp = new iTunesApp();

            LogInitialize();
            ProcessPlaylists();

            _iTunesApp.Quit();
            _iTunesApp = null;

            Finished?.Invoke();
        }

        public void ImportPlaylist(string strPlaylist)
        {
            var reader = new StreamReader(strPlaylist, Encoding.Default, true);
            string playlistPath = Path.GetDirectoryName(strPlaylist);
            string line;
            string track;

            LogOpen();
            CurrentPlaylist = strPlaylist;
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                track = Path.Combine(playlistPath, line);
                ImportTrack(track);
            }
            LogClose();

            reader.Close();
        }

        #endregion

        #region Private Methods

        private void ProcessTracks()
        {
            ImportTracks(_TracksPath);
            ClearResults();
        }

        private void ProcessPlaylists()
        {
            ImportPlaylists(_PlaylistPath);
            ClearResults();
        }

        private void ImportTracks(string path)
        {
            var root = new DirectoryInfo(path);
            var subfolders = root.GetDirectories();
            var tracks = Directory.GetFiles(path);

            // Recursive cases (if any)
            foreach (DirectoryInfo dirInfo in subfolders)
                ImportTracks(dirInfo.FullName);

            // Base cases (if any)
            foreach (string track in tracks)
            {
                CurrentFolder = Path.GetDirectoryName(track);
                CurrentFile = Path.GetFileName(track);
                ImportTrack(track);
                StatusChanged?.Invoke();
            }

        }

        private void ClearResults()
        {
            CurrentFolder = "";
            CurrentFile = "";
            StatusChanged?.Invoke();
        }

        private void ImportPlaylists(string playlistPath)
        {
            var root = new DirectoryInfo(playlistPath);
            var subfolders = root.GetDirectories();
            var tracks = Directory.GetFiles(playlistPath, "*.m3u");

            // Recursive cases (if any)
            foreach (DirectoryInfo directoryInfo in subfolders)
            {
                if (directoryInfo.Attributes == FileAttributes.Directory)
                {
                    ImportPlaylists(directoryInfo.FullName);
                }
            }

            // Base cases (if any)
            foreach (string playlist in tracks)
            {
                CurrentFolder = Path.GetDirectoryName(playlist);
                CurrentFile = Path.GetFileName(playlist);
                ImportPlaylist(playlist);
                StatusChanged?.Invoke();
            }

        }

        // I could have a 3rd mode to just look for orphan entries, but I run with Import 
        // mode enough that I can just have those two pieces work together
        private void ImportTrack(string trackPath)
        {
            IITOperationStatus status;
            if (ShouldImport(trackPath) && !DEBUG_MODE && Mode == ModeType.Import)
            {
                Debug.WriteLine(trackPath);

                //var testTrack1 = _iTunesApp.LibraryPlaylist.Tracks.get_ItemByName(trackPath);
                //var testTrack2 = _iTunesApp.LibraryPlaylist.Tracks.get_ItemByName("xxxxxxxxxx");

                status = _iTunesApp.LibraryPlaylist.AddFile(trackPath);

                if (status == null)
                {
                    _BadTracks.Add(trackPath);
                }
                else
                {
                    // This is not effective, but I can't find another way to access the collection
                    foreach (IITFileOrCDTrack track in status.Tracks)
                        _IndexesOfPlaylistTracks.Add(track.Index);

                    System.Threading.Thread.Sleep(1);
                }
            }
        }

        private bool ShouldImport(string trackPath)
        {
            var result = default(bool);

            if (trackPath.ToLower().EndsWith("mp3"))
            {
                if (File.Exists(trackPath))
                {
                    result = true;
                }
                else
                {
                    _SomeFilesMissing = true;
                    LogMissingFile(trackPath);
                }
            }
            else
            {
                // Ignore file type
            }

            return result;
        }

        private void LogMissingFile(string strMissingTrack)
        {
            string missingFilePath = Path.GetDirectoryName(strMissingTrack);
            string missingFile = Path.GetFileName(strMissingTrack);

            if (!string.IsNullOrEmpty(CurrentPlaylist))
            {
                _LogWriter.WriteLine(CurrentPlaylist);
            }
            _LogWriter.WriteLine(missingFilePath);
            _LogWriter.WriteLine(missingFile);
            _LogWriter.WriteLine();
        }

        // TODO: clone method and create HashSet of Locations
        // Only add items if missing from hash
        // Test
        // Add to SVN
        private void LogOrphanTracks()
        {
            var allTracks = _iTunesApp.LibraryPlaylist.Tracks;

            LogOpen();
            _LogWriter.WriteLine();
            _LogWriter.WriteLine("The following tracks are in iTunes but not in M3U playlists");
            _LogWriter.WriteLine(" 1) Good: add to M3U");
            _LogWriter.WriteLine(" 2) Bad: delete from iTunes");
            _LogWriter.WriteLine("-----------------------------------------------------------");

            foreach (IITTrack track in allTracks)
            {
                if (track is IITFileOrCDTrack)
                {
                    int libraryTrackIndex = track.Index;

                    if (!_IndexesOfPlaylistTracks.Contains(libraryTrackIndex))
                    {
                        IITFileOrCDTrack fileTrack = (IITFileOrCDTrack)track;
                        if (!fileTrack.Podcast)
                        {
                            _LogWriter.WriteLine("{0}{1}{2}", fileTrack.Artist + Constants.vbTab, fileTrack.Name + Constants.vbTab, fileTrack.Location);
                        }
                    }
                }
            }

            LogClose();

        }

        private void LogBadTracks()
        {
            if (_BadTracks.Count == 0)
                return;

            LogOpen();
            _LogWriter.WriteLine();
            _LogWriter.WriteLine("The following tracks are bad");

            foreach (string badTrack in _BadTracks)
            {
                _LogWriter.WriteLine(badTrack);
            }

            LogClose();
        }

        private void LogInitialize()
        {
            LogOpen();
            _LogWriter.WriteLine("---------------------------------------------------------------");
            _LogWriter.WriteLine(DateTime.Now);
            _LogWriter.WriteLine("---------------------------------------------------------------");
            _LogWriter.WriteLine();
            _LogWriter.WriteLine("The following tracks are in M3U playlists but not in the file system");
            _LogWriter.WriteLine("--------------------------------------------------------------------");
            LogClose();
        }

        private void LogOpen()
        {
            _LogWriter = new StreamWriter(Path.Combine(LOG_PATH, LOG_FILE), true);
        }

        private void LogClose()
        {
            _LogWriter.Close();
            _LogWriter = null;
        }

        #endregion

    }
}
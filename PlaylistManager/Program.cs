using System;

namespace PlaylistManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Facade facade = new Facade();

            //To debug
            //args = new string[] { "-l", "NP" };
            //args = new string[] { "-s", "NP" };

            if (args.Length != 2)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("PlaylistManager -l myPlaylist: logs myPlaylist tracks and related play count");
                Console.WriteLine("PlaylistManager -s myPlaylist: sets myPlaylist play count based on last logged entry");
            }

            var command = args[0];
            var playlist = args[1];

            facade.Run(command, playlist);

            Console.ReadLine();
        }
    }
}
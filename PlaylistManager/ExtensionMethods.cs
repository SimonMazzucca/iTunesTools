using iTunesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistManager
{
    public static class ExtensionMethods
    {

        public static IITPlaylist GetPlaylist(this IITPlaylistCollection playlists, string name)
        {
            //Is there a better way then to look at all fucking playlists ?
            foreach (IITPlaylist pl in playlists)
            {
                if (pl.Name == name)
                    return pl;
            }

            return null;
        }

    }
}

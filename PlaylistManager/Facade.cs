using PlaylistManager.Services;
using System;

namespace PlaylistManager
{
    public class Facade
    {
        internal void Run(string command, string playlist)
        {
            IService service;

            if (command == "-l") 
            {
                service = new PlayCountLogger();
            } 
            else if (command == "-s") 
            {
                service = new PlayCountSetter();
            }
            else
            {
                throw new NotImplementedException("Command not supported: " + command);
            }

            service.Run();
        }
    }
}

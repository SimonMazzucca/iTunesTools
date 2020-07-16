using System;

namespace PlaylistManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Facade logger = new Facade();
            logger.Run();

            Console.ReadLine();
        }
    }
}
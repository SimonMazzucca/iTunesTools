using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTunesReporting
{
    class Program
    {
        static void Main(string[] args)
        {
            Facade facade = new Facade();
            facade.Run();

            //Console.ReadLine();
        }
    }
}

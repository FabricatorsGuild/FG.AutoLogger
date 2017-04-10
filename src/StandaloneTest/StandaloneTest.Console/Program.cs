using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandaloneTest.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var standaloneTester = new StandaloneTester();
            standaloneTester.Run();

            System.Console.ReadKey();

        }
    }
}

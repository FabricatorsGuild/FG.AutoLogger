using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ConsoleApplication1.Diagnostics;
using ConsoleApplication1.Loggers;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ObjectMother.Current.GetName();
            IConsoleRunnerLogger logger = new ConsoleRunnerLogger(Process.GetCurrentProcess().Id, Environment.MachineName);
            var consoleRunner = new ConsoleRunner(logger) { Process = Process.GetCurrentProcess(), Name = ObjectMother.Current.GetName() };
            consoleRunner.Run();
        }
    }
}

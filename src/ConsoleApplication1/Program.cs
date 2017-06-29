using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ConsoleApplication1.Loggers;
using Microsoft.ServiceFabric.Actors;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ObjectMother.Current.GetName();
            /*IConsoleRunnerLogger logger = new ConsoleRunnerLogger(Process.GetCurrentProcess().Id, Environment.MachineName, new ActorId("xxx"));
            var consoleRunner = new ConsoleRunner(logger) { Process = Process.GetCurrentProcess(), Name = ObjectMother.Current.GetName() };
            consoleRunner.Run();*/

            /*
            var dependencyLogger = new DependencyLogger(Process.GetCurrentProcess().Id, Environment.MachineName, 55.ToString());

            while (true)
            {
                using (dependencyLogger.RecieveMessage("hello_recieved"))

                {
                    using (dependencyLogger.CallExternalComponent(new Uri("https://goodbye_call"), "dsaöjflsd fsdf"))
                    {
                        Task.Delay(1000).GetAwaiter().GetResult();
                    }
                }
            }
            */
        }
    }
}

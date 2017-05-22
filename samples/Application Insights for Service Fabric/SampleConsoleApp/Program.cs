using System;
using System.Diagnostics;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            ObjectMother.Current.GetName();
            var actorId = new Microsoft.ServiceFabric.Actors.ActorId("xxx");
            IConsoleRunnerLogger logger = new ConsoleRunnerLogger(Process.GetCurrentProcess().Id, Environment.MachineName, actorId);
            var consoleRunner = new ConsoleRunner(logger) { Process = Process.GetCurrentProcess(), Name = ObjectMother.Current.GetName() };
            consoleRunner.Run();
        }
    }
}

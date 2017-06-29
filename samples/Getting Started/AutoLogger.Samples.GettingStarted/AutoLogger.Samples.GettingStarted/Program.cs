using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoLogger.Samples.GettingStarted
{
	class Program
	{
		static void Main(string[] args)
		{
			var logger = new ConsoleLogger(Process.GetCurrentProcess());
			logger.StartRunMain(args);

			Console.WriteLine("Press any key");
			var key = Console.ReadKey();
			logger.KeyPressed(key);

			logger.StopRunMain();
		}
	}
}

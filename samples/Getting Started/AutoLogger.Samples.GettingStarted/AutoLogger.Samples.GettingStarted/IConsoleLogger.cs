using System;

namespace AutoLogger.Samples.GettingStarted
{
	public interface IConsoleLogger
	{
		void StartRunMain(string[] args);
		void StopRunMain();	
		void KeyPressed(ConsoleKeyInfo key);
	}
}
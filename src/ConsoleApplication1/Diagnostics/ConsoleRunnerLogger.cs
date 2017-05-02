/*******************************************************************************************
*  This class is autogenerated from the class ConsoleRunnerLogger
*  Do not directly update this class as changes will be lost on rebuild.
*******************************************************************************************/
using System;
using ConsoleApplication1.Loggers;

namespace ConsoleApplication1.Diagnostics
{
	internal sealed class ConsoleRunnerLogger : IConsoleRunnerLogger
	{
		

		public ConsoleRunnerLogger(
			)
		{
			
		}

		public void RunnerCreated(
			)
		{
			Sample.Current.RunnerCreated(
				
			);

			System.Diagnostics.Debug.WriteLine($"[ConsoleRunner] ERR: RunnerCreated");
           
    
		}



		public void RunnerDestroyed(
			)
		{
			Sample.Current.RunnerDestroyed(
				
			);

			System.Diagnostics.Debug.WriteLine($"[ConsoleRunner] ERR: RunnerDestroyed");
           
    
		}



		public void WaitingForKeyPress(
			)
		{
			Sample.Current.WaitingForKeyPress(
				
			);

			System.Diagnostics.Debug.WriteLine($"[ConsoleRunner] ERR: WaitingForKeyPress");
           
    
		}



		public void KeyPressed(
			System.ConsoleKey key)
		{
			Sample.Current.KeyPressed(
				key
			);

			System.Diagnostics.Debug.WriteLine($"[ConsoleRunner] ERR: KeyPressed");
           
			System.Diagnostics.Debug.WriteLine($"\tkey.ToString():\t{key.ToString()}");
    
		}



		public void UnsupportedKeyError(
			System.Exception ex)
		{
			Sample.Current.UnsupportedKeyError(
				ex
			);

			System.Diagnostics.Debug.WriteLine($"[ConsoleRunner, Error] ERR: UnsupportedKeyError");
           
			System.Diagnostics.Debug.WriteLine($"\tex.Message:\t{ex.Message}");
			System.Diagnostics.Debug.WriteLine($"\tex.Source:\t{ex.Source}");
			System.Diagnostics.Debug.WriteLine($"\tex.GetType().FullName:\t{ex.GetType().FullName}");
			System.Diagnostics.Debug.WriteLine($"\tex.AsJson():\t{ex.AsJson()}");
    
		}



		public void StartLoop(
			)
		{
			Sample.Current.StartLoop(
				
			);
			_loopStopwatch.Restart();

			System.Diagnostics.Debug.WriteLine($"[ConsoleRunner] ERR: StartLoop");
           
    
		}

		private System.Diagnostics.Stopwatch _loopStopwatch = new System.Diagnostics.Stopwatch();


		public void StopLoop(
			)
		{
			Sample.Current.StopLoop(
				
			);
			_loopStopwatch.Stop();

			System.Diagnostics.Debug.WriteLine($"[ConsoleRunner] ERR: StopLoop");
           
    
		}



		public void RandomIntsGenerated(
			int[] values)
		{
			Sample.Current.RandomIntsGenerated(
				values
			);

			System.Diagnostics.Debug.WriteLine($"[ConsoleRunner] ERR: RandomIntsGenerated");
           
			System.Diagnostics.Debug.WriteLine($"\tvalues.ToString():\t{values.ToString()}");
    
		}



	}
}

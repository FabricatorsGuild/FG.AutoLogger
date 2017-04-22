/*******************************************************************************************
*  This class is autogenerated from the class ConsoleRunnerLogger
*  Do not directly update this class as changes will be lost on rebuild.
*******************************************************************************************/
using System;
using ConsoleApplication1.Loggers;

namespace ConsoleApplication1
{
	internal sealed class ConsoleRunnerLogger : IConsoleRunnerLogger
	{
		private readonly bool _autogenerated;
		private readonly string _machineName;

		public ConsoleRunnerLogger(
			bool autogenerated,
			string machineName)
		{
			_autogenerated = autogenerated;
			_machineName = machineName;
		}

		public void RunnerCreated(
			)
		{
			DefaultEventSource.eventsource.Current.RunnerCreated(
				_autogenerated, 
				_machineName
			);

			System.Diagnostics.Debug.WriteLine($"[] ERR: RunnerCreated");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
    
		}


		public void RunnerDestroyed(
			)
		{
			DefaultEventSource.eventsource.Current.RunnerDestroyed(
				_autogenerated, 
				_machineName
			);

			System.Diagnostics.Debug.WriteLine($"[] ERR: RunnerDestroyed");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
    
		}


		public void WaitingForKeyPress(
			)
		{
			DefaultEventSource.eventsource.Current.WaitingForKeyPress(
				_autogenerated, 
				_machineName
			);

			System.Diagnostics.Debug.WriteLine($"[] ERR: WaitingForKeyPress");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
    
		}


		public void KeyPressed(
			System.ConsoleKey key)
		{
			DefaultEventSource.eventsource.Current.KeyPressed(
				_autogenerated, 
				_machineName, 
				key
			);

			System.Diagnostics.Debug.WriteLine($"[] ERR: KeyPressed");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\tkey.ToString():\t{key.ToString()}");
    
		}


		public void UnsupportedKeyError(
			System.Exception ex)
		{
			DefaultEventSource.eventsource.Current.UnsupportedKeyError(
				_autogenerated, 
				_machineName, 
				ex
			);

			System.Diagnostics.Debug.WriteLine($"[Error] ERR: UnsupportedKeyError");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\tex.Message:\t{ex.Message}");
			System.Diagnostics.Debug.WriteLine($"\tex.Source:\t{ex.Source}");
			System.Diagnostics.Debug.WriteLine($"\tex.GetType().FullName:\t{ex.GetType().FullName}");
			System.Diagnostics.Debug.WriteLine($"\tex.AsJson():\t{ex.AsJson()}");
    
		}


		public void StartLoop(
			)
		{
			DefaultEventSource.eventsource.Current.StartLoop(
				_autogenerated, 
				_machineName
			);

			System.Diagnostics.Debug.WriteLine($"[] ERR: StartLoop");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
    
		}


		public void StopLoop(
			)
		{
			DefaultEventSource.eventsource.Current.StopLoop(
				_autogenerated, 
				_machineName
			);

			System.Diagnostics.Debug.WriteLine($"[] ERR: StopLoop");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
    
		}


		public void RandomIntsGenerated(
			int[] values)
		{
			DefaultEventSource.eventsource.Current.RandomIntsGenerated(
				_autogenerated, 
				_machineName, 
				values
			);

			System.Diagnostics.Debug.WriteLine($"[] ERR: RandomIntsGenerated");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\tvalues.ToString():\t{values.ToString()}");
    
		}


	}
}
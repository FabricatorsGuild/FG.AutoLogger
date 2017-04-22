/*******************************************************************************************
*  This class is autogenerated from the class ConsoleLogger
*  Do not directly update this class as changes will be lost on rebuild.
*******************************************************************************************/
using System;
using ConsoleApplication1.Loggers;

namespace ConsoleApplication1
{
	internal sealed class ConsoleLogger : IConsoleLogger
	{
		private readonly bool _autogenerated;
		private readonly string _machineName;

		public ConsoleLogger(
			bool autogenerated,
			string machineName)
		{
			_autogenerated = autogenerated;
			_machineName = machineName;
		}

		public void SayHello(
			string message)
		{
			DefaultEventSource.eventsource.Current.SayHello(
				_autogenerated, 
				_machineName, 
				message
			);

			System.Diagnostics.Debug.WriteLine($"[] ERR: SayHello");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\tmessage:\t{message}");
    
		}


		public void Message(
			string message)
		{
			DefaultEventSource.eventsource.Current.Message(
				_autogenerated, 
				_machineName, 
				message
			);

			System.Diagnostics.Debug.WriteLine($"[] ERR: Message");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\tmessage:\t{message}");
    
		}


		public void Error(
			System.Exception exception)
		{
			DefaultEventSource.eventsource.Current.Error(
				_autogenerated, 
				_machineName, 
				exception
			);

			System.Diagnostics.Debug.WriteLine($"[Error] ERR: Error");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\texception.Message:\t{exception.Message}");
			System.Diagnostics.Debug.WriteLine($"\texception.Source:\t{exception.Source}");
			System.Diagnostics.Debug.WriteLine($"\texception.GetType().FullName:\t{exception.GetType().FullName}");
			System.Diagnostics.Debug.WriteLine($"\texception.AsJson():\t{exception.AsJson()}");
    
		}


		public void SayGoodbye(
			string goodbye,
			System.DateTime nightTime)
		{
			DefaultEventSource.eventsource.Current.SayGoodbye(
				_autogenerated, 
				_machineName, 
				goodbye, 
				nightTime
			);

			System.Diagnostics.Debug.WriteLine($"[] ERR: SayGoodbye");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\tgoodbye:\t{goodbye}");
			System.Diagnostics.Debug.WriteLine($"\tnightTime.ToString():\t{nightTime.ToString()}");
    
		}


		public void Special(
			ConsoleApplication1.Loggers.Special special)
		{
			DefaultEventSource.eventsource.Current.Special(
				_autogenerated, 
				_machineName, 
				special
			);

			System.Diagnostics.Debug.WriteLine($"[] ERR: Special");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\tspecial.ToString():\t{special.ToString()}");
    
		}


	}
}

/*******************************************************************************************
*  This class is autogenerated from the class ConsoleLogger
*  Do not directly update this class as changes will be lost on rebuild.
*******************************************************************************************/
using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace AutoLogger.Samples.GettingStarted
{
	internal sealed partial class DefaultEventSource
	{

		private const int StartRunMainEventId = 1001;

		[Event(StartRunMainEventId, Level = EventLevel.LogAlways, Message = "Start Run Main {2}", Keywords = Keywords.Console, Opcode = EventOpcode.Start)]
		private void StartRunMain(
			string machineName, 
			int processId, 
			string args)
		{
			WriteEvent(
				StartRunMainEventId, 
				machineName, 
				processId, 
				args);
		}

		[NonEvent]
		public void StartRunMain(
			System.Diagnostics.Process process, 
			string[] args)
		{
			if (this.IsEnabled())
			{
				StartRunMain(
					process.MachineName, 
					process.Id, 
					args.ToString());
			}
		}


		private const int StopRunMainEventId = 2002;

		[Event(StopRunMainEventId, Level = EventLevel.LogAlways, Message = "Stop Run Main", Keywords = Keywords.Console, Opcode = EventOpcode.Stop)]
		private void StopRunMain(
			string machineName, 
			int processId)
		{
			WriteEvent(
				StopRunMainEventId, 
				machineName, 
				processId);
		}

		[NonEvent]
		public void StopRunMain(
			System.Diagnostics.Process process)
		{
			if (this.IsEnabled())
			{
				StopRunMain(
					process.MachineName, 
					process.Id);
			}
		}


		private const int KeyPressedEventId = 3003;

		[Event(KeyPressedEventId, Level = EventLevel.LogAlways, Message = "Key Pressed {2}", Keywords = Keywords.Console)]
		private void KeyPressed(
			string machineName, 
			int processId, 
			string key)
		{
			WriteEvent(
				KeyPressedEventId, 
				machineName, 
				processId, 
				key);
		}

		[NonEvent]
		public void KeyPressed(
			System.Diagnostics.Process process, 
			System.ConsoleKeyInfo key)
		{
			if (this.IsEnabled())
			{
				KeyPressed(
					process.MachineName, 
					process.Id, 
					key.ToString());
			}
		}


	}
}
/*******************************************************************************************
*  This class is autogenerated from the class DependencyLogger
*  Do not directly update this class as changes will be lost on rebuild.
*******************************************************************************************/
using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace FG.Diagnostics.AutoLogger.Samples.ConsoleApplication1
{
	internal sealed partial class FGDiagnosticsAutoLoggerSamplesConsoleApplication1EventSource
	{

		private const int StartCallExternalComponentEventId = 3001;

		[Event(StartCallExternalComponentEventId, Level = EventLevel.LogAlways, Message = "Start Call External Component {2} {3}", Keywords = Keywords.Dependency, Opcode = EventOpcode.Start, Task = Tasks.CallExternalComponent)]
		private void StartCallExternalComponent(
			bool autogenerated, 
			string machineName, 
			string requestName, 
			string content)
		{
			WriteEvent(
				StartCallExternalComponentEventId, 
				autogenerated, 
				machineName, 
				requestName, 
				content);
		}

		[NonEvent]
		public void StartCallExternalComponent(
			bool autogenerated, 
			string machineName, 
			System.Uri requestName, 
			string content)
		{
			if (this.IsEnabled())
			{
				StartCallExternalComponent(
					autogenerated, 
					Environment.MachineName, 
					requestName.ToString(), 
					content);
			}
		}


		private const int StopCallExternalComponentEventId = 6002;

		[Event(StopCallExternalComponentEventId, Level = EventLevel.LogAlways, Message = "Stop Call External Component {2} {3}", Keywords = Keywords.Dependency, Opcode = EventOpcode.Stop, Task = Tasks.CallExternalComponent)]
		private void StopCallExternalComponent(
			bool autogenerated, 
			string machineName, 
			string requestName, 
			string content)
		{
			WriteEvent(
				StopCallExternalComponentEventId, 
				autogenerated, 
				machineName, 
				requestName, 
				content);
		}

		[NonEvent]
		public void StopCallExternalComponent(
			bool autogenerated, 
			string machineName, 
			System.Uri requestName, 
			string content)
		{
			if (this.IsEnabled())
			{
				StopCallExternalComponent(
					autogenerated, 
					Environment.MachineName, 
					requestName.ToString(), 
					content);
			}
		}


		private const int StartRecieveMessageEventId = 9003;

		[Event(StartRecieveMessageEventId, Level = EventLevel.LogAlways, Message = "{2}", Keywords = Keywords.Dependency, Opcode = EventOpcode.Start, Task = Tasks.RecieveMessage)]
		public void StartRecieveMessage(
			bool autogenerated, 
			string machineName, 
			string message)
		{
			WriteEvent(
				StartRecieveMessageEventId, 
				autogenerated, 
				machineName, 
				message);
		}


		private const int StopRecieveMessageEventId = 12004;

		[Event(StopRecieveMessageEventId, Level = EventLevel.LogAlways, Message = "{2}", Keywords = Keywords.Dependency, Opcode = EventOpcode.Stop, Task = Tasks.RecieveMessage)]
		public void StopRecieveMessage(
			bool autogenerated, 
			string machineName, 
			string message)
		{
			WriteEvent(
				StopRecieveMessageEventId, 
				autogenerated, 
				machineName, 
				message);
		}


	}
}
/*******************************************************************************************
*  This class is autogenerated from the class ConsoleRunnerLogger
*  Do not directly update this class as changes will be lost on rebuild.
*******************************************************************************************/
using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Samples
{
	internal sealed partial class DefaultEventSource
	{

		private const int RunnerCreatedEventId = 1001;

		[Event(RunnerCreatedEventId, Level = EventLevel.LogAlways, Message = "Runner Created", Keywords = Keywords.ConsoleRunner)]
		private void RunnerCreated(
			int processId, 
			string machineName, 
			string actor)
		{
			WriteEvent(
				RunnerCreatedEventId, 
				processId, 
				machineName, 
				actor);
		}

		[NonEvent]
		public void RunnerCreated(
			int processId, 
			string machineName, 
			Microsoft.ServiceFabric.Actors.ActorId actorId)
		{
			if (this.IsEnabled())
			{
				RunnerCreated(
					processId, 
					Environment.MachineName, 
					actorId.ToString());
			}
		}


		private const int RunnerDestroyedEventId = 2002;

		[Event(RunnerDestroyedEventId, Level = EventLevel.LogAlways, Message = "Runner Destroyed", Keywords = Keywords.ConsoleRunner)]
		private void RunnerDestroyed(
			int processId, 
			string machineName, 
			string actor)
		{
			WriteEvent(
				RunnerDestroyedEventId, 
				processId, 
				machineName, 
				actor);
		}

		[NonEvent]
		public void RunnerDestroyed(
			int processId, 
			string machineName, 
			Microsoft.ServiceFabric.Actors.ActorId actorId)
		{
			if (this.IsEnabled())
			{
				RunnerDestroyed(
					processId, 
					Environment.MachineName, 
					actorId.ToString());
			}
		}


		private const int WaitingForKeyPressEventId = 3003;

		[Event(WaitingForKeyPressEventId, Level = EventLevel.LogAlways, Message = "Waiting For Key Press", Keywords = Keywords.ConsoleRunner)]
		private void WaitingForKeyPress(
			int processId, 
			string machineName, 
			string actor)
		{
			WriteEvent(
				WaitingForKeyPressEventId, 
				processId, 
				machineName, 
				actor);
		}

		[NonEvent]
		public void WaitingForKeyPress(
			int processId, 
			string machineName, 
			Microsoft.ServiceFabric.Actors.ActorId actorId)
		{
			if (this.IsEnabled())
			{
				WaitingForKeyPress(
					processId, 
					Environment.MachineName, 
					actorId.ToString());
			}
		}


		private const int KeyPressedEventId = 4004;

		[Event(KeyPressedEventId, Level = EventLevel.LogAlways, Message = "Key Pressed {3}", Keywords = Keywords.ConsoleRunner)]
		private void KeyPressed(
			int processId, 
			string machineName, 
			string actor, 
			string key)
		{
			WriteEvent(
				KeyPressedEventId, 
				processId, 
				machineName, 
				actor, 
				key);
		}

		[NonEvent]
		public void KeyPressed(
			int processId, 
			string machineName, 
			Microsoft.ServiceFabric.Actors.ActorId actorId, 
			System.ConsoleKey key)
		{
			if (this.IsEnabled())
			{
				KeyPressed(
					processId, 
					Environment.MachineName, 
					actorId.ToString(), 
					key.ToString());
			}
		}


		private const int UnsupportedKeyErrorEventId = 5005;

		[Event(UnsupportedKeyErrorEventId, Level = EventLevel.LogAlways, Message = "{3}", Keywords = Keywords.ConsoleRunner | Keywords.Error)]
		private void UnsupportedKeyError(
			int processId, 
			string machineName, 
			string actor, 
			string message, 
			string source, 
			string exceptionTypeName, 
			string exception)
		{
			WriteEvent(
				UnsupportedKeyErrorEventId, 
				processId, 
				machineName, 
				actor, 
				message, 
				source, 
				exceptionTypeName, 
				exception);
		}

		[NonEvent]
		public void UnsupportedKeyError(
			int processId, 
			string machineName, 
			Microsoft.ServiceFabric.Actors.ActorId actorId, 
			System.Exception ex)
		{
			if (this.IsEnabled())
			{
				UnsupportedKeyError(
					processId, 
					Environment.MachineName, 
					actorId.ToString(), 
					ex.Message, 
					ex.Source, 
					ex.GetType().FullName, 
					ex.AsJson());
			}
		}


		private const int StartLoopEventId = 6006;

		[Event(StartLoopEventId, Level = EventLevel.LogAlways, Message = "Start Loop", Keywords = Keywords.ConsoleRunner, Opcode = EventOpcode.Start)]
		private void StartLoop(
			int processId, 
			string machineName, 
			string actor)
		{
			WriteEvent(
				StartLoopEventId, 
				processId, 
				machineName, 
				actor);
		}

		[NonEvent]
		public void StartLoop(
			int processId, 
			string machineName, 
			Microsoft.ServiceFabric.Actors.ActorId actorId)
		{
			if (this.IsEnabled())
			{
				StartLoop(
					processId, 
					Environment.MachineName, 
					actorId.ToString());
			}
		}


		private const int StopLoopEventId = 7007;

		[Event(StopLoopEventId, Level = EventLevel.LogAlways, Message = "Stop Loop", Keywords = Keywords.ConsoleRunner, Opcode = EventOpcode.Stop)]
		private void StopLoop(
			int processId, 
			string machineName, 
			string actor)
		{
			WriteEvent(
				StopLoopEventId, 
				processId, 
				machineName, 
				actor);
		}

		[NonEvent]
		public void StopLoop(
			int processId, 
			string machineName, 
			Microsoft.ServiceFabric.Actors.ActorId actorId)
		{
			if (this.IsEnabled())
			{
				StopLoop(
					processId, 
					Environment.MachineName, 
					actorId.ToString());
			}
		}


		private const int RandomIntsGeneratedEventId = 8008;

		[Event(RandomIntsGeneratedEventId, Level = EventLevel.LogAlways, Message = "Random Ints Generated {3}", Keywords = Keywords.ConsoleRunner)]
		private void RandomIntsGenerated(
			int processId, 
			string machineName, 
			string actor, 
			string values)
		{
			WriteEvent(
				RandomIntsGeneratedEventId, 
				processId, 
				machineName, 
				actor, 
				values);
		}

		[NonEvent]
		public void RandomIntsGenerated(
			int processId, 
			string machineName, 
			Microsoft.ServiceFabric.Actors.ActorId actorId, 
			int[] values)
		{
			if (this.IsEnabled())
			{
				RandomIntsGenerated(
					processId, 
					Environment.MachineName, 
					actorId.ToString(), 
					values.ToString());
			}
		}


	}
}
/*******************************************************************************************
*  This class is autogenerated from the class IDomainLogger
*  Do not directly update this class as changes will be lost on rebuild.
*******************************************************************************************/
using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace StandaloneTest
{
	internal sealed partial class DefaultEventSource
	{

        
		private const int StandaloneTestInvokedEventId = 1000;

		[Event(StandaloneTestInvokedEventId, Level = EventLevel.LogAlways, Message = "{3}", Keywords = Keywords.Domain)]
		public void StandaloneTestInvoked(
			int processId, 
			Guid correlationId, 
			string message, 
			int tickcount)
		{
			WriteEvent(
				StandaloneTestInvokedEventId,
				processId, 
				correlationId, 
				message, 
				tickcount);
		}

        
		[NonEvent]
		public void StandaloneTestFailed(
			int processId, 
			Guid correlationId, 
			System.Exception exception)
		{
			if (this.IsEnabled())
			{
				StandaloneTestFailed(
					processId, 
					correlationId, 
					exception.Message, 
					exception.Source, 
					exception.GetType().FullName, 
					exception.AsJson());
			}
		}

		private const int StandaloneTestFailedEventId = 1001;

		[Event(StandaloneTestFailedEventId, Level = EventLevel.LogAlways, Message = "{5}", Keywords = Keywords.Domain)]
		private void StandaloneTestFailed(
			int processId, 
			Guid correlationId, 
			string message, 
			string source, 
			string exceptionTypeName, 
			string exception)
		{
			WriteEvent(
				StandaloneTestFailedEventId,
				processId, 
				correlationId, 
				message, 
				source, 
				exceptionTypeName, 
				exception);
		}

        
		private const int StandaloneTestRunningEventId = 1002;

		[Event(StandaloneTestRunningEventId, Level = EventLevel.LogAlways, Message = "{2}", Keywords = Keywords.Domain)]
		public void StandaloneTestRunning(
			int processId, 
			Guid correlationId, 
			int iteration)
		{
			WriteEvent(
				StandaloneTestRunningEventId,
				processId, 
				correlationId, 
				iteration);
		}

	}
}
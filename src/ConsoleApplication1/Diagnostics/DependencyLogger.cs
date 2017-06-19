/*******************************************************************************************
*  This class is autogenerated from the class DependencyLogger
*  Do not directly update this class as changes will be lost on rebuild.
*******************************************************************************************/
using System;
using ConsoleApplication1.Loggers;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using CodeEffect.Diagnostics.EventSourceGenerator.AI;


namespace ConsoleApplication1.Diagnostics
{
	internal sealed class DependencyLogger : IDependencyLogger
	{
		private readonly int _processId;
		private readonly string _machineName;
		private readonly string _requestName;
		// Hello from extension
		private readonly Microsoft.ApplicationInsights.TelemetryClient _telemetryClient;

		public DependencyLogger(
			int processId,
			string machineName,
			string requestName)
		{
			_processId = processId;
			_machineName = machineName;
			_requestName = requestName;
			// Do stuff in the constructor
			
            _telemetryClient = new Microsoft.ApplicationInsights.TelemetryClient();
            _telemetryClient.Context.User.Id = Environment.UserName;
            _telemetryClient.Context.Session.Id = Guid.NewGuid().ToString();
            _telemetryClient.Context.Device.OperatingSystem = Environment.OSVersion.ToString();

		}

		public System.IDisposable CallExternalComponent(
			System.Uri requestName,
			string content)
		{
			Sample.Current.StartCallExternalComponent(
				_processId, 
				_machineName, 
				requestName, 
				content
			);

			System.Diagnostics.Debug.WriteLine($"[Dependency] ERR: StartCallExternalComponent");
           
			System.Diagnostics.Debug.WriteLine($"\t_processId:\t{_processId}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\trequestName.ToString():\t{requestName.ToString()}");
			System.Diagnostics.Debug.WriteLine($"\tcontent:\t{content}");
			_callExternalComponentStopwatch.Restart();

			var callExternalComponentOperationHolder = _telemetryClient.StartOperation<DependencyTelemetry>(requestName.ToString() ?? "callExternalComponent");
			callExternalComponentOperationHolder.Telemetry.Properties.Add("ProcessId", _processId.ToString());
			callExternalComponentOperationHolder.Telemetry.Properties.Add("MachineName", Environment.MachineName);
			callExternalComponentOperationHolder.Telemetry.Properties.Add("RequestName", requestName.ToString());
			callExternalComponentOperationHolder.Telemetry.Properties.Add("Content", content);
			return new ScopeWrapper<DependencyTelemetry>(_telemetryClient, callExternalComponentOperationHolder, () => StopCallExternalComponent(requestName,content));
    
		}

		private System.Diagnostics.Stopwatch _callExternalComponentStopwatch = new System.Diagnostics.Stopwatch();



		public void StopCallExternalComponent(
			System.Uri requestName,
			string content)
		{
			Sample.Current.StopCallExternalComponent(
				_processId, 
				_machineName, 
				requestName, 
				content
			);

			System.Diagnostics.Debug.WriteLine($"[Dependency] ERR: StopCallExternalComponent");
           
			System.Diagnostics.Debug.WriteLine($"\t_processId:\t{_processId}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\trequestName.ToString():\t{requestName.ToString()}");
			System.Diagnostics.Debug.WriteLine($"\tcontent:\t{content}");
			_callExternalComponentStopwatch.Stop();
    
		}




		public System.IDisposable RecieveMessage(
			string message)
		{
			Sample.Current.StartRecieveMessage(
				_processId, 
				_machineName, 
				_requestName, 
				message
			);

			System.Diagnostics.Debug.WriteLine($"[Dependency] ERR: StartRecieveMessage");
           
			System.Diagnostics.Debug.WriteLine($"\t_processId:\t{_processId}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\tSystem.Diagnostics.Process.GetCurrentProcess().Id.ToString():\t{System.Diagnostics.Process.GetCurrentProcess().Id.ToString()}");
			System.Diagnostics.Debug.WriteLine($"\tmessage:\t{message}");
			_recieveMessageStopwatch.Restart();

			var recieveMessageOperationHolder = _telemetryClient.StartOperation<RequestTelemetry>(System.Diagnostics.Process.GetCurrentProcess().Id.ToString() ?? "recieveMessage");
			recieveMessageOperationHolder.Telemetry.Properties.Add("ProcessId", _processId.ToString());
			recieveMessageOperationHolder.Telemetry.Properties.Add("MachineName", Environment.MachineName);
			recieveMessageOperationHolder.Telemetry.Properties.Add("RequestName", System.Diagnostics.Process.GetCurrentProcess().Id.ToString());
			recieveMessageOperationHolder.Telemetry.Properties.Add("Message", message);
			return new ScopeWrapper<RequestTelemetry>(_telemetryClient, recieveMessageOperationHolder, () => StopRecieveMessage(message));
    
		}

		private System.Diagnostics.Stopwatch _recieveMessageStopwatch = new System.Diagnostics.Stopwatch();



		public void StopRecieveMessage(
			string message)
		{
			Sample.Current.StopRecieveMessage(
				_processId, 
				_machineName, 
				_requestName, 
				message
			);

			System.Diagnostics.Debug.WriteLine($"[Dependency] ERR: StopRecieveMessage");
           
			System.Diagnostics.Debug.WriteLine($"\t_processId:\t{_processId}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\tSystem.Diagnostics.Process.GetCurrentProcess().Id.ToString():\t{System.Diagnostics.Process.GetCurrentProcess().Id.ToString()}");
			System.Diagnostics.Debug.WriteLine($"\tmessage:\t{message}");
			_recieveMessageStopwatch.Stop();
    
		}




		public void DoDirtyStuff(
			)
		{
			Sample.Current.DoDirtyStuff(
				_processId, 
				_machineName, 
				_requestName
			);

			System.Diagnostics.Debug.WriteLine($"[Dependency] ERR: DoDirtyStuff");
           
			System.Diagnostics.Debug.WriteLine($"\t_processId:\t{_processId}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\tSystem.Diagnostics.Process.GetCurrentProcess().Id.ToString():\t{System.Diagnostics.Process.GetCurrentProcess().Id.ToString()}");
			_telemetryClient.TrackEvent(
	            nameof(DoDirtyStuff),
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
	                {"ProcessId", _processId.ToString()},
                    {"MachineName", Environment.MachineName},
                    {"RequestName", System.Diagnostics.Process.GetCurrentProcess().Id.ToString()}
	            });
    
		}




	}
}

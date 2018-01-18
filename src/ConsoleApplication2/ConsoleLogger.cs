/*******************************************************************************************
*  This class is autogenerated from the class ConsoleLogger
*  Do not directly update this class as changes will be lost on rebuild.
*******************************************************************************************/
using System;
using System.Collections.Generic;
using FG.Diagnostics.AutoLogger.Samples.ConsoleApplication1.Loggers;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System.Runtime.Remoting.Messaging;


namespace Ce.Labs.Samples
{
	internal sealed class ConsoleLogger : IConsoleLogger
	{
	    private sealed class ScopeWrapper : IDisposable
        {
            private readonly IEnumerable<IDisposable> _disposables;

            public ScopeWrapper(IEnumerable<IDisposable> disposables)
            {
                _disposables = disposables;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    foreach (var disposable in _disposables)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

	    private sealed class ScopeWrapperWithAction : IDisposable
        {
            private readonly Action _onStop;

            internal static IDisposable Wrap(Func<IDisposable> wrap)
            {
                return wrap();
            }

            public ScopeWrapperWithAction(Action onStop)
            {
                _onStop = onStop;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _onStop?.Invoke();
                }
            }
        }


		private readonly bool _autogenerated;
		private readonly string _machineName;
		// Hello from extension
		private readonly Microsoft.ApplicationInsights.TelemetryClient _telemetryClient;

        public sealed class OperationHolder
        {
            public static void StartOperation(IOperationHolder<RequestTelemetry> aiOperationHolder)
            {
                OperationHolder.Current = new OperationHolder() {AIOperationHolder = aiOperationHolder};
            }

            public static IOperationHolder<RequestTelemetry> StopOperation()
            {
                var aiOperationHolder = OperationHolder.Current.AIOperationHolder;
                OperationHolder.Current = null;

                return aiOperationHolder;
            }

            private IOperationHolder<RequestTelemetry> AIOperationHolder { get; set; }

            private static readonly string ContextKey = Guid.NewGuid().ToString();

            public static OperationHolder Current
            {
                get { return (OperationHolder)CallContext.LogicalGetData(ContextKey); }
                internal set
                {
                    if (value == null)
                    {
                        CallContext.FreeNamedDataSlot(ContextKey);
                    }
                    else
                    {
                        CallContext.LogicalSetData(ContextKey, value);
                    }
                }
            }
        }

		public ConsoleLogger(
			bool autogenerated,
			string machineName)
		{
			_autogenerated = autogenerated;
			_machineName = machineName;
			// Do stuff in the constructor
			
            _telemetryClient = new Microsoft.ApplicationInsights.TelemetryClient();
            _telemetryClient.Context.User.Id = Environment.UserName;
            _telemetryClient.Context.Session.Id = Guid.NewGuid().ToString();
            _telemetryClient.Context.Device.OperatingSystem = Environment.OSVersion.ToString();

		}

		public void SayHello(
			string message)
		{
			FGDiagnosticsAutoLoggerSamplesConsoleApplication1EventSource.Current.SayHello(
				_autogenerated, 
				_machineName, 
				message
			);

			System.Diagnostics.Debug.WriteLine($"[Console] ERR: SayHello");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\tmessage:\t{message}");
			_telemetryClient.TrackEvent(
	            nameof(SayHello),
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
	                {"Autogenerated", _autogenerated.ToString()},
                    {"MachineName", Environment.MachineName},
                    {"Message", message}
	            });
    
		}




		public void Message(
			string message)
		{
			FGDiagnosticsAutoLoggerSamplesConsoleApplication1EventSource.Current.Message(
				_autogenerated, 
				_machineName, 
				message
			);

			System.Diagnostics.Debug.WriteLine($"[Console] ERR: Message");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\tmessage:\t{message}");
			_telemetryClient.TrackEvent(
	            nameof(Message),
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
	                {"Autogenerated", _autogenerated.ToString()},
                    {"MachineName", Environment.MachineName},
                    {"Message", message}
	            });
    
		}




		public void Error(
			System.Exception exception)
		{
			FGDiagnosticsAutoLoggerSamplesConsoleApplication1EventSource.Current.Error(
				_autogenerated, 
				_machineName, 
				exception
			);

			System.Diagnostics.Debug.WriteLine($"[Console, Error] ERR: Error");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\texception.Message:\t{exception.Message}");
			System.Diagnostics.Debug.WriteLine($"\texception.Source:\t{exception.Source}");
			System.Diagnostics.Debug.WriteLine($"\texception.GetType().FullName:\t{exception.GetType().FullName}");
			System.Diagnostics.Debug.WriteLine($"\texception.AsJson():\t{exception.AsJson()}");
			_telemetryClient.TrackException(
	            exception,
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
                    { "Name", "Error" },
	                {"Autogenerated", _autogenerated.ToString()},
                    {"MachineName", Environment.MachineName},
                    {"Message", exception.Message},
                    {"Source", exception.Source},
                    {"ExceptionTypeName", exception.GetType().FullName},
                    {"Exception", exception.AsJson()}
	            });
    
		}




		public void SayGoodbye(
			string goodbye,
			System.DateTime nightTime)
		{
			FGDiagnosticsAutoLoggerSamplesConsoleApplication1EventSource.Current.SayGoodbye(
				_autogenerated, 
				_machineName, 
				goodbye, 
				nightTime
			);

			System.Diagnostics.Debug.WriteLine($"[Console] ERR: SayGoodbye");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\tgoodbye:\t{goodbye}");
			System.Diagnostics.Debug.WriteLine($"\tnightTime.ToString():\t{nightTime.ToString()}");
			_telemetryClient.TrackEvent(
	            nameof(SayGoodbye),
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
	                {"Autogenerated", _autogenerated.ToString()},
                    {"MachineName", Environment.MachineName},
                    {"Goodbye", goodbye},
                    {"NightTime", nightTime.ToString()}
	            });
    
		}




		public void Specially(
			FG.Diagnostics.AutoLogger.Samples.ConsoleApplication1.Loggers.Special special)
		{
			FGDiagnosticsAutoLoggerSamplesConsoleApplication1EventSource.Current.Specially(
				_autogenerated, 
				_machineName, 
				special
			);

			System.Diagnostics.Debug.WriteLine($"[Console] ERR: Specially");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\tspecial.A:\t{special.A}");
			System.Diagnostics.Debug.WriteLine($"\tspecial.B:\t{special.B}");
			System.Diagnostics.Debug.WriteLine($"\tspecial.C:\t{special.C}");
			_telemetryClient.TrackEvent(
	            nameof(Specially),
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
	                {"Autogenerated", _autogenerated.ToString()},
                    {"MachineName", Environment.MachineName},
                    {"A", special.A.ToString()},
                    {"B", special.B},
                    {"C", special.C.ToString()}
	            });
    
		}




		public void StartHello(
			)
		{
			FGDiagnosticsAutoLoggerSamplesConsoleApplication1EventSource.Current.StartHello(
				_autogenerated, 
				_machineName
			);

			System.Diagnostics.Debug.WriteLine($"[Console] ERR: StartHello");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			_helloStopwatch.Restart();

			var helloOperationHolder = _telemetryClient.StartOperation<RequestTelemetry>("hello");
	       helloOperationHolder.Telemetry.Properties.Add("Autogenerated", _autogenerated.ToString());
			helloOperationHolder.Telemetry.Properties.Add("MachineName", Environment.MachineName);
	       OperationHolder.StartOperation(helloOperationHolder);
    
		}

		private System.Diagnostics.Stopwatch _helloStopwatch = new System.Diagnostics.Stopwatch();



		public void StopHello(
			)
		{
			FGDiagnosticsAutoLoggerSamplesConsoleApplication1EventSource.Current.StopHello(
				_autogenerated, 
				_machineName
			);

			System.Diagnostics.Debug.WriteLine($"[Console] ERR: StopHello");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			_helloStopwatch.Stop();

			var helloOperationHolder = OperationHolder.StopOperation();
			_telemetryClient.StopOperation(helloOperationHolder);
			helloOperationHolder.Dispose();
    
		}




		public void LogOtherObject(
			FG.Diagnostics.AutoLogger.Samples.ConsoleApplication1.Loggers.OtherDataCarryingObject data)
		{
			FGDiagnosticsAutoLoggerSamplesConsoleApplication1EventSource.Current.LogOtherObject(
				_autogenerated, 
				_machineName, 
				data
			);

			System.Diagnostics.Debug.WriteLine($"[Console] ERR: LogOtherObject");
           
			System.Diagnostics.Debug.WriteLine($"\t_autogenerated:\t{_autogenerated}");
			System.Diagnostics.Debug.WriteLine($"\tEnvironment.MachineName:\t{Environment.MachineName}");
			System.Diagnostics.Debug.WriteLine($"\tdata.TheValue:\t{data.TheValue}");
			_telemetryClient.TrackEvent(
	            nameof(LogOtherObject),
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
	                {"Autogenerated", _autogenerated.ToString()},
                    {"MachineName", Environment.MachineName},
                    {"TheValue", data.TheValue}
	            });
    
		}




	}
}

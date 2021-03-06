/*******************************************************************************************
*  This class is autogenerated from the class CommunicationLogger
*  Do not directly update this class as changes will be lost on rebuild.
*******************************************************************************************/
using System;
using System.Collections.Generic;
using WebApiService.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System.Runtime.Remoting.Messaging;


namespace WebApiService
{
	internal sealed class CommunicationLogger : ICommunicationLogger
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


		private readonly System.Fabric.StatelessServiceContext _context;
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

		public CommunicationLogger(
			System.Fabric.StatelessServiceContext context)
		{
			_context = context;
			
            _telemetryClient = new Microsoft.ApplicationInsights.TelemetryClient();
            _telemetryClient.Context.User.Id = Environment.UserName;
            _telemetryClient.Context.Session.Id = Guid.NewGuid().ToString();
            _telemetryClient.Context.Device.OperatingSystem = Environment.OSVersion.ToString();

		}

		public void EnumeratingPartitions(
			System.Uri serviceUri)
		{
			WebApiServiceEventSource.Current.EnumeratingPartitions(
				_context, 
				serviceUri
			);
			_telemetryClient.TrackEvent(
	            nameof(EnumeratingPartitions),
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
	                {"ServiceName", _context.ServiceName.ToString()},
                    {"ServiceTypeName", _context.ServiceTypeName},
                    {"ReplicaOrInstanceId", _context.InstanceId.ToString()},
                    {"PartitionId", _context.PartitionId.ToString()},
                    {"ApplicationName", _context.CodePackageActivationContext.ApplicationName},
                    {"ApplicationTypeName", _context.CodePackageActivationContext.ApplicationTypeName},
                    {"NodeName", _context.NodeContext.NodeName},
                    {"ServiceUri", serviceUri.ToString()}
	            });
    
		}



		public void FailedToEnumeratePartitions(
			System.Uri serviceUri,
			System.Exception ex)
		{
			WebApiServiceEventSource.Current.FailedToEnumeratePartitions(
				_context, 
				serviceUri, 
				ex
			);
			_telemetryClient.TrackException(
	            ex,
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
                    { "Name", "FailedToEnumeratePartitions" },
	                {"ServiceName", _context.ServiceName.ToString()},
                    {"ServiceTypeName", _context.ServiceTypeName},
                    {"ReplicaOrInstanceId", _context.InstanceId.ToString()},
                    {"PartitionId", _context.PartitionId.ToString()},
                    {"ApplicationName", _context.CodePackageActivationContext.ApplicationName},
                    {"ApplicationTypeName", _context.CodePackageActivationContext.ApplicationTypeName},
                    {"NodeName", _context.NodeContext.NodeName},
                    {"ServiceUri", serviceUri.ToString()},
                    {"Message", ex.Message},
                    {"Source", ex.Source},
                    {"ExceptionTypeName", ex.GetType().FullName},
                    {"Exception", ex.AsJson()}
	            });
    
		}



		public void EnumeratedExistingPartitions(
			System.Uri serviceUri,
			System.Collections.Generic.IEnumerable<System.Fabric.ServicePartitionInformation> partitions)
		{
			WebApiServiceEventSource.Current.EnumeratedExistingPartitions(
				_context, 
				serviceUri, 
				partitions
			);
			_telemetryClient.TrackEvent(
	            nameof(EnumeratedExistingPartitions),
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
	                {"ServiceName", _context.ServiceName.ToString()},
                    {"ServiceTypeName", _context.ServiceTypeName},
                    {"ReplicaOrInstanceId", _context.InstanceId.ToString()},
                    {"PartitionId", _context.PartitionId.ToString()},
                    {"ApplicationName", _context.CodePackageActivationContext.ApplicationName},
                    {"ApplicationTypeName", _context.CodePackageActivationContext.ApplicationTypeName},
                    {"NodeName", _context.NodeContext.NodeName},
                    {"ServiceUri", serviceUri.ToString()},
                    {"Partitions", partitions.ToString()}
	            });
    
		}



		public void EnumeratedAndCachedPartitions(
			System.Uri serviceUri,
			System.Collections.Generic.IEnumerable<System.Fabric.ServicePartitionInformation> partitions)
		{
			WebApiServiceEventSource.Current.EnumeratedAndCachedPartitions(
				_context, 
				serviceUri, 
				partitions
			);
			_telemetryClient.TrackEvent(
	            nameof(EnumeratedAndCachedPartitions),
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
	                {"ServiceName", _context.ServiceName.ToString()},
                    {"ServiceTypeName", _context.ServiceTypeName},
                    {"ReplicaOrInstanceId", _context.InstanceId.ToString()},
                    {"PartitionId", _context.PartitionId.ToString()},
                    {"ApplicationName", _context.CodePackageActivationContext.ApplicationName},
                    {"ApplicationTypeName", _context.CodePackageActivationContext.ApplicationTypeName},
                    {"NodeName", _context.NodeContext.NodeName},
                    {"ServiceUri", serviceUri.ToString()},
                    {"Partitions", partitions.ToString()}
	            });
    
		}




        public System.IDisposable CallActor(
			System.Uri requestUri,
			string actorMethodName,
			FG.ServiceFabric.Actors.Remoting.Runtime.ActorMessageHeaders actorMessageHeaders,
			FG.ServiceFabric.Services.Remoting.FabricTransport.CustomServiceRequestHeader customServiceRequestHeader)
		{
		    return new ScopeWrapper(new IDisposable[]
		    {

                ScopeWrapperWithAction.Wrap(() =>
		        {
			WebApiServiceEventSource.Current.StartCallActor(
				_context, 
				requestUri, 
				actorMethodName, 
				actorMessageHeaders, 
				customServiceRequestHeader
			);
    
		            return new ScopeWrapperWithAction(() =>
		            {
			WebApiServiceEventSource.Current.StopCallActor(
				_context, 
				requestUri, 
				actorMethodName, 
				actorMessageHeaders, 
				customServiceRequestHeader
			);
    
		            });
		        }),


                ScopeWrapperWithAction.Wrap(() =>
		        {

			            var callActorOperationHolder = _telemetryClient.StartOperation<DependencyTelemetry>(requestUri.ToString() ?? "callActor");
			            callActorOperationHolder.Telemetry.Properties.Add("ServiceName", _context.ServiceName.ToString());
			callActorOperationHolder.Telemetry.Properties.Add("ServiceTypeName", _context.ServiceTypeName);
			callActorOperationHolder.Telemetry.Properties.Add("ReplicaOrInstanceId", _context.InstanceId.ToString());
			callActorOperationHolder.Telemetry.Properties.Add("PartitionId", _context.PartitionId.ToString());
			callActorOperationHolder.Telemetry.Properties.Add("ApplicationName", _context.CodePackageActivationContext.ApplicationName);
			callActorOperationHolder.Telemetry.Properties.Add("ApplicationTypeName", _context.CodePackageActivationContext.ApplicationTypeName);
			callActorOperationHolder.Telemetry.Properties.Add("NodeName", _context.NodeContext.NodeName);
			callActorOperationHolder.Telemetry.Properties.Add("RequestUri", requestUri.ToString());
			callActorOperationHolder.Telemetry.Properties.Add("ActorMethodName", actorMethodName);
			callActorOperationHolder.Telemetry.Properties.Add("InterfaceId", (actorMessageHeaders?.InterfaceId ?? 0).ToString());
			callActorOperationHolder.Telemetry.Properties.Add("MethodId", (actorMessageHeaders?.MethodId ?? 0).ToString());
			callActorOperationHolder.Telemetry.Properties.Add("ActorId", actorMessageHeaders?.ActorId.ToString());
			callActorOperationHolder.Telemetry.Properties.Add("UserId", customServiceRequestHeader?.GetHeader("userId"));
			callActorOperationHolder.Telemetry.Properties.Add("CorrelationId", customServiceRequestHeader?.GetHeader("correlationId"));
    
		            return new ScopeWrapperWithAction(() =>
		            {

			            _telemetryClient.StopOperation<DependencyTelemetry>(callActorOperationHolder);
    
		            });
		        }),


		    });
		}





		public void CallActorFailed(
			System.Uri requestUri,
			string actorMethodName,
			FG.ServiceFabric.Actors.Remoting.Runtime.ActorMessageHeaders actorMessageHeaders,
			FG.ServiceFabric.Services.Remoting.FabricTransport.CustomServiceRequestHeader customServiceRequestHeader,
			System.Exception ex)
		{
			WebApiServiceEventSource.Current.CallActorFailed(
				_context, 
				requestUri, 
				actorMethodName, 
				actorMessageHeaders, 
				customServiceRequestHeader, 
				ex
			);
			_telemetryClient.TrackException(
	            ex,
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
                    { "Name", "CallActorFailed" },
	                {"ServiceName", _context.ServiceName.ToString()},
                    {"ServiceTypeName", _context.ServiceTypeName},
                    {"ReplicaOrInstanceId", _context.InstanceId.ToString()},
                    {"PartitionId", _context.PartitionId.ToString()},
                    {"ApplicationName", _context.CodePackageActivationContext.ApplicationName},
                    {"ApplicationTypeName", _context.CodePackageActivationContext.ApplicationTypeName},
                    {"NodeName", _context.NodeContext.NodeName},
                    {"RequestUri", requestUri.ToString()},
                    {"ActorMethodName", actorMethodName},
                    {"InterfaceId", (actorMessageHeaders?.InterfaceId ?? 0).ToString()},
                    {"MethodId", (actorMessageHeaders?.MethodId ?? 0).ToString()},
                    {"ActorId", actorMessageHeaders?.ActorId.ToString()},
                    {"UserId", customServiceRequestHeader?.GetHeader("userId")},
                    {"CorrelationId", customServiceRequestHeader?.GetHeader("correlationId")},
                    {"Message", ex.Message},
                    {"Source", ex.Source},
                    {"ExceptionTypeName", ex.GetType().FullName},
                    {"Exception", ex.AsJson()}
	            });
    
		}




        public System.IDisposable CallService(
			System.Uri requestUri,
			string serviceMethodName,
			Microsoft.ServiceFabric.Services.Remoting.ServiceRemotingMessageHeaders serviceMessageHeaders,
			FG.ServiceFabric.Services.Remoting.FabricTransport.CustomServiceRequestHeader customServiceRequestHeader)
		{
		    return new ScopeWrapper(new IDisposable[]
		    {

                ScopeWrapperWithAction.Wrap(() =>
		        {
			WebApiServiceEventSource.Current.StartCallService(
				_context, 
				requestUri, 
				serviceMethodName, 
				serviceMessageHeaders, 
				customServiceRequestHeader
			);
    
		            return new ScopeWrapperWithAction(() =>
		            {
			WebApiServiceEventSource.Current.StopCallService(
				_context, 
				requestUri, 
				serviceMethodName, 
				serviceMessageHeaders, 
				customServiceRequestHeader
			);
    
		            });
		        }),


                ScopeWrapperWithAction.Wrap(() =>
		        {

			            var callServiceOperationHolder = _telemetryClient.StartOperation<DependencyTelemetry>(requestUri.ToString() ?? "callService");
			            callServiceOperationHolder.Telemetry.Properties.Add("ServiceName", _context.ServiceName.ToString());
			callServiceOperationHolder.Telemetry.Properties.Add("ServiceTypeName", _context.ServiceTypeName);
			callServiceOperationHolder.Telemetry.Properties.Add("ReplicaOrInstanceId", _context.InstanceId.ToString());
			callServiceOperationHolder.Telemetry.Properties.Add("PartitionId", _context.PartitionId.ToString());
			callServiceOperationHolder.Telemetry.Properties.Add("ApplicationName", _context.CodePackageActivationContext.ApplicationName);
			callServiceOperationHolder.Telemetry.Properties.Add("ApplicationTypeName", _context.CodePackageActivationContext.ApplicationTypeName);
			callServiceOperationHolder.Telemetry.Properties.Add("NodeName", _context.NodeContext.NodeName);
			callServiceOperationHolder.Telemetry.Properties.Add("RequestUri", requestUri.ToString());
			callServiceOperationHolder.Telemetry.Properties.Add("ServiceMethodName", serviceMethodName);
			callServiceOperationHolder.Telemetry.Properties.Add("InterfaceId", (serviceMessageHeaders?.InterfaceId ?? 0).ToString());
			callServiceOperationHolder.Telemetry.Properties.Add("MethodId", (serviceMessageHeaders?.MethodId ?? 0).ToString());
			callServiceOperationHolder.Telemetry.Properties.Add("UserId", customServiceRequestHeader?.GetHeader("userId"));
			callServiceOperationHolder.Telemetry.Properties.Add("CorrelationId", customServiceRequestHeader?.GetHeader("correlationId"));
    
		            return new ScopeWrapperWithAction(() =>
		            {

			            _telemetryClient.StopOperation<DependencyTelemetry>(callServiceOperationHolder);
    
		            });
		        }),


		    });
		}





		public void CallServiceFailed(
			System.Uri requestUri,
			string serviceMethodName,
			Microsoft.ServiceFabric.Services.Remoting.ServiceRemotingMessageHeaders serviceMessageHeaders,
			FG.ServiceFabric.Services.Remoting.FabricTransport.CustomServiceRequestHeader customServiceRequestHeader,
			System.Exception ex)
		{
			WebApiServiceEventSource.Current.CallServiceFailed(
				_context, 
				requestUri, 
				serviceMethodName, 
				serviceMessageHeaders, 
				customServiceRequestHeader, 
				ex
			);
			_telemetryClient.TrackException(
	            ex,
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
                    { "Name", "CallServiceFailed" },
	                {"ServiceName", _context.ServiceName.ToString()},
                    {"ServiceTypeName", _context.ServiceTypeName},
                    {"ReplicaOrInstanceId", _context.InstanceId.ToString()},
                    {"PartitionId", _context.PartitionId.ToString()},
                    {"ApplicationName", _context.CodePackageActivationContext.ApplicationName},
                    {"ApplicationTypeName", _context.CodePackageActivationContext.ApplicationTypeName},
                    {"NodeName", _context.NodeContext.NodeName},
                    {"RequestUri", requestUri.ToString()},
                    {"ServiceMethodName", serviceMethodName},
                    {"InterfaceId", (serviceMessageHeaders?.InterfaceId ?? 0).ToString()},
                    {"MethodId", (serviceMessageHeaders?.MethodId ?? 0).ToString()},
                    {"UserId", customServiceRequestHeader?.GetHeader("userId")},
                    {"CorrelationId", customServiceRequestHeader?.GetHeader("correlationId")},
                    {"Message", ex.Message},
                    {"Source", ex.Source},
                    {"ExceptionTypeName", ex.GetType().FullName},
                    {"Exception", ex.AsJson()}
	            });
    
		}



		public void ServiceClientFailed(
			System.Uri requestUri,
			FG.ServiceFabric.Services.Remoting.FabricTransport.CustomServiceRequestHeader customServiceRequestHeader,
			System.Exception ex)
		{
			WebApiServiceEventSource.Current.ServiceClientFailed(
				_context, 
				requestUri, 
				customServiceRequestHeader, 
				ex
			);
			_telemetryClient.TrackException(
	            ex,
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
                    { "Name", "ServiceClientFailed" },
	                {"ServiceName", _context.ServiceName.ToString()},
                    {"ServiceTypeName", _context.ServiceTypeName},
                    {"ReplicaOrInstanceId", _context.InstanceId.ToString()},
                    {"PartitionId", _context.PartitionId.ToString()},
                    {"ApplicationName", _context.CodePackageActivationContext.ApplicationName},
                    {"ApplicationTypeName", _context.CodePackageActivationContext.ApplicationTypeName},
                    {"NodeName", _context.NodeContext.NodeName},
                    {"RequestUri", requestUri.ToString()},
                    {"UserId", customServiceRequestHeader?.GetHeader("userId")},
                    {"CorrelationId", customServiceRequestHeader?.GetHeader("correlationId")},
                    {"Message", ex.Message},
                    {"Source", ex.Source},
                    {"ExceptionTypeName", ex.GetType().FullName},
                    {"Exception", ex.AsJson()}
	            });
    
		}



		public void StartRequestContext(
			System.Collections.Generic.IEnumerable<FG.ServiceFabric.Services.Remoting.FabricTransport.ServiceRequestHeader> headers)
		{
			WebApiServiceEventSource.Current.StartRequestContext(
				_context, 
				headers
			);

			var requestContextOperationHolder = _telemetryClient.StartOperation<RequestTelemetry>("requestContext");
	       requestContextOperationHolder.Telemetry.Properties.Add("ServiceName", _context.ServiceName.ToString());
			requestContextOperationHolder.Telemetry.Properties.Add("ServiceTypeName", _context.ServiceTypeName);
			requestContextOperationHolder.Telemetry.Properties.Add("ReplicaOrInstanceId", _context.InstanceId.ToString());
			requestContextOperationHolder.Telemetry.Properties.Add("PartitionId", _context.PartitionId.ToString());
			requestContextOperationHolder.Telemetry.Properties.Add("ApplicationName", _context.CodePackageActivationContext.ApplicationName);
			requestContextOperationHolder.Telemetry.Properties.Add("ApplicationTypeName", _context.CodePackageActivationContext.ApplicationTypeName);
			requestContextOperationHolder.Telemetry.Properties.Add("NodeName", _context.NodeContext.NodeName);
			requestContextOperationHolder.Telemetry.Properties.Add("Headers", headers.ToString());
	       OperationHolder.StartOperation(requestContextOperationHolder);
    
		}



		public void StopRequestContext(
			System.Collections.Generic.IEnumerable<FG.ServiceFabric.Services.Remoting.FabricTransport.ServiceRequestHeader> headers)
		{
			WebApiServiceEventSource.Current.StopRequestContext(
				_context, 
				headers
			);

			var requestContextOperationHolder = OperationHolder.StopOperation();
			_telemetryClient.StopOperation(requestContextOperationHolder);
			requestContextOperationHolder.Dispose();
    
		}



		public void FailedRequestContext(
			System.Collections.Generic.IEnumerable<FG.ServiceFabric.Services.Remoting.FabricTransport.ServiceRequestHeader> headers,
			System.Exception exception)
		{
			WebApiServiceEventSource.Current.FailedRequestContext(
				_context, 
				headers, 
				exception
			);
			_telemetryClient.TrackException(
	            exception,
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
                    { "Name", "FailedRequestContext" },
	                {"ServiceName", _context.ServiceName.ToString()},
                    {"ServiceTypeName", _context.ServiceTypeName},
                    {"ReplicaOrInstanceId", _context.InstanceId.ToString()},
                    {"PartitionId", _context.PartitionId.ToString()},
                    {"ApplicationName", _context.CodePackageActivationContext.ApplicationName},
                    {"ApplicationTypeName", _context.CodePackageActivationContext.ApplicationTypeName},
                    {"NodeName", _context.NodeContext.NodeName},
                    {"Headers", headers.ToString()},
                    {"Message", exception.Message},
                    {"Source", exception.Source},
                    {"ExceptionTypeName", exception.GetType().FullName},
                    {"Exception", exception.AsJson()}
	            });
    
		}



		public void FailedToReadCustomServiceMessageHeader(
			Microsoft.ServiceFabric.Services.Remoting.ServiceRemotingMessageHeaders serviceRemotingMessageHeaders,
			System.Exception ex)
		{
			WebApiServiceEventSource.Current.FailedToReadCustomServiceMessageHeader(
				_context, 
				serviceRemotingMessageHeaders, 
				ex
			);
			_telemetryClient.TrackException(
	            ex,
	            new System.Collections.Generic.Dictionary<string, string>()
	            {
                    { "Name", "FailedToReadCustomServiceMessageHeader" },
	                {"ServiceName", _context.ServiceName.ToString()},
                    {"ServiceTypeName", _context.ServiceTypeName},
                    {"ReplicaOrInstanceId", _context.InstanceId.ToString()},
                    {"PartitionId", _context.PartitionId.ToString()},
                    {"ApplicationName", _context.CodePackageActivationContext.ApplicationName},
                    {"ApplicationTypeName", _context.CodePackageActivationContext.ApplicationTypeName},
                    {"NodeName", _context.NodeContext.NodeName},
                    {"InterfaceId", (serviceRemotingMessageHeaders?.InterfaceId ?? 0).ToString()},
                    {"MethodId", (serviceRemotingMessageHeaders?.MethodId ?? 0).ToString()},
                    {"Message", ex.Message},
                    {"Source", ex.Source},
                    {"ExceptionTypeName", ex.GetType().FullName},
                    {"Exception", ex.AsJson()}
	            });
    
		}



	}
}

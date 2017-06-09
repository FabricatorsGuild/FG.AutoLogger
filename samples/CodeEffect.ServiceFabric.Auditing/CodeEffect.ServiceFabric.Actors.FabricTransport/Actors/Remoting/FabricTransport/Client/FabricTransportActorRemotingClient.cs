using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading.Tasks;
using CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics;
using CodeEffect.ServiceFabric.Actors.FabricTransport.Utils;
using CodeEffect.ServiceFabric.Actors.Remoting.Runtime;
using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Builder;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace CodeEffect.ServiceFabric.Actors.Remoting.FabricTransport.Client
{
    public class FabricTransportActorRemotingClient : IServiceRemotingClient, ICommunicationClient
    {
        private readonly IServiceRemotingClient _innerClient;
        private readonly Uri _serviceUri;
        private readonly IServiceClientLogger _logger;
        private readonly MethodDispatcherBase _actorMethodDispatcher;
        private readonly MethodDispatcherBase _serviceMethodDispatcher;

        private static readonly ConcurrentDictionary<long, string> ActorMethodMap = new ConcurrentDictionary<long, string>();

        private string GetActorMethodName(ActorMessageHeaders actorMessageHeaders)
        {
            if (actorMessageHeaders == null) return null;
            try
            {
                var methodName = "-";
                var lookup = HashUtil.Combine(actorMessageHeaders.InterfaceId, actorMessageHeaders.MethodId);
                if (ActorMethodMap.ContainsKey(lookup))
                {
                    methodName = ActorMethodMap[lookup];
                    return methodName;
                }

                methodName = _actorMethodDispatcher.GetMethodDispatcherMapName(
                    actorMessageHeaders.InterfaceId, actorMessageHeaders.MethodId);
                ActorMethodMap[lookup] = methodName;
                return methodName;
            }
            catch (Exception ex)
            {
                // ignored
                //_logger?.FailedToGetActorMethodName(actorMessageHeaders, ex);
            }
            return null;
        }

        private string GetServiceMethodName(ServiceRemotingMessageHeaders messageHeaders)
        {
            if (messageHeaders == null) return null;
            try
            {
                var methodName = "-";
                var lookup = HashUtil.Combine(messageHeaders.InterfaceId, messageHeaders.MethodId);
                if (ActorMethodMap.ContainsKey(lookup))
                {
                    methodName = ActorMethodMap[lookup];
                    return methodName;
                }

                methodName = _serviceMethodDispatcher.GetMethodDispatcherMapName(
                    messageHeaders.InterfaceId, messageHeaders.MethodId);
                ActorMethodMap[lookup] = methodName;
                return methodName;
            }
            catch (Exception ex)
            {
                // ignored
                //_logger?.FailedToGetActorMethodName(actorMessageHeaders, ex);
            }
            return null;
        }
        
        public FabricTransportActorRemotingClient(IServiceRemotingClient innerClient, Uri serviceUri, IServiceClientLogger logger, 
            MethodDispatcherBase actorMethodDispatcher, MethodDispatcherBase serviceMethodDispatcher)
        {
            _innerClient = innerClient;
            _serviceUri = serviceUri;
            _logger = logger;
            _actorMethodDispatcher = actorMethodDispatcher;
            _serviceMethodDispatcher = serviceMethodDispatcher;
        }

        ~FabricTransportActorRemotingClient()
        {
            if (this._innerClient == null) return;
            // ReSharper disable once SuspiciousTypeConversion.Global
            var disposable = this._innerClient as IDisposable;
            disposable?.Dispose();
        }        

        Task<byte[]> IServiceRemotingClient.RequestResponseAsync(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
        {
            var customServiceRequestHeader = UpdateAndGetMessageHeaders(messageHeaders);
            var actorMessageHeaders = GetActorMessageHeaders(messageHeaders);

            if (actorMessageHeaders != null)
            {
                return RequestActorResponseAsync(messageHeaders, actorMessageHeaders, customServiceRequestHeader, requestBody);
            }
            else
            {
                return RequestServiceResponseAsync(messageHeaders, customServiceRequestHeader, requestBody);
            }
        }

        private Task<byte[]> RequestActorResponseAsync(ServiceRemotingMessageHeaders messageHeaders, ActorMessageHeaders actorMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader, byte[] requestBody)
        {
            var methodName = GetActorMethodName(actorMessageHeaders);
            using (_logger.CallActor(_serviceUri, methodName, actorMessageHeaders, customServiceRequestHeader))
            {
                try
                {
                    var result = this._innerClient.RequestResponseAsync(messageHeaders, requestBody);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.CallActorFailed(_serviceUri, methodName, actorMessageHeaders, customServiceRequestHeader, ex);
                    throw;
                }
            }
        }

        private Task<byte[]> RequestServiceResponseAsync(ServiceRemotingMessageHeaders messageHeaders, CustomServiceRequestHeader customServiceRequestHeader, byte[] requestBody)
        {
            var methodName = GetServiceMethodName(messageHeaders);
            using (_logger.CallService(_serviceUri, methodName, messageHeaders, customServiceRequestHeader))
            {
                try
                {
                    var result = this._innerClient.RequestResponseAsync(messageHeaders, requestBody);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.CallServiceFailed(_serviceUri, methodName, messageHeaders, customServiceRequestHeader, ex);
                    throw;
                }
            }
        }

        void IServiceRemotingClient.SendOneWay(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
        {
            var customServiceRequestHeader = UpdateAndGetMessageHeaders(messageHeaders);
            var actorMessageHeaders = GetActorMessageHeaders(messageHeaders);

            if (actorMessageHeaders != null)
            {
                SendActorOneWay(messageHeaders, actorMessageHeaders, customServiceRequestHeader, requestBody);
            }
            else
            {
                SendServiceOneWay(messageHeaders, customServiceRequestHeader, requestBody);
            }
        }


        private Task<byte[]> SendActorOneWay(ServiceRemotingMessageHeaders messageHeaders, ActorMessageHeaders actorMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader, byte[] requestBody)
        {
            var methodName = GetActorMethodName(actorMessageHeaders);
            using (_logger.CallActor(_serviceUri, methodName, actorMessageHeaders, customServiceRequestHeader))
            {
                try
                {
                    var result = this._innerClient.RequestResponseAsync(messageHeaders, requestBody);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.CallActorFailed(_serviceUri, methodName, actorMessageHeaders, customServiceRequestHeader, ex);
                    throw;
                }
            }
        }

        private Task<byte[]> SendServiceOneWay(ServiceRemotingMessageHeaders messageHeaders, CustomServiceRequestHeader customServiceRequestHeader, byte[] requestBody)
        {
            var methodName = GetServiceMethodName(messageHeaders);
            using (_logger.CallService(_serviceUri, methodName, messageHeaders, customServiceRequestHeader))
            {
                try
                {
                    var result = this._innerClient.RequestResponseAsync(messageHeaders, requestBody);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.CallServiceFailed(_serviceUri, methodName, messageHeaders, customServiceRequestHeader, ex);
                    throw;
                }
            }
        }

        public ResolvedServicePartition ResolvedServicePartition
        {
            get { return this._innerClient.ResolvedServicePartition; }
            set { this._innerClient.ResolvedServicePartition = value; }
        }

        public string ListenerName
        {
            get { return this._innerClient.ListenerName; }
            set { this._innerClient.ListenerName = value; }
        }
        public ResolvedServiceEndpoint Endpoint
        {
            get { return this._innerClient.Endpoint; }
            set { this._innerClient.Endpoint = value; }
        }

        private ActorMessageHeaders GetActorMessageHeaders(ServiceRemotingMessageHeaders messageHeaders)
        {
            ActorMessageHeaders actorMessageHeaders = null;
            if (ActorMessageHeaders.TryFromServiceMessageHeaders(messageHeaders, out actorMessageHeaders))
            {
                
            }
            return actorMessageHeaders;
        }

        private CustomServiceRequestHeader UpdateAndGetMessageHeaders(ServiceRemotingMessageHeaders messageHeaders)
        {
            if ((ServiceRequestContext.Current != null) && (ServiceRequestContext.Current?.Headers?.Any() ?? false))
            {
                messageHeaders.AddHeaders(ServiceRequestContext.Current.Headers);
            }
            else if (ServiceRequestContext.Current?[ServiceRequestContextKeys.CorrelationId] != null || ServiceRequestContext.Current?[ServiceRequestContextKeys.UserId] != null)
            {
                var header = new CustomServiceRequestHeader()
                    .AddHeader(ServiceRequestContextKeys.CorrelationId, ServiceRequestContext.Current?[ServiceRequestContextKeys.CorrelationId])
                    .AddHeader(ServiceRequestContextKeys.UserId, ServiceRequestContext.Current?[ServiceRequestContextKeys.UserId]);

                messageHeaders.AddHeader(header);
            }
            var customServiceRequestHeader = messageHeaders.GetCustomServiceRequestHeader(_logger) ?? new CustomServiceRequestHeader();
            return customServiceRequestHeader;
        }
    }
}
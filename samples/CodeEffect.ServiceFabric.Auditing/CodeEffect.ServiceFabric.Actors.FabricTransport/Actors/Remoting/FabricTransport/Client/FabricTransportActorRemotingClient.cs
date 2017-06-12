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
using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Builder;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace CodeEffect.ServiceFabric.Actors.Remoting.FabricTransport.Client
{
    public class FabricTransportActorRemotingClient : FabricTransportServiceRemotingClient
    {
        private readonly MethodDispatcherBase _actorMethodDispatcher;
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
        
        public FabricTransportActorRemotingClient(IServiceRemotingClient innerClient, Uri serviceUri, IServiceClientLogger logger, 
            MethodDispatcherBase actorMethodDispatcher, MethodDispatcherBase serviceMethodDispatcher)
            : base(innerClient, serviceUri, logger, serviceMethodDispatcher)
        {
            _actorMethodDispatcher = actorMethodDispatcher;
        }

        ~FabricTransportActorRemotingClient()
        {
            if (this.InnerClient == null) return;
            // ReSharper disable once SuspiciousTypeConversion.Global
            var disposable = this.InnerClient as IDisposable;
            disposable?.Dispose();
        }

        protected override Task<byte[]> RequestServiceResponseAsync(ServiceRemotingMessageHeaders messageHeaders,
            CustomServiceRequestHeader customServiceRequestHeader, byte[] requestBody)
        {
            var actorMessageHeaders = GetActorMessageHeaders(messageHeaders);
            if (actorMessageHeaders != null)
            {
                return RequestActorResponseAsync(messageHeaders, actorMessageHeaders, customServiceRequestHeader, requestBody);
            }
            return base.RequestServiceResponseAsync(messageHeaders, customServiceRequestHeader, requestBody);
        }        

        private Task<byte[]> RequestActorResponseAsync(ServiceRemotingMessageHeaders messageHeaders, ActorMessageHeaders actorMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader, byte[] requestBody)
        {
            var methodName = GetActorMethodName(actorMessageHeaders);
            using (Logger.CallActor(ServiceUri, methodName, actorMessageHeaders, customServiceRequestHeader))
            {
                try
                {
                    var result = this.InnerClient.RequestResponseAsync(messageHeaders, requestBody);
                    return result;
                }
                catch (Exception ex)
                {
                    Logger.CallActorFailed(ServiceUri, methodName, actorMessageHeaders, customServiceRequestHeader, ex);
                    throw;
                }
            }
        }

        protected override Task<byte[]> SendServiceOneWay(ServiceRemotingMessageHeaders messageHeaders, CustomServiceRequestHeader customServiceRequestHeader, byte[] requestBody)
        {
            var actorMessageHeaders = GetActorMessageHeaders(messageHeaders);
            if (actorMessageHeaders != null)
            {
                return SendActorOneWay(messageHeaders, actorMessageHeaders, customServiceRequestHeader, requestBody);
            }
            return base.SendServiceOneWay(messageHeaders, customServiceRequestHeader, requestBody);
        }
        
        private Task<byte[]> SendActorOneWay(ServiceRemotingMessageHeaders messageHeaders, ActorMessageHeaders actorMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader, byte[] requestBody)
        {
            var methodName = GetActorMethodName(actorMessageHeaders);
            using (Logger.CallActor(ServiceUri, methodName, actorMessageHeaders, customServiceRequestHeader))
            {
                try
                {
                    var result = this.InnerClient.RequestResponseAsync(messageHeaders, requestBody);
                    return result;
                }
                catch (Exception ex)
                {
                    Logger.CallActorFailed(ServiceUri, methodName, actorMessageHeaders, customServiceRequestHeader, ex);
                    throw;
                }
            }
        }

        private static ActorMessageHeaders GetActorMessageHeaders(ServiceRemotingMessageHeaders messageHeaders)
        {
            ActorMessageHeaders actorMessageHeaders = null;
            if (ActorMessageHeaders.TryFromServiceMessageHeaders(messageHeaders, out actorMessageHeaders))
            {
                
            }
            return actorMessageHeaders;
        }
    }
}
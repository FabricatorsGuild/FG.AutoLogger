using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics;
using CodeEffect.ServiceFabric.Actors.FabricTransport.Utils;
using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport;
using CodeEffect.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace CodeEffect.ServiceFabric.Actors.Remoting.Runtime
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ActorServiceRemotingDispatcher : Microsoft.ServiceFabric.Actors.Remoting.Runtime.ActorServiceRemotingDispatcher
    {
        private static readonly ConcurrentDictionary<long, string> MethodMap = new ConcurrentDictionary<long, string>();

        public IActorServiceCommunicationLogger CommunicationLogger { get; set; }

        private string GetActorMethodName(ActorMessageHeaders actorMessageHeaders)
        {
            if (actorMessageHeaders == null) return null;
            try
            {
                var methodName = "-";
                var lookup = HashUtil.Combine(actorMessageHeaders.InterfaceId, actorMessageHeaders.MethodId);
                if (MethodMap.ContainsKey(lookup))
                {
                    methodName = MethodMap[lookup];
                    return methodName;
                }

                methodName = ((Microsoft.ServiceFabric.Actors.Remoting.Runtime.ActorServiceRemotingDispatcher)this).GetMethodDispatcherMapName(
                    actorMessageHeaders.InterfaceId, actorMessageHeaders.MethodId);
                MethodMap[lookup] = methodName;
                return methodName;
            }
            catch (Exception ex)
            {
                // ignored
                CommunicationLogger?.FailedToGetActorMethodName(actorMessageHeaders, ex);
            }
            return null;
        }        

        public ActorServiceRemotingDispatcher(ActorService actorService) : base(actorService) { }

        public override async Task<byte[]> RequestResponseAsync(IServiceRemotingRequestContext requestContext, ServiceRemotingMessageHeaders messageHeaders, byte[] requestBodyBytes)
        {
            var actorMessageHeaders = messageHeaders.GetActorMessageHeaders(CommunicationLogger);
            var methodName = GetActorMethodName(actorMessageHeaders);
            var customHeader = messageHeaders.GetCustomServiceRequestHeader(CommunicationLogger) ?? new CustomServiceRequestHeader();

            CommunicationLogger?.StartActorMessageRecieved(methodName, customHeader);

            byte[] result = null;
            try
            {
                result = await this.RunInRequestContext(
                    async () => await base.RequestResponseAsync(
                        requestContext,
                        messageHeaders,
                        requestBodyBytes),
                    customHeader);

                CommunicationLogger?.ActorMessageDispatched(methodName, customHeader);
            }
            catch (Exception ex)
            {
                CommunicationLogger?.ActorMessageFailed(methodName, customHeader, ex);
                throw;
            }
            finally
            {
                CommunicationLogger?.StopActorMessageRecieved(methodName, customHeader);
            }
            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using CodeEffect.ServiceFabric.Actors.Remoting.Runtime;
using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

namespace CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics
{
    public interface IActorServiceCommunicationLogger : IServiceCommunicationLogger
    {
        void StartActorMessageRecieved(string methodName, CustomServiceRequestHeader headers);
        void StopActorMessageRecieved(string methodName, CustomServiceRequestHeader headers);

        void ActorMessageDispatched(string methodName, CustomServiceRequestHeader headers);

        void ActorMessageFailed(string methodName, CustomServiceRequestHeader headers, Exception ex);

        void ActorMessageHandled(string methodName, CustomServiceRequestHeader headers);

        void FailedToGetActorMethodName(ActorMessageHeaders actorMessageHeaders, Exception ex);

        void FailedToReadActorMessageHeaders(ServiceRemotingMessageHeaders serviceRemotingMessageHeaders, Exception ex);
    }
}
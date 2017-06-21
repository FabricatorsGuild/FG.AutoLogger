using System;
using System.Collections.Generic;
using CodeEffect.ServiceFabric.Actors.Remoting.Runtime;
using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

namespace CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics
{
    public interface IActorServiceCommunicationLogger : IServiceCommunicationLogger
    {
        IDisposable RecieveActorMessage(Uri requestUri, string actorMethodName, ActorMessageHeaders actorMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader);

        void RecieveActorMessageFailed(Uri requestUri, string actorMethodName, ActorMessageHeaders actorMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader,
            Exception ex);

        void FailedToGetActorMethodName(ActorMessageHeaders actorMessageHeaders, Exception ex);

        void FailedToReadActorMessageHeaders(ServiceRemotingMessageHeaders serviceRemotingMessageHeaders, Exception ex);        
    }
}
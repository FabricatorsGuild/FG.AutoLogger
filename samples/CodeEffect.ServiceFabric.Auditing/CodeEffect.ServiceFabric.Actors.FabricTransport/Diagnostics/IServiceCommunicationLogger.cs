using System;
using System.Collections.Generic;
using CodeEffect.ServiceFabric.Actors.Remoting.Runtime;
using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

namespace CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics

{
    public interface IServiceCommunicationLogger : IServiceClientLogger
    {
        IDisposable RecieveActorMessage(Uri requestUri, ActorMessageHeaders actorMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader);

        void RecieveActorMessageFailed(Uri requestUri, ActorMessageHeaders actorMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader,
            Exception ex);

        void FailedToGetActorMethodName(ActorMessageHeaders actorMessageHeaders, Exception ex);

        void FailedToReadActorMessageHeaders(ServiceRemotingMessageHeaders serviceRemotingMessageHeaders, Exception ex);



        IDisposable RecieveServiceMessage(Uri requestUri, ActorMessageHeaders actorMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader);

        void RecieveServiceMessageFailed(Uri requestUri, ActorMessageHeaders actorMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader,
            Exception ex);

        void FailedToGetServiceMethodName(int interfaceId, int methodId, Exception ex);

        void FailedToReadCustomServiceMessageHeader(ServiceRemotingMessageHeaders serviceRemotingMessageHeaders, Exception ex);





        void StartMessageRecieved(string methodName, CustomServiceRequestHeader headers);
        void StopMessageRecieved(string methodName, CustomServiceRequestHeader headers);

        void MessageDispatched(string methodName, CustomServiceRequestHeader headers);

        void MessageFailed(string methodName, CustomServiceRequestHeader headers, Exception ex);

        void MessageHandled(string methodName, CustomServiceRequestHeader headers);
    }


}
using System;
using System.Collections.Generic;
using CodeEffect.ServiceFabric.Actors.Remoting.Runtime;

namespace CodeEffect.ServiceFabric.Services.Remoting.FabricTransport
{
    public interface IServiceClientLogger
    {
        IDisposable CallService(Uri requestUri, ActorMessageHeaders actorMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader);

        void CallServiceFailed(Uri requestUri, ActorMessageHeaders actorMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader, Exception ex);



        void StartMessageSend(Uri requestUri, CustomServiceRequestHeader headers);
        void StopMessageSend(Uri requestUri, CustomServiceRequestHeader headers);
        void FailedtoSendMessage(Uri requestUri, CustomServiceRequestHeader header, Exception ex);


        void StartRequestContext(IEnumerable<ServiceRequestHeader> headers);
        void StopRequestContext(IEnumerable<ServiceRequestHeader> headers);
        void FailedRequestContext(IEnumerable<ServiceRequestHeader> headers, Exception exception);
    }
}
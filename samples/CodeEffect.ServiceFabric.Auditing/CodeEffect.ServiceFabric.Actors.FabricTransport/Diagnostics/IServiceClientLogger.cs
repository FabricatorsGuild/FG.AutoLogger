using System;
using System.Collections.Generic;
using CodeEffect.ServiceFabric.Actors.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;

namespace CodeEffect.ServiceFabric.Services.Remoting.FabricTransport
{
    public interface IServiceClientLogger
    {
        IDisposable CallActor(Uri requestUri, string actorMethodName, ActorMessageHeaders actorMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader);

        void CallActorFailed(Uri requestUri, string actorMethodName, ActorMessageHeaders actorMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader, Exception ex);



        IDisposable CallService(Uri requestUri, string serviceMethodName, ServiceRemotingMessageHeaders serviceMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader);

        void CallServiceFailed(Uri requestUri, string serviceMethodName, ServiceRemotingMessageHeaders serviceMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader, Exception ex);


        void FailedtoSendMessage(Uri requestUri, CustomServiceRequestHeader header, Exception ex);


        void StartRequestContext(IEnumerable<ServiceRequestHeader> headers);
        void StopRequestContext(IEnumerable<ServiceRequestHeader> headers);
        void FailedRequestContext(IEnumerable<ServiceRequestHeader> headers, Exception exception);
    }
}
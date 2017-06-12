using System;
using System.Collections.Generic;
using CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics;
using CodeEffect.ServiceFabric.Actors.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;

namespace CodeEffect.ServiceFabric.Services.Remoting.FabricTransport
{
    public interface IServiceClientLogger : IServiceRemotingLogger
    {
        IDisposable CallActor(Uri requestUri, string actorMethodName, ActorMessageHeaders actorMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader);

        void CallActorFailed(Uri requestUri, string actorMethodName, ActorMessageHeaders actorMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader, Exception ex);



        IDisposable CallService(Uri requestUri, string serviceMethodName, ServiceRemotingMessageHeaders serviceMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader);

        void CallServiceFailed(Uri requestUri, string serviceMethodName, ServiceRemotingMessageHeaders serviceMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader, Exception ex);


        void ServiceClientFailed(Uri requestUri, CustomServiceRequestHeader customServiceRequestHeader, Exception ex);

    }
}
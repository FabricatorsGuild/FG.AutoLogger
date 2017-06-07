using System;
using System.Collections.Generic;
using CodeEffect.ServiceFabric.Actors.Remoting.Runtime;
using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

namespace CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics
{
    public interface IServiceCommunicationLogger : IServiceClientLogger
    {
        IDisposable RecieveServiceMessage(Uri requestUri, string serviceMethodName, ServiceRemotingMessageHeaders serviceMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader);

        void RecieveServiceMessageFailed(Uri requestUri, string serviceMethodName, ServiceRemotingMessageHeaders serviceMessageHeaders, CustomServiceRequestHeader customServiceRequestHeader,
            Exception ex);

        void FailedToGetServiceMethodName(Uri requestUri, int interfaceId, int methodId, Exception ex);

        void FailedToReadCustomServiceMessageHeader(ServiceRemotingMessageHeaders serviceRemotingMessageHeaders, Exception ex);        
    }
}
using System;
using System.Collections.Generic;

namespace CodeEffect.ServiceFabric.Services.Remoting.FabricTransport
{
    public interface IServiceClientLogger
    {
        IDisposable CallService(Uri requestUri, CustomServiceRequestHeader headers);

        void StartMessageSend(Uri requestUri, CustomServiceRequestHeader headers);
        void StopMessageSend(Uri requestUri, CustomServiceRequestHeader headers);
        void FailedtoSendMessage(Uri requestUri, CustomServiceRequestHeader header, Exception ex);


        void StartRequestContext(IEnumerable<ServiceRequestHeader> headers);
        void StopRequestContext(IEnumerable<ServiceRequestHeader> headers);
        void FailedRequestContext(IEnumerable<ServiceRequestHeader> headers, Exception exception);
    }
}
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics;

namespace CodeEffect.ServiceFabric.Services.Remoting.FabricTransport
{
    public sealed class ServiceRequestContext
    {
        private static readonly string ContextKey = Guid.NewGuid().ToString();

        public ServiceRequestContext()
        {
        }
        public ServiceRequestContext(IEnumerable<ServiceRequestHeader> headers)
        {
            Headers = headers;
        }

        public IServiceCommunicationLogger Logger { get; set; }

        public IEnumerable<ServiceRequestHeader> Headers { get; set; }
        public string CorrelationId { get; set; }

        public Uri RequestUri { get; set; }

        public string UserId { get; set; }

        public static void SetHeaders(IEnumerable<ServiceRequestHeader> headers)
        {
            Current.Headers = headers;
        }

        public static ServiceRequestContext Current
        {
            get { return (ServiceRequestContext) CallContext.LogicalGetData(ContextKey); }
            internal set
            {
                if (value == null)
                {
                    CallContext.FreeNamedDataSlot(ContextKey);
                }
                else
                {
                    CallContext.LogicalSetData(ContextKey, value);
                }
            }
        }
    }
}
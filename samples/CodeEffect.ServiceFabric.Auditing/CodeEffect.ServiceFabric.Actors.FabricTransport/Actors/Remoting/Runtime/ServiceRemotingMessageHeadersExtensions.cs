using System;
using CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics;
using CodeEffect.ServiceFabric.Actors.FabricTransport.Utils;
using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

namespace CodeEffect.ServiceFabric.Actors.Remoting.Runtime
{
    public static class ServiceRemotingMessageHeadersExtensions
    {
        internal static CustomServiceRequestHeader GetCustomServiceRequestHeader(this ServiceRemotingMessageHeaders messageHeaders, IServiceCommunicationLogger logger)
        {
            try
            {
                CustomServiceRequestHeader customServiceRequestHeader;
                if (CustomServiceRequestHeader.TryFromServiceMessageHeaders(messageHeaders, out customServiceRequestHeader))
                {
                    return customServiceRequestHeader;
                }
            }
            catch (Exception ex)
            {
                // ignored
                logger?.FailedToReadCustomServiceMessageHeader(messageHeaders, ex);
            }
            return null;
        }

        internal static ActorMessageHeaders GetActorMessageHeaders(this ServiceRemotingMessageHeaders messageHeaders, IActorServiceCommunicationLogger logger)
        {
            try
            {
                ActorMessageHeaders actorMessageHeaders;
                if (ActorMessageHeaders.TryFromServiceMessageHeaders(messageHeaders, out actorMessageHeaders))
                {
                    return actorMessageHeaders;
                }
            }
            catch (Exception ex)
            {
                // ignored
                logger?.FailedToReadActorMessageHeaders( messageHeaders, ex);
            }
            return null;
        }
    }
}
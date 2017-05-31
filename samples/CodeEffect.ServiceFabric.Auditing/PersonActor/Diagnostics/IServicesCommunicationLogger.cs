using CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics;
using CodeEffect.ServiceFabric.Actors.FabricTransport.Utils;
using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport;

namespace PersonActor.Diagnostics
{
    public interface IServicesCommunicationLogger :
        IActorServiceCommunicationLogger,
        IPartitionHelperLogger,
        IServiceClientLogger
    {
          
    }
}
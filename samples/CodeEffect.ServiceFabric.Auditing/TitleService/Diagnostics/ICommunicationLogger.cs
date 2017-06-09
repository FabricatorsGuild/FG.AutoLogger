using CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics;
using CodeEffect.ServiceFabric.Actors.FabricTransport.Utils;
using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport;

namespace TitleService.Diagnostics
{
    public interface ICommunicationLogger :
        IActorServiceCommunicationLogger,
        IPartitionHelperLogger,
        IServiceClientLogger
    {

    }
}
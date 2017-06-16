using FG.ServiceFabric.Diagnostics;
using FG.ServiceFabric.Services.Remoting.Runtime.Client;

namespace PersonActor.Diagnostics
{
    public interface IServiceCommunicationLogger :
        IActorServiceCommunicationLogger,
        IPartitionHelperLogger,
        IActorClientLogger,
        IServiceClientLogger
    {
    }
}
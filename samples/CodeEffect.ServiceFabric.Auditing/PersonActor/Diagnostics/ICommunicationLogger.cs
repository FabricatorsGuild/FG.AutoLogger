using FG.ServiceFabric.Diagnostics;
using FG.ServiceFabric.Services.Remoting.Runtime.Client;

namespace PersonActor.Diagnostics
{
    public interface ICommunicationLogger :
        IActorServiceCommunicationLogger,
        IPartitionHelperLogger,
        IActorClientLogger,
        IServiceClientLogger
    {
    }
}
using FG.ServiceFabric.Diagnostics;
using FG.ServiceFabric.Services.Remoting.Runtime.Client;

namespace PersonActor.Diagnostics
{
    public interface IActorCommunicationLogger :
        IPartitionHelperLogger,
        IActorClientLogger,
        IServiceClientLogger
    {
        
    }
}
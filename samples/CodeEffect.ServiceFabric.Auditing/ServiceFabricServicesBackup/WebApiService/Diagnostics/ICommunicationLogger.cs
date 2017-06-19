using FG.ServiceFabric.Diagnostics;
using FG.ServiceFabric.Services.Remoting.Runtime.Client;

namespace WebApiService.Diagnostics
{
    public interface ICommunicationLogger : 
        IPartitionHelperLogger,
        IActorClientLogger,
        IServiceClientLogger
    {
    }
}
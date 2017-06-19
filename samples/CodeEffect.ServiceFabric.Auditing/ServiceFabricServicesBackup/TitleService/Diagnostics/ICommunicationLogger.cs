using FG.ServiceFabric.Diagnostics;
using FG.ServiceFabric.Services.Remoting.Runtime.Client;

namespace TitleService.Diagnostics
{
    public interface ICommunicationLogger :
        IServiceCommunicationLogger,
        IPartitionHelperLogger,
        IServiceClientLogger
    {

    }
}
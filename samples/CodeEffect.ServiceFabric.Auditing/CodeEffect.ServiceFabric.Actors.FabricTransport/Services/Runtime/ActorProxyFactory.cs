using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace CodeEffect.ServiceFabric.Services.Runtime
{
	public class ActorProxyFactory : Microsoft.ServiceFabric.Actors.Client.ActorProxyFactory
	{
		protected override IServiceRemotingClientFactory CreateServiceRemotingClientFactory(IServiceRemotingCallbackClient callbackClient)
		{

			var factory = base.CreateServiceRemotingClientFactory(callbackClient);
			

			return factory;
		}
	}
}
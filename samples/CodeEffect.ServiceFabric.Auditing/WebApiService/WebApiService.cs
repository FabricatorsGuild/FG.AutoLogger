using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using TinyIoC;
using WebApiService.Diagnostics;

namespace WebApiService
{
	/// <summary>
	/// The FabricRuntime creates an instance of this class for each service type instance. 
	/// </summary>
	internal sealed class WebApiService : StatelessService
	{
		private TinyIoCContainer Container { get; set; }

		public WebApiService(StatelessServiceContext context)
			: base(context)
		{
			Container = new TinyIoCContainer();
			Container.Register<StatelessServiceContext>(context);
			Container.Register<WebApiService>(this);
			Container.Register<IOwinCommunicationLogger>(new OwinCommunicationLogger(context));
		}

		/// <summary>
		/// Optional override to create listeners (like tcp, http) for this service instance.
		/// </summary>
		/// <returns>The collection of listeners.</returns>
		protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
		{
			return new ServiceInstanceListener[]
			{
				new ServiceInstanceListener(serviceContext => new OwinCommunicationListener(appBuilder => Startup.ConfigureApp(appBuilder, Container), serviceContext, Container.Resolve<IOwinCommunicationLogger>(), "ServiceEndpoint"))
			};
		}
	}
}

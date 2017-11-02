using System;
using System.Diagnostics;
using System.Fabric;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Owin;
using WebApiService.Diagnostics;

namespace WebApiService
{
	internal class OwinCommunicationListener : ICommunicationListener
	{
		private readonly IOwinCommunicationLogger logger;
		private readonly Action<IAppBuilder> startup;
		private readonly ServiceContext serviceContext;
		private readonly string endpointName;
		private readonly string appRoot;

		private IDisposable webApp;
		private string publishAddress;
		private string listeningAddress;

		public OwinCommunicationListener(Action<IAppBuilder> startup, ServiceContext serviceContext, IOwinCommunicationLogger logger, string endpointName)
			: this(startup, serviceContext, logger, endpointName, null)
		{
		}

		public OwinCommunicationListener(Action<IAppBuilder> startup, ServiceContext serviceContext, IOwinCommunicationLogger logger, string endpointName, string appRoot)
		{
			if (startup == null)
			{
				throw new ArgumentNullException(nameof(startup));
			}

			if (serviceContext == null)
			{
				throw new ArgumentNullException(nameof(serviceContext));
			}

			if (endpointName == null)
			{
				throw new ArgumentNullException(nameof(endpointName));
			}

			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			this.startup = startup;
			this.serviceContext = serviceContext;
			this.endpointName = endpointName;
			this.logger = logger;
			this.appRoot = appRoot;
		}

		public bool ListenOnSecondary { get; set; }

		public Task<string> OpenAsync(CancellationToken cancellationToken)
		{
			var serviceEndpoint = this.serviceContext.CodePackageActivationContext.GetEndpoint(this.endpointName);
			int port = serviceEndpoint.Port;

			if (this.serviceContext is StatefulServiceContext)
			{
				StatefulServiceContext statefulServiceContext = this.serviceContext as StatefulServiceContext;

				this.listeningAddress = string.Format(
					CultureInfo.InvariantCulture,
					"http://+:{0}/{1}{2}/{3}/{4}",
					port,
					string.IsNullOrWhiteSpace(this.appRoot)
						? string.Empty
						: this.appRoot.TrimEnd('/') + '/',
					statefulServiceContext.PartitionId,
					statefulServiceContext.ReplicaId,
					Guid.NewGuid());
			}
			else if (this.serviceContext is StatelessServiceContext)
			{
				this.listeningAddress = string.Format(
					CultureInfo.InvariantCulture,
					"http://+:{0}/{1}",
					port,
					string.IsNullOrWhiteSpace(this.appRoot)
						? string.Empty
						: this.appRoot.TrimEnd('/') + '/');
			}
			else
			{
				throw new InvalidOperationException();
			}

			this.publishAddress = this.listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

			try
			{
				this.logger.StartingWebServer(this.listeningAddress);

				this.webApp = WebApp.Start(this.listeningAddress, appBuilder => this.startup.Invoke(appBuilder));

				this.logger.ListeningOn(this.publishAddress);

				return Task.FromResult(this.publishAddress);
			}
			catch (Exception ex)
			{
				this.logger.WebServerFailed(ex);

				this.StopWebServer();

				throw;
			}
		}

		public Task CloseAsync(CancellationToken cancellationToken)
		{
			this.logger.ClosingWebServer();

			this.StopWebServer();

			return Task.FromResult(true);
		}

		public void Abort()
		{
			this.logger.AbortingWebServer();

			this.StopWebServer();
		}

		private void StopWebServer()
		{
			if (this.webApp != null)
			{
				try
				{
					this.webApp.Dispose();
				}
				catch (ObjectDisposedException)
				{
					// no-op
				}
			}
		}
	}
}

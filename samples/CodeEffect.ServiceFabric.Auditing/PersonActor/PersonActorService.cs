	using System;
using System.Collections.Generic;
using System.Fabric;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
	using CodeEffect.ServiceFabric.Actors.Remoting.FabricTransport;
	using CodeEffect.ServiceFabric.Actors.Remoting.Runtime;
	using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport;
	using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Query;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
	using PersonActor.Diagnostics;
	using PersonActor.Interfaces;

namespace PersonActor
{

	public class PersonActorService : ActorService, IPersonActorService
	{
		private readonly IPersonActorServiceLogger _logger;
        private readonly IServicesCommunicationLogger _communicationLogger;

        public PersonActorService(StatefulServiceContext context, ActorTypeInformation actorTypeInfo, Func<ActorService, ActorId, ActorBase> actorFactory = null, Func<ActorBase, IActorStateProvider, IActorStateManager> stateManagerFactory = null, IActorStateProvider stateProvider = null, ActorServiceSettings settings = null) : base(context, actorTypeInfo, actorFactory, stateManagerFactory, stateProvider, settings)
        {
            _logger = new PersonActorServiceLogger(this, Guid.NewGuid(), "");
            _communicationLogger = new ServicesCommunicationLogger(this.Context);            
		}

		protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
		{
			return new[]
			{
				new ServiceReplicaListener(ctxt => new FabricTransportActorServiceRemotingListener(ctxt,
				new ActorServiceRemotingDispatcher(this), new FabricTransportRemotingListenerSettings()
					{
						MaxConcurrentCalls = 1000,
					}
				))
			};
		}

		protected override async Task RunAsync(CancellationToken cancellationToken)
		{
			while (true)
			{
				foreach (var name in ObjectMother.Names)
				{
				    var correlationId = Guid.NewGuid();
				    var actorProxyFactory =
				        new ActorProxyFactory(
				            callbackClient =>
				                FabricTransportActorRemotingHelpers.CreateServiceRemotingClientFactory(typeof(IPersonActorService), callbackClient, _communicationLogger,
				                    correlationId.ToString()));
                    //var actorProxyFactory = new ActorProxyFactory();
				    await actorProxyFactory.RunInContext(async factory =>
				        {
				            var title = ObjectMother.Titles[Environment.TickCount % ObjectMother.Titles.Length];

				            var proxy = actorProxyFactory.CreateActorProxy<IPersonActor>(new ActorId(name));
				            await proxy.SetTitleAsync(title, cancellationToken);

				            _logger.PersonGenerated(name, title);

				        },
				        _communicationLogger,
				        new CustomServiceRequestHeader(new Dictionary<string, string>() {{"name", "service itself"}, {"correlation-id", Guid.NewGuid().ToString()}})
				    );

					await Task.Delay(200000, cancellationToken);						
				}
			}
		}

		public async Task<IDictionary<string, Person>> GetPersons(CancellationToken cancellationToken)
		{
			ContinuationToken continuationToken = null;
			//var actors = await this.StateProvider.GetActorsAsync(100, continuationToken, cancellationToken);

			var results = 0;
			var maxResults = 500;
			var result = new Dictionary<string, Person>();
			do
			{
				var page = await this.StateProvider.GetActorsAsync(100, continuationToken, cancellationToken);
				foreach (var actor in page.Items)
				{
					var actorState = await this.StateProvider.LoadStateAsync<Person>(actor, "state", cancellationToken);
					result.Add(actor.GetStringId(), actorState);
					results++;
				}
				if (results >= maxResults) return result;
				continuationToken = page.ContinuationToken;
			}
			while (continuationToken != null);

			return result;
		}
	}
}
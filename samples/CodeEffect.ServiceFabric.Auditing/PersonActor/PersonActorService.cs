using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using FG.ServiceFabric.Actors.Client;
using FG.ServiceFabric.Actors.Remoting.Runtime;
using FG.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Query;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using PersonActor.Diagnostics;
using PersonActor.Interfaces;
using TitleService;

namespace PersonActor
{

    public class PersonActorService : ActorService, IPersonActorService
    {
        private readonly IPersonActorServiceLogger _logger;
        private readonly ICommunicationLogger _communicationLogger;

        public PersonActorService(StatefulServiceContext context, ActorTypeInformation actorTypeInfo, Func<ActorService, ActorId, ActorBase> actorFactory = null,
            Func<ActorBase, IActorStateProvider, IActorStateManager> stateManagerFactory = null, IActorStateProvider stateProvider = null,
            ActorServiceSettings settings = null) : base(context, actorTypeInfo, actorFactory, stateManagerFactory, stateProvider, settings)
        {
            _logger = new PersonActorServiceLogger(this, ServiceRequestContext.Current);
            _communicationLogger = new CommunicationLogger(this.Context);
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                this.CreateServiceReplicaListener(_communicationLogger),
            };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                foreach (var name in ObjectMother.Names)
                {
                    var correlationId = Guid.NewGuid().ToString();
                    using (new ServiceRequestContextWrapper() {CorrelationId = correlationId, UserId = Environment.UserName})
                    {
                        using (_logger.RunAsyncLoop())
                        {
                            try
                            {
                                var serviceProxyFactory = new FG.ServiceFabric.Services.Remoting.Runtime.Client.ServiceProxyFactory(_communicationLogger);
                                var serviceProxy = serviceProxyFactory.CreateServiceProxy<ITitleService>(
                                    new Uri($"{this.Context.CodePackageActivationContext.ApplicationName}/TitleService"), 
                                    new ServicePartitionKey(0));
                                var titles = await serviceProxy.GetTitlesAsync(cancellationToken);

                                var title = titles[Environment.TickCount % titles.Length];

                                var actorProxyFactory = new ActorProxyFactory(_communicationLogger);
                                var proxy = actorProxyFactory.CreateActorProxy<IPersonActor>(new ActorId(name));
                                await proxy.SetTitleAsync(title, cancellationToken);

                                _logger.PersonGenerated(name, title);
                            }
                            catch (Exception ex)
                            {
                                _logger.RunAsyncLoopFailed(ex);
                            }
                        }
                    }

                    await Task.Delay(10000, cancellationToken);
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
            } while (continuationToken != null);

            return result;
        }
    }
}
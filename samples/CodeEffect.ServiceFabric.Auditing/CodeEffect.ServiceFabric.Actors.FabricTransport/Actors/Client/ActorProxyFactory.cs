using System;
using CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics;
using CodeEffect.ServiceFabric.Actors.Remoting.FabricTransport;
using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport;
using CodeEffect.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace CodeEffect.ServiceFabric.Actors.FabricTransport.Actors.Client
{ 
    public class ActorProxyFactory : IActorProxyFactory
    {
        private readonly object _lock = new object();
        private IActorProxyFactory _innerActorProxyFactory;

        public IServiceCommunicationLogger Logger { get; private set; }

        public ActorProxyFactory(IServiceCommunicationLogger logger)
        {
            Logger = logger;
        }

        private IServiceRemotingClientFactory CreateServiceRemotingClientFactory(IServiceRemotingCallbackClient serviceRemotingCallbackClient, Type actorInterfaceType)
        {

            return FabricTransportActorRemotingHelpers.CreateServiceRemotingClientFactory(actorInterfaceType, serviceRemotingCallbackClient, Logger,
                ServiceRequestContext.Current.CorrelationId);
        }

        private IActorProxyFactory GetInnerFactory(Type actorInterfaceType)
        {
            //TODO: Should we do this? It will introduce the same issue as in the Actor base (i.e. factory locked to one specific ActorInterface...)
            if (_innerActorProxyFactory != null)
            {
                return _innerActorProxyFactory;
            }

            lock (_lock)
            {
                if (_innerActorProxyFactory != null)
                {
                    _innerActorProxyFactory =
                        new Microsoft.ServiceFabric.Actors.Client.ActorProxyFactory(client => CreateServiceRemotingClientFactory(client, actorInterfaceType));
                }
                return _innerActorProxyFactory;
            }
        }

        public TActorInterface CreateActorProxy<TActorInterface>(ActorId actorId, string applicationName = null, string serviceName = null, string listenerName = null) where TActorInterface : IActor
        {

            var proxy = GetInnerFactory(typeof(TActorInterface)).CreateActorProxy<TActorInterface>(actorId, applicationName, serviceName, listenerName);
            UpdateRequestContext(proxy.GetActorReference().ServiceUri);
            return proxy;
        }

        public TActorInterface CreateActorProxy<TActorInterface>(Uri serviceUri, ActorId actorId, string listenerName = null) where TActorInterface : IActor
        {
            var proxy = GetInnerFactory(typeof(TActorInterface)).CreateActorProxy<TActorInterface>(serviceUri, actorId, listenerName);
            UpdateRequestContext(proxy.GetActorReference().ServiceUri);
            return proxy;
        }

        public TServiceInterface CreateActorServiceProxy<TServiceInterface>(Uri serviceUri, ActorId actorId, string listenerName = null) where TServiceInterface : IService
        {
            var proxy = GetInnerFactory(typeof(TServiceInterface)).CreateActorServiceProxy<TServiceInterface>(serviceUri, actorId, listenerName);
            UpdateRequestContext(proxy.GetServiceContext().ServiceName);
            return proxy;
        }

        public TServiceInterface CreateActorServiceProxy<TServiceInterface>(Uri serviceUri, long partitionKey, string listenerName = null) where TServiceInterface : IService
        {
            var proxy = GetInnerFactory(typeof(TServiceInterface)).CreateActorServiceProxy<TServiceInterface>(serviceUri, partitionKey, listenerName);
            UpdateRequestContext(proxy.GetServiceContext().ServiceName);
            return proxy;
        }

        private void UpdateRequestContext(Uri serviceUri)
        {
            if (ServiceRequestContext.Current != null)
            {
                ServiceRequestContext.Current.RequestUri = serviceUri;

                if (ServiceRequestContext.Current.CorrelationId == null)
                {
                    ServiceRequestContext.Current.CorrelationId = Guid.NewGuid().ToString();
                }
            }


        }
    }
}
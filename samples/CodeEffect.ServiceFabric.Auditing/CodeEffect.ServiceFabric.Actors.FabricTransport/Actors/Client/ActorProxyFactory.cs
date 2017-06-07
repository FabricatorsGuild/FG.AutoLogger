using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics;
using CodeEffect.ServiceFabric.Actors.Remoting.FabricTransport;
using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport;
using CodeEffect.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Builder;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace CodeEffect.ServiceFabric.Actors.FabricTransport.Actors.Client
{ 
    public class ActorProxyFactory : IActorProxyFactory
    {
        private readonly object _lock = new object();
        private volatile IActorProxyFactory _innerActorProxyFactory;
        private volatile MethodDispatcherBase _methodDispatcher;
        private ConcurrentDictionary<int, ConcurrentDictionary<int, string>> _actorMethodNameMap;

        public IServiceCommunicationLogger Logger { get; private set; }

        public ActorProxyFactory(IServiceCommunicationLogger logger)
        {
            Logger = logger;
        }

        private IServiceRemotingClientFactory CreateServiceRemotingClientFactory(IServiceRemotingCallbackClient serviceRemotingCallbackClient, Type actorInterfaceType)
        {

            return FabricTransportActorRemotingHelpers.CreateServiceRemotingClientFactory(
                actorInterfaceType, 
                serviceRemotingCallbackClient, 
                Logger,
                ServiceRequestContext.Current?[ServiceRequestContextKeys.CorrelationId],
                _methodDispatcher);
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
                if (_innerActorProxyFactory == null)
                {
                    _innerActorProxyFactory = new Microsoft.ServiceFabric.Actors.Client.ActorProxyFactory(client => CreateServiceRemotingClientFactory(client, actorInterfaceType));
                }
                return _innerActorProxyFactory;
            }
        }

        private MethodDispatcherBase GetOrDiscoverActorMethodDispatcher(Type actorInterfaceType)
        {
            if (_methodDispatcher != null)
            {
                return _methodDispatcher;
            }

            lock (_lock)
            {
                if (_methodDispatcher == null)
                {
                    _methodDispatcher = GetActorMethodInformation(actorInterfaceType);
                }
                return _methodDispatcher;
            }
        }

        public TActorInterface CreateActorProxy<TActorInterface>(ActorId actorId, string applicationName = null, string serviceName = null, string listenerName = null) where TActorInterface : IActor
        {
            GetOrDiscoverActorMethodDispatcher(typeof(TActorInterface));
            var proxy = GetInnerFactory(typeof(TActorInterface)).CreateActorProxy<TActorInterface>(actorId, applicationName, serviceName, listenerName);
            UpdateRequestContext(proxy.GetActorReference().ServiceUri);
            return proxy;
        }

        public TActorInterface CreateActorProxy<TActorInterface>(Uri serviceUri, ActorId actorId, string listenerName = null) where TActorInterface : IActor
        {
            GetOrDiscoverActorMethodDispatcher(typeof(TActorInterface));
            var proxy = GetInnerFactory(typeof(TActorInterface)).CreateActorProxy<TActorInterface>(serviceUri, actorId, listenerName);
            UpdateRequestContext(proxy.GetActorReference().ServiceUri);
            return proxy;
        }

        public TServiceInterface CreateActorServiceProxy<TServiceInterface>(Uri serviceUri, ActorId actorId, string listenerName = null) where TServiceInterface : IService
        {
            var proxy = GetInnerFactory(typeof(TServiceInterface)).CreateActorServiceProxy<TServiceInterface>(serviceUri, actorId, listenerName);
            UpdateRequestContext(serviceUri);
            return proxy;
        }

        public TServiceInterface CreateActorServiceProxy<TServiceInterface>(Uri serviceUri, long partitionKey, string listenerName = null) where TServiceInterface : IService
        {
            var proxy = GetInnerFactory(typeof(TServiceInterface)).CreateActorServiceProxy<TServiceInterface>(serviceUri, partitionKey, listenerName);
            UpdateRequestContext(serviceUri);
            return proxy;
        }

        private void UpdateRequestContext(Uri serviceUri)
        {
            if (ServiceRequestContext.Current != null)
            {
                ServiceRequestContext.Current[ServiceRequestContextKeys.RequestUri] = serviceUri?.ToString();

                if (ServiceRequestContext.Current[ServiceRequestContextKeys.CorrelationId] == null)
                {
                    ServiceRequestContext.Current[ServiceRequestContextKeys.CorrelationId] = Guid.NewGuid().ToString();
                }
            }
        }

        private MethodDispatcherBase GetActorMethodInformation(Type actorInterfaceType )
        {
            var codeBuilder =
                typeof(Microsoft.ServiceFabric.Actors.Client.ActorProxyFactory)?.Assembly.GetType(
                        "Microsoft.ServiceFabric.Actors.Remoting.Builder.ActorCodeBuilder")?
                    .GetField("Singleton", BindingFlags.NonPublic | BindingFlags.Static)?
                    .GetValue(null);


            var codeBuilderType = codeBuilder?.GetType();

            var codeBuilderInterface = codeBuilderType?.GetInterface("ICodeBuilder");
            var getOrBuilderMethodDispatcherMethod = codeBuilderInterface?.GetMethod("GetOrBuilderMethodDispatcher");
            var methodDispatcher = getOrBuilderMethodDispatcherMethod?.Invoke(codeBuilder, new object[] { actorInterfaceType });

            var methodDispatcherType = methodDispatcher.GetType();
            var methodDispatcherBase = methodDispatcherType?.GetProperty("MethodDispatcher", BindingFlags.Public | BindingFlags.Instance)?.GetValue(methodDispatcher) as MethodDispatcherBase;

            return methodDispatcherBase;

                /*
            var actorInterfaceTypeCodeBuilderMethodDispatcher = codeBuilder?.GetType().GetInterface("ICodeBuilder").GetMethod("GetOrBuilderMethodDispatcher").Invoke(codeBuilder, new object[] { actorInterfaceType });
            actorInterfaceTypeCodeBuilderMethodDispatcher?.GetType().GetProperty("MethodDispatcher", BindingFlags.Public | BindingFlags.Instance).GetValue(codeBuilder.GetType().GetInterface("ICodeBuilder").GetMethod("GetOrBuilderMethodDispatcher").Invoke(codeBuilder, new object[] { actorInterfaceType }))

            var getOrBuilderMethodDispatcherMethod =
                codeBuilder.GetType().GetMethod("Microsoft.ServiceFabric.Services.Remoting.Builder.ICodeBuilder.GetOrBuilderMethodDispatcher");

            var eventCodeBuilderField = codeBuilder?.GetType().GetField("eventCodeBuilder", BindingFlags.Instance | BindingFlags.NonPublic);
            var eventCodeBuilder = eventCodeBuilderField?.GetValue(codeBuilder);

            var buildMethodDispatcherMethod = eventCodeBuilder?.GetType().GetMethod("BuildMethodDispatcher", BindingFlags.Instance | BindingFlags.NonPublic);
            var methodDispatcherBuildResult = buildMethodDispatcherMethod?.Invoke(eventCodeBuilder, new object[] {actorInterfaceType});

            var methodDispatcherProperty = methodDispatcherBuildResult?.GetType().GetProperty("MethodDispatcher");
            var methodDispatcherBase = methodDispatcherProperty?.GetValue(methodDispatcherBuildResult) as MethodDispatcherBase;*/
            return methodDispatcherBase;            
        }

    }
}
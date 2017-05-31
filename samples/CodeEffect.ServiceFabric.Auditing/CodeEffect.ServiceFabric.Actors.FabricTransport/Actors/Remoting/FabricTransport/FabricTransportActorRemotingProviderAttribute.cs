using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics;
using CodeEffect.ServiceFabric.Services.Communication;
using Microsoft.ServiceFabric.Actors.Generator;
using Microsoft.ServiceFabric.Actors.Remoting;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace CodeEffect.ServiceFabric.Actors.Remoting.FabricTransport
{
    public class FabricTransportActorRemotingHelpers
    {
        internal static ActorRemotingProviderAttribute GetProvider(IEnumerable<Type> types = null)
        {
            if (types != null)
            {
                foreach (var t in types)
                {
                    var attribute = t.GetTypeInfo().Assembly.GetCustomAttribute<ActorRemotingProviderAttribute>();
                    if (attribute != null)
                    {
                        return attribute;
                    }
                }
            }
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                var attribute = assembly.GetCustomAttribute<ActorRemotingProviderAttribute>();
                if (attribute != null)
                {
                    return attribute;
                }
            }
            return new FabricTransport.FabricTransportActorRemotingProviderAttribute();
        }

        public static IEnumerable<IExceptionHandler> GetExceptionHandlers(Type actorInterfaceType, params Type[] additionalTypes)
        {
            var types = new[] {actorInterfaceType}.Union(additionalTypes);
            foreach (var t in types)
            {
                var attribute = t.GetTypeInfo().Assembly.GetCustomAttribute<FabricTransportRemotingExceptionHandlerAttribute>();
                IExceptionHandler instance = null;
                if (attribute != null)
                {
                   var exceptionHandlerType = attribute.ExceptionHandlerType;
                    if (exceptionHandlerType.GetInterface(nameof(IExceptionHandler), false) != null)
                    {
                        try
                        {
                            instance = Activator.CreateInstance(exceptionHandlerType) as IExceptionHandler;
                            
                        }
                        catch (Exception ex)
                        {
                            // TODO: Log this?
                        }
                    }
                }
                if (instance != null)
                {
                    yield return instance;
                }
            }            
        }

        public static IServiceRemotingClientFactory CreateServiceRemotingClientFactory(Type actorInterfaceType, IServiceRemotingCallbackClient callbackClient, IServiceCommunicationLogger logger, string correlationId)
        {
            var fabricTransportSettings = GetDefaultFabricTransportSettings("TransportSettings");
            var exceptionHandlers = GetExceptionHandlers(actorInterfaceType);
            return
                (IServiceRemotingClientFactory) new CodeEffect.ServiceFabric.Actors.Remoting.FabricTransport.Client.FabricTransportActorRemotingClientFactory(
                    new FabricTransportActorRemotingClientFactory(
                        fabricTransportSettings,
                        callbackClient,
                        (IServicePartitionResolver) null,
                        exceptionHandlers,
                        traceId: correlationId), logger);
        }
        

        /// <summary>
        ///  FabricTransportSettings returns the default Settings .Loads the configuration file from default Config Package"Config" , if not found then try to load from  default config file "ClientExeName.Settings.xml"  from Client Exe directory.
        /// </summary>
        /// <param name="sectionName">Name of the section within the configuration file. If not found section in configuration file, it will return the default Settings</param>
        /// <returns></returns>
        private static FabricTransportRemotingSettings GetDefaultFabricTransportSettings(string sectionName = "TransportSettings")
        {
            FabricTransportRemotingSettings settings = (FabricTransportRemotingSettings)null;
            if (!FabricTransportRemotingSettings.TryLoadFrom(sectionName, out settings, (string)null, (string)null))
            {
                settings = new FabricTransportRemotingSettings();
            }
            return settings;
        }

        internal static FabricTransportRemotingListenerSettings GetActorListenerSettings(ActorService actorService)
        {
            FabricTransportRemotingListenerSettings listenerSettings;
            if (!FabricTransportRemotingListenerSettings.TryLoadFrom(ActorNameFormat.GetFabricServiceTransportSettingsSectionName(actorService.ActorTypeInformation.ImplementationType), out listenerSettings, (string)null))
                listenerSettings = GetDefaultFabricTransportListenerSettings("TransportSettings");
            return listenerSettings;
        }

        internal static FabricTransportRemotingListenerSettings GetDefaultFabricTransportListenerSettings(string sectionName = "TransportSettings")
        {
            FabricTransportRemotingListenerSettings listenerSettings = (FabricTransportRemotingListenerSettings)null;
            if (!FabricTransportRemotingListenerSettings.TryLoadFrom(sectionName, out listenerSettings, (string)null))
            {
                listenerSettings = new FabricTransportRemotingListenerSettings();
            }
            return listenerSettings;
        }
    }


    public class FabricTransportActorRemotingProviderAttribute : Microsoft.ServiceFabric.Actors.Remoting.FabricTransport.FabricTransportActorRemotingProviderAttribute
    {
        private IEnumerable<Type> _exceptionHandlerTypes;

        private IEnumerable<IExceptionHandler> GetExceptionHandlers()
        {
            var exceptionHandlers =
                _exceptionHandlerTypes?
                    .Where(type => type.GetInterface(nameof(IExceptionHandler), false) != null)
                    .Select(exceptionHandlerType => (IExceptionHandler) Activator.CreateInstance(exceptionHandlerType))
                    .Union(
                        _exceptionHandlerTypes?
                            .Where(type => type.IsSubclassOf(typeof(Exception)) || type == typeof(Exception))
                            .Select(exceptionType => (IExceptionHandler) Activator.CreateInstance(
                                    typeof(ExceptionHandler<>).MakeGenericType(new Type[] {exceptionType}))))
                    .ToArray();
            return exceptionHandlers;
        }

        /// <summary>
        ///     Creates a service remoting client factory to connect to the remoted actor interfaces.
        /// </summary>
        /// <param name="callbackClient">
        ///     Client implementation where the callbacks should be dispatched.
        /// </param>
        /// <returns>
        ///     A <see cref="T:Microsoft.ServiceFabric.Actors.Remoting.FabricTransport.FabricTransportActorRemotingClientFactory" />
        ///     as <see cref="T:Microsoft.ServiceFabric.Services.Remoting.Client.IServiceRemotingClientFactory" />
        ///     that can be used with <see cref="T:Microsoft.ServiceFabric.Actors.Client.ActorProxyFactory" /> to
        ///     generate actor proxy to talk to the actor over remoted actor interface.
        /// </returns>
        public override IServiceRemotingClientFactory CreateServiceRemotingClientFactory(IServiceRemotingCallbackClient callbackClient)
        {
            //Microsoft.ServiceFabric.Services.Remoting.FabricTransport.FabricTransportRemotingSettings
            FabricTransportRemotingSettings fabricTransportSettings = GetDefaultFabricTransportSettings("TransportSettings");
            fabricTransportSettings.MaxMessageSize = this.GetAndValidateMaxMessageSize(fabricTransportSettings.MaxMessageSize);
            fabricTransportSettings.OperationTimeout = this.GetandValidateOperationTimeout(fabricTransportSettings.OperationTimeout);
            fabricTransportSettings.KeepAliveTimeout = this.GetandValidateKeepAliveTimeout(fabricTransportSettings.KeepAliveTimeout);
            var exceptionHandlers = GetExceptionHandlers();
            return (IServiceRemotingClientFactory)new CodeEffect.ServiceFabric.Actors.Remoting.FabricTransport.Client.FabricTransportActorRemotingClientFactory(
                new FabricTransportActorRemotingClientFactory(
                fabricTransportSettings,
                callbackClient,
                (IServicePartitionResolver)null,
                exceptionHandlers,
                traceId: (string)null), null);
        }

        public override IServiceRemotingListener CreateServiceRemotingListener(ActorService actorService)
        {
            FabricTransportRemotingListenerSettings listenerSettings = GetActorListenerSettings(actorService);
            listenerSettings.MaxMessageSize = this.GetAndValidateMaxMessageSize(listenerSettings.MaxMessageSize);
            listenerSettings.OperationTimeout = this.GetandValidateOperationTimeout(listenerSettings.OperationTimeout);
            listenerSettings.KeepAliveTimeout = this.GetandValidateKeepAliveTimeout(listenerSettings.KeepAliveTimeout);

            var serviceRemotingDispatcher = ActorServiceRemotingDispatcherAttribute.GetServiceRemotingDispatcher(actorService);

            return (IServiceRemotingListener)new FabricTransportActorServiceRemotingListener(
                serviceContext: actorService.Context,
                messageHandler: serviceRemotingDispatcher,
                listenerSettings: listenerSettings);
        }

        public FabricTransportActorRemotingProviderAttribute()
        {

        }

        public FabricTransportActorRemotingProviderAttribute(params Type[] exceptionHandlerTypes)
        {
            _exceptionHandlerTypes = exceptionHandlerTypes;
        }

        public IEnumerable<Type> ExceptionHandlerTypes
        {
            get { return _exceptionHandlerTypes; }
            set { _exceptionHandlerTypes = value; }
        }

        private long GetAndValidateMaxMessageSize(long maxMessageSize)
        {
            if (this.MaxMessageSize <= 0L)
                return maxMessageSize;
            return this.MaxMessageSize;
        }

        private TimeSpan GetandValidateOperationTimeout(TimeSpan operationTimeout)
        {
            if (this.OperationTimeoutInSeconds <= 0L)
                return operationTimeout;
            return TimeSpan.FromSeconds((double)this.OperationTimeoutInSeconds);
        }

        private TimeSpan GetandValidateKeepAliveTimeout(TimeSpan keepAliveTimeout)
        {
            if (this.KeepAliveTimeoutInSeconds <= 0L)
                return keepAliveTimeout;
            return TimeSpan.FromSeconds((double)this.KeepAliveTimeoutInSeconds);
        }


        /// <summary>
        ///  FabricTransportSettings returns the default Settings .Loads the configuration file from default Config Package"Config" , if not found then try to load from  default config file "ClientExeName.Settings.xml"  from Client Exe directory.
        /// </summary>
        /// <param name="sectionName">Name of the section within the configuration file. If not found section in configuration file, it will return the default Settings</param>
        /// <returns></returns>
        private static FabricTransportRemotingSettings GetDefaultFabricTransportSettings(string sectionName = "TransportSettings")
        {
            FabricTransportRemotingSettings settings = (FabricTransportRemotingSettings)null;
            if (!FabricTransportRemotingSettings.TryLoadFrom(sectionName, out settings, (string)null, (string)null))
            {
                settings = new FabricTransportRemotingSettings();
            }
            return settings;
        }

        internal static FabricTransportRemotingListenerSettings GetActorListenerSettings(ActorService actorService)
        {
            FabricTransportRemotingListenerSettings listenerSettings;
            if (!FabricTransportRemotingListenerSettings.TryLoadFrom(ActorNameFormat.GetFabricServiceTransportSettingsSectionName(actorService.ActorTypeInformation.ImplementationType), out listenerSettings, (string)null))
                listenerSettings = GetDefaultFabricTransportListenerSettings("TransportSettings");
            return listenerSettings;
        }

        internal static FabricTransportRemotingListenerSettings GetDefaultFabricTransportListenerSettings(string sectionName = "TransportSettings")
        {
            FabricTransportRemotingListenerSettings listenerSettings = (FabricTransportRemotingListenerSettings)null;
            if (!FabricTransportRemotingListenerSettings.TryLoadFrom(sectionName, out listenerSettings, (string)null))
            {
                listenerSettings = new FabricTransportRemotingListenerSettings();
            }
            return listenerSettings;
        }
    }	
}
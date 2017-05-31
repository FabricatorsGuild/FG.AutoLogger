using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading.Tasks;
using CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics;
using CodeEffect.ServiceFabric.Actors.FabricTransport.Utils;
using CodeEffect.ServiceFabric.Actors.Remoting.Runtime;
using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace CodeEffect.ServiceFabric.Actors.Remoting.FabricTransport.Client
{
    public class FabricTransportActorRemotingClient : IServiceRemotingClient, ICommunicationClient
    {
        private readonly IServiceRemotingClient _innerClient;
        private readonly Uri _serviceUri;
        private readonly IServiceCommunicationLogger _logger;

        private static readonly ConcurrentDictionary<long, string> MethodMap = new ConcurrentDictionary<long, string>();

        public IActorServiceCommunicationLogger CommunicationLogger { get; set; }
        
        public FabricTransportActorRemotingClient(IServiceRemotingClient innerClient, Uri serviceUri, IServiceCommunicationLogger logger)
        {
            _innerClient = innerClient;
            _serviceUri = serviceUri;
            _logger = logger;
        }

        ~FabricTransportActorRemotingClient()
        {
            if (this._innerClient == null) return;
            // ReSharper disable once SuspiciousTypeConversion.Global
            var disposable = this._innerClient as IDisposable;
            disposable?.Dispose();
        }        

        Task<byte[]> IServiceRemotingClient.RequestResponseAsync(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
        {
            if (ServiceRequestContext.Current != null && ServiceRequestContext.Current.Headers.Any())
            {
                messageHeaders.AddHeaders(ServiceRequestContext.Current.Headers);
            }
            var headers = messageHeaders.GetCustomServiceRequestHeader(_logger) ?? new CustomServiceRequestHeader();
            try
            {
                _logger.StartMessageSend(_serviceUri, headers);
                var result = this._innerClient.RequestResponseAsync(messageHeaders, requestBody);
                return result;
            }
            catch (Exception ex)
            {
                _logger.FailedtoSendMessage(_serviceUri, headers, ex);
                throw ex;
            }
            finally
            {
                _logger.StopMessageSend(_serviceUri, headers);
            }
        }

        void IServiceRemotingClient.SendOneWay(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
        {
            if (ServiceRequestContext.Current != null && ServiceRequestContext.Current.Headers.Any())
            {
                messageHeaders.AddHeaders(ServiceRequestContext.Current.Headers);
            }
            var headers = messageHeaders.GetCustomServiceRequestHeader(_logger) ?? new CustomServiceRequestHeader();
            try
            {
                _logger.StartMessageSend(_serviceUri, headers);
                this._innerClient.SendOneWay(messageHeaders, requestBody);
            }
            catch (Exception ex)
            {
                _logger.FailedtoSendMessage(_serviceUri, headers, ex);
                throw ex;
            }
            finally
            {
                _logger.StopMessageSend(_serviceUri, headers);
            }         
        }

        public ResolvedServicePartition ResolvedServicePartition
        {
            get { return this._innerClient.ResolvedServicePartition; }
            set { this._innerClient.ResolvedServicePartition = value; }
        }

        public string ListenerName
        {
            get { return this._innerClient.ListenerName; }
            set { this._innerClient.ListenerName = value; }
        }
        public ResolvedServiceEndpoint Endpoint
        {
            get { return this._innerClient.Endpoint; }
            set { this._innerClient.Endpoint = value; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FG.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Runtime;
using TitleService.Diagnostics;

namespace TitleService
{
    internal sealed class TitleService : StatefulService, ITitleService
    {
        private readonly ICommunicationLogger _communicationLogger;

        public TitleService(StatefulServiceContext context)
            : base(context)
        {
            _communicationLogger = new CommunicationLogger(this.Context);
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[]
            {
                this.CreateServiceReplicaListener(this.Context, _communicationLogger),
            };
        }
        
        public Task<string[]> GetTitlesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(ObjectMother.Titles);
        }

        public Task UpdateTitleAsync(string person, string title, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string[]> GetPersonsWithTitleAsync(string title, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public static class ObjectMother
    {
        public static string[] Titles = new string[]
        {
            "Doctor",
            "Overlord",
            "Mister",
            "Fraulein",
        };
    }
}

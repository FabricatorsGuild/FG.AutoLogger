using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Runtime;

namespace TitleService
{
    internal sealed class TitleService : StatefulService, ITitleService
    {
        public TitleService(StatefulServiceContext context)
            : base(context)
        {
            
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[]
            {
                
            };
        }
        
        public Task<string[]> GetTitlesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
}

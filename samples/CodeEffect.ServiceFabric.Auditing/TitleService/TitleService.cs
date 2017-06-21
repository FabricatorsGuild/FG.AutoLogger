using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using FG.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using TitleService.Diagnostics;

namespace TitleService
{
    internal sealed class TitleService : StatefulService, ITitleService
    {
        private readonly ICommunicationLogger _communicationLogger;

		private readonly IDictionary<string, PersonStatistics> _personStatistics = new ConcurrentDictionary<string, PersonStatistics>();

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
	        var personStatistic = _personStatistics.ContainsKey(title) ? _personStatistics[title] : new PersonStatistics() {Title = title, Persons = new string[0]};

	        var persons = new List<string>(personStatistic.Persons) {person};
	        personStatistic.Persons = persons.ToArray();

	        _personStatistics[title] = personStatistic;
			return Task.FromResult(true);
        }

        public Task<string[]> GetPersonsWithTitleAsync(string title, CancellationToken cancellationToken)
        {
			var personStatistic = _personStatistics.ContainsKey(title) ? _personStatistics[title] : new PersonStatistics() { Title = title, Persons = new string[0] };
	        return Task.FromResult(personStatistic.Persons);
        }
    }

	public class PersonStatistics
	{
		public string Title { get; set; }
		public string[] Persons { get; set; }
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

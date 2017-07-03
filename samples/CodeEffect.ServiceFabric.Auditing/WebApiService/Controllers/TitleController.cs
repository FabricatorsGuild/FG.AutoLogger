using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Common;
using FG.ServiceFabric.Services.Remoting.Runtime.Client;
using Microsoft.ServiceFabric.Services.Client;
using TitleService;
using WebApiService.Diagnostics;

namespace WebApiService.Controllers
{
	[ServiceRequestActionFilter]
	public class TitleController : ApiController, ILoggableController
	{
		private readonly object _lock = new object();

		private readonly IWebApiLogger _logger;
		private readonly ICommunicationLogger _servicesCommunicationLogger;

		private static PartitionHelper _partitionHelper;

		private readonly ServiceRequestContextWrapperX _contextScope;

		private PartitionHelper GetOrCreatePartitionHelper()
		{
			if (_partitionHelper != null)
			{
				return _partitionHelper;
			}

			lock (_lock)
			{
				if (_partitionHelper == null)
				{
					_partitionHelper = new PartitionHelper();
				}
				return _partitionHelper;
			}
		}

		public IWebApiLogger Logger => _logger;
		public IDisposable RequestLoggingContext { get; set; }

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			_contextScope.Dispose();
		}

		public TitleController(StatelessServiceContext context)
		{
			_contextScope = new ServiceRequestContextWrapperX(correlationId: Guid.NewGuid().ToString(), userId: "mainframe64/Kapten_rödskägg");

			_logger = new WebApiLogger(context);
			_servicesCommunicationLogger = new CommunicationLogger(context);

			_logger.ActivatingController(_contextScope.CorrelationId, _contextScope.UserId);
		}


		// GET api/values 
		public async Task<IDictionary<string, IList<string>>> Get()
		{
			var serviceUri = new Uri($"{FabricRuntime.GetActivationContext().ApplicationName}/TitleService");
			var allPersons = new Dictionary<string, IList<string>>();

			var ct = CancellationToken.None;
			var partitionKeys = await GetOrCreatePartitionHelper().GetInt64Partitions(serviceUri, _servicesCommunicationLogger);
			foreach (var partitionKey in partitionKeys)
			{
				var serviceProxyFactory = new FG.ServiceFabric.Services.Remoting.Runtime.Client.ServiceProxyFactory(_servicesCommunicationLogger);
				var proxy = serviceProxyFactory.CreateServiceProxy<ITitleService>(
					serviceUri, 
					new ServicePartitionKey(partitionKey.LowKey));

				var partition = $"{partitionKey.LowKey}-{partitionKey.HighKey}";
				var partitionTitles = new Dictionary<string, IDictionary<string, string>>();

				var titles = await proxy.GetTitlesAsync(ct);
				foreach (var title in titles)
				{
					var persons = await proxy.GetPersonsWithTitleAsync(title, ct);
					var personsByTitle = new List<string>(persons);

					if (allPersons.ContainsKey(title))
					{
						personsByTitle.AddRange(allPersons[title]);
					}
					allPersons[title] = personsByTitle;
				}
			}

			return allPersons;
		}

	}
}
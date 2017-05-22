using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace StatelessRunnerService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class StatelessRunnerService : StatelessService
    {
        private readonly IStatelessRunnerServiceLogger _logger;

        public StatelessRunnerService(StatelessServiceContext context)
            : base(context)
        {
            _logger = new StatelessRunnerServiceLogger(this.Context);
            /*
            telemetryClient.Context.User.Id = Environment.UserName;
            telemetryClient.Context.Session.Id = Guid.NewGuid().ToString();
            telemetryClient.Context.Device.OperatingSystem = Environment.OSVersion.ToString();             */
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {

            long iterations = 0;

            while (true)
            {
                _logger.StartRunAsyncLoop((int) iterations, Guid.NewGuid());

                cancellationToken.ThrowIfCancellationRequested();

                _logger.RunAsyncWorkCompleted((int) iterations);

                var startTime = DateTime.UtcNow;
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
                finally
                {
                    _logger.Waiting((int) stopwatch.ElapsedMilliseconds);
                }

                _logger.StopRunAsyncLoop();
            }

        }
    }
}

using DRNJ.Petro.Components.Aggregator;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DRNJ.Petro.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IAggregator Aggregator;
        private readonly int PollIntervalInMinutes;
        public Worker(ILogger<Worker> logger, IAggregator aggregator, int pollInterval)
        {
            this._logger = logger;
            this.Aggregator = aggregator;
            this.PollIntervalInMinutes = pollInterval;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Starting up");
            while (!stoppingToken.IsCancellationRequested)
            {
                //--------------------------------
                // Only basic exception handling |
                //--------------------------------
                try
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                    await this.Aggregator.Start(DateTime.Now);

                    //------------------------------------------------
                    // Wait poll interval minutes - Calculated in mS |
                    //------------------------------------------------
                    await Task.Delay(PollIntervalInMinutes * 60 * 1000, stoppingToken);

                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Worker Exception");
                    throw ex;
                }

            }
        }
    }
}

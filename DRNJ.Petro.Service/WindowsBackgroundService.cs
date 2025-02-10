using DRNJ.Petro.Components.Aggregate;

namespace DRNJ.Petro.Service;


/// <summary>
/// Background service - boilerplate code
/// </summary>
public class WindowsBackgroundService : BackgroundService
{
    private readonly ILogger<WindowsBackgroundService> _logger;
    private readonly IAggregator Aggregator;
    private readonly int PollIntervalInMinutes;

    public WindowsBackgroundService(ILogger<WindowsBackgroundService> logger, IAggregator aggregator, int pollInterval)
    {
        this._logger = logger;
        this.Aggregator = aggregator;
        this.PollIntervalInMinutes = pollInterval;

    }

    /// <summary>
    /// Main method - infinite loop firing off worker then sleeping for poll interval
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning("Worker running at: {time}", DateTimeOffset.Now);

                //-------------------------------------------------
                // Spawn task to do the work                      |
                //-------------------------------------------------

                Task.Run(() => this.Aggregator.Start(DateTime.Now));

                //------------------------------------------------
                // Wait poll interval minutes - Calculated in mS |
                //------------------------------------------------

                await Task.Delay(PollIntervalInMinutes * 60 * 1000, stoppingToken);

            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal Error in Windows Service. Service will Terminate. Error : {Message}", ex.Message);

            // Terminates this process and returns an exit code to the operating system.
            // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
            // performs one of two scenarios:
            // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
            // 2. When set to "StopHost": will cleanly stop the host, and log errors.
            //
            // In order for the Windows Service Management system to leverage configured
            // recovery options, we need to terminate the process with a non-zero exit code.
            Environment.Exit(1);
        }
    }
}


using Boiler.Server.Framework;

namespace Boiler.Server.HostedServices;

public class DataCleanerService : IHostedService
{
    private readonly ILogger<DataCleanerService> _logger;
    private readonly IServiceProvider _serviceProvider;

    private static readonly TimeSpan RunPeriod = TimeSpan.FromMinutes(30);
    private Timer _timer;

    public DataCleanerService(ILogger<DataCleanerService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Spustila se služba pro mazání starých záznamů.");

        ScheduleNextRun(cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Služba pro mazání starých záznamů byla zastavena.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    private void OnSyncTimerTick(object state)
    {
        var cancellationToken = (CancellationToken)state;

        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ServerDbContext>();

            try
            {
                var records = dbContext.Temperatures.Where(x => x.Time < TimeExtension.PragueDateNowAsUtc().AddMonths(-1)).ToList();
                dbContext.RemoveRange(records);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Nepodařilo se vymazat staré záznamy.");
            }
        }

        if (!cancellationToken.IsCancellationRequested)
        {
            ScheduleNextRun(cancellationToken);
        }
    }


    private void ScheduleNextRun(CancellationToken cancellationToken)
    {
        if (_timer != null)
            _timer.Dispose();
        _timer = new Timer(OnSyncTimerTick, cancellationToken, RunPeriod, Timeout.InfiniteTimeSpan);
    }
}

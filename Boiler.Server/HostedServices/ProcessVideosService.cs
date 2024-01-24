using Boiler.Server.Services.Camera;

namespace Boiler.Server.HostedServices;

public class ProcessVideosService : IHostedService
{
    private readonly ILogger<DataCleanerService> _logger;
    private readonly IServiceProvider _serviceProvider;

    private static readonly TimeSpan RunPeriod = TimeSpan.FromMinutes(30);
    private Timer _timer;

    public ProcessVideosService(ILogger<DataCleanerService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Spustila se služba pro zpracování videa.");

        ScheduleNextRun(cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Služba pro zpracování videa byla zastavena.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    private void OnSyncTimerTick(object state)
    {
        var cancellationToken = (CancellationToken)state;

        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ServerDbContext>();
            var faceService = scope.ServiceProvider.GetRequiredService<IFaceService>();

            try
            {
                var videos = dbContext.Video.Where(x => x.Processed == false).ToList();
                foreach (var video in videos)
                {
                    var detectedFaces = faceService.DetectFaces(video);
                    dbContext.AddRange(detectedFaces);
                    video.Processed = true;
                    dbContext.Video.Update(video);
                }
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Nepodařilo se zpracovat video.");
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

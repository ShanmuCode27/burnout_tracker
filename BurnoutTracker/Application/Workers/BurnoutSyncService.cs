using BurnoutTracker.Application.Services;

namespace BurnoutTracker.Application.Workers
{
    public class BurnoutSyncService: BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BurnoutSyncService> _logger;

        public BurnoutSyncService(IServiceScopeFactory scopeFactory, ILogger<BurnoutSyncService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var burnoutService = scope.ServiceProvider.GetRequiredService<BurnoutDetectionService>();

                try
                {
                    await burnoutService.ProcessBurnoutStatesForAllConnectionsAsync();
                    _logger.LogInformation("Burnout sync completed at {Time}", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during burnout sync");
                }

                await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
            }
        }
    }
}

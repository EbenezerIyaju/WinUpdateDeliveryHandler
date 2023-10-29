namespace WinUpdateDelivery
{
    public class DeliveryOptmizationHandler : BackgroundService
    {
        private readonly ILogger<DeliveryOptmizationHandler> _logger;

        public DeliveryOptmizationHandler(ILogger<DeliveryOptmizationHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
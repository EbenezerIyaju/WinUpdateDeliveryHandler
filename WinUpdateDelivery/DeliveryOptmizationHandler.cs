using System.ServiceProcess;
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
                // Stop Windows Delivery Optimization service
                StopService("DoSvc");

                // Stop Windows Update service
                StopService("wuauserv");

                await Task.Delay(5000, stoppingToken); // Check every 5 seconds
            }
        }

        private void StopService(string serviceName)
        {
            using (ServiceController service = new ServiceController(serviceName))
            {
                if (service.Status != ServiceControllerStatus.Stopped)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(5));
                    _logger.LogInformation($"{serviceName} service stopped.");
                }
            }
        }
    }
}
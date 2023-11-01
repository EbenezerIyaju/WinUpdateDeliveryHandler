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
            try
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
            catch (OperationCanceledException)
            {
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);

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

        private void StopService(string serviceName)
        {
            using (ServiceController service = new ServiceController(serviceName))
            {
                if (service.Status != ServiceControllerStatus.Stopped)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(5));
                    _logger.LogInformation($"{serviceName} service stopped." );
                }
            }
        }
    }
}
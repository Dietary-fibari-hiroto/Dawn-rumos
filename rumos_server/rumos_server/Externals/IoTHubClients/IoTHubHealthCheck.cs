using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace rumos_server.Externals.IoTHubClients
{
    public class IoTHubHealthCheck : IHealthCheck
    {
        private readonly IoTHubConnectionService _iotHub;
        private readonly ILogger<IoTHubHealthCheck> _logger;

        public IoTHubHealthCheck(
            IoTHubConnectionService iotHub,
            ILogger<IoTHubHealthCheck> logger)
        {
            _iotHub = iotHub;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // 修正: CancellationToken なし
                var stats = await _iotHub.RegistryManager.GetRegistryStatisticsAsync();

                return HealthCheckResult.Healthy(
                    "IoT Hub is connected",
                    new Dictionary<string, object>
                    {
                { "totalDevices", stats.TotalDeviceCount },
                { "enabledDevices", stats.EnabledDeviceCount }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IoT Hub health check failed");
                return HealthCheckResult.Unhealthy("IoT Hub is not accessible", ex);
            }
        }
    }
}
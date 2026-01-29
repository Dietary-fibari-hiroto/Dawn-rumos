using Microsoft.Extensions.Hosting;

namespace rumos_server.Externals.IoTHubClients
{
    public sealed class IoTHubHostedService : BackgroundService
    {
        private readonly IoTHubConnectionService _iotHub;

        public IoTHubHostedService(IoTHubConnectionService iotHub)
        {
            _iotHub = iotHub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _iotHub.OpenAsync(stoppingToken);
        }
    }
}
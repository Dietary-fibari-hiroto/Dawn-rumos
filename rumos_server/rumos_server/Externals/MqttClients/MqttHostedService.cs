using Microsoft.Extensions.Hosting;

namespace rumos_server.Externals.MqttClients
{
    public sealed class MqttHostedService:BackgroundService
    {
        private readonly MqttConnectionService _mqtt;

        public MqttHostedService(MqttConnectionService mqtt)
        {
            _mqtt = mqtt;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // アプリ起動時に接続
            await _mqtt.ConnectAsync(stoppingToken);
        }
    }
}

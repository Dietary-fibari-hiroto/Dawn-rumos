using MQTTnet;
using System.Text.Json;
using MQTTnet.Protocol;

namespace rumos_server.Externals.MqttClients
{
    public sealed class MqttService
    {
        private readonly MqttConnectionService _conn;

        public MqttService(MqttConnectionService conn)
        {
            _conn = conn;
        }

        public async Task SendColorAsyncForAll(LedColor color,CancellationToken ct = default)
        {
            await _conn.EnsureConnectedAsync(ct);

            var payload = JsonSerializer.Serialize(color);
            var msg = new MqttApplicationMessageBuilder()
                .WithTopic("dawn/led/all")
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false)
                .Build();

            await _conn.Client.PublishAsync(msg, ct);
        }

        //特定のデバイスにリクエスト
        public async Task SendColorAsync(LedColor color,string DeviceName,CancellationToken ct = default)
        {
            await _conn.EnsureConnectedAsync(ct);

            var payload = JsonSerializer.Serialize(color);
            var msg = new MqttApplicationMessageBuilder()
                .WithTopic($"dawn/led/{DeviceName}")
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false)
                .Build();

            await _conn.Client.PublishAsync( msg, ct);

        }
    }
}

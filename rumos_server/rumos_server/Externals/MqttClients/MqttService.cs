using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using rumos_server.Features.DTOs;
using System.Text;
using System.Text.Json;

namespace rumos_server.Externals.MqttClients
{
    public sealed class MqttService
    {
        private readonly MqttConnectionService _conn;

        public MqttService(MqttConnectionService conn)
        {
            _conn = conn;
        }

        //全デバイスに発光リクエスト
        private bool _isResponseHandlerSet = false;

        public async Task SendColorAsyncForAll(LedColor color, CancellationToken ct = default)
        {
            await _conn.EnsureConnectedAsync(ct);

            if (!_isResponseHandlerSet)
            {
                await _conn.Client.SubscribeAsync(
                    new MqttTopicFilterBuilder()
                        .WithTopic("dawn/led/response")
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                        .Build()
                );

                _conn.Client.ApplicationMessageReceivedAsync += async e =>
                {
                    if (e.ApplicationMessage.Topic == "dawn/led/response")
                    {
                        var seg = e.ApplicationMessage.PayloadSegment;
                        var payload = Encoding.UTF8.GetString(seg.Array, seg.Offset, seg.Count);
                        Console.WriteLine($"[Response] {payload}");

                        var status = JsonSerializer.Deserialize<DeviceDto>(payload);
                        // TODO: 状態を反映する処理
                    }
                    await Task.CompletedTask;
                };

                _isResponseHandlerSet = true;
            }

            // publish
            var payloadToSend = JsonSerializer.Serialize(color);
            var msg = new MqttApplicationMessageBuilder()
                .WithTopic("dawn/led/all")
                .WithPayload(payloadToSend)
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

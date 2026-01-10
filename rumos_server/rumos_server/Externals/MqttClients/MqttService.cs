using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Text;
using System.Text.Json;

namespace rumos_server.Externals.MqttClients
{
    /// <summary>
    /// MQTTメッセージ送信のビジネスロジック
    /// Scopedライフサイクルでリクエストごとに新しいインスタンスが作成される
    /// </summary>
    public sealed class MqttService
    {
        private readonly MqttConnectionService _conn;
        private readonly ILogger<MqttService> _logger;

        // レスポンストピック（定数化）
        private const string RESPONSE_TOPIC = "dawn/led/response";
        private const string ALL_DEVICES_TOPIC = "dawn/led/all";
        private const string DEVICE_TOPIC_PREFIX = "dawn/led/";

        public MqttService(
            MqttConnectionService conn,
            ILogger<MqttService> logger)
        {
            _conn = conn;
            _logger = logger;
        }

        /// <summary>
        /// 全デバイスに一斉送信
        /// </summary>
        public async Task SendColorAsyncForAll(LedColor color, CancellationToken ct = default)
        {
            // 接続確認
            await _conn.EnsureConnectedAsync(ct);

            // レスポンスハンドラを登録（Singletonで1回だけ）
            await _conn.EnsureResponseHandlerAsync(
                RESPONSE_TOPIC,
                HandleResponseAsync,
                ct);

            // メッセージ送信
            var payload = JsonSerializer.Serialize(color);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(ALL_DEVICES_TOPIC)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false)
                .Build();

            _logger.LogInformation(
                "Sending color to all devices: R={R}, G={G}, B={B}, Brightness={Brightness}, Mode={Mode}",
                color.R, color.G, color.B, color.Brightness, color.Mode);

            await _conn.Client.PublishAsync(message, ct);

            _logger.LogInformation("Color sent to all devices successfully");
        }

        /// <summary>
        /// 特定デバイスに送信
        /// </summary>
        public async Task SendColorAsync(
            LedColor color,
            string deviceName,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
            {
                throw new ArgumentException("Device name cannot be null or empty", nameof(deviceName));
            }

            // 接続確認
            await _conn.EnsureConnectedAsync(ct);

            // メッセージ送信
            var topic = $"{DEVICE_TOPIC_PREFIX}{deviceName}";
            var payload = JsonSerializer.Serialize(color);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false)
                .Build();

            _logger.LogInformation(
                "Sending color to device '{Device}': R={R}, G={G}, B={B}, Brightness={Brightness}, Mode={Mode}",
                deviceName, color.R, color.G, color.B, color.Brightness, color.Mode);

            await _conn.Client.PublishAsync(message, ct);

            _logger.LogInformation("Color sent to device '{Device}' successfully", deviceName);
        }

        /// <summary>
        /// 複数デバイスに個別送信
        /// </summary>
        public async Task SendColorsToBatchAsync(
            Dictionary<string, LedColor> deviceColors,
            CancellationToken ct = default)
        {
            if (deviceColors == null || !deviceColors.Any())
            {
                throw new ArgumentException("Device colors dictionary cannot be null or empty", nameof(deviceColors));
            }

            await _conn.EnsureConnectedAsync(ct);

            _logger.LogInformation("Sending colors to {Count} devices", deviceColors.Count);

            var tasks = deviceColors.Select(kvp =>
                SendColorAsync(kvp.Value, kvp.Key, ct));

            await Task.WhenAll(tasks);

            _logger.LogInformation("Colors sent to all {Count} devices successfully", deviceColors.Count);
        }

        /// <summary>
        /// デバイスからのレスポンスを処理
        /// </summary>
        private Task HandleResponseAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                if (e.ApplicationMessage.Topic != RESPONSE_TOPIC)
                {
                    return Task.CompletedTask;
                }

                var segment = e.ApplicationMessage.PayloadSegment;
                var payload = Encoding.UTF8.GetString(
                    segment.Array,
                    segment.Offset,
                    segment.Count);

                _logger.LogInformation("Received device response: {Payload}", payload);

                // JSONデシリアライズして処理
                try
                {
                    var response = JsonSerializer.Deserialize<DeviceResponse>(payload);
                    if (response != null)
                    {
                        _logger.LogInformation(
                            "Device '{Device}' status: {Status}",
                            response.DeviceName ?? "Unknown",
                            response.Status ?? "Unknown");

                        // TODO: データベースに状態を保存
                        // TODO: SignalRでクライアントに通知
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize device response");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling device response");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// デバイスのステータスをリクエスト
        /// </summary>
        public async Task RequestDeviceStatusAsync(
            string deviceName,
            CancellationToken ct = default)
        {
            await _conn.EnsureConnectedAsync(ct);

            var topic = $"{DEVICE_TOPIC_PREFIX}{deviceName}/status/request";
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload("get_status")
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            _logger.LogInformation("Requesting status from device '{Device}'", deviceName);

            await _conn.Client.PublishAsync(message, ct);
        }
    }

    /// <summary>
    /// デバイスからのレスポンスのデータ構造
    /// </summary>
    public class DeviceResponse
    {
        public string? DeviceName { get; set; }
        public string? Status { get; set; }
        public int? R { get; set; }
        public int? G { get; set; }
        public int? B { get; set; }
        public int? Brightness { get; set; }
        public string? Mode { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
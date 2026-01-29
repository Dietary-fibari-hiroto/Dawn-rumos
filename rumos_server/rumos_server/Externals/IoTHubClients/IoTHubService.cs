using Microsoft.Azure.Devices;
using System.Text;
using System.Text.Json;

namespace rumos_server.Externals.IoTHubClients
{
    /// <summary>
    /// IoT Hub メッセージ送信のビジネスロジック
    /// </summary>
    public sealed class IoTHubService
    {
        private readonly IoTHubConnectionService _conn;
        private readonly ILogger<IoTHubService> _logger;

        public IoTHubService(
            IoTHubConnectionService conn,
            ILogger<IoTHubService> logger)
        {
            _conn = conn;
            _logger = logger;
        }

        /// <summary>
        /// 特定デバイスにLEDカラーを送信 (C2D)
        /// </summary>
        public async Task SendColorAsync(
            LedColor color,
            string deviceId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new ArgumentException("Device ID cannot be null or empty", nameof(deviceId));
            }

            var payload = JsonSerializer.Serialize(color);
            var message = new Message(Encoding.UTF8.GetBytes(payload))
            {
                ContentType = "application/json",
                ContentEncoding = "utf-8",
                MessageId = Guid.NewGuid().ToString(),
            };
            message.Properties.Add("command", "set_color");

            _logger.LogInformation(
                "Sending C2D to device '{Device}': R={R}, G={G}, B={B}, Brightness={Brightness}, Mode={Mode}",
                deviceId, color.R, color.G, color.B, color.Brightness, color.Mode);

            try
            {
                // 修正: CancellationToken ではなく TimeSpan? を受け取る
                await _conn.ServiceClient.SendAsync(deviceId, message);
                _logger.LogInformation("C2D sent to device '{Device}' successfully", deviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send C2D to device '{Device}'", deviceId);
                throw;
            }
        }

        /// <summary>
        /// 複数デバイスに送信
        /// </summary>
        public async Task SendColorsToBatchAsync(
            Dictionary<string, LedColor> deviceColors,
            CancellationToken ct = default)
        {
            if (deviceColors == null || !deviceColors.Any())
            {
                throw new ArgumentException("Device colors cannot be null or empty", nameof(deviceColors));
            }

            _logger.LogInformation("Sending colors to {Count} devices", deviceColors.Count);

            var tasks = deviceColors.Select(kvp =>
                SendColorAsync(kvp.Value, kvp.Key, ct));

            await Task.WhenAll(tasks);

            _logger.LogInformation("Colors sent to {Count} devices successfully", deviceColors.Count);
        }

        /// <summary>
        /// 全デバイスに送信（デバイスIDリストが必要）
        /// </summary>
        public async Task SendColorToAllAsync(
            LedColor color,
            IEnumerable<string> deviceIds,
            CancellationToken ct = default)
        {
            var ids = deviceIds.ToList();
            _logger.LogInformation("Sending color to all {Count} devices", ids.Count);

            var tasks = ids.Select(id => SendColorAsync(color, id, ct));
            await Task.WhenAll(tasks);

            _logger.LogInformation("Color sent to all devices successfully");
        }

        /// <summary>
        /// ダイレクトメソッド呼び出し（即時応答が必要な場合）
        /// </summary>
        /// <summary>
        /// ダイレクトメソッド呼び出し
        /// </summary>
        public async Task<int> InvokeMethodAsync(
            string deviceId,
            string methodName,
            object payload,
            CancellationToken ct = default)
        {
            var methodInvocation = new CloudToDeviceMethod(methodName)
            {
                ResponseTimeout = TimeSpan.FromSeconds(30)
            };

            methodInvocation.SetPayloadJson(JsonSerializer.Serialize(payload));

            _logger.LogInformation(
                "Invoking method '{Method}' on device '{Device}'",
                methodName, deviceId);

            // 修正: CancellationToken なし
            var response = await _conn.ServiceClient.InvokeDeviceMethodAsync(
                deviceId, methodInvocation);

            _logger.LogInformation(
                "Method '{Method}' response from '{Device}': Status={Status}",
                methodName, deviceId, response.Status);

            return response.Status;
        }
    }

    /// <summary>
    /// LEDカラーモデル（既存のMqttModels.csと同じ）
    /// </summary>
    public sealed class LedColor
    {
        public int R { get; set; } = 0;
        public int G { get; set; } = 0;
        public int B { get; set; } = 0;
        public int Brightness { get; set; } = 0;
        public string Mode { get; set; } = "normal";
    }
}
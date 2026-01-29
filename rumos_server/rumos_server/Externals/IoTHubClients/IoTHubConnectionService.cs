using Microsoft.Azure.Devices;
using Microsoft.Extensions.Options;

namespace rumos_server.Externals.IoTHubClients
{
    /// <summary>
    /// IoT Hub接続を管理するSingletonサービス
    /// </summary>
    public sealed class IoTHubConnectionService : IAsyncDisposable
    {
        private readonly ILogger<IoTHubConnectionService> _logger;
        private readonly ServiceClient _serviceClient;
        private readonly RegistryManager _registryManager;
        private volatile bool _disposed;

        public IoTHubConnectionService(
            IOptions<IoTHubSettings> options,
            ILogger<IoTHubConnectionService> logger)
        {
            _logger = logger;
            var settings = options.Value;

            if (string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                throw new ArgumentException("IoT Hub connection string is required");
            }

            // C2Dメッセージ送信用
            _serviceClient = ServiceClient.CreateFromConnectionString(
                settings.ConnectionString,
                TransportType.Amqp);

            // デバイス管理用（オプション）
            _registryManager = RegistryManager.CreateFromConnectionString(
                settings.ConnectionString);

            _logger.LogInformation("IoT Hub connection service initialized");
        }

        public ServiceClient ServiceClient => _serviceClient;
        public RegistryManager RegistryManager => _registryManager;

        /// <summary>
        /// 接続を開く
        /// </summary>
        public async Task OpenAsync(CancellationToken ct = default)
        {
            await _serviceClient.OpenAsync();
            _logger.LogInformation("IoT Hub ServiceClient opened");
        }

        /// <summary>
        /// デバイスが存在するか確認
        /// </summary>
        public async Task<bool> DeviceExistsAsync(string deviceId, CancellationToken ct = default)
        {
            try
            {
                var device = await _registryManager.GetDeviceAsync(deviceId, ct);
                return device != null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to check device existence: {DeviceId}", deviceId);
                return false;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            _logger.LogInformation("Disposing IoT Hub connection...");

            try
            {
                await _serviceClient.CloseAsync();
                _serviceClient.Dispose();
                _registryManager.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during IoT Hub disposal");
            }

            _logger.LogInformation("IoT Hub connection disposed");
        }
    }
}
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace rumos_server.Externals.MqttClients
{
    /// <summary>
    /// MQTT接続のヘルスチェック
    /// /health エンドポイントでMQTT接続状態を確認できる
    /// </summary>
    public class MqttHealthCheck : IHealthCheck
    {
        private readonly MqttConnectionService _mqttConnection;
        private readonly ILogger<MqttHealthCheck> _logger;

        public MqttHealthCheck(
            MqttConnectionService mqttConnection,
            ILogger<MqttHealthCheck> logger)
        {
            _mqttConnection = mqttConnection;
            _logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (_mqttConnection.IsConnected)
                {
                    _logger.LogDebug("MQTT health check: Healthy");

                    return Task.FromResult(
                        HealthCheckResult.Healthy(
                            "MQTT broker is connected",
                            new Dictionary<string, object>
                            {
                                { "connected", true },
                                { "broker", "MQTT Broker" }
                            }));
                }
                else
                {
                    _logger.LogWarning("MQTT health check: Unhealthy - Not connected");

                    return Task.FromResult(
                        HealthCheckResult.Unhealthy(
                            "MQTT broker is not connected",
                            data: new Dictionary<string, object>
                            {
                                { "connected", false },
                                { "status", "Attempting to reconnect..." }
                            }));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MQTT health check failed");

                return Task.FromResult(
                    HealthCheckResult.Unhealthy(
                        "MQTT health check threw an exception",
                        ex,
                        new Dictionary<string, object>
                        {
                            { "error", ex.Message }
                        }));
            }
        }
    }
}
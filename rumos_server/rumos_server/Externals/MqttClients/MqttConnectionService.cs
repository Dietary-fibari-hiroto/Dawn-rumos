using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Microsoft.Extensions.Options;

namespace rumos_server.Externals.MqttClients
{
    /// <summary>
    /// MQTT接続を管理するSingletonサービス
    /// 自動再接続、接続状態監視、レスポンスハンドラ管理を担当
    /// </summary>
    public sealed class MqttConnectionService : IAsyncDisposable
    {
        private readonly ILogger<MqttConnectionService> _logger;
        private readonly IMqttClient _client;
        private readonly MqttClientOptions _options;
        private readonly SemaphoreSlim _connectLock = new(1, 1);
        private readonly string _clientId;

        // イベントハンドラ
        private Func<IMqttClient, Task>? _onConnectedAsync;
        private Func<MqttApplicationMessageReceivedEventArgs, Task>? _responseHandler;

        // 状態管理
        private volatile bool _disposed;
        private volatile bool _responseHandlerSet = false;
        private int _reconnectAttempts = 0;
        private const int MAX_RECONNECT_ATTEMPTS = 50; // 最大再試行回数

        public MqttConnectionService(
            IOptions<MqttSettings> opt,
            ILogger<MqttConnectionService> logger)
        {
            _logger = logger;
            var settings = opt.Value;
            _clientId = settings.ClientId;

            _client = new MqttFactory().CreateMqttClient();

            // 接続オプション構築
            var builder = new MqttClientOptionsBuilder()
                .WithClientId(settings.ClientId)
                .WithTcpServer(settings.Broker, settings.Port)
                .WithCleanSession(settings.CleanSession)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(settings.KeepAliveSeconds));

            // 認証情報
            if (!string.IsNullOrWhiteSpace(settings.Username))
            {
                builder = builder.WithCredentials(settings.Username, settings.Password);
            }

            // TLS/SSL
            if (settings.UseTls)
            {
                builder = builder.WithTls();
            }

            // 遺言メッセージ（Will Message）
            var will = new MqttApplicationMessageBuilder()
                .WithTopic($"system/clients/{settings.ClientId}/status")
                .WithPayload("offline")
                .WithRetainFlag(true)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            builder = builder.WithWillTopic(will.Topic)
                .WithWillPayload(will.PayloadSegment)
                .WithWillRetain(will.Retain)
                .WithWillQualityOfServiceLevel(will.QualityOfServiceLevel);

            _options = builder.Build();

            // イベントハンドラ登録
            RegisterEventHandlers(settings);
        }

        /// <summary>
        /// イベントハンドラを登録
        /// </summary>
        private void RegisterEventHandlers(MqttSettings settings)
        {
            // 接続成功時
            _client.ConnectedAsync += async args =>
            {
                _reconnectAttempts = 0; // 再接続カウンタリセット
                _logger.LogInformation(
                    "MQTT connected successfully. ClientId: {ClientId}, Server: {Server}:{Port}",
                    _clientId, settings.Broker, settings.Port);

                // オンラインステータスをPublish
                await PublishOnlineStatusAsync();

                // 再接続時のハンドラ実行（購読再登録など）
                if (_onConnectedAsync is not null)
                {
                    try
                    {
                        await _onConnectedAsync(_client);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "OnConnected handler failed");
                    }
                }
            };

            // 切断時
            _client.DisconnectedAsync += async args =>
            {
                if (_disposed) return;

                _logger.LogWarning(
                    "MQTT disconnected. Reason: {Reason}, ClientWasConnected: {WasConnected}",
                    args.Reason, args.ClientWasConnected);

                // 自動再接続
                await ReconnectAsync();
            };
        }

        /// <summary>
        /// オンラインステータスを送信
        /// </summary>
        private async Task PublishOnlineStatusAsync()
        {
            try
            {
                var onlineMsg = new MqttApplicationMessageBuilder()
                    .WithTopic($"system/clients/{_clientId}/status")
                    .WithPayload("online")
                    .WithRetainFlag(true)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();

                await _client.PublishAsync(onlineMsg);
                _logger.LogInformation("Published online status");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to publish online status");
            }
        }

        /// <summary>
        /// 自動再接続ロジック（指数バックオフ）
        /// </summary>
        private async Task ReconnectAsync()
        {
            var delay = TimeSpan.FromSeconds(2);
            var maxDelay = TimeSpan.FromSeconds(30);

            while (!_client.IsConnected && !_disposed && _reconnectAttempts < MAX_RECONNECT_ATTEMPTS)
            {
                _reconnectAttempts++;

                try
                {
                    _logger.LogInformation(
                        "Attempting to reconnect... (Attempt {Attempt}/{MaxAttempts})",
                        _reconnectAttempts, MAX_RECONNECT_ATTEMPTS);

                    await _client.ConnectAsync(_options);
                    return; // 成功したら終了
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Reconnect attempt {Attempt} failed. Retrying in {Delay} seconds",
                        _reconnectAttempts, delay.TotalSeconds);

                    await Task.Delay(delay);

                    // 指数バックオフ（2秒 → 4秒 → 8秒 → ... → 最大30秒）
                    delay = TimeSpan.FromSeconds(
                        Math.Min(delay.TotalSeconds * 2, maxDelay.TotalSeconds));
                }
            }

            // 最大試行回数に達した場合
            if (_reconnectAttempts >= MAX_RECONNECT_ATTEMPTS)
            {
                _logger.LogError(
                    "Failed to reconnect after {Attempts} attempts. Giving up.",
                    MAX_RECONNECT_ATTEMPTS);

                throw new MqttBrokerNotFoundException(
                    $"Failed to connect to MQTT broker after {MAX_RECONNECT_ATTEMPTS} attempts");
            }
        }

        /// <summary>
        /// 初回接続（多重接続防止付き）
        /// </summary>
        public async Task ConnectAsync(CancellationToken ct = default)
        {
            if (_client.IsConnected)
            {
                _logger.LogDebug("Already connected to MQTT broker");
                return;
            }

            await _connectLock.WaitAsync(ct);
            try
            {
                if (!_client.IsConnected)
                {
                    _logger.LogInformation("Connecting to MQTT broker...");
                    await _client.ConnectAsync(_options, ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to MQTT broker");
                throw new MqttBrokerNotFoundException(
                    "Failed to connect to MQTT broker. Check broker address and credentials.", ex);
            }
            finally
            {
                _connectLock.Release();
            }
        }

        /// <summary>
        /// 接続確認（切断されていたら再接続）
        /// </summary>
        public async Task EnsureConnectedAsync(CancellationToken ct = default)
        {
            if (!_client.IsConnected)
            {
                _logger.LogWarning("Not connected to MQTT broker. Attempting to connect...");
                await ConnectAsync(ct);
            }
        }

        /// <summary>
        /// 再接続時のハンドラを登録（購読の再登録などに使用）
        /// </summary>
        public void RegisterOnConnectedHandler(Func<IMqttClient, Task> onConnectedAsync)
        {
            _onConnectedAsync = onConnectedAsync;

            // すでに接続済みなら即実行
            if (_client.IsConnected)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await onConnectedAsync(_client);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "OnConnected handler failed during registration");
                    }
                });
            }
        }

        /// <summary>
        /// レスポンスハンドラを登録（Singletonで1回だけ登録）
        /// </summary>
        public async Task EnsureResponseHandlerAsync(
            string responseTopic,
            Func<MqttApplicationMessageReceivedEventArgs, Task> handler,
            CancellationToken ct = default)
        {
            if (_responseHandlerSet) return;

            await _connectLock.WaitAsync(ct);
            try
            {
                if (_responseHandlerSet) return;

                await EnsureConnectedAsync(ct);

                // レスポンストピックをSubscribe
                var subscribeOptions = new MqttTopicFilterBuilder()
                    .WithTopic(responseTopic)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                    .Build();

                await _client.SubscribeAsync(subscribeOptions, ct);

                // ハンドラ登録
                _responseHandler = handler;
                _client.ApplicationMessageReceivedAsync += handler;

                _responseHandlerSet = true;

                _logger.LogInformation("Response handler registered for topic: {Topic}", responseTopic);
            }
            finally
            {
                _connectLock.Release();
            }
        }

        /// <summary>
        /// クライアントへのアクセス（外部から直接使用可能）
        /// </summary>
        public IMqttClient Client => _client;

        /// <summary>
        /// 接続状態を取得
        /// </summary>
        public bool IsConnected => _client.IsConnected;

        /// <summary>
        /// クリーンアップ
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            _disposed = true;

            _logger.LogInformation("Disposing MQTT connection...");

            try
            {
                // レスポンスハンドラ解除
                if (_responseHandler is not null)
                {
                    _client.ApplicationMessageReceivedAsync -= _responseHandler;
                }

                // 接続中なら切断
                if (_client.IsConnected)
                {
                    await _client.DisconnectAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during MQTT disposal");
            }
            finally
            {
                _client?.Dispose();
                _connectLock.Dispose();
            }

            _logger.LogInformation("MQTT connection disposed");
        }
    }
}
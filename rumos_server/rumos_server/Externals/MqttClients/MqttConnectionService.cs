using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Microsoft.Extensions;
using Microsoft.Extensions.Options;

namespace rumos_server.Externals.MqttClients
{
    public sealed class MqttConnectionService : IAsyncDisposable
    {
        private readonly ILogger<MqttConnectionService> _logger;
        private readonly IMqttClient _client;//Mqttクライアント本体
        private readonly MqttClientOptions _options; //接続オプション
        private readonly SemaphoreSlim _connectLock = new(1,1);//同時接続防止オプション
        private Func<IMqttClient, Task>? _onConnectedAsync;
        private volatile bool _disposed;

        public MqttConnectionService(IOptions<MqttSettings> opt,ILogger<MqttConnectionService> logger)
        {
            _logger = logger;
            var s = opt.Value; //設定情報(Broker,port,ClientIdなど)

            _client = new MqttFactory().CreateMqttClient(); //MQTTクライアント作成

            //---接続オプション構築---
            var builder = new MqttClientOptionsBuilder()
                .WithClientId(s.ClientId) //クライアントid指定
                .WithTcpServer(s.Broker, s.Port) //ブローカーのアドレスとポート
                .WithCleanSession(s.CleanSession) ///セッションを保持するか
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(s.KeepAliveSeconds)); //keepAlive間隔

            //ユーザー名とパスワードを後で指定できるようにしとく
            if (!string.IsNullOrWhiteSpace(s.Username))
            {
                builder = builder.WithCredentials(s.Username, s.Password);
            }

            //TLSを有効化準備(未実装)
            if (s.UseTls)
            {
                builder = builder.WithTls();
            }

            //---遺言メッセージ ---

            var will = new MqttApplicationMessageBuilder()
                .WithTopic($"system/clients/{s.ClientId}/status") //遺言を送るトピック
                .WithPayload("offline") //遺言の中身
                .WithRetainFlag(true) //retainで保持
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce) //QoS=1
                .Build();

            //上記設定をまとめてオプション化
            _options = builder.Build();

            //---接続成功時イベント---
            _client.ConnectedAsync += async _ =>
            {
                _logger.LogInformation("MQTT conneted as {ClientId}", s.ClientId);

                //接続したらオンラインステータスをpublish
                try
                {
                    var onlineMsg = new MqttApplicationMessageBuilder()
                    .WithTopic($"system/clients/{s.ClientId}/status")
                    .WithPayload("online")
                    .WithRetainFlag(true)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();

                    await _client.PublishAsync(onlineMsg);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to publish online status.");
                }

                //再接続後に購読し直すなどのハンドラがあれば実行
                if(_onConnectedAsync is not null)
                {
                    try { await _onConnectedAsync(_client); }
                    catch (Exception ex) { _logger.LogError(ex, "OnConnected handler failed."); }
                }
            };

            //---切断時イベント---
            _client.DisconnectedAsync += async e =>
            {
                if (_disposed) return;//Disposed済みなら無視
                _logger.LogWarning("MQTT disconnected. Reason: {Reason}. Will try to reconnect.", e.ReasonString);

                //再接続ループ(指数バックオフ方式)
                var delay = TimeSpan.FromSeconds(2); //初期リトライ間隔
                var maxDelay = TimeSpan.FromSeconds(30); //最大リトライ間隔

                while (!_client.IsConnected && !_disposed)
                {
                    try
                    {
                        await _client.ConnectAsync(_options);
                        return;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Reconnect failed. Retry in {Delay} sec.", delay.TotalSeconds);
                        await Task.Delay(delay);
                        delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 2, maxDelay.TotalSeconds));
                    }
                }

            };

            
        }

        /// <summary>
        /// 起動時などに呼び出して接続する。多重接続はロックで防止。
        /// </summary>
        public async Task ConnectAsync(CancellationToken ct = default)
        {
            if (_client.IsConnected) return;

            await _connectLock.WaitAsync(ct);
            try
            {
                if (!_client.IsConnected)
                {
                    await _client.ConnectAsync(_options, ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ブローカーに接続できませんでした。再試行します。");
                Console.WriteLine("接続失敗。再試行します。:",ex.Message);
                // ここで再接続ループに入れる or ユーザーに通知
            }
            finally
            {
                _connectLock.Release();
            }
        }





        /// <summary>
        /// 再接続時に購読再登録などを行うためのハンドラを登録
        /// </summary>
        public void RegisterOnConnectedHandler(Func<IMqttClient, Task> onConnectedAsync)
        {
            _onConnectedAsync = onConnectedAsync;
            // すでに接続済みなら即実行
            if (_client.IsConnected)
            {
                _ = onConnectedAsync(_client);
            }
        }


        /// <summary>
        /// 発行などの前に呼んで接続確認し、必要なら接続を張る
        /// </summary>
        public async Task EnsureConnectedAsync(CancellationToken ct = default)
        {
            if (!_client.IsConnected)
            {
                await ConnectAsync(ct);
            }
        }

        public IMqttClient Client => _client;// 外部から直接使えるように公開

        //Dispose時にクリーンに切断
        public async ValueTask DisposeAsync()
        {
            _disposed = true;
            try
            {
                if (_client.IsConnected)
                {
                    await _client.DisconnectAsync();
                }
            }
            catch (MqttBrokerNotFoundException ex)
            {
                Console.WriteLine($"接続失敗: {ex.Message}");
            }
            _client?.Dispose();
            _connectLock.Dispose();
        }

    }
}

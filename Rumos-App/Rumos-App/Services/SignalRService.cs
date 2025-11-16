using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Rumos_App.DTOs;

namespace Rumos_App.Services
{
    public class SignalRService
    {
        private readonly ILogger<SignalRService> _logger;
        public SignalRService(ILogger<SignalRService> logger)
        {
            _logger = logger;
        }
        private HubConnection? _hubConnection;

        public event Action<string>? ConnectionStatusChanged;
        public event Action<string, string>? MessageStatusChanged;
        public event Action<DeviceDto>? DeviceStatusChanged;

        public async Task InitializeAsync()
        {
            try
            {
                // すでに存在してるなら再利用
                if (_hubConnection == null)
                {
                    _hubConnection = new HubConnectionBuilder()
                        .WithUrl("https://localhost:7032/test")
                        .WithAutomaticReconnect()
                        .Build();

                    // 状態変化イベント
                    _hubConnection.Reconnecting += (ex) =>
                    {
                        ConnectionStatusChanged?.Invoke("🔄 再接続中...");
                        return Task.CompletedTask;
                    };

                    _hubConnection.Reconnected += (id) =>
                    {
                        ConnectionStatusChanged?.Invoke("✅ 再接続成功！");
                        return Task.CompletedTask;
                    };

                    _hubConnection.Closed += async (ex) =>
                    {
                        ConnectionStatusChanged?.Invoke("❌ 切断されました。再接続を試みます...");
                        await Task.Delay(2000);
                        await InitializeAsync();
                    };

                    // メッセージ受信イベント
                    _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
                    {

                        _logger.LogInformation($"📨 {message} : {user}");
                        MessageStatusChanged?.Invoke(user, message);
;                    });
                }

                // 状態を確認して接続
                if (_hubConnection.State == HubConnectionState.Disconnected)
                {
                    try
                    {
                        await _hubConnection.StartAsync();
                        ConnectionStatusChanged?.Invoke("✅ 接続成功！");
                        Debug.WriteLine("✅ Connected to SignalR hub");
                    }
                    catch (Exception ex)
                    {
                        ConnectionStatusChanged?.Invoke($"❌ 接続失敗: {ex.Message}");
                        Debug.WriteLine($"❌ SignalR connection failed: {ex}");
                    }
                }
                else
                {
                    Debug.WriteLine($"⚠️ SignalR already in state: {_hubConnection.State}");
                }
            }
            catch (Exception ex)
            {
                ConnectionStatusChanged?.Invoke($"❌ 初期化エラー: {ex.Message}");
                Debug.WriteLine($"❌ InitializeAsync exception: {ex}");
            }
        }

        public HubConnectionState? GetState() => _hubConnection?.State;

        public async Task SendMessageAsync(string user, string message)
        {
            if (_hubConnection is not null && _hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("SendMessage", user, message);
            }
            else
            {
                ConnectionStatusChanged?.Invoke("⚠️ 接続されていません。");
            }
        }

    }    
}
